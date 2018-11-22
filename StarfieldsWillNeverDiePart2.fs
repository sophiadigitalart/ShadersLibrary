/*{
	"CREDIT" : "Starfields Will Never Die Part 2 by WAHa_06x36",
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
// https://www.shadertoy.com/view/MlKBWw
// brightness = pow(2.521, -magnitude)
vec3 starColour(float bv, float brightness) {
	if (bv < -0.4) bv = -0.4;
	if (bv > 2.0) bv=2.0;

	float r = 0.0;
	if (bv < 0.00) { float t=(bv+0.40)/(0.00+0.40); r=0.61+(0.11*t)+(0.1*t*t); }
	else if (bv < 0.40) { float t=(bv-0.00)/(0.40-0.00); r=0.83+0.17*t; }
	else { r = 1.00; }

	float g = 0.0;
	if (bv < 0.00) { float t=(bv+0.40)/(0.00+0.40); g=0.70+(0.07*t)+(0.1*t*t); }
	else if (bv < 0.40) { float t=(bv-0.00)/(0.40-0.00); g=0.87+(0.11*t); }
	else if (bv < 1.60) { float t=(bv-0.40)/(1.60-0.40); g=0.98-(0.16*t); }
	else { float t=(bv-1.60)/(2.00-1.60); g=0.82-(0.5*t*t); }

	float b = 0.0;
	if (bv < 0.40) { b = 1.00; }
	else if (bv < 1.50) { float t=(bv-0.40)/(1.50-0.40); b=1.00-(0.47*t)+(0.1*t*t); }
	else if (bv < 1.94) { float t=(bv-1.50)/(1.94-1.50); b=0.63-(0.6*t*t); }
	else { b = 0.0; }

	vec3 linear = pow(vec3(r, g, b), vec3(2.2));

	return pow(brightness * 3.0 * linear / (linear.x + linear.y + linear.z), vec3(1.0 / 2.2));
}

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

void main(void)
{  
  vec3 colour = vec3(0.0);
	for(float i = 0.0; i < 5.0; i++) {
		vec2 p = iZoom * (2.0 * gl_FragCoord.xy - RENDERSIZE.xy) / min(RENDERSIZE.x, RENDERSIZE.y);
		vec3 v = vec3(p, 1.0 - length(p) * 0.2);
	
		float ta = TIME * 0.1;
		mat3 m=mat3(
			0.0,1.0,0.0,
			-sin(ta),0.0,cos(ta),
			cos(ta),0.0,sin(ta));
		m*=m*m;
		m*=m;
		v=m*v;
	
		float a = (atan(v.y, v.x) / 3.141592 / 2.0 + 0.5);
		float slice = floor(a * 1000.0);
		float phase = rand(vec2(slice, 0.0 + i * 10.0));
		float dist = rand(vec2(slice, 1.0 + i * 10.0)) * 3.0;
		float hue = rand(vec2(slice, 2.0 + i * 10.0));
		float bright = pow(rand(vec2(slice, 3.0)), 5.0);
	
		float z = dist / length(v.xy) * v.z;
		float Z = mod(z + phase + TIME * 0.6, 1.0);
		float d = sqrt(z * z + dist * dist);
	
		float c = exp(-Z * 40.0 + 0.3) / (d * d + 1.0);
		colour += starColour(hue * 2.4 - 0.4, c * 2.0 * bright);
	}
	fragColor = vec4(colour, 1.0);
}
