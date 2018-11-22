/*{
	"CREDIT" : "Copy of GhostCube by ianmethyst",
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
		"NAME": "iChannel0",
		"TYPE" : "image"
		},
		{
			"NAME": "iZoom",
			"TYPE" : "float",
			"MIN" : 0.0,
			"MAX" : 1.0,
			"DEFAULT" : 1.0
		},
		{
			"NAME": "iSteps",
			"TYPE" : "float",
			"MIN" : 2.0,
			"MAX" : 75.0,
			"DEFAULT" : 19.0
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
// https://www.shadertoy.com/view/4tyBWm
float sdBox( vec3 p, vec3 b )
{
  vec3 d = abs(p) - b;
  return min(max(d.x,max(d.y,d.z)),0.0) +
         length(max(d,0.0));
}

float map(vec3 p)
{
    float t = TIME;
    p.xz *= mat2(cos(t), sin(t), -sin(t), cos(t));
    p.xy *= mat2(cos(t), sin(t), -sin(t), cos(t));
    p.yz *= mat2(cos(t), sin(t), -sin(t), cos(t));
    
    float k = sdBox(p, vec3(1.0));
    float o = 0.85;
	k = max(k, -sdBox(p, vec3(2.0, o, o)));
    k = max(k, -sdBox(p, vec3(o, 2.0, o)));
    k = max(k, -sdBox(p, vec3(o, o, 2.0)));
    return k;
}

float trace(vec3 o, vec3 r)
{
 	float t = 0.0;
    for (int i = 0; i < 32; ++i) {
        vec3 p = o + r * t;
        float d = map(p) * 0.9;
        t += d;
    }
    return t;
}

void main(void)
{  
    vec2 uv = iZoom * gl_FragCoord.xy / RENDERSIZE.xy;
    
    vec4 old = texture(iChannel0, uv);
    vec4 old2 = texture(iChannel0, uv);
    
    uv = uv * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    
    vec3 o = vec3(0.0, 0.0, -2.5);
    vec3 r = vec3(uv, 0.8);
    
    float t = trace(o, r);
    
    vec3 fog = vec3(1.0) / (1.0 + t * t * 0.1) * 0.1;
    
    float c = TIME * 5.0 + uv.x;
    fog *= vec3(sin(c)*cos(c*2.0), cos(c)*cos(c*2.0), sin(c)) * 0.5 + 0.5;
    
    fog += old.xyz * 0.6 + old2.xyz * 0.37;
    
	fragColor = vec4(fog, 1.0);
}
