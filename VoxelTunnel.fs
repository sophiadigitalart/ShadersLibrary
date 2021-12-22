/*{
	"CREDIT" : "Voxel Tunnel by lsdlive",
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
	]
}
*/
// https://www.shadertoy.com/view/MscBRs
// @lsdlive

// This was my shader for the shader showdown at Outline demoparty 2018 in Nederland.
// Shader showdown is a live-coding competition where two participants are
// facing each other during 25 minutes.
// (Round 1)

// I don't have access to the code I typed at the event, so it might be
// slightly different.

// Original algorithm on shadertoy from fb39ca4: https://www.shadertoy.com/view/4dX3zl
// I used the implementation from shane: https://www.shadertoy.com/view/MdVSDh

// Thanks to shadertoy community & shader showdown paris.

// This is under CC-BY-NC-SA (shadertoy default licence)


mat2 r2d(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, s, -s, c);
}

vec2 path(float t) {
    float a = sin(t*.2 + 1.5), b = sin(t*.2);
    return vec2(2.*a, a*b);
}

float g = 0.;
float de(vec3 p) {
    p.xy -= path(p.z);

    float d = -length(p.xy) + 4.;// tunnel (inverted cylinder)

    p.xy += vec2(cos(p.z + TIME)*sin(TIME), cos(p.z + TIME));
    p.z -= 6. + TIME * 6.;
    d = min(d, dot(p, normalize(sign(p))) - 1.); // octahedron (LJ's formula)
    // I added this in the last 1-2 minutes, but I'm not sure if I like it actually!

    // Trick inspired by balkhan's shadertoys.
    // Usually, in raymarch shaders it gives a glow effect,
    // here, it gives a colors patchwork & transparent voxels effects.
    g += .015 / (.01 + d * d);
    return d;
}

void main(void)
{
    vec2 uv = iZoom * gl_FragCoord.xy / RENDERSIZE.xy; 
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float dt = TIME * 6.;
    vec3 ro = vec3(0, 0, -5. + dt);
    vec3 ta = vec3(0, 0, dt);

    ro.xy += path(ro.z);
    ta.xy += path(ta.z);

    vec3 fwd = normalize(ta - ro);
    vec3 right = cross(fwd, vec3(0, 1, 0));
    vec3 up = cross(right, fwd);
    vec3 rd = normalize(fwd + uv.x*right + uv.y*up);

    rd.xy *= r2d(sin(-ro.x / 3.14)*.3);

    // Raycast in 3d to get voxels.
    // Algorithm fully explained here in 2D (just look at dde algo):
    // http://lodev.org/cgtutor/raycasting.html
    // Basically, tracing a ray in a 3d grid space, and looking for 
    // each voxel (think pixel with a third dimension) traversed by the ray.
    vec3 p = floor(ro) + .5;
    vec3 mask;
    vec3 drd = 1. / abs(rd);
    rd = sign(rd);
    vec3 side = drd * (rd * (p - ro) + .5);

    float t = 0., ri = 0.;
    for (float i = 0.; i < 1.; i += .01) {
        ri = i;

        /*
        // sphere tracing algorithm (for comparison)
        p = ro + rd * t;
        float d = de(p);
        if(d<.001) break;
        t += d;
        */

        if (de(p) < 0.) break;// distance field
                              // we test if we are inside the surface

        mask = step(side, side.yzx) * step(side, side.zxy);
        // minimum value between x,y,z, output 0 or 1

        side += drd * mask;
        p += rd * mask;
    }
    t = length(p - ro);

    vec3 c = vec3(1) * length(mask * vec3(1., .5, .75));
    c = mix(vec3(.2, .2, .7), vec3(.2, .1, .2), c);
    c += g * .4;
    c.r += sin(TIME)*.2 + sin(p.z*.5 - TIME * 6.);// red rings
    c = mix(c, vec3(.2, .1, .2), 1. - exp(-.001*t*t));// fog

    gl_FragColor = vec4(c, 1.0);
}
