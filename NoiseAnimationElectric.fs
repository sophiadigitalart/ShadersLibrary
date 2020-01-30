/*{
	"CREDIT" : "NoiseAnimationElectric by nimitz",
	"CATEGORIES" : [
		"ci"
	],
	"DESCRIPTION": "",
	"INPUTS": [
		{
			"NAME": "background",
			"TYPE": "bool",
			"DEFAULT": false
		},
		{
			"NAME": "blurAmount",
			"TYPE": "float"
		},
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
// https://www.shadertoy.com/view/ldlXRS
// Noise animation - Electric
// by nimitz (stormoid.com) (twitter: @stormoid)
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License
// Contact the author for other licensing options

//The domain is displaced by two fbm calls one for each axis.
//Turbulent fbm (aka ridged) is used for better effect.

#define tm TIME*0.15
#define tau 6.2831853

mat2 makem2(in float theta){float c = cos(theta);float s = sin(theta);return mat2(c,-s,s,c);}
//float noise( in vec2 x ){return texture(iChannel0, x*.01).x;}
// procedural noise
float hash(const float n)
{
	return fract(sin(n)*29712.15073);
}

float noise(const vec2 x)
{
	vec2 p=floor(x); vec2 f=fract(x);
	f=f*f*(3.0-2.0*f);
	float n=p.x+p.y;
	float r1=mix(mix(hash(n+0.0),hash(n+1.0),f.x),mix(hash(n),hash(n+1.0),f.x),f.y);
    float r2=mix(mix(hash(n),hash(n+1.0),f.x),mix(hash(n),hash(n+1.0),f.x),f.y);
	return mix(r1,r2,f.y);
}
float fbm(in vec2 p)
{	
	float z=2.;
	float rz = 0.;
	vec2 bp = p;
	for (float i= 1.;i < 6.;i++)
	{
		rz+= abs((noise(p)-0.5)*2.)/z;
		z = z*2.;
		p = p*2.;
	}
	return rz;
}

float dualfbm(in vec2 p)
{
    //get two rotated fbm calls and displace the domain
	vec2 p2 = p*.7;
	vec2 basis = vec2(fbm(p2-tm*1.6),fbm(p2+tm*1.7));
	basis = (basis-.5)*.2;
	p += basis;
	
	//coloring
	return fbm(p*makem2(tm*0.2));
}

float circ(vec2 p) 
{
	float r = length(p);
	r = log(sqrt(r));
	return abs(mod(r*4.,tau)-3.14)*3.+.2;

}


void main(void)
{
	//setup system
	vec2 p = gl_FragCoord.xy / RENDERSIZE.xy-0.5;
	p.x *= RENDERSIZE.x/RENDERSIZE.y;
	p*=4.;
	
    float rz = background ? dualfbm(p) : 0.1;
	
	//rings
	p /= exp(mod(tm*10.,3.14159));
	rz *= pow(abs((0.1-circ(p))),.9);
	
	//final color
	vec3 col = vec3(.2,0.1,0.4)/rz;
	col=pow(abs(col),vec3(.99));
	fragColor = vec4(col,1.);	
	    
  
}
