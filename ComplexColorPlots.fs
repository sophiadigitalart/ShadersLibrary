/*{
"CREDIT" : "ComplexColorPlots by allemangD",
"CATEGORIES" : [
"ci"
],
"DESCRIPTION": "https://www.shadertoy.com/view/lsycRW",
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

vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec2 comp(in float x) { return vec2(x, 0.); }
vec2 comp(in int x) { return vec2(float(x), 0.); }
vec2 comp(in vec2 x) { return x; }

vec2 cMul(in vec2 a, in vec2 b)
{
	return vec2(a.x*b.x - a.y*b.y, a.x*b.y + a.y*b.x);
}
vec2 cMul(in float a, in vec2 b)  { return cMul(comp(a), comp(b)); }
vec2 cMul(in int a, in vec2 b)    { return cMul(comp(a), comp(b)); }
vec2 cMul(in vec2 a, in float b)  { return cMul(comp(a), comp(b)); }
vec2 cMul(in vec2 a, in int b)    { return cMul(comp(a), comp(b)); }
vec2 cMul(in float a, in float b) { return cMul(comp(a), comp(b)); }

vec2 cInv(in vec2 b)
{
    return vec2(b.x, -b.y)/length(b)/length(b);
}
vec2 cInv(in float a)  { return cInv(comp(a)); }
vec2 cInv(in int a)    { return cInv(comp(a)); }

vec2 cDiv(in vec2 a, in vec2 b)
{
    return cMul(a, cInv(b));
}
vec2 cDiv(in float a, in vec2 b)  { return cDiv(comp(a), comp(b)); }
vec2 cDiv(in int a, in vec2 b)    { return cDiv(comp(a), comp(b)); }
vec2 cDiv(in vec2 a, in float b)  { return cDiv(comp(a), comp(b)); }
vec2 cDiv(in vec2 a, in int b)    { return cDiv(comp(a), comp(b)); }
vec2 cDiv(in float a, in float b) { return cDiv(comp(a), comp(b)); }

vec2 cPow(in vec2 b1, in vec2 c2)
{
    float l, a, r, t, x, y;
    
    l = log(length(b1));
    a = atan(b1.y, b1.x);
    
    r = exp(c2.x * l - c2.y * a);
    t = c2.x * a + c2.y * l;
    
    x = r * cos(t);
    y = r * sin(t);
    
    return vec2(x, y);
}
vec2 cPow(in float a, in vec2 b)  { return cPow(comp(a), comp(b)); }
vec2 cPow(in int a, in vec2 b)    { return cPow(comp(a), comp(b)); }
vec2 cPow(in vec2 a, in float b)  { return cPow(comp(a), comp(b)); }
vec2 cPow(in vec2 a, in int b)    { return cPow(comp(a), comp(b)); }
vec2 cPow(in float a, in float b) { return cPow(comp(a), comp(b)); }

vec2 cExp(in vec2 a) {
    return exp(a.x) * vec2(cos(a.y), sin(a.y));
}
vec2 cExp(in float a)  { return cExp(comp(a)); }
vec2 cExp(in int a)    { return cExp(comp(a)); }

vec2 cExpi(in vec2 a) {
    return exp(-a.y) * vec2(cos(a.x), sin(a.x));
}
vec2 cExpi(in float a)  { return cExpi(comp(a)); }
vec2 cExpi(in int a)    { return cExpi(comp(a)); }

vec2 cLog(in vec2 a) {
    return vec2(log(length(a)), atan(a.y, a.x));
}
vec2 cLog(in float a)  { return cLog(comp(a)); }
vec2 cLog(in int a)    { return cLog(comp(a)); }

vec2 cSin(in vec2 a) {
    return cDiv(cExpi(a)-cExpi(-a), vec2(0.,2.));
}
vec2 cSin(in float a)  { return cSin(comp(a)); }
vec2 cSin(in int a)    { return cSin(comp(a)); }

vec2 cCos(in vec2 a) {
    return (cExpi(a)+cExpi(-a))/2.;
}
vec2 cCos(in float a)  { return cCos(comp(a)); }
vec2 cCos(in int a)    { return cCos(comp(a)); }

vec2 cCon(in vec2 a) {
    return a * vec2(1, -1);
}
vec2 cCon(in float a)  { return comp(a); }
vec2 cCon(in int a)    { return comp(a); }

vec2 f(in vec2 z) {
    vec2 u = cMul(vec2(3, 0), cExpi(TIME/4.));
    vec2 v = cMul(vec2(1, 0), cExpi(TIME/2.));
    vec2 w = cMul(vec2(8, 0), cExpi(-TIME));
    
	vec2 a = cPow(z, 1) + u;
    vec2 b = cInv(cPow(z, 2) + v);
    vec2 c = cPow(z, 3) + w;
    
    return cMul(cMul(a, b), c);
}
void mainImage(out vec4 fragColor, in vec2 fragCoord)
{
    float SCALE = 8.0;
    
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = fragCoord.xy/RENDERSIZE.xy - vec2(.5);
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;
    uv *= SCALE;
    
    vec2 o = f(uv);
    float h = atan(o.y, o.x);
    vec3 col = hsv2rgb(vec3(h / 6.28, 1., 1.));
    
    float sa, sb;
    float w = 0.05;
    
    sa = log(length(o));
    sa = abs(sa - floor(sa));
    col *= mix(.8, 1.0, sa);
    
    sb = atan(o.y, o.x);
    sb = (sb + 3.1415)/6.2832 * 8.0;
    sb = abs(sb - floor(sb));
    
    if (sb < w || sb > 1. - w)
    	col = mix(col, vec3(1), smoothstep(.025, .0, abs(fract(sb + .5 + w) - .5 - w)));
    
    // Output to screen
    fragColor = vec4(col,1.0);
}
  void main(void) { mainImage(gl_FragColor, gl_FragCoord.xy); }