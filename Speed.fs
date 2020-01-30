/*{
	"CREDIT" : "Speed by lsdlive",
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
			"MAX" : 75.0,
			"DEFAULT" : 19.0
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
// https://www.shadertoy.com/view/lsycDG

// @lsdlive

// Doodling session for live-coding or sketching new ideas.
// Thanks to iq, mercury, lj, shane, shau, aiekick, balkhan
// & all shadertoyers.
// Greets to all the shader showdown paris gang.


mat2 r2d(float a) {
	float c = cos(a), s = sin(a);
	return mat2(c, s, -s, c);
}

float rep(float p, float m) {
	return mod(p - m*.5, m) - m*.5;
}

vec3 rep(vec3 p, float m) {
	return mod(p - m*.5, m) - m*.5;
}

float cmin(float a, float b, float k) {
	return min(min(a, b), (a - k + b) * sqrt(.5));
}

float stmin(float a, float b, float k, float n) {
	float s = k / n;
	float u = b - k;
	return min(min(a, b), .5 * (u + a + abs((mod(u - a + s, 2. * s)) - s)));
}

float length8(vec2 p) {
	vec2 q = p*p*p*p*p*p*p*p;
	return pow(q.x + q.y, 1. / 8.);
}

float torus82(vec3 p, vec2 d) {
	vec2 q = vec2(length(p.xz) - d.x, p.y);
	return length8(q) - d.y;
}

float path(float t) {
	return cos(t*.1)*2.;
}

float g = 0.;
float de(vec3 p) {

	p.x -= path(p.z);
    
    vec3 q = p;
    q.x += sin(q.z*.2)*4.;
    q.y += cos(q.z*.3)*4.;
    q += TIME*2.;
    q.yz += sin(TIME*.2)*4.;
    q = rep(q, 1.);
    float s1 = length(q) - .01 + sin(TIME*30.)*.004;

	p.z = rep(p.z, 3.);

	float d = torus82(p.xzy, vec2(1., .1));
	float pl = p.y + 2.4 + p.y;//*texture(iChannel1, p.xz*.1).r*1.
    float pl2 = p.y + .7;
	d = min(d, pl-d*.9);
    d = cmin(d, pl2, .1);

	p.x = abs(p.x) - 1.;
	float cyl = length(p.xy) - .05;
	d = stmin(d, cyl, .1, 4.);
   
    d = min(d, s1);

	g += .015 / (.01 + d*d);
	return d;
}
void main(void)
{  		
    vec2 uv = (gl_FragCoord.xy - .5*RENDERSIZE.xy) / -RENDERSIZE.y*iZoom;

    float dt = TIME * iTimeMultiplier;
    vec3 ro = vec3(0, 0, -3. + dt);
	vec3 ta = vec3(0, 0, dt);
	ro.x += path(ro.z);
	ta.x += path(ta.z);

	vec3 fwd = normalize(ta - ro);
	vec3 left = cross(vec3(0, 1, 0), fwd);
	vec3 up = cross(fwd, left);
	vec3 rd = normalize(fwd + uv.x*left + uv.y*up);

	rd.xy *= r2d(sin(-ro.x / 3.14)*.4);

	vec3 p;
	float t = 0., ri;
	for (float i = 0.; i < 1.; i += .01) {
		ri = i;
		p = ro + rd*t;
		float d = de(p);
		if (d < .001) break;
		t += d*.3;
	}

	vec3 bg = vec3(.2, .1, .2);
	vec3 col = bg;
	col = mix(vec3(.4, .52, .6)*1.5, bg,  uv.x*uv.y*uv.y+ri);
    col += g*.01;
    col.b += sin(p.z*.1)*.1;
	col = mix(col, bg, 1. - exp(-.01*t*t));
    
	fragColor = vec4(col, 1.);
}
