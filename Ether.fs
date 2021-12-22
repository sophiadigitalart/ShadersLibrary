/*{
	"CREDIT" : "Ether by nimitz",
	"CATEGORIES" : [
		"ci"
	],
	"DESCRIPTION": "",
	"INPUTS": [
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
	]
}
*/

// https://www.shadertoy.com/view/MsjSW3

mat2 m(float a){float c=cos(a), s=sin(a);return mat2(c,-s,s,c);}
float map(vec3 p){
    p.xz*= m(TIME*0.4);p.xy*= m(TIME*0.3);
    vec3 q = p*2.+TIME*1.;
    return length(p+vec3(sin(TIME*0.7)))*log(length(p)+1.) + sin(q.x+sin(q.z+sin(q.y)))*0.5 - 1.;
}

void main(void)
{
	
	vec2 p = gl_FragCoord.xy/RENDERSIZE.y - vec2(.9,.5);
    vec3 cl = vec3(0.);
    float d = 2.5;
    for(int i=0; i<=5; i++)	{
		vec3 p = vec3(0,0,5.) + normalize(vec3(p, -1.))*d;
        float rz = map(p);
		float f =  clamp((rz - map(p+.1))*0.5, -.1, 1. );
        vec3 l = vec3(0.1,0.3,.4) + vec3(5., 2.5, 3.)*f;
        cl = cl*l + (1.-smoothstep(0., 2.5, rz))*.7*l;
		d += min(rz, 1.);
	}
    gl_FragColor = vec4(cl, 1.);
}
