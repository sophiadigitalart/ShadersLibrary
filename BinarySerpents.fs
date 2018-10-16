/*{
"CREDIT" : "BinarySerpents by Trisomie21",
"CATEGORIES" : [
"ci"
],
"DESCRIPTION": "https://www.shadertoy.com/view/MslGRH",
 "INPUTS": [
   {
			"NAME": "iChannel0",
			"TYPE": "image"
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
0.0, 
1.0, 
0.0, 
1.0
]}
],
}
*/
// https://www.shadertoy.com/view/MslGRH

// With tweaks by PauloFalcao

float BinarySerpentsTexture3D(vec3 n, float res){
  n = floor(n*res+.5);
  return fract(sin((n.x+n.y*1e5+n.z*1e7)*1e-4)*1e5);
}

float BinarySerpentsmap( vec3 p ){
    p.x+=sin(p.z*4.0+TIME*4.0)*0.1*cos(TIME*0.1);
    p = mod(p,vec3(1.0, 1.0, 1.0))-0.5;
    return length(p.xy)-.1;
}
void mainImage(out vec4 fragColor, in vec2 fragCoord)
{
   vec2 uv = iZoom * fragCoord.xy/RENDERSIZE.xy * 2.0 - 0.5;
  	 vec3 camPos = vec3(cos(TIME*0.3), sin(TIME*0.3), 1.5);
    vec3 camTarget = vec3(0.0, 0.0, 0.0);

    vec3 camDir = normalize(camTarget-camPos);
    vec3 camUp  = normalize(vec3(0.0, 1.0, 0.0));
    vec3 camSide = cross(camDir, camUp);
    float focus = 2.0;

    vec3 rayDir = normalize(camSide*uv.x + camUp*uv.y + camDir*focus);
    vec3 ray = camPos;
    float d = 0.0, total_d = 0.0;
    const int MAX_MARCH = 100;
    const float MAX_DISTANCE = 5.0;
    float c = 1.0;
    for(int i=0; i<MAX_MARCH; ++i) {
        d = BinarySerpentsmap(ray);
        total_d += d;
        ray += rayDir * d;
        if(abs(d)<0.001) { break; }
        if(total_d>MAX_DISTANCE) { c = 0.; total_d=MAX_DISTANCE; break; }
    }
	
    float fog = 3.1;
    vec3 result = vec3( vec3(iColor.r, iColor.g, iColor.b) * (fog - total_d) / fog );

    ray.z -= 5.+TIME*.5;
    float r = BinarySerpentsTexture3D(ray, 33.);

  fragColor = vec4(result*(step(r,.3)+r*.2+.1),1.0);
}


void main(void) { mainImage(gl_FragColor, gl_FragCoord.xy); }