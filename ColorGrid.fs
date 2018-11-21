/*{
	"CREDIT" : "ColorGrid by iq",
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
// https://www.shadertoy.com/view/4dBSRK
void main(void) 
{
	vec2  px = 4.0*(-RENDERSIZE.xy + 2.0*gl_FragCoord.xy)/RENDERSIZE.y;
	float id = 0.5 + 0.5*cos(TIME + sin(dot(floor(px+0.5),vec2(113.1,17.81)))*43758.545);
	vec3 co = 0.5 + 0.5*cos(TIME + 3.5*id + vec3(0.0,1.57,3.14) );
	vec2 pa = smoothstep( 0.0, 0.2, id*(0.5 + 0.5*cos(6.2831*px)) );
	fragColor = vec4( co*pa.x*pa.y, 1.0 );
}
