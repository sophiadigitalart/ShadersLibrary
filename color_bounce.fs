/*
{
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "INPUTS": [
    
  ]
}
*/


#ifdef GL_ES
precision mediump float;
#endif


void main( void ) {

	vec2 position = ( gl_FragCoord.xy / RENDERSIZE.x );

	float color = 1.0-pow(length(position-vec2(0.5,0.25)),cos(TIME)*0.5+0.5);
	gl_FragColor = vec4( vec3( color,cos(color*20.0)*0.5+0.5,tan(color*50.0)), 1.0 );

}