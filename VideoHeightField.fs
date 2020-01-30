/*{
	"CREDIT" : "Video Heightfield by simesgreen",
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
			"NAME": "iZoom",
			"TYPE" : "float",
			"MIN" : 0.1,
			"MAX" : 2.0,
			"DEFAULT" : 1.0
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
// https://www.shadertoy.com/view/Xss3zr
// @simesgreen

const int _Steps = 64;
const vec3 lightDir = vec3(0.577, 0.577, 0.577);

// transforms
vec3 rotateX(vec3 p, float a)
{
    float sa = sin(a);
    float ca = cos(a);
    vec3 r;
    r.x = p.x;
    r.y = ca*p.y - sa*p.z;
    r.z = sa*p.y + ca*p.z;
    return r;
}

vec3 rotateY(vec3 p, float a)
{
    float sa = sin(a);
    float ca = cos(a);
    vec3 r;
    r.x = ca*p.x + sa*p.z;
    r.y = p.y;
    r.z = -sa*p.x + ca*p.z;
    return r;
}

bool
intersectBox(vec3 ro, vec3 rd, vec3 boxmin, vec3 boxmax, out float tnear, out float tfar)
{
	// compute intersection of ray with all six bbox planes
	vec3 invR = 1.0 / rd;
	vec3 tbot = invR * (boxmin - ro);
	vec3 ttop = invR * (boxmax - ro);
	// re-order intersections to find smallest and largest on each axis
	vec3 tmin = min (ttop, tbot);
	vec3 tmax = max (ttop, tbot);
	// find the largest tmin and the smallest tmax
	vec2 t0 = max (tmin.xx, tmin.yz);
	tnear = max (t0.x, t0.y);
	t0 = min (tmax.xx, tmax.yz);
	tfar = min (t0.x, t0.y);
	// check for hit
	bool hit;
	if ((tnear > tfar)) 
		hit = false;
	else
		hit = true;
	return hit;
}

/*
float luminance(sampler2D tex, vec2 uv)
{
	//BL vec3 c = textureLod(tex, uv, 0.0).xyz;
	vec3 c = IMG_NORM_PIXEL(inputImage, uv).xyz;	
	//vec3 c = vec3(1.0);	
    return dot(c, vec3(0.33, 0.33, 0.33));
}*/

vec2 worldToTex(vec3 p)
{
	vec2 uv = p.xz*0.5+0.5;
	uv.y = 1.0 - uv.y;
	return uv;
}

float heightField(vec3 p)
{
	//return sin(p.x*4.0)*sin(p.z*4.0);
	//return luminance(0, p.xz*0.5+0.5)*2.0-1.0;
	//return luminance(inputImage, worldToTex(p))*0.5;
	return IMG_NORM_PIXEL(inputImage,  worldToTex(p)).x;
}

bool traceHeightField(vec3 ro, vec3 rayStep, out vec3 hitPos)
{
	vec3 p = ro;
	bool hit = false;
	float pH = 0.0;
	vec3 pP = p;
	for(int i=0; i<_Steps; i++) {
		float h = heightField(p);
		if ((p.y < h) && !hit) {
			hit = true;
			//hitPos = p;
			// interpolate based on height
            hitPos = mix(pP, p, (pH - pP.y) / ((p.y - pP.y) - (h - pH)));
		}
		pH = h;
		pP = p;
		p += rayStep;
	}
	return hit;
}

vec3 background(vec3 rd)
{
     return mix(vec3(1.0, 0.0, 1.0), iColor.rgb, abs(rd.y));
}

void main(void)
{
    vec2 pixel = iZoom *((gl_FragCoord.xy / RENDERSIZE.xy) * 2.0-1.0);

    // compute ray origin and direction
    float asp = RENDERSIZE.x / RENDERSIZE.y;
    vec3 rd = normalize(vec3(asp*pixel.x, pixel.y, -2.0));
    vec3 ro = vec3(0.0, 0.0, 2.0);
		
	vec2 mouse = iMouse.xy / RENDERSIZE.xy;

	// rotate view
    float ax = -0.7;
	if (iMouse.x > 0.0) {
    	ax = -(1.0 - mouse.y)*2.0 - 1.0;
	}
    rd = rotateX(rd, ax);
    ro = rotateX(ro, ax);
		
	//float ay = sin(TIME*0.00002);
    //rd = rotateY(rd, ay);
    //ro = rotateY(ro, ay);
	
	// intersect with bounding box
    bool hit;	
	const vec3 boxMin = vec3(-1.0, -0.01, -1.0);
	const vec3 boxMax = vec3(1.0, 0.5, 1.0);
	float tnear, tfar;
	hit = intersectBox(ro, rd, boxMin, boxMax, tnear, tfar);

	tnear -= 0.0001;
	vec3 pnear = ro + rd*tnear;
    vec3 pfar = ro + rd*tfar;
	
    float stepSize = length(pfar - pnear) / float(_Steps);
	
    vec3 rgb = background(rd);
    if(hit)
    {
    	// intersect with heightfield
		ro = pnear;
		vec3 hitPos;
		hit = traceHeightField(ro, rd*stepSize, hitPos);
		if (hit) {
			//rgb = hitPos*0.5+0.5;
			vec2 uv = worldToTex(hitPos);
			//rgb = texture(iChannel0, uv).xyz;
            rgb = IMG_NORM_PIXEL(inputImage, uv).xyz;
			

			// shadows
			hitPos += vec3(0.0, 0.01, 0.0);
			bool shadow = traceHeightField(hitPos, lightDir*0.01, hitPos);
			if (shadow) {
				rgb *= 0.75;
			}
			
		}
     }

    fragColor=vec4(rgb, 1.0);
	
}

