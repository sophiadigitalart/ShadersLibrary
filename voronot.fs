/*{
	"DESCRIPTION": "https://www.shadertoy.com/view/4tXGW4",
	"CREDIT": "Voronot by Trisomie21",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
	],
}*/

#define iResolution RENDERSIZE

void mainImage( out vec4 f, in vec2 w ) {
	
	vec2  r = iResolution.xy, p = w - r*.5;
	float d = length(p) / r.y, c=1., x = pow(d, .1), y = atan(p.x, p.y) / 6.28;
	
	for (float i = 0.; i < 3.; ++i)    
		c = min(c, length(fract(vec2(x - TIME*i*.005, fract(y + i*.125)*.5)*20.)*2.-1.));

	f = vec4(d+20.*c*d*d*(.6-d));
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}