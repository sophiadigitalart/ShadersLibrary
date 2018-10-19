/*{
	"CREDIT" : "12869.0 by Unknown",
	"CATEGORIES" : [
		"ci"
	],
	"DESCRIPTION": "",
	"INPUTS": [
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
// glsl.heroku.come#12869.0

#define POINTS 10.0
#define RADIUS 100.0
#define BRIGHTNESS 0.95
#define COLOR vec3(0.7, 0.9, 1.2)
#define SMOOTHNESS 2.5

#define LAG_A 2.325
#define LAG_B 3.825
#define LAG_C 8.825

vec2 getPoint(float n) {
	float t = TIME * 0.1;
	//vec2 center = RENDERSIZE.xy / 2.0;
	vec2 center = gl_FragCoord.xy / 2.0;
	vec2 p = (
		  vec2(100.0, 0.0) * sin(t *  2.5 + n * LAG_A)
		+ vec2(0.0, 100.0) * sin(t * -1.5 + n * LAG_B)
		+ vec2(20.0, 50.0) * cos(t * 0.05 + n * LAG_C)
		+ vec2(50.0, 10.0) * sin(t * 0.15 + n)
	);
	return center + p;
}

void main() {
	vec2 uv = gl_FragCoord.xy - iMouse.xy;// - 0.5;
	uv.x *= float(RENDERSIZE.x )/ float(RENDERSIZE.y);
	float b = 0.0;
	
	for (float i = 0.0; i < POINTS; i += 1.0) {
		vec2 p = getPoint(i);
		float d = 1.0 - clamp(distance(p, uv) / RADIUS, 0.0, 1.0);
		b += pow(d, SMOOTHNESS);
	}
	
	vec3 c = b + (
		  (sin(b * 30.0) - 1.0) * vec3(0.1, 0.4, 0.15)
		+ (cos(b * 10.0) + 1.0) * vec3(0.8, 0.5, 0.25)
	);
	
	gl_FragColor = vec4(c * BRIGHTNESS * COLOR, 1.0);
}


