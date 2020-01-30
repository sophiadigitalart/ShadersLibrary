/*{
	"CREDIT" : "Galactic Corridors 2 by vox",
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
			"MAX" : 75.0,
			"DEFAULT" : 19.0
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
// https://www.shadertoy.com/view/Xtc3Rn
//-----------------CONSTANTS MACROS-----------------

#define PI 3.14159265359
#define E 2.7182818284
#define GR 1.61803398875

//-----------------UTILITY MACROS-----------------

#define localTime (sin(((sin(float(__LINE__)*100.0)*GR/PI+GR/PI/E)*TIME * iTimeMultiplier+100.0)/100.0)*100.0)
#define saw(x) (acos(cos(x))/PI)
#define sphereN(uv) (clamp(1.0-length(uv*2.0-1.0), 0.0, 1.0))
#define clip(x) (smoothstep(0.5-GR/PI/E, .5+GR/PI/E, x))
#define zero(x) (smoothstep(-1.0/PI/E/GR, 1.0/PI/E/GR, sin(x*PI/2.0))*2.0-1.0)
#define TIMES_DETAILED (1.0)
#define angle(uv) (atan((uv).y, (uv).x))
#define angle_percent(uv) ((angle(uv)/PI+1.0)/2.0)
#define absMin(x,y) (abs(x) < abs(y) ? x: y)
#define quadrant(uv) (absMin((zero(uv).x), (zero(uv).y))+floor(uv.x/2.0)+floor(uv.y/2.0))

#define flux(x) (vec3(cos(x),cos(4.0*PI/3.0+x),cos(2.0*PI/3.0+x))*.5+.5)
#define rotatePoint(p,n,theta) (p*cos(theta)+cross(n,p)*sin(theta)+n*dot(p,n) *(1.0-cos(theta)))
#define GUASS(x) (smoothstep(0.0, 1.0/GR/PI/E, saw(x*PI/2.0)*(1.0-saw(x*PI/2.0))))

#define GRID_COUNT (50.0)
#define hash(p) (fract(sin(vec2( dot(p,vec2(127.5,313.7)),dot(p,vec2(239.5,185.3))))*43458.3453))

#define MAX_DIM (max(RENDERSIZE.x, RENDERSIZE.y))



vec3 powa(vec3 x, float y){ return vec3(pow(x.x, y), pow(x.y, y), pow(x.z, y)); }

float seedling = 0.0;

vec2 spiral(vec2 uv)
{
    float turns = 5.0;
    float r = pow(log(length(uv)+1.), 1.175);
    float theta = atan(uv.y, uv.x)*turns-r*PI;
    return vec2(saw(r*PI+TIME), saw(theta+TIME*1.1));
}

vec2 cmul(vec2 v1, vec2 v2) {
    return vec2(v1.x * v2.x - v1.y * v2.y, v1.y * v2.x + v1.x * v2.y);
}

vec2 cdiv(vec2 v1, vec2 v2) {
    return vec2(v1.x * v2.x + v1.y * v2.y, v1.y * v2.x - v1.x * v2.y) / dot(v2, v2);
}

vec2 mobius(vec2 uv, vec2 multa, vec2 offa, vec2 multb, vec2 offb)
{
    return saw(cdiv(cmul(uv, multa) + offa, cmul(uv, multb) + offb)*PI)*2.0-1.0;
}

vec2 square_map(vec2 uv)
{
    return (rotatePoint(vec3(uv+vec2(cos(seedling*PI), cos(seedling*GR)), 0.0), vec3(0.0, 0.0, 1.0), localTime/PI).xy*(1.0+sin(localTime+seedling)/PI/E/GR)
            +vec2(cos(localTime+seedling)+sin(localTime+seedling)));
}

vec2 iterate_square(vec2 uv, vec2 dxdy, out float magnification)
{
    vec2 a = uv+vec2(0.0,       0.0);
    vec2 b = uv+vec2(dxdy.x,    0.0);
    vec2 c = uv+vec2(dxdy.x,    dxdy.y);
    vec2 d = uv+vec2(0.0,       dxdy.y);//((fragCoord.xy + vec2(0.0, 1.0)) / RENDERSIZE.xy * 2.0 - 1.0) * aspect;

    vec2 ma = square_map(a);
    vec2 mb = square_map(b);
    vec2 mc = square_map(c);
    vec2 md = square_map(d);
    
    float da = length(mb-ma);
    float db = length(mc-mb);
    float dc = length(md-mc);
    float dd = length(ma-md);
    
    float stretch = max(max(max(da/dxdy.x,db/dxdy.y),dc/dxdy.x),dd/dxdy.y);
    
    magnification = stretch;
    
    return square_map(uv);
}
vec2 mobius_map(vec2 uv, vec2 multa, vec2 offa, vec2 multb, vec2 offb)
{
    return mobius(uv, multa, offa, multb, offb);
}

vec2 iterate_mobius(vec2 uv, vec2 dxdy, out float magnification, vec2 multa, vec2 offa, vec2 multb, vec2 offb)
{
    vec2 a = uv+vec2(0.0,       0.0);
    vec2 b = uv+vec2(dxdy.x,    0.0);
    vec2 c = uv+vec2(dxdy.x,    dxdy.y);
    vec2 d = uv+vec2(0.0,       dxdy.y);//((fragCoord.xy + vec2(0.0, 1.0)) / RENDERSIZE.xy * 2.0 - 1.0) * aspect;

    vec2 ma = mobius_map(a, multa, offa, multb, offb);
    vec2 mb = mobius_map(b, multa, offa, multb, offb);
    vec2 mc = mobius_map(c, multa, offa, multb, offb);
    vec2 md = mobius_map(d, multa, offa, multb, offb);
    
    float da = length(mb-ma);
    float db = length(mc-mb);
    float dc = length(md-mc);
    float dd = length(ma-md);
    
    float stretch = max(max(max(da/dxdy.x,db/dxdy.y),dc/dxdy.x),dd/dxdy.y);
    
    magnification = stretch;
    
    return mobius_map(uv, multa, offa, multb, offb);
}
vec3 phase(float map)
{
    return vec3(saw(map),
                saw(4.0*PI/3.0+map),
                saw(2.0*PI/3.0+map));
}

float lowAverage()
{
    const int iters = 32;
    float product = 1.0;
    float sum = 0.0;
    
    float smallest = 0.0;
    
    for(int i = 0; i < iters; i++)
    {
        //float sound = texture2D(iChannel0, vec2(float(i)/float(iters), 0.5)).r;
        float sound = IMG_NORM_PIXEL(inputImage, vec2(float(i)/float(iters), 0.5)).r;
        smallest = 
        
        product *= sound;
        sum += sound;
    }
    return max(sum/float(iters), pow(product, 1.0/float(iters)));
}

float last_height = 0.0;
vec3 hash3( vec2 p)

{

    vec3 q = vec3( dot(p,vec2(123.4,234.5)),

    dot(p,vec2(456.7,321.0)),

    dot(p,vec2(432.1,543.2)) );

    return fract(sin(q)*12345.678);

}

vec4 galaxy(vec2 uv)
{
    uv *= 5.0;
      
    float r1 = log(length(uv)+1.)*2.0;
    float r2 = pow(log(length(uv)+1.)*3.0, .5);
    
    float rotation = localTime;
    
    float theta1 = atan(uv.y, uv.x)-r1*PI+rotation*.5+seedling;
    float theta2 = atan(uv.y, uv.x)-r2*PI+rotation*.5+seedling;
    
    vec4 color = vec4(flux(localTime+seedling), 1.0);
    
    vec4 final = acos(1.0-(cos(theta1)*cos(theta1)+sqrt(cos(theta1+PI)*cos(theta1+PI)))/2.0)*(1.0-log(r1+1.))*color
              + cos(1.0-(cos(theta2)*cos(theta2)+cos(theta2+PI/2.)*cos(theta2+PI/2.))/2.0)*(1.25-log(r2+1.))*color;
         
    final.rgba += color;
    
    final /= r1;
    
    final *= 2.0;
    
    float weight = clamp(length(clamp(final.rgb, 0.0, 1.0)), 0.0, 1.0);
    return final;
}
void main(void)
{  		
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy * iZoom;
    
	//vec4 sample = texture2D(iChannel1, uv);
    vec4 sample = IMG_NORM_PIXEL(inputImage, uv);
    float scale = exp(sin(localTime))*E+GR;
    uv = uv*scale-scale/2.0;
    uv.x *= RENDERSIZE.x/RENDERSIZE.y;
    uv = rotatePoint(vec3(uv, 0.0), vec3(0.0, 0.0, 1.0), localTime/PI).xy;
    vec2 uv0 = uv;
    uv += cos(vec2(localTime, localTime/GR));
    float r = length(uv);

    
    float map = localTime;
    float noise = 1.0;
    float spounge = localTime*4.0*PI;
    const int max_iterations = 4;
    int target = max_iterations;//-int(saw(spounge)*float(max_iterations)/2.0);
    
    vec2 multa, multb, offa, offb;
    
    float antispeckle = 1.0; 
    float magnification = 1.0;
  
    vec3 color = vec3(1.0);
    vec3 accum = vec3(0.0);
    float sum = 0.0;
    float anticolor = 1.0;
    seedling = 0.0;
    
    float black, white;
    white = 0.0;
        
    float border_color = 0.0;
    float border = 0.0;
    
    vec4 hit = vec4(0.0);
    
    for(int i = 0; i < max_iterations; i++)
    {
        float iteration = float(i)/float(max_iterations);
        
        multa = cos(vec2(localTime*1.1, localTime*1.2)+iteration*PI);
        offa = cos(vec2(localTime*1.3, localTime*1.4)+iteration*PI)*PI;
        multb = cos(vec2(localTime*1.5, localTime*1.6)+iteration*PI);
        offb = cos(vec2(localTime*1.7, localTime*1.8)+iteration*PI);
        
        uv = iterate_square(uv, .5/RENDERSIZE.xy, magnification);
        float weight = smoothstep(0.0, 0.25, magnification);
        antispeckle *= smoothstep(0.0, 1.0/TIMES_DETAILED, sqrt(1.0/(1.0+magnification)));
        
        float q = quadrant(uv);
        seedling += q+float(i);

        map += (q+seedling)*antispeckle;
        float shift = localTime;

        border = max(border, (smoothstep(1.0-1.0/GR/E/PI, 1.0, (cos(uv.y*PI)))));

        border = max(border, (smoothstep(1.0-1.0/GR/E/PI, 1.0, (cos(uv.x*PI)))));
        
        float stripes = map*1.0*PI;//*floor(log(max(RENDERSIZE.x, RENDERSIZE.y))/log(2.0));
        float black = smoothstep(0.0, .75, saw(stripes))*clamp(1.0-abs(border), 0.0, 1.0);
        float white = smoothstep(0.75, 1.0, saw(stripes))*black;

        vec3 final = flux(map*2.0*PI+shift+float(i))*black+white;


        color *= (final);
        accum += final;
        sum += 1.0;
        anticolor *= white;

        if(i != 0)
        {
            hit += galaxy(saw(uv*PI/2.0)*2.0-1.0)*clamp(1.0-length(hit.rgb), 0.0, 1.0)*(1.0-border);
            
            uv = iterate_mobius(uv, .5/RENDERSIZE.xy, magnification, multa, offa, multb, offb);
            antispeckle *= smoothstep(0.0, 1.0/TIMES_DETAILED, sqrt(1.0/(1.0+magnification)));
            border = max(border, (smoothstep(1.0-1.0/GR/E/PI, 1.0, (cos(uv.y*PI)))));

            border = max(border, (smoothstep(1.0-1.0/GR/E/PI, 1.0, (cos(uv.x*PI)))));

        }
    }
    

    scale = 32.;
    vec2 gridPosition = floor(uv0 * scale) / scale;
    vec2 randomOffset = hash(gridPosition) * 2. - 1.;
    vec2 localGridPositionCenter = fract(uv0 * scale) - .5;
    float stars = mix(0., 1., step(length(localGridPositionCenter + randomOffset * .5), .1));

    float map2 = (stars+length(randomOffset))*PI*2.0;

    float twinkle = saw(localTime+map2);

    hit = hit+clamp(vec4(flux(map2+localTime)*PI+twinkle, 1.0)*stars*twinkle*PI, 0.0, 1.0)*clamp(1.0-(border), 0.0, 1.0);
    
    color = powa(color, 1.0/float(max_iterations));
    
    antispeckle = pow(antispeckle, 1.0/float(max_iterations));
    
    fragColor.rgb = (color+accum/sum)*(1.0-border);
    fragColor.a = 1.0;
    
    fragColor = hit;
}
