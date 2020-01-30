/*{
	"CREDIT" : "ShaderforthLace by Daeken",
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
			"MIN" : 0.1,
			"MAX" : 2.0,
			"DEFAULT" : 1.0
		},
		{
			"NAME": "iSteps",
			"TYPE" : "float",
			"MIN" : 2.0,
			"MAX" : 128.0,
			"DEFAULT" : 64.0
		},
		{
			"NAME": "iTimeMultiplier",
			"TYPE" : "float",
			"MIN" : 0.01,
			"MAX" : 10.0,
			"DEFAULT" : 1.0
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

// https://www.shadertoy.com/view/4ssSR8

vec2 move(vec2, float);
vec2 cart_polar(vec2);
vec2 polar_cart(vec2);
float point_distance_line(vec2, vec2, vec2);
vec2 polar_norm(vec2);
vec2 move(vec2 pos, float t) {
	return polar_norm(vec2(((pos).x) + (t) + (sin((t) * ((pos).x))) * 0.1, ((pos).y) + (sin((t) * ((pos).x))) * 0.5));
}
vec2 cart_polar(vec2 p) {
	return vec2(atan((p).y, (p).x), length(p));
}
vec2 polar_cart(vec2 p) {
	return (vec2(cos((p).x), sin((p).x))) * ((p).y);
}
float point_distance_line(vec2 a, vec2 b, vec2 point) {
	float h = clamp((dot((point) - (a), (b) - (a))) / (length((b) - (a))), 0.0, 1.0);
	return length((point) - (a) - ((b) - (a)) * (h));
}
vec2 polar_norm(vec2 p) {
	return cart_polar(polar_cart(p));
}
void main(void) {
	vec2 uv = 2.0 * iZoom * (gl_FragCoord.xy/RENDERSIZE.xy- 0.5);
	uv.x *= float(RENDERSIZE.x )/ float(RENDERSIZE.y);
	uv.x -= 0.0;
	uv.y -= 0.0;	
	vec2 p = cart_polar(uv);
	float t = (TIME * iTimeMultiplier) + 1.0;
	t = (t) - 25.0;
	vec2 start = cart_polar(vec2(0.0, 0.5));
	vec2 last = start;
	vec3 color = vec3(0.0, 0.0, 0.0);
	for(int temp_2 = 0; temp_2 < 25; ++temp_2) {
		vec2 cp = move(start, t);
		vec2 h = vec2(0.01, 0.0);
		float dist = (abs(min(point_distance_line(cp, last, p), point_distance_line(polar_cart(cp), polar_cart(last), polar_cart(p))))) / (length((vec2((min(point_distance_line(cp, last, (p) + (h)), point_distance_line(polar_cart(cp), polar_cart(last), polar_cart((p) + (h))))) - (min(point_distance_line(cp, last, (p) - (h)), point_distance_line(polar_cart(cp), polar_cart(last), polar_cart((p) - (h))))), (min(point_distance_line(cp, last, (p) + ((h).yx)), point_distance_line(polar_cart(cp), polar_cart(last), polar_cart((p) + ((h).yx))))) - (min(point_distance_line(cp, last, (p) - ((h).yx)), point_distance_line(polar_cart(cp), polar_cart(last), polar_cart((p) - ((h).yx))))))) / 2.0 * ((h).x)));
		last = cp;
		float hit = (smoothstep(0.0, 0.001, 0.1 / (dist))) * 10.0;
		float high = (smoothstep(0.0, 0.001, 0.1 / (dist))) * 10.0;
		color = (vec3(hit, (high) * (abs(sin((t) / 100.0))), (hit) * (mix(0.1, 1.2, (float(temp_2)) / 25.0)))) + (color);
		t = (t) + 333.3333;
	}
	fragColor = vec4((color) / 2.0, 1.0);
}

