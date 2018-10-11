# ISF Shader Library for HeavyM

![Preview](HeavyMshadertoy.jpg)

Shader library for HeavyM, ISF format, mostly from https://Shadertoy.com

##### Usage:
Git clone in Documents\HeavyM\ShadersLibrary to use them.

##### Contributing:
In case you want to contribute, instead of cloning this repo, fork it and :
Add shaders by copying Template.fs, edit, test, then share by doing a pull request!

##### Thumbnails:
png / jpg / gif size 120 x 68px

##### Standard uniforms:
HeavyM expect standard uniform variables named RENDERSIZE, TIME...
To ease the shader editing process, I propose to use defines to match shadertoy uniform variable names:
```
#define iResolution RENDERSIZE
#define iTime TIME 
```

##### Other unicorns:
iMouse, IDate and some other uniform variables are not currently supported, so I add them in the INPUTS section:
```
"INPUTS": [
    {
			"NAME" :	"iMouse",
			"TYPE" :	"point2D",
			"DEFAULT" :	[ 0.0, 0.0 ],
			"MAX" : 	[ 640.0, 480.0 ],
			"MIN" :  	[ 0.0, 0.0 ]
		}
    ]    
```

![Result:](iMouse.gif)

Check this [shader](ExplodedMandelbulb.fs) for more input types. 

