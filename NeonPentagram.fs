/*{
	"CREDIT" : "NeonPentagram by Unknown",
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

// https://www.shadertoy.com/view/MlcXDB
#define PI 3.14159265359
#define TWO_PI 6.28318530718

vec3 hsv2rgb(vec3 c) {
  vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
  vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
  return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float polygon(vec2 st, int numVertices) {
  float a = atan(st.x, st.y) + PI;
  float r = TWO_PI / float(numVertices);

  return cos(floor(.5+a/r)*r-a)*length(st);;
}

void main() {

    vec2 st = (gl_FragCoord.xy / RENDERSIZE.xy) * 2. - 1.;
    st.x *= RENDERSIZE.x/RENDERSIZE.y;

    float d = polygon(st, 5);
    float f = mod(fract(d * 7.0) + TIME / 4.0, 1.0);
    d = smoothstep(0.0, f, d);

    vec3 c = hsv2rgb(vec3(mod(d + TIME / 8.0 + (1.0 - length(st) / 4.0), 1.0), 1.0, 0.9));
    c = mix(c, vec3(0, 0, 0), length(st) * 0.3);

    fragColor = vec4(c,1.0);
}


