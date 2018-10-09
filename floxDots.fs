/*{
	"CREDIT": "floxdots",
  "CATEGORIES" : [
    "dots",
    "circle"
  ],
  "DESCRIPTION" : "based on https:\/\/www.shadertoy.com\/view\/MtlGz8.",
  "INPUTS" : [
	{
		"NAME": 	"scale",
		"TYPE": 	"float",
		"DEFAULT": 	2.0,
		"MIN": 		0.5,
		"MAX": 		3.0
	}
  ]
}
*/

const float divs = 12.0;
    
void main(void)
{
    vec2 div = vec2( divs*scale, divs*RENDERSIZE.y/RENDERSIZE.x );
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    uv -= 0.5;                                  // center on screen
    float b = 4.0*divs/RENDERSIZE.x;           // blur over 2.4 pixels
    vec2 xy = div*uv;
    
    vec2 S;
    S.x = (xy.x + xy.y)*(xy.x - xy.y)*0.5;      // "velocity potential"
    S.y = xy.x*xy.y;                            // stream function
    S.x -= TIME*3.0;                     // animate stream
    
    vec2 sxy = sin(3.14159*S);
    float a = sxy.x * sxy.y;                    // combine sine waves using product
    
    a = 0.5*a + 0.5;                            // remap to [0..1]
    a = smoothstep( 0.85-b, 0.85+b, a );        // threshold
    
    float c = sqrt( a );                        // correct for gamma
    gl_FragColor = vec4(c, c, c, 1.0);
}