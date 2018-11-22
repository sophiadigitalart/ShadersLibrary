/*{
	"DESCRIPTION": "https://www.shadertoy.com/view/4tKXWw",
	"CREDIT": "Dots On Planes by cacheflowe",
	"CATEGORIES": [
		"dots"
	],
	"INPUTS": [
	],
}*/

#define iResolution RENDERSIZE
void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	float time = TIME * 0.25;
    vec2 uv = (2. * fragCoord.xy - iResolution.xy) / iResolution.y;
    uv *= vec2(0.35, 1.);
    
    // z-rotation
    float zRot = 0.5 * sin(time);
    uv *= mat2(cos(zRot), sin(zRot), -sin(zRot), cos(zRot));
    
    // 3d params
    // 3d plane technique from: http://glslsandbox.com/e#37557.0 
    float horizon = 0.5 * cos(time); 
    float fov = 0.25 + 0.015 * sin(time); 
	float scaling = 0.1;
    
    // create a 2nd uv with warped perspective
	vec3 p = vec3(uv.x, fov, uv.y - horizon);      
	vec2 s = vec2(p.x/p.z, p.y/p.z) * scaling;
    
    // wobble the perspective-warped uv 
    float oscFreq = 12.;
    float oscAmp = 0.03;
    float zScroll = sin(time) * 0.1; // reverses direction between top & bottom
    s += vec2(zScroll, oscAmp * sin(time + s.x * oscFreq));
    
    // y-rotation
    float yRot = sin(time);
    s *= mat2(cos(yRot), sin(yRot), -sin(yRot), cos(yRot));
	
	// normal drawing here
    // draw dot grid
    float gridSize = 50. + 2. * sin(time);
    s = fract(s * gridSize) - 0.5;
    float col = 1. - smoothstep(0.25 + 0.1 * sin(time), 0.35 + 0.1 * sin(time), length(s));
    
  	// fade into distance
	col *= p.z * p.z * 5.0;
  
	fragColor = vec4(vec3(col),1.0);
}
void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}