/*{
	"CREDIT" : "TriplanarDisplacementMapping by mla",
	"CATEGORIES" : [
		"ci"
	],
	"DESCRIPTION": "",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE" : "image"
		},
		{
			"NAME": "dorescale",
			"TYPE": "bool",
			"DEFAULT": true
		},
		{
			"NAME": "doadjust3d",
			"TYPE": "bool",
			"DEFAULT": true
		},
		{
			"NAME": "do3d",
			"TYPE": "bool",
			"DEFAULT": true
		},
		{
			"NAME": "dolengthdisplacement",
			"TYPE": "bool",
			"DEFAULT": false
		},		
        {
			"NAME": "doflipz",
			"TYPE": "bool",
			"DEFAULT": true
		},
        {
			"NAME": "doweightcorrection",
			"TYPE": "bool",
			"DEFAULT": true
		},
		{
			"NAME": "dovariabledisplacement",
			"TYPE": "bool",
			"DEFAULT": false
		},
		{
			"NAME": "justtexy",
			"TYPE": "bool",
			"DEFAULT": false
		},
		{
			"NAME": "tscale",
			"TYPE" : "float",
			"MIN" : 0.0,
			"MAX" : 1.0,
			"DEFAULT" : 0.5
		},
		{
			"NAME": "dscale",
			"TYPE" : "float",
			"MIN" : 0.0,
			"MAX" : 2.0,
			"DEFAULT" : 1.2
		},
		{
			"NAME": "iZoom",
			"TYPE" : "float",
			"MIN" : 0.5,
			"MAX" : 2.0,
			"DEFAULT" : 1.2
		},
		{
			"NAME": "iSteps",
			"TYPE" : "float",
			"MIN" : 2.0,
			"MAX" : 128.0,
			"DEFAULT" : 64.0
		},
		{
			"NAME": "iTimeMultiplier",
			"TYPE" : "float",
			"MIN" : 0.01,
			"MAX" : 10.0,
			"DEFAULT" : 1.0
		},
		{
			"NAME" :"iMouse",
			"TYPE" : "point2D",
			"DEFAULT" : [0.0, 0.0],
			"MAX" : [640.0, 480.0],
			"MIN" : [0.0, 0.0]
		},
		{
			"NAME": "iColor", 
			"TYPE" : "color", 
			"DEFAULT" : [
				0.9, 
				0.6, 
				0.0, 
				1.0
			]
		}
	],
}
*/
// https://www.shadertoy.com/view/3dlGWH

////////////////////////////////////////////////////////////////////////////////
//
// Created by Matthew Arcus, 2018.
//
// Triplanar texture displacement mapping. Raymarch a sphere with displacement
// texture computed using triplanar mapping. Choice of two different textures,
// use 't' to select. If you change the textures and are on Linux, you might
// need to change filtering in the texture settings to "linear".
//
// This make raymarching harder and we have to introduce some serious
// fudge factors when getting close to the surface (see marchRay).
//
// Borrows some framework code from slimyfrog's recent shaders, eg:
// https://www.shadertoy.com/view/WslGDH
//
// <mouse>,<up>,<down>: change view
// <left>,<right>: change texture mapping scale
// <pageup>,<pagedown>: change displacement scale
// 't': choose texture
// 'r': disable rotation
// for other keys, mostly for experimentation, see code.
//
////////////////////////////////////////////////////////////////////////////////

// Texture mapping parameters
int sampler = 0;
vec2 toffset = vec2(0); // texture offset
//float tscale = 0.1;     // texture coords scale
//float dscale = 1.1;     // displacement scale

// Lighting parameters
vec3 light = vec3(1,10,-4);
float ambient = 0.6;
float diffuse = 0.4;
float specular = 0.4;
float specularpow = 4.0;
vec3 specularcolor = vec3(1);

// Raymarching parameters
float MAX_DISTANCE = 8.0;
float MIN_DISTANCE = 0.01;
int MAXSTEPS = 500;
float limit = 2.5;
float slow = 0.1;

// Geometry

float PI = 3.14159;

vec2 rotate(vec2 p, float t) {
  return p * cos(t) + vec2(p.y, -p.x) * sin(t);
}

// Adapted from: https://www.shadertoy.com/view/MlXcRl but somewhat modified.
// Uses global tscale and toffset variables.
vec4 triplanar(vec3 n, bool adjust3d, bool rescale) {
  if (doflipz) n.z = -n.z;
  vec4 texx = IMG_NORM_PIXEL(inputImage, tscale*(0.5*n.yz+0.5+toffset));
  vec4 texy = IMG_PIXEL(inputImage, tscale*(0.5*n.zx+0.5+toffset));
  vec4 texz = IMG_NORM_PIXEL(inputImage, tscale*(0.5*n.xy+0.5+toffset));
  if (rescale) {
    // Without this, the texture is repeated in the upper
    // and lower hemispheres.
    texx = 2.0*texx - 1.0;
    texy = 2.0*texy - 1.0;
    texz = 2.0*texz - 1.0;
  }
  if (adjust3d) {
    // Mirror direction of displacement in opposite hemispheres.
    texx.x *= sign(n.x);
    texy.y *= sign(n.y);
    texz.z *= sign(n.z);
  }
  if (justtexy) return texy;
  vec3 weights = abs(n);
  if (doweightcorrection) weights /= dot(weights,vec3(1)); // Keep spherical!
  // Matrix multiplication as weighted sum of columns
  return mat4(texx,texy,texz,vec4(0))*vec4(weights,0);
}

float sphereDf(vec3 p, vec3 centre, float radius) {
  vec3 n = normalize(p-centre);
  vec3 tex = triplanar(n,doadjust3d,dorescale).xyz;
  float k = sin(0.25*PI*(TIME-9.5));//!dovariabledisplacement? 1.0: 
  if (do3d) {
    vec3 displacement = tex;
    displacement *= k*dscale;
    float dist = length(p-centre-displacement) - radius;
    return dist;
  } else {
    float displacement = dolengthdisplacement? length(tex): tex.x;
    displacement *= k*dscale;
    float dist = length(p-centre) - radius - displacement;
    return dist;
  }
}

float sceneDf(vec3 p) {
  // Sphere parameters
  vec3 centre = vec3(0);
  float radius = 1.0;
  return sphereDf(p,centre,radius);
}

vec3 calcNormal(vec3 p)
{
  float e = 0.01;
  vec3 normal = vec3(sceneDf(vec3(p.x+e,p.y,p.z)) - sceneDf(vec3(p.x-e,p.y,p.z)),
                     sceneDf(vec3(p.x,p.y+e,p.z)) - sceneDf(vec3(p.x,p.y-e,p.z)),
                     sceneDf(vec3(p.x,p.y,p.z+e)) - sceneDf(vec3(p.x,p.y,p.z-e)));
  return normalize(normal);
}

vec3 processLighting(vec3 baseColor, vec3 dir, vec3 surfacePoint) {
  vec3 normal = calcNormal(surfacePoint);
  
  vec3 color = baseColor*ambient;
  if (dot(light,normal) > 1e-4) {
    //if (!keypress(CHAR_D)) 
    color += baseColor*diffuse*dot(light,normal);
    //if (!keypress(CHAR_S)) {
      float s = pow(max(0.0,dot(reflect(light,normal),vec3(dir))),specularpow);
      color += specular*s*specularcolor;
    //}
  }
  return color;
}

bool marchRay(vec3 startPos, vec3 dir, out vec3 color) {
  vec3 p = startPos;
  bool checksteps = false;//keypress(CHAR_Q);
  for (int i = 0; i < MAXSTEPS; i++) {
    //assert(i < MAXSTEPS-1);
    //if (checksteps) assert(i < 200);
    if (length(p) > MAX_DISTANCE) return false;
    float dist = sceneDf(p);
    if (dist <= MIN_DISTANCE) break;
    // Proceed cautiously when "close" to surface
    if (dist > limit) dist = dist-limit+slow;
    else dist = slow*dist;
    p += dist*dir;
  }
  vec3 baseColor = vec3(0.8);
  //if (!keypress(CHAR_U)) 
  baseColor = triplanar(normalize(p),doadjust3d,dorescale).xyz;
  //if (keypress(CHAR_A)) {
    // Show axes
    //float d = min(abs(p.x),min(abs(p.y),abs(p.z)));
    //if (d < 0.02) baseColor = vec3(0.5,0,0);
  //}
  color = processLighting(baseColor,dir,p);
  return true;
}

vec3 transform(vec3 p) {
  if (iMouse.x > 0.0) {
    float theta = 2.0*(2.0*iMouse.y-RENDERSIZE.y)/RENDERSIZE.y*PI;
    float phi = 2.0*(2.0*iMouse.x-RENDERSIZE.x)/RENDERSIZE.x*PI;
    p.yz = rotate(p.yz,theta);
    p.zx = rotate(p.zx,-phi);
  }
  //if (!keypress(CHAR_R)) {
    //p.yz = rotate(p.yz,TIME * 0.125);
    //p.zx = rotate(p.zx,-TIME * 0.2);
  //}
  return p;
}


void main(void)
{
    
  toffset = 0.02*TIME*vec2(1,1.1);

    
  vec3 eye = vec3(0,0,-3);
  eye *= iZoom;
  vec2 uv = (2.0*gl_FragCoord.xy-RENDERSIZE.xy)/RENDERSIZE.y;
  vec3 dir = vec3(uv,2);
  light = normalize(light);
  eye = transform(eye);
  dir = transform(dir);
  dir = normalize(dir);
  light = transform(light);
    
  vec3 color;
  if (!marchRay(eye,dir,color)) {
    color = (1.0-gl_FragCoord.y/RENDERSIZE.y)*vec3(0.3,0,0.3);
  }
    
  //if (keypress(CHAR_G)) color = sqrt(color);
  //if (alert && keypress(CHAR_X)) color.x = 1.0;
  //if (gl_FragCoord.y < 20.0 && keypress(CHAR_K)) {
  //  int key = int(gl_FragCoord.x*26.0/RENDERSIZE.x);
  //  if (keypress(CHAR_A+key)) color = vec3(1,1,0);
  //}
  fragColor = vec4(color,1);
	
}
