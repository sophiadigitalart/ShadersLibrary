/*{
	"CREDIT" : "NoiseAnimation3D by nimitz",
	"CATEGORIES" : [
		"ci"
	],
	"DESCRIPTION": "",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "blurAmount",
			"TYPE": "float"
		},
		{
			"NAME": "iZoom",
			"TYPE" : "float",
			"MIN" : 0.0,
			"MAX" : 1.0,
			"DEFAULT" : 1.0
		},
		{
			"NAME": "iSteps",
			"TYPE" : "float",
			"MIN" : 2.0,
			"MAX" : 75.0,
			"DEFAULT" : 19.0
		},
		{
			"NAME" :"iMouse",
			"TYPE" : "point2D",
			"DEFAULT" : [0.0, 0.0],
			"MAX" : [640.0, 480.0],
			"MIN" : [0.0, 0.0]
		},
		{
			"NAME": "iColor", 
			"TYPE" : "color", 
			"DEFAULT" : [
				0.9, 
				0.6, 
				0.0, 
				1.0
			]
		}
	],
}
*/
// https://www.shadertoy.com/view/XdfXRj
//Noise animation - 3D
//by nimitz (twitter: @stormoid)

//The noise is 3d fbm noise, but each noise grab is displaced
//by a sin-grid, then blended with a ratio with the previous noise

//Most of the raymarching code is from iq's "Clouds"
//define FLAT for a non-spherical noise object

//#define FLAT

#define STEPS 22
#define NOISE_INTENSITY 3.

#define time TIME

//iq's ubiquitous 3d noise
/*float noise(in vec3 p)
{
	vec3 ip = floor(p);
    vec3 f = fract(p);
	f = f*f*(3.0-2.0*f);
	
	vec2 uv = (ip.xy+vec2(37.0,17.0)*ip.z) + f.xy;
	//vec2 rg = textureLod( iChannel0, (uv+ 0.5)/256.0, 0.0 ).yx;
	vec2 rg = IMG_THIS_PIXEL(inputImage).yx;
	return mix(rg.x, rg.y, f.z);
}*/
vec3 hash(vec3 p) 
{
    p = vec3(dot(p, vec3(127.1, 311.7, 74.7)),
        dot(p, vec3(269.5, 183.3, 246.1)),
        dot(p, vec3(113.5, 271.9, 124.6)));

    return -1.0 + 2.0*fract(sin(p)*43758.5453123);
}
float noise(in vec3 p)
{
    vec3 i = floor(p);
    vec3 f = fract(p);

    vec3 u = f * f*(3.0 - 2.0*f);

    return mix(mix(mix(dot(hash(i + vec3(0.0, 0.0, 0.0)), f - vec3(0.0, 0.0, 0.0)),
        dot(hash(i + vec3(1.0, 0.0, 0.0)), f - vec3(1.0, 0.0, 0.0)), u.x),
        mix(dot(hash(i + vec3(0.0, 1.0, 0.0)), f - vec3(0.0, 1.0, 0.0)),
            dot(hash(i + vec3(1.0, 1.0, 0.0)), f - vec3(1.0, 1.0, 0.0)), u.x), u.y),
        mix(mix(dot(hash(i + vec3(0.0, 0.0, 1.0)), f - vec3(0.0, 0.0, 1.0)),
            dot(hash(i + vec3(1.0, 0.0, 1.0)), f - vec3(1.0, 0.0, 1.0)), u.x),
            mix(dot(hash(i + vec3(0.0, 1.0, 1.0)), f - vec3(0.0, 1.0, 1.0)),
                dot(hash(i + vec3(1.0, 1.0, 1.0)), f - vec3(1.0, 1.0, 1.0)), u.x), u.y), u.z);
}
mat3 m3 = mat3( 0.00,  0.80,  0.60,
              -0.80,  0.36, -0.48,
              -0.60, -0.48,  0.64 );

float grid(vec3 p)
{
	float s = sin(p.x)*cos(p.y);
	//float s = sin(p.x)*cos(p.y);
	return s;
}

float flow(in vec3 p)
{
	float z=2.;
	float rz = 0.;
	vec3 bp = p;
	for (float i= 1.;i < 5.;i++ )
	{
		//movement
		p += time*0.25;
		bp -= time*.3;
		
		//displacement map
		vec3 gr = vec3(grid(p*3.-time*1.),grid(p*3.5+4.-time*1.),grid(p*4.+4.-time*1.));
		p += gr*0.15;
		rz+= (sin(noise(p)*8.)*0.5+0.5) /z;
		
		//advection factor (.1 = billowing, .9 high advection)
		p = mix(bp,p,.7);
		
		//scale and rotate
		z *= 2.;
		p *= 2.01;
		p*=m3;
		bp *= 1.7;
		bp*=m3;
	}
	return rz;	
}

vec4 map(in vec3 p)
{
	#ifdef FLAT
	float d = -1.1;
	#else
	float d = 1.5-dot(p,p);
	#endif
	vec3 q = p+vec3(1.0,0.,0.)*time*.2;
	float f = flow(q);

	d += NOISE_INTENSITY*f;
	d = clamp(d, 0.0, 1.0);
	
	vec4 res = vec4(d);
	//color
	res.xyz = mix( 3.*vec3(.7,0.95,0.5), vec3(0.4,.5,.5), res.x );
	return res;
}


vec4 raymarch( in vec3 ro, in vec3 rd )
{
	vec4 sum = vec4(0);

	float t = 2.1;
	for(int i=0; i<STEPS; i++)
	{
		if( sum.a > 0.99 ) continue;

		vec3 pos = ro + t*rd;
		vec4 col = map( pos );
		
		//lights
		#if 1
		float dif =  clamp((col.w - map(pos+.2).w)/.5, 0.1, 1. );
        vec3 lin = vec3(0.5,0.2,.5)*1. + .5*vec3(2., 0.8, 1.)*dif;
		col.xyz *= lin;
		#endif
		
		col.a *= .2;
		col.rgb *= col.a;
		sum = sum + col*(1. - sum.a);
		//fixed step
		t += 0.06;
	}
	sum.b += sum.w*0.45;
	return clamp(sum, 0.0, 1.0);
}

void main(void)
{
	vec2 q = gl_FragCoord.xy / RENDERSIZE.xy;
	vec2 p = -1.0 + 2.0*q;
	p.x *= RENDERSIZE.x/ RENDERSIZE.y*0.95;
	vec2 mo = -1.0 + 2.0*iMouse.xy / RENDERSIZE.xy;
	mo.x += time*0.025;
	p*= 2.5;
	
	//camera
	vec3 ro = 4.0*normalize(vec3(cos(2.75-3.0*mo.x), -mo.y, sin(2.75-3.0*mo.x)));
	vec3 ta = vec3(0.);
	vec3 ww = normalize(ta - ro);
	vec3 uu = normalize(cross(vec3(0.,1.,0.), ww));
	vec3 vv = normalize(cross(ww,uu));
	vec3 rd = normalize(p.x*uu + p.y*vv + 5.*ww);

	
	vec4 col = raymarch(ro, rd);
	    
    gl_FragColor = vec4(col.rgb, 1.0);
}
