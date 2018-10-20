/*{
	"CREDIT" : "130187 by Unknown",
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
// glsl.heroku.come#13018.7
#define LINES 8.0
#define BRIGHTNESS 0.5

const vec3 ORANGE = vec3(1.4, 0.8, 0.4);
const vec3 BLUE = vec3(0.5, 0.9, 1.3);
const vec3 GREEN = vec3(0.9, 1.4, 0.4);
const vec3 RED = vec3(1.8, 0.4, 0.3);

void main() {
    float x, y, xpos, ypos;
    float t = TIME * 10.0;
    vec3 c = vec3(0.0);

    xpos = iZoom * gl_FragCoord.x / RENDERSIZE.x;
    ypos = iZoom * gl_FragCoord.y / RENDERSIZE.y;
    //xpos -= 0.0;
    //ypos -= 0.0;gl_FragCoord.xy / RENDERSIZE.xy
    x = xpos;
    for (float i = 0.0; i < LINES; i += 1.0) {
        for(float j = 0.0; j < 2.0; j += 1.0){
            y = ypos
            + (0.30 * sin(x * 2.000 +( i * 1.5 + j) * 0.4 + t * 0.050)
               + 0.100 * cos(x * 6.350 + (i  + j) * 0.7 + t * 0.050 * j)
               + 0.024 * sin(x * 12.35 + ( i + j * 4.0 ) * 0.8 + t * 0.034 * (8.0 *  j))
               + 0.5);
            
            c += vec3(1.0 - pow(clamp(abs(1.0 - y) * 5.0, 0.0,1.0), 0.25));
        }
    }
    
    c *= mix(
             mix(ORANGE, BLUE, xpos)
             , mix(GREEN, RED, xpos)
             ,(sin(t * 0.02) + 1.0) * 0.45
             ) * BRIGHTNESS;
    
    gl_FragColor = vec4(c, 1.0);
}
