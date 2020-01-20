/*{
	"CREDIT" : "silver maze by flockaroo",
	"CATEGORIES" : [
		"ci"
	],
	"DESCRIPTION": "",
	"INPUTS": [
        {
			"NAME": "iChannel0",
			"TYPE" : "image"
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

// https://www.shadertoy.com/view/lssfzX
// created by florian berger (flockaroo) - 2017
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

// silver maze

// some more non periodic tilings experiments
vec2 tr_i(vec2 p)
{
    return (p*vec2(1,.5*sqrt(3.))+vec2(.5*p.y,0));
}

vec2 tr(vec2 p)
{
    return (p-vec2(p.y/sqrt(3.),0))/vec2(1,.5*sqrt(3.));
}

void getTri(vec2 p, inout vec2 p1, inout vec2 p2, inout vec2 p3, float size)
{
    vec2 pt=tr(p)/size;
    vec2 pf=floor(pt);
    vec2 pc=ceil(pt);
    p1=vec2(pf.x,pc.y);
    p2=vec2(pc.x,pf.y);
    p3=pc;
    if(dot(pt-pf,vec2(1))<1.) p3=pf;
    p1=tr_i(p1)*size;
    p2=tr_i(p2)*size;
    p3=tr_i(p3)*size;
}

float tri01(float x)
{
    return abs(fract(x)-.5)*2.;
}

vec4 getRand(vec2 p)
{
    vec2 texc=(floor(p)+.5)/RENDERSIZE.xy;
    return texture(iChannel0,texc);
}

// get some 3d rand values by multiplying 2d rand in xy, yz, zx plane
vec4 getRand(vec3 pos)
{
    vec4 r = vec4(1.0);
    r*=texture(iChannel0,pos.xy)*2.-1.;
    r*=texture(iChannel0,pos.xz)*2.-1.;
    r*=texture(iChannel0,pos.zy)*2.-1.;
    return r;
}

#define PI 3.14159265

// 2d distance field of pattern
float dist(vec2 p, float period, float size)
{
    vec2 p1,p2,p3;
    getTri(p,p1,p2,p3,size);
    vec4 rnd=getRand((p1+p2+p3)/3./size*2.);
	float r=rnd.x;
	float r2=rnd.y;
	float r3=rnd.z;
    if(fract(r*2.)>.3333) { vec2 d=p3; p3=p2; p2=p1; p1=d; }
    if(fract(r*2.)>.6666) { vec2 d=p3; p3=p2; p2=p1; p1=d; }
    float d = 10000.;
    float ang;
    ang = acos(dot(normalize(p-p1),normalize(p3-p1)));
    d = min(d,length(p-p1)+1.0*(floor(r2*2.)*2.-1.)*period*ang/PI*3.);
    ang = acos(dot(normalize(p-p2),normalize(p3-p2)));
    d = min(d,length(p-p2)+1.0*(floor(r3*2.)*2.-1.)*period*ang/PI*3.);
    float arg=(d-.5*size)/period;
    return tri01(arg)*.5*period;
}

// final distance funtion
float dist(vec3 pos)
{
    pos-=.00012*getRand(pos*3.0*.7).xyz;
    pos-=.00030*getRand(pos*1.3*.7).xyz;
    pos-=.00080*getRand(pos*0.5*.7).xyz;
    vec3 p1,p2,p3;
    float d = 10000.;
    
    // plane
	d=min(d,pos.z+.1);
    
    float d2d=dist(pos.xy,.16,.8);
    d=min(d,sqrt(d2d*d2d+pos.z*pos.z)-.025);
    
    return d;
}

vec3 getGrad(vec3 pos, float eps)
{
    vec2 d=vec2(eps,0);
    float d0=dist(pos);
    return vec3(dist(pos+d.xyy)-d0,
                dist(pos+d.yxy)-d0,
                dist(pos+d.yyx)-d0)/eps;
                
}

// march it...
vec4 march(inout vec3 pos, vec3 dir)
{
    // cull the sphere
    //if(length(pos-dir*dot(dir,pos))>1.05) 
    //	return vec4(0,0,0,1);
    
    float eps=0.001;
    float bg=1.0;
    for(int cnt=0;cnt<132;cnt++)
    {
        float d = dist(pos);
        pos+=d*dir;
        if(d<eps) { bg=0.0; break; }
    }
    vec3 n = getGrad(pos,.001);
    return vec4(n,bg); // .w=1 => background
}

void main(void)
{
    // screen coord -1..1
    float aspect=RENDERSIZE.y/RENDERSIZE.x;
    vec2 sc = (gl_FragCoord.xy/RENDERSIZE.xy)*2.-1.;
    // viewer position
    float phi = TIME*.08;
    vec3 pos = 17.*vec3(cos(phi),sin(phi),.3);
    // pixel view direction--
    vec3 fwd   = normalize(-pos.xyz*vec3(1,1,0)+pos.yxz*vec3(-1,1,0)*.15*sin(TIME*.3)+vec3(1,1,-17));
    vec3 right = normalize(fwd.yxz*vec3(1,-1,0));
    vec3 up    = normalize(cross(right,fwd));
    vec3 dir = normalize(fwd*2.6+sc.x*right+sc.y*up*aspect);
    // rotate view around x,z
    
    // march it...
   	vec4 n=march(pos,dir);
    float bg=n.w;
        
    // calc some ambient occlusion
    float ao=1.;
    #if 0
    // calc simple ao by stepping along radius
    ao*=dist(pos*1.02)/.02;
    ao*=dist(pos*1.05)/.05;
    ao*=dist(pos*1.1)/.1;
    #else
    // calc ao by stepping along normal
    ao*=dist(pos+n.xyz*.02)/.02;
    ao*=dist(pos+n.xyz*.05)/.05;
    ao*=dist(pos+n.xyz*.10)/.10;
    #endif
    // adjust contrast of ao
    ao=pow(ao,.4);
    
    // reflection dir
    vec3 R = dir-2.0*dot(dir,n.xyz)*n.xyz;
    R = R.yzx;
    
    vec3 c = vec3(1);
    // simply add some parts of the normal to the color
    // gives impression of 3 lights from different dir with different color temperature
    c += n.xyz*.1+.1;

    //  reflection of cubemap
    //c *= texture(iChannel1,R).xyz*.7+.4;
    
    // add some depth darkening
	//c*=clamp(-dot(dir,pos)*.7+.7, .2, 1.);
    
    // apply ambient occlusion
    c*=ao;
    
    // apply background
    if(bg>=.5) c=vec3(.0,.0,.75)-.17;
    
    // vignetting
    float vign = (1.1-.35*dot(sc.xy,sc.xy));
    
	gl_FragColor = vec4(c*vign,1);
}
