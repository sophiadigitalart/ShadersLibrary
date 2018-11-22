/*{
	"DESCRIPTION": "https://www.shadertoy.com/view/llycWd",
	"CREDIT": "shaping functions by Simplyfire",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
	],
}*/

#define iResolution RENDERSIZE
#define iTime TIME

#define pi 3.14159265359

uniform vec2 u_resolution;
uniform float u_time;

vec3 hsb2rgb( in vec3 c){
 vec3 rgb = clamp(abs(mod(c.x*6.0+vec3(0.0,4.0,2.0), 6.0)-3.0)-1.0, 0.0, 1.0 );
 rgb = rgb*rgb*(3.0-2.0*rgb);  return c.z * mix(vec3(1.0), rgb, c.y);
}

vec3 rect(vec2 uv, vec2 c, vec2 s, vec2 off){
  float p = max(smoothstep(c.x+s.x,c.x+s.x+off.x, uv.x),
                smoothstep(c.y+s.y,c.y+s.y+off.y,uv.y));
  float q = max(smoothstep(c.x-s.x,c.x-s.x-off.x, uv.x),
                smoothstep(c.y-s.y,c.y-s.y-off.y,uv.y));
  return vec3(1.-max(p,q));
}

float map(float x, float a1, float a2, float b1, float b2){
  return b1 + (b2-b1) * (x-a1) / (a2-a1);
}

vec3 ellipse(vec2 uv, vec2 c, float r){
  float d = distance(uv,c);
  return vec3(1.-smoothstep(r, r+0.08, d));
}

vec3 shape(vec2 st, int N, float scl, float smth, float rot){
  // Remap the space to -1. to 1.
  st = st *2.-1.;
  // Angle and radius from the current pixel
  float a = atan(st.x,st.y)+pi+u_time*rot;
  float r = pi*2./float(N);
  // Shaping function that modulate the distance
  float d = cos(floor(.5+a/r)*r-a)*length(st*2.)/scl;
  return vec3(1.0-smoothstep(r,r+smth,d));
}

float maxrect(vec2 uv, vec2 c){
	return max(abs(c.x-uv.x), abs(c.y-uv.y));
}

float minrect(vec2 uv, vec2 c){
	return min(abs(c.x-uv.x), abs(c.y-uv.y));
}

float dist(vec2 uv, vec2 c){
	return distance(uv,c);
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
   float t = TIME;
   vec2 uv = fragCoord.xy / iResolution.xy;
   vec2 c = vec2(.5);
   float d0 = dist(uv, c);
   float d1 = maxrect(uv, c);
   float d2 = minrect(uv, c);
   vec3 color = vec3(0.,0.,0.);
   float v = .5+.5*sin(d2/d1/d0*.8+(t*1.5));
   color.r = map(d0/2.+t/30., 0., 0.5, .0, 1.);
   color.g = 1.-d0*.5;
   color.b = .6-d0*2.+v;

   fragColor = vec4(hsb2rgb(color),1.);
 }

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}