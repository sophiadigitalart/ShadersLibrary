/*{
	"CREDIT" : "HypnoSwirl by Scottapotamas",
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
		}
,
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
,
		{
			"NAME": "iZoom", 
			"TYPE" : "float", 
			"MIN" : -3,
			"MAX" : 3,
			"DEFAULT" : 1
		}
	],
}
*/
// https://www.shadertoy.com/view/4ds3WH

#define halfPhase 3.5
#define speed_modifier 1.5

void main(void) {
	vec2 p = -1.0 +  iZoom * 2.0 * gl_FragCoord.xy / RENDERSIZE.xy; 
	p.x -= 0.0;
	p.y -= 0.0;

	float activeTime = TIME * speed_modifier;
	vec3 col; 
	float timeMorph = 0.0;
	
	p *= 7.0;
	
	float a = atan(p.y,p.x);
	float r = sqrt(dot(p,p));
	
	if(mod(activeTime, 2.0 * halfPhase) < halfPhase)
		timeMorph = mod(activeTime, halfPhase);
	else
		timeMorph = (halfPhase - mod(activeTime, halfPhase));	
		
	timeMorph = 2.0*timeMorph + 1.0;
	
	float w = 0.25 + 3.0*(sin(activeTime + 1.0*r)+ 3.0*cos(activeTime + 5.0*a)/timeMorph);
	float x = 0.8 + 3.0*(sin(activeTime + 1.0*r)+ 3.0*cos(activeTime + 5.0*a)/timeMorph);
	
	col = vec3(iColor.r,iColor.g,iColor.b)*1.1;

	gl_FragColor = vec4(col*w*x,1.0);
}
