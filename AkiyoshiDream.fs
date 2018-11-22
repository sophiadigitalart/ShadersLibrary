/*{
	"CREDIT" : "AkiyoshiDream by bigblueboo",
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
// https://www.shadertoy.com/view/ldsSRS
const float PI = 3.14159265358979323846264;
void main(void)
{
   vec2 uv = iZoom * gl_FragCoord.xy / RENDERSIZE.xy;
   uv.x -= 0.0;
   uv.y -= 0.0;
   vec2 coord = uv - vec2(.5,.5);
   coord.y *= RENDERSIZE.y / RENDERSIZE.x;
   float angle = atan(coord.y, coord.x);
   float dist = length(coord);
   
   float brightness = .25 + .25 * 
      sin(48.0*angle + dist*PI + sin(angle*1.0)*(dist + (.5+.5*sin(-PI/2.0+TIME*PI))*mod(TIME,2.0)) * 2.0 * PI);
   brightness += .25 + .25 * sin(pow(dist,.5) / .707 * PI * 32.0 - TIME * PI * .5);
   if (dist < .01) brightness *= (dist / .01);

   fragColor = vec4(brightness, brightness, brightness,1.0);
}
