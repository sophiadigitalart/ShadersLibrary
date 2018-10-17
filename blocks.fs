/*
{
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "INPUTS": [
    {
      "MAX": [
        1,
        1
      ],
      "MIN": [
        0,
        0
      ],
      "NAME": "mouse",
      "TYPE": "point2D"
    }
  ]
}
*/


//MG - raymarching
//distance function(s) provided by
//http://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm

// Colorizer edit, rotation edit, and depth fix by ABraker
#ifdef GL_ES
precision mediump float;
#endif


#define MIN	0.0
#define MAX	60.0
#define DELTA	0.01
#define ITER	300

float sphere(vec3 p, float r) {
	p = mod(p,2.0)-0.5*2.0;
	return length(p)-r;
}

// Thanks to http://glsl.heroku.com/e#15207.1
mat4 rotationMatrix(vec3 axis, float angle)
{
    axis = normalize(axis);
    float s = sin(angle);
    float c = cos(angle);
    float oc = 1.0 - c;
    
    return mat4(oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,  0.0,
                oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,  0.0,
                oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c,           0.0,
                0.0,                                0.0,                                0.0,                                1.0);
}

float sdBox( vec3 p, vec3 b )
{
	p = mod(p,2.0)-0.5*2.0;
	vec3 d = abs(p) - b;
	return min(max(d.x,max(d.y,d.z)),0.0) +	length(max(d,0.0));
}

float castRay(vec3 o,vec3 d) {
	float delta = MAX;
	float t = MIN;
	for (int i = 0;i <= ITER;i += 1) {
		vec3 p = o+d*t;
		
		delta = sdBox(p,vec3(sin(TIME)/8.0+0.5,sin(TIME)/8.0+0.5,0.3));
		t += delta;
		if (delta/t-DELTA <= 0.0) {return float(i)/2.0;}  // EDIT BY ABRAKER: dividing by t seems to get rid of the glitches in far depth
	}
	return 0.0;
}

void main() {
	vec2 p=(gl_FragCoord.xy/RENDERSIZE.y)*1.0;
	p.x-=RENDERSIZE.x/RENDERSIZE.y*0.5;p.y-=0.5;
	vec3 o = vec3(TIME,0,cos(TIME));
	mat4 rot = rotationMatrix(vec3(1, 0, 0.0), mouse.y*2.0-1.0)*rotationMatrix(vec3(0, 1, 0), mouse.x*2.0-1.0);
	vec3 d = normalize(vec3(p.x,p.y,1.0));
	
	vec4 d2 = rot*vec4(d, 1.0);
	
	
	float t = castRay(o,d2.xyz);
	
	// I have no idea why assigning the same thing to a variable changes color output
	vec4 colorize = vec4(sin(t/4.0+TIME)/8.0, cos(t/5.0+TIME)/8.0, cos(t/3.0+TIME)/8.0,0);
	
	if (t < MAX) 
	{
	 t = 1.0-t/MAX;
	 gl_FragColor = vec4(t,t,t,1.0)+ vec4(sin(t/4.0+TIME)/8.0, cos(t/5.0+TIME/2.0)/8.0, cos(t/3.0+TIME)/8.0,0); // +colorize
	} 
	else
	 gl_FragColor = vec4(0,0,0,0);
}