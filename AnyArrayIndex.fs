/*{ 
"CREDIT" : "AnyArrayIndex by Author",
"CATEGORIES" : [
"ci"
],
"DESCRIPTION": "https://www.shadertoy.com/view/ldj3Rd",
 "INPUTS": [
{
"NAME": "iZoom",
"TYPE" : "float",
"MIN" : 0.0,
"MAX" : 1.0,
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
0.0, 
1.0, 
0.0, 
1.0
]
}
],
}
*/
vec3 AAIPalette[7]; // the color AAIPalette is stored here

// just pick a color based on c value
vec3 AAIGetColor(float c) 
{
   c=mod(c,7.); // cycle AAIPalette
   int p=0;
   vec3 color3=vec3(0.);
   for(int i=0;i<7;i++) {
      if (float(i)-c<=.0) { // check loop index against color value
         color3=AAIPalette[i]; // store color picked   
      }
   }
   return color3;
}

// get a gradient of the AAIPalette based on c value, with a smooth parameter (0...1)
vec3 AAIGetSMColor(float c, float s) 
{
    s*=.5;
    c=mod(c-.5,7.);
    vec3 color1=vec3(0.0),color2=vec3(0.0);
    for(int i=0;i<7;i++) {
        if (float(i)-c<=.0) {
            color1 = AAIPalette[i];
            color2 = AAIPalette[(i+1>6)?0:i+1];
        }
    }
    // smooth mix the two colors
    return mix(color1,color2,smoothstep(.5-s,.5+s,fract(c)));
}

// https://www.shadertoy.com/view/ldj3Rd
void mainImage(out vec4 fragColor, in vec2 fragCoord)
{
    vec2 uv = iZoom *  fragCoord.xy/RENDERSIZE.xy;
   vec3 col=vec3(0.);
   // define the colors  
    AAIPalette[6]=vec3(iColor.r*255,000,000)/255.;
    AAIPalette[5]=vec3(iColor.r*255,iColor.g*127,000)/255.;
    AAIPalette[4]=vec3(iColor.r*255,iColor.g*255,000)/255.;
    AAIPalette[3]=vec3(000,iColor.g*255,000)/255.;
    AAIPalette[2]=vec3(000,000,iColor.b*255)/255.;
    AAIPalette[1]=vec3(iColor.r*075,000,iColor.b*130)/255.;
    AAIPalette[0]=vec3(iColor.r*143,000,iColor.b*255)/255.;
    // rainbow
    //  AAIPalette[6]=vec3(255,000,000)/255.;
    //  AAIPalette[5]=vec3(255,127,000)/255.;
    //  AAIPalette[4]=vec3(255,255,000)/255.;
    //  AAIPalette[3]=vec3(000,255,000)/255.;
    //  AAIPalette[2]=vec3(000,000,255)/255.;
    //  AAIPalette[1]=vec3(075,000,130)/255.;
    //  AAIPalette[0]=vec3(143,000,255)/255.;
   vec2 p=(uv-.5);
   p.x*=RENDERSIZE.x/RENDERSIZE.y;
   
   // fractal
   float a=TIME*.05;   
   float ot=1000.;
   float otl=1000.;
   mat2 rot=mat2(cos(a),sin(a),-sin(a),cos(a));
   for(int i=0;i<14;i++) {
      p=abs(p+1.)-1.;
      p=p*1.25;
      p*=rot;
      ot=min(ot,abs(min(abs(p.y),abs(p.x))-.75)); //orbit trap 1
      ot=max(ot,length(p)*.02); //orbit trap 2
      otl=min(otl,abs(length(p)-.75)); //orbit trap 3
   }
    ot=pow(max(0.,1.-ot),10.); //orbit trap (0 to 1)
    if (length(max(vec2(0.),abs(uv-.5)-vec2(.485,.47)))>0.0) col*=0.; // border   
   
    col=AAIGetSMColor(ot*7.+length(uv)*14.,1.); //get color gradient for orbit trap value   
    col=mix(vec3(length(col)),col,.8)-pow(max(0.,1.-otl),25.)*.8+.1; 
   
    fragColor = vec4(col,1.0);
}
void main(void) { 
    mainImage(gl_FragColor, gl_FragCoord.xy); 
}