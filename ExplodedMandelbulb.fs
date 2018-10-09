/*{
	"DESCRIPTION": "https://www.shadertoy.com/view/4ldfz7",
	"CREDIT": "by efairbanks",
	"CATEGORIES": [
		"Fractal"
	],
	"INPUTS": [
        {
			"NAME": "Decimal",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.1
		},
        {
			"NAME": "Liste",
			"LABEL": "mode",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2,
				3
			],
			"LABELS": [
				"Item1",
				"Item2",
				"Item3",
				"Item4"
			],
			"DEFAULT": 0
		},
        {
			"NAME" :	"iMouse",
			"TYPE" :	"point2D",
			"DEFAULT" :	[ 0.0, 0.0 ],
			"MAX" : 	[ 640.0, 480.0 ],
			"MIN" :  	[ 0.0, 0.0 ]
		},
		{
			"NAME": "Couleur",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				1.0,
				0.0,
				1.0
			]
		},
		{
			"NAME": "Booleen",
			"TYPE": "bool",
			"DEFAULT": 0.0
		}
	],
}*/

vec3 look(vec2 xy, vec3 origin, vec3 target)
{
	vec3 up=normalize(vec3(0.,1.,0.));
    vec3 fwd=normalize(target-origin);
    vec3 right=normalize(cross(fwd,up));
    up=normalize(cross(fwd,right));
    return normalize(fwd+right*xy.x+up*xy.y);
}
float map(vec3 pos)
{
    float Power=(1.-iMouse.x/RENDERSIZE.x)*8.6+1.;
   	vec3 z = pos;
	float dr = 1.0;
	float r = 0.0;
	for (int i = 0; i < 80 ; i++) {
		r = length(z);
        // if the length of the vector escapes toward
        // infinity, we're not hitting this thing
		if (r>100.) break;
		
		// convert to polar coordinates
		float theta = acos(z.z/r);
		float phi = atan(z.y,z.x);
		dr =  pow( r, Power-1.0)*Power*dr + 1.0;
		
		// scale and rotate the point
		float zr = pow(r*1.0,Power)+0.2;
		theta = theta*Power+46.57;
		phi = phi*Power+53.37;
		
		// convert back to cartesian coordinates
		z = zr*vec3(sin(theta)*cos(phi), sin(phi)*sin(theta), cos(theta));
        
        // add the original point to the new one and recurse/repeat
		z+=pos;
	}
    // fudge distance estimation from fractal
    // using some calculus-y math I don't quite understand
	return 0.5*log(r)*r/dr;
}
#define MAX_DISTANCE 14.
#define MAX_STEPS 40
float march(vec3 origin,vec3 ray,int steps)
{
    float t=.05;
    for(int i=0;i<MAX_STEPS; i++)
    {
		float d=map(origin+ray*t);
        if(d<0.0005||d>=MAX_DISTANCE||i>=steps) break;
        t+=d*0.95;
    }
    return min(t,MAX_DISTANCE);
}
vec3 hsv2rgb(vec3 c) {
  vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
  vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
  return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}
vec3 normal(vec3 p,float epsilon){vec2 e=vec2(epsilon,0.);return normalize(vec3(map(p+e.xyy)-map(p-e.xyy),map(p+e.yxy)-map(p-e.yxy),map(p+e.yyx)-map(p-e.yyx)));}
void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    // let's render!
    vec2 uv=(fragCoord/RENDERSIZE.xy-.5)*2.;
	uv.x=uv.x*RENDERSIZE.x/RENDERSIZE.y;   
    vec3 camera=vec3(1.);
    camera=vec3(sin(TIME/4.),sin(TIME/4.),cos(TIME/4.))*1.2;
    vec3 ray=look(uv,camera,vec3(0.));
	float dist=march(camera,ray,MAX_STEPS);
    vec3 hit=camera+ray*dist;
    float ao=pow(1.-dist/MAX_DISTANCE,20.);
    float diffuse=clamp(dot(normal(hit,0.01*dist),normalize(camera)),0.5,1.);
    float shade=diffuse*ao*0.5+ao*0.5;
    vec3 color=hsv2rgb(vec3(length(hit)*.5+0.6,sin(length(hit*ao)*50.)*0.2+.8,shade*2.));
    fragColor = vec4(color,1.0);
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}