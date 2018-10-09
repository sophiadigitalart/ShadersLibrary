/*{
	"DESCRIPTION": "https://www.shadertoy.com/view/4lVyDt",
	"CREDIT": "3d dot ring by iridule",
	"CATEGORIES": [
		"dots"
	],
	"INPUTS": [
	],
}*/

#define R RENDERSIZE.xy
#define T TIME

vec2 polarRep(vec2 U, float n) {
    n = 6.283/n;
    float a = atan(U.y, U.x),
        r = length(U);
    a = mod(a+n/2.,n) - n/2.;
    U = r * vec2(cos(a), sin(a));
    return .5* ( U+U - vec2(1,0) );
}

mat2 rotate(float a) {
    float c = cos(a),
        s = sin(a);
    return mat2(c, s, -s, c);
}


float ring(vec2 uv, float n, float s, float f) {
    uv = polarRep(uv, n);
    return smoothstep(s + f, s, length(uv));
}


void mainImage(out vec4 O, in vec2 I) {

	vec2 uv = (2. * I - R) / R.y;
    vec3 col = vec3(0.);
    
    uv.x += .1 * cos(T * .2);
    uv.y += .1 * sin(T * .2);
	
    float k = 12.;
    float n = 8.;
    for (float i = 0., s = 1. / n; i < 1.; i += s) {
        
        float t = fract(T * .1 + i);
        float z = smoothstep(1., .1, t);
        float f = smoothstep(0., 1., t) *
            smoothstep(1., .8, t);
        
        uv *= rotate(i * .15);
        col += ring(uv * z, k, .03, .0085) * f;
    
    }
    
    col *= col;

	O = vec4(col, 1.);

}


void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}