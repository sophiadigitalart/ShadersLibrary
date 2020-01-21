/*{
	"CREDIT" : "hexler330 by Unknown",
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
void main(void) {vec2 uv = 2 * (gl_FragCoord.xy / RENDERSIZE.xy - vec2(0.5));float radius = length(uv);float angle = atan(uv.y, uv.x);float col = .0;col += 1.5*sin(TIME + 13.0 * angle + uv.y * 20);col += cos(.9 * uv.x * angle * 60.0 + radius * 5.0 - TIME * 2.);fragColor = (1.2 - radius) * vec4(vec3(col), 1.0);}