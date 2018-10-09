/*{
	"DESCRIPTION": "https://www.shadertoy.com/view/lslGRH",
	"CREDIT": "NanoTubes by Trisomie21",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
	],
}*/

// With tweaks from fernlightning

float rand(vec3 n) {
  n = floor(n);
  return fract(sin((n.x+n.y*1e2+n.z*1e4)*1e-4)*1e5);
}

// .x is distance, .y = colour
vec2 map( vec3 p ) {
	const float RADIUS = 0.25;

	// cylinder
	vec3 f = fract( p ) - 0.5;
	float d = length( f.xy );
        float cr = rand( p );
	float cd = d - cr*RADIUS;

	// end - calc (rand) radius at more stable pos
	p.z -= 0.5;
	float rr = rand( p );
	float rn = d - rr*RADIUS;
    float rm = abs( fract( p.z ) - 0.5 );  // offset so at end of cylinder
       
	float rd = sqrt( rn*rn + rm*rm ); // end with ring

	return (cd < rd) ?  vec2( cd, cr ) : vec2( rd, rr ); // min
}

void mainImage( out vec4 fragColor, in vec2 fragCoord ) {
    vec2 pos = (fragCoord.xy*2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    vec3 camPos = vec3(cos(TIME*0.3), sin(TIME*0.3), 3.5);
    vec3 camTarget = vec3(0.0, 0.0, .0);

    vec3 camDir = normalize(camTarget-camPos);
    vec3 camUp  = normalize(vec3(0.0, 1.0, 0.0));
    vec3 camSide = cross(camDir, camUp);
    float focus = 1.8;

    vec3 rayDir = normalize(camSide*pos.x + camUp*pos.y + camDir*focus);
    vec3 ray = camPos;
    float m = 0.32;
    vec2 d;
    float total_d = 0.;
    const int MAX_MARCH = 100;
    const float MAX_DISTANCE = 100.0;
    for(int i=0; i<MAX_MARCH; ++i) {
        d = map(ray-vec3(0.,0.,TIME/2.));
        total_d += d.x;
        ray += rayDir * d.x;
        m += 1.0;
        if(abs(d.x)<0.01) { break; }
        if(total_d>MAX_DISTANCE) { total_d=MAX_DISTANCE; break; }
    }

    float c = (total_d)*0.0001;
    vec4 result = vec4( 1.0-vec3(c, c, c) - vec3(0.025, 0.025, 0.02)*m*0.8, 1.0 );
    fragColor = result*d.y;
}


void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}