/*{
	"CREDIT" : "Tiny data.path by yx",
	"CATEGORIES" : [
		"ci"
	],
	"DESCRIPTION": "",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE" : "image"
		},
			{
		"NAME": "iChannel0",
		"TYPE" : "image"
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
			"DEFAULT" : 4.0
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
// https://www.shadertoy.com/view/Ml3cDS

void main(void)
{  
    vec2 u=iZoom * abs(gl_FragCoord.xy/RENDERSIZE.xy-.5);
    u.x*=3.;

	fragColor = vec4(mix( 
        .3-.3*pow(u.x/length(u),5.), 
        step(.5,fract(sin(dot(floor(vec2(2./u.x,iSteps/u.x*u.y)),vec2(12.13,4.47)))+TIME)),
        u.x>u.y
    )*max(u.x,u.y)*2.);
}
