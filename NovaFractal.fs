/*{
"CREDIT" : "NovaFractal by gleurop",
"CATEGORIES" : [
"ci"
],
"DESCRIPTION": "https://www.shadertoy.com/view/Xsl3DM",
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
"DEFAULT" : 1.0},
 {"NAME" :"iMouse",
"TYPE" : "point2D",
"DEFAULT" : [0.0, 0.0],
"MAX" : [640.0, 480.0],
"MIN" : [0.0, 0.0]},
{
"NAME": "iColor", 
"TYPE" : "color", 
"DEFAULT" : [
0.7, 
0.5, 
0.0, 
1.0
]}
],
}
*/

void mainImage( out vec4 fragColor, in vec2 fragCoord ) {
	vec2 p = fragCoord.xy / RENDERSIZE.xy * 2.0 - 1.0;
	p.x *= RENDERSIZE.x / RENDERSIZE.y;
	vec2 c = p;
	float iter = 0.0;
	vec4 color = vec4(0.0);
	for (int i = 0; i < 40; i++) {
		//vec4 g = texture2D(iChannel0, c);
		//color += g;
		float phi = atan(c.y, c.x) + TIME*0.01*iter;
		float r = dot(c,c);
		if (r < 16.0) {
    		c.x = ((cos(2.0*phi))/r) + p.x;
    		c.y = (-sin(2.0*phi)/r) + p.y;
		
    		iter++;
		}
	}
	fragColor = vec4(color / 40.0 + max(0.75 - iter / 40.0, 0.0));
}void main(void) { mainImage(gl_FragColor, gl_FragCoord.xy); }