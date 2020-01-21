/*{
	"CREDIT" : "stripeytorus by fb39ca4",
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
		},
		{
			"NAME": "iBackgroundColor", 
			"TYPE" : "color", 
			"DEFAULT" : [
				0.6, 
				0.9, 
				0.0, 
				1.0
			]
		}
	],
}
*/

//  https://www.shadertoy.com/view/MsX3Wj Stripey Torus Interior
//Thank you iquilez for some of the primitive distance functions!


//const float PI = 3.14159265358979323846264;


//const int MAX_PRIMARY_RAY_STEPS = 64; //decrease this number if it runs slow on your computer

vec2 StripeyTorusRotate2d(vec2 v, float a) { 
	return vec2(v.x * cos(a) - v.y * sin(a), v.y * cos(a) + v.x * sin(a)); 
}

float StripeyTorusSdTorus( vec3 p, vec2 t ) {
  vec2 q = vec2(length(p.xz)-t.x,p.y);
  return length(q)-t.y;
}

float StripeyTorusDistanceField(vec3 p) {
	return -StripeyTorusSdTorus(p, vec2(4.0, 3.0));
}

vec3 StripeyTorusCastRay(vec3 pos, vec3 dir, float treshold) {
	for (int i = 0; i < iSteps; i++) {
			float dist = StripeyTorusDistanceField(pos);
			//if (abs(dist) < treshold) break;
			pos += dist * dir;
	}
	return pos;
}

void main(void)
{
	//vec4 mousePos = (iMouse / RENDERSIZE.xyxy) * 2.0 - 1.0;
	vec4 mousePos = vec4(0.0, 0.0, 1.0, 1.0);

	//vec2 screenPos = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
	vec2 uv = 2.0 * gl_FragCoord.xy / RENDERSIZE.xy - 1.0; 
	uv.x -= 0.0;
	uv.y -= 0.0;

	vec3 cameraPos = vec3(0.0, 0.0, -3.8);
	
	vec3 cameraDir = vec3(0.0, 0.0, 0.5);
	vec3 planeU = vec3(1.0, 0.0, 0.0) * 0.8;
	vec3 planeV = vec3(0.0, RENDERSIZE.y / RENDERSIZE.x * 1.0, 0.0);
	vec3 rayDir = normalize(cameraDir + uv.x * planeU + uv.y * planeV);
		
	vec3 rayPos = StripeyTorusCastRay(cameraPos, rayDir, 0.01);
	
	float majorAngle = atan(rayPos.z, rayPos.x);
	float minorAngle = atan(rayPos.y, length(rayPos.xz) - 4.0);
		
	float edge = mod(8.0 * (minorAngle + majorAngle + TIME) / 3.14159, 1.0);
	//float color = edge < 0.7 ? smoothstep(edge, edge+0.03, 0.5) : 1.0-smoothstep(edge, edge+0.03, 0.96);
	//float color = step(mod(8.0 * (minorAngle + majorAngle + TIME) / PI, 1.0), 0.5);
	//color -= 0.20 * step(mod(1.0 * (minorAngle + 1.0 * majorAngle + PI / 2.0) / PI, 1.0), 0.2);
	vec3 color = edge < 0.7 ? iBackgroundColor.rgb : iColor.rgb;
	//gl_FragColor = vec4(color);
	gl_FragColor = vec4(color, 1.0);
}