/*
{
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "INPUTS": [
    
  ]
}
*/

#ifdef GL_ES
precision mediump float;
#endif
 
// my first raymarching \o/ - thanks to iq and his wonderful tools
 
 
vec3 rotateX(vec3 p, float f) { return vec3(p.x, cos(f)*p.y - sin(f)*p.z, sin(f)*p.y + cos(f)*p.z); }
vec3 rotateY(vec3 p, float f) { return vec3(cos(f)*p.x + sin(f)*p.z, p.y, cos(f)*p.z - sin(f)*p.x); }
vec3 rotateZ(vec3 p, float f) { return vec3(cos(f)*p.x - sin(f)*p.y, sin(f)*p.x + cos(f)*p.y, p.z);}
 
float obj_udRoundBox(vec3 p) { // ray marching objects
	vec3 b = vec3(.3);
	p = rotateZ(rotateY(rotateX(p, 0.22*TIME), 0.33*TIME), 0.11*TIME);
	return length(max(abs(p)-b,0.0))-.01;
}
#define pi    3.1415926535897932384626433832795 //pi
 
vec4 spuke(vec4 pos)
{
	vec2 p   =((pos.z+vv_FragNormCoord*pi)+(sin((((length(sin(vv_FragNormCoord*(pos.xy)+TIME*pi)))+(cos((vv_FragNormCoord-TIME*pi)/pi)))))*vv_FragNormCoord))+pos.xy*pos.z; 
	vec3 col = vec3( 0.0, 0.0, 0.0 );
	float ca = 0.0;
	for( int j = 1; j < 14; j++ )
	{
		p *= 1.4;
		float jj = float( j );
		
		for( int i = 1; i <10; i++ )  
		{
			vec2 newp = p*0.96;
			float ii = float( i );
			newp.x += 1.2 / ( ii + jj ) * sin( ii * p.y + (p.x*.3) + cos(TIME/pi/pi)*pi*pi + 0.003 * ( jj / ii ) ) + 1.0;
			newp.y += 0.8 / ( ii + jj ) * cos( ii * p.x + (p.y*.3) + sin(TIME/pi/pi)*pi*pi + 0.003 * ( jj / ii ) ) - 1.0;
			p=newp;
			
		
		}
		p   *= 0.9;
		col += vec3( 0.5 * sin( pi * p.x ) + 0.5, 0.5 * sin( pi * p.y ) + 0.5, 0.5 * sin( pi * p.x ) * cos( pi * p.y ) + 0.5 )*(0.5*sin(pos.z*pi)+0.5);
		ca  += 0.7;
	}
	col /= ca;
	return vec4( col * col * col, 1.0 );
}
void main(void) {
	vec2 q = gl_FragCoord.xy/max(RENDERSIZE.x, RENDERSIZE.y);
	vec2 vPos = 2.*q;
	vPos += vec2(-1., -.5);
 
	// Camera setup
	vec3 camUp = vec3(10.,10.,0.);
	vec3 camlookAt = vec3(0.);
	vec3 camPos = vec3(1.);
	vec3 camDir = normalize(camlookAt - camPos);
	vec3 u = normalize(cross(camUp, camDir));
	vec3 v = cross(camDir, u);
	vec3 vcv = camPos + camDir;
	vec3 scrCoord = vPos.x*u*1. + vPos.y*v*1.;
	vec3 scp = normalize(scrCoord - camPos);
 
	// Raymarching
	const vec3 e = vec3(0.0005, 0.005, 0.0005);
	const float maxd = 6.;
	float d = .05;
	vec3 p;
 
	float f = 0.5;
	for(int i = 0; i < 3; i++) {
	    	if ((abs(d) < .04) || (f > maxd)) break;
	    	f += d;
	    	p = vec3(2.) + scp*f;
	    	d = obj_udRoundBox(p + .01 * sin(TIME));
	}
  
	if (f < maxd) { // cube
		vec3 col = vec3(abs(sin(TIME))*.2+.5, abs(sin(TIME-3.1416/8.))*.2+.5, abs(sin(TIME+3.1416/8.))*.2+.5);
		vec3 n = vec3(d - obj_udRoundBox(p - e.xyy * cos(TIME+p.y)), d - obj_udRoundBox(p - e.yxy * cos(TIME-p.x)), d - obj_udRoundBox(p - e.yyx));
		float b = dot(normalize(n), normalize(camPos - p));
		gl_FragColor=vec4((p*-b*col + pow(b, 16.))*(1. - f*.01), 1.);
	} else { // background, thanks to: http://glsl.heroku.com/e#15441.0
		
		gl_FragColor = spuke(mix ( vec4(vec3(p*15.), 1.), vec4(0.), .5 * sin(TIME)));
	}
}
 