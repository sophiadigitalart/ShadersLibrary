/*{
	"CREDIT" : "Promethean by nimitz",
	"CATEGORIES" : [
		"ci"
	],
	"DESCRIPTION": "",
	"INPUTS": [
		{
			"NAME": "iZoom",
			"TYPE" : "float",
			"MIN" : 0.1,
			"MAX" : 2.0,
			"DEFAULT" : 1.0
		},
		{
			"NAME": "iSteps",
			"TYPE" : "float",
			"MIN" : 2.0,
			"MAX" : 128.0,
			"DEFAULT" : 64.0
		},
		{
			"NAME": "iTimeMultiplier",
			"TYPE" : "float",
			"MIN" : 0.01,
			"MAX" : 10.0,
			"DEFAULT" : 1.0
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
// Promethean by nimitz (twitter: @stormoid)
// https://www.shadertoy.com/view/4tB3zV
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License
// Contact the author for other licensing options

//More "Spark-ish" look
#define PALETTE vec3(0.0,.2,1.5)
#define STEPS 45
#define ALPHA_WEIGHT 0.044
#define BASE_STEP 0.11

vec2 mo;
vec2 rot(in vec2 p, in float a){float c = cos(a), s = sin(a);return p*mat2(c,s,-s,c);}
float hash21(in vec2 n){ return fract(sin(dot(n, vec2(12.9898, 4.1414))) * 43758.5453); }

// procedural noise
float hash(const float n)
{
	return fract(sin(n)*29712.15073);
}

float noise(const vec3 x)
{
	vec3 p=floor(x); vec3 f=fract(x);
	f=f*f*(3.0-2.0*f);
	float n=p.x+p.y+p.z;
	float r1=mix(mix(hash(n+0.0),hash(n+1.0),f.x),mix(hash(n),hash(n+1.0),f.x),f.y);
    float r2=mix(mix(hash(n),hash(n+1.0),f.x),mix(hash(n),hash(n+1.0),f.x),f.y);
	return mix(r1,r2,f.z);
}

float fbm(in vec3 p)
{
    p *= 2.5 + mo.y*2.;
    float rz = 0., z = 1.;
    for(int i=0;i<4;i++)
    {
        float n = noise(p + TIME*.5);
        rz += (sin(n*4.3)*1.-.45)*z;
        z *= .47;
        p *= 3.;
    }
    return rz;
}

float dsph(in vec3 p)
{
    float r = dot(p,p);
    vec2 sph = vec2(acos(p.y/r), atan(p.x, p.z));
    r += sin(sph.y*2.+sin(sph.x*2.)*5.)*0.8;
    return r;
}

vec4 map(in vec3 p)
{
    float dtp = dsph(p); //Inversion basis is a deformed sphere
	p = .7*p/(dtp + .1);
    p.xz = rot(p.xz, p.y*2.);
    #ifdef SPARKS
    p = 6.*p/(dtp - 6.);
    p = 7.5*p/(dtp + 7.);
    float r = clamp(fbm(p)*1.5-exp2(dtp*0.7-2.73), 0., 1.);
    vec4 col = vec4(1.)*r*r;
    #else
    p = 6.*p/(dtp - 5.4);
    p = 7.*p/(dtp + 6.);
    float r = clamp(fbm(p)*1.5-exp2(dtp*0.7-2.75), 0., 1.);
    vec4 col = vec4(1.)*r;
    #endif
    vec3 lv = mix(p,vec3(.25),1.25);
    float grd =  clamp((col.w - fbm(p+lv*.045))*4.5, 0.01, 2. );
    col.rgb *= grd*vec3(.9, 1., .43) + vec3(.05,0.1,0.0);
    col.a *= clamp(dtp*0.5-.14,0.,1.)*0.7 + 0.3;
    
    return col;
}

vec4 vmarch(in vec3 ro, in vec3 rd)
{
	vec4 rz = vec4(0);
	float t = 2.4;
    t += 0.03*hash21(gl_FragCoord.xy);
	for(int i=0; i<STEPS; i++)
	{
		if(rz.a > 0.99 || t > 6.)break;

		vec3 pos = ro + t*rd;
        vec4 col = map(pos);
        float den = col.a;
        col.a *= ALPHA_WEIGHT;
		col.rgb *= col.a*1.4;
		rz = rz + col*(1. - rz.a);   
        t += BASE_STEP-den*BASE_STEP;
	}
    
    rz.rgb += PALETTE*rz.w;
    return rz;
}

void main(void)
{
	vec2 p = gl_FragCoord.xy/RENDERSIZE.xy*2. - 1.;
	p.x *= RENDERSIZE.x/RENDERSIZE.y*0.95;
	mo = 2.0*iMouse.xy/RENDERSIZE.xy;
    mo = (mo==vec2(.0))?mo=vec2(0.5,2.):mo;
    mo.x += TIME*0.01;
	
	vec3 ro = 4.0*normalize(vec3(cos(2.75-3.0*mo.x), sin(TIME*0.22)*0.2, sin(2.75-3.0*mo.x)));
	vec3 eye = normalize(vec3(0) - ro);
	vec3 rgt = normalize(cross(vec3(0,1,0), eye));
	vec3 up = cross(eye,rgt);
	vec3 rd = normalize(p.x*rgt + p.y*up + 2.3*eye);
	
	vec4 col = vmarch(ro, rd);
    fragColor = vec4(col.rgb, 1.0);
}
