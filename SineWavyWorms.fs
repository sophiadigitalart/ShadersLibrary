/*{
	"CREDIT" : "SineWavyWorms by Unknown",
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
// https://www.shadertoy.com/view/4dKSzz
float worm(vec2 fragCoord, vec2 scroll, float tiltFactor){
    // sine-wave based baseline (causing the horizontal waviness)
    float xLine = sin(fragCoord.x*0.02+scroll.x);
    // add nother wave for some more randomness
    xLine += sin(fragCoord.x*0.01)*2.5;
    // tilt
    xLine += fragCoord.x*tiltFactor;


    // cursor used for the sine-based sinPos, offet by the above xLine for waviness
    float yCursor = (xLine+fragCoord.y*0.2);
    // multiply factor; high value means more/narrower horizontal bands
    yCursor *= 0.1;
    // vertical sine-pos; causing the vertically stacked bands
    float val = sin(yCursor+scroll.y);

    
    // hardness; 0.0 means all black, 1.0 means blurry edges, 10.0 means high-contrast edges
	val *= 26.0;
 	// increase; higher value means more white
    val += -25.0;

    return val;
}

void main(void)
{
	//vec2 uv = iZoom * (gl_FragCoord.xy / RENDERSIZE.xy);
	//vec2 uv = (2.0*iZoom * gl_TexCoord[0].st) - 1.0;
	// global scroll movement speed
    vec2 scroll = vec2(2.0, -0.1) * TIME; //*6.0;
    float tilt = 0.0;
 
	float c = clamp(worm(gl_FragCoord.xy, scroll, tilt), 0.0, 1.0);
    c += clamp(worm(gl_FragCoord.xy, scroll+vec2(5.0, 3.0), tilt+0.01), 0.0, 1.0);
    c += clamp(worm(gl_FragCoord.xy, scroll+vec2(50.0, 2.0), tilt+0.003), 0.0, 1.0);
	
	gl_FragColor = vec4(vec3(c), 1.0);
}


