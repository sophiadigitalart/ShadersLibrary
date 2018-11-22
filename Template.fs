/*{
	"DESCRIPTION": "https://www.shadertoy.com/view/xxxxxx",
	"CREDIT": "ShaderName by Author",
	"CATEGORIES": [
		"dots"
	],
	"INPUTS": [
        {
			"NAME": "Decimal",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.1
		},
        {
			"NAME": "Liste",
			"LABEL": "mode",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2,
				3
			],
			"LABELS": [
				"Item1",
				"Item2",
				"Item3",
				"Item4"
			],
			"DEFAULT": 0
		},
        {
			"NAME" :	"iMouse",
			"TYPE" :	"point2D",
			"DEFAULT" :	[ 0.0, 0.0 ],
			"MAX" : 	[ 640.0, 480.0 ],
			"MIN" :  	[ 0.0, 0.0 ]
		},
		{
			"NAME": "Couleur",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				1.0,
				0.0,
				1.0
			]
		},
		{
			"NAME": "Booleen",
			"TYPE": "bool",
			"DEFAULT": 0.0
		}
	],
}*/

#define iResolution RENDERSIZE
#define iTime TIME

// functions start

// functions end

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	// start

    // end
}
void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}