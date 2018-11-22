/*{
	"CREDIT" : "HypnoticRipples by Cha",
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
		},
		{
			"NAME": "iZoom", 
			"TYPE" : "float", 
			"MIN" : -3,
			"MAX" : 3,
			"DEFAULT" : 1
		}
	],
}
*/
// https://www.shadertoy.com/view/ldX3zr

vec2 HRCenter = vec2(0.5,0.5);
float HRSpeed = 0.035;
float HRInvAr = RENDERSIZE.y / RENDERSIZE.x;

void main(void)
{
   vec2 uv = 2.0 * iZoom * (gl_FragCoord.xy/RENDERSIZE.xy- 0.25);
   uv.x -= 0.0;
   uv.y -= 0.0;
vec3 col = vec4(uv,0.5+0.5*sin(TIME),1.0).xyz;
   
     vec3 texcol;
			
	float x = (HRCenter.x-uv.x);
	float y = (HRCenter.y-uv.y) *HRInvAr;
		
	//float r = -sqrt(x*x + y*y); //uncoment this line to symmetric ripples
	float r = -(x*x + y*y);
	float z = 1.0 + 0.5*sin((r+TIME*HRSpeed)/0.013);
	
	texcol.x = z;
	texcol.y = z;
	texcol.z = z;
	
 gl_FragColor = vec4(col*texcol,1.0);
}
