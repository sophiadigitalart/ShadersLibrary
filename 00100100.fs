/*{
	"CREDIT" : "00100100 by mafik",
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
// functions begin
// https://www.shadertoy.com/view/lsS3DK

float Wave00100100(float x, float s) {
	return sin(x + mod(TIME * s, 3.1415 * 2.))/2. + .5;
}
// https://www.shadertoy.com/view/lsS3DK
void main(void)
{
	vec2 uv = iZoom * gl_FragCoord.xy / RENDERSIZE.xy;
	uv.x -= 0.0;
	uv.y -= 0.0;
	uv.x -= 0.5;
	uv.y -= 0.5;
	uv.x *= 2.;
	uv.y *= 2.;
	
	float t = mod(TIME* 2., 3.1415 * 2.);
	float a = 0.;
	for(float i = 1.; i <= 3.; ++i) {	
		a += sin(t * (i * 2. - 1.)) / (i * 2. - 1.);
	}
	a = a * 1.15 / 2. + .5;
	
	float power = 2. / (1. - min(a, .98));
	float x = pow((1.+2.*a)*abs(uv.x), power);
	float y = pow(abs(uv.y), power);
	float r = RENDERSIZE.y / 2.;
	float v = pow(x+y, 1./power);
	float l = (1. - v) * r;
	float l2 = (r/2. - l*(1.-a));
	float s = clamp(min(l, l2), 0., 1.);
	vec3 col = vec3(1.,1.,1.); // vec4(Wave00100100(uv.x,3.), Wave00100100(uv.y,5.), Wave00100100(uv.x*uv.y,7.),1.0);
	gl_FragColor = vec4(col * s,1.0);
}
