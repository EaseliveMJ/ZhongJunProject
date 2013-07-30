/*
Shader description:
APPLYMODE = 1
WORLDPOSITION = 0
WORLDNORMAL = 0
WORLDNBT = 0
WORLDVIEW = 0
NORMALMAPTYPE = 0
PARALLAXMAPCOUNT = 0
BASEMAPCOUNT = 1
NORMALMAPCOUNT = 0
DARKMAPCOUNT = 1
DETAILMAPCOUNT = 0
BUMPMAPCOUNT = 0
GLOSSMAPCOUNT = 1
GLOWMAPCOUNT = 0
CUSTOMMAP00COUNT = 0
CUSTOMMAP01COUNT = 0
CUSTOMMAP02COUNT = 0
CUSTOMMAP03COUNT = 0
CUSTOMMAP04COUNT = 0
DECALMAPCOUNT = 0
FOGENABLED = 0
ENVMAPTYPE = 3
PROJLIGHTMAPCOUNT = 0
PROJLIGHTMAPTYPES = 0
PROJLIGHTMAPCLIPPED = 0
PROJSHADOWMAPCOUNT = 0
PROJSHADOWMAPTYPES = 0
PROJSHADOWMAPCLIPPED = 0
PERVERTEXLIGHTING = 1
UVSETFORMAP00 = 0
UVSETFORMAP01 = 1
UVSETFORMAP02 = 2
UVSETFORMAP03 = 0
UVSETFORMAP04 = 0
UVSETFORMAP05 = 0
UVSETFORMAP06 = 0
UVSETFORMAP07 = 0
UVSETFORMAP08 = 0
UVSETFORMAP09 = 0
UVSETFORMAP10 = 0
UVSETFORMAP11 = 0
POINTLIGHTCOUNT = 0
SPOTLIGHTCOUNT = 0
DIRLIGHTCOUNT = 0
SHADOWMAPFORLIGHT = 0
SPECULAR = 0
AMBDIFFEMISSIVE = 0
LIGHTINGMODE = 1
APPLYAMBIENT = 0
BASEMAPALPHAONLY = 0
APPLYEMISSIVE = 0
SHADOWTECHNIQUE = 0
ALPHATEST = 0
*/

//---------------------------------------------------------------------------
// Types:
//---------------------------------------------------------------------------

//---------------------------------------------------------------------------
// Constant variables:
//---------------------------------------------------------------------------

sampler2D baseMap  : register(s0);
sampler2D darkMap  : register(s1);

bool darkEnable;
float lightDimmer;
float3	FogColor : GLOBAL;

//---------------------------------------------------------------------------
// Functions:
//---------------------------------------------------------------------------

/*

    Separate a float4 into a float3 and a float.   
    
*/

void SplitColorAndOpacity(float4 ColorAndOpacity,
    out float3 Color,
    out float Opacity)
{
    Color.rgb = ColorAndOpacity.rgb;
    Opacity = ColorAndOpacity.a;    
}
//---------------------------------------------------------------------------
/*

    This fragment is responsible for sampling a texture and returning its value
    as a RGB value.
    
*/

void TextureRGBSample(float2 TexCoord,
    sampler2D Sampler,
    bool Saturate,
    out float3 ColorOut)
{
    ColorOut.rgb = tex2D(Sampler, TexCoord).rgb;
    if (Saturate)
    {
        ColorOut.rgb = saturate(ColorOut.rgb);
    }    
}
//---------------------------------------------------------------------------
/*

    This fragment is responsible for sampling a texture and returning its value
    as a RGB value and an A value.
    
*/

void TextureRGBASample(float2 TexCoord,
    sampler2D Sampler,
    bool Saturate,
    out float4 ColorOut)
{
    ColorOut = tex2D(Sampler, TexCoord);
    if (Saturate)
    {
        ColorOut = saturate(ColorOut);
    }    
}
//---------------------------------------------------------------------------
/*

    This fragment is responsible for multiplying two float3's. 
    
*/

void MultiplyFloat3(float3 V1,
    float3 V2,
    out float3 Output)
{
    Output = V1 * V2;    
}
//---------------------------------------------------------------------------
/*

    This fragment is responsible for multiplying two floats. 
    
*/

void MultiplyFloat(float V1,
    float V2,
    out float Output)
{
    Output = V1 * V2;    
}
//---------------------------------------------------------------------------
/*

    This fragment is responsible for computing the final RGB color.
    
*/

void CompositeFinalRGBColor(float3 DiffuseColor,
    float3 SpecularColor,
    out float3 OutputColor)
{
    OutputColor.rgb = DiffuseColor.rgb + SpecularColor.rgb;   
}
//---------------------------------------------------------------------------
/*

    This fragment is responsible for computing the final RGBA color.
    
*/

void CompositeFinalRGBAColor(float3 FinalColor,
    float FinalOpacity,
    out float4 OutputColor)
{
    OutputColor.rgb = FinalColor.rgb;
    OutputColor.a = saturate(FinalOpacity);    
}
//---------------------------------------------------------------------------
//---------------------------------------------------------------------------
// Input:
//---------------------------------------------------------------------------

struct Input
{
    float4 PosProjected : POSITION0;
    float4 DiffuseAccum : TEXCOORD0;
    float2 UVSet0 : TEXCOORD1;
    float2 UVSet1 : TEXCOORD2;
	float  FogOut : TEXCOORD5;	
};

//---------------------------------------------------------------------------
// Output:
//---------------------------------------------------------------------------

struct Output
{
    float4 Color0 : COLOR0;	
};

//---------------------------------------------------------------------------
// Main():
//---------------------------------------------------------------------------

Output pMain(Input In)
{
    Output Out;
	
	// Function call #0			// split diffuse color & alpha
	float3 DiffuseColor;
	float  DiffuseOpacity;
	
    SplitColorAndOpacity(In.DiffuseAccum, DiffuseColor, DiffuseOpacity);

	// Function call #1			// baseMap采样
	float4 BaseMapColor;
    TextureRGBASample(In.UVSet0, baseMap, bool(false), BaseMapColor);
	
	// Function call #2			// split base color & alpha
	float3 BaseColor;
	float  BaseOpacity;
    SplitColorAndOpacity(BaseMapColor, BaseColor, BaseOpacity);	
	
	// Function call #3			// darkMap采样
    float3 DarkColor = float3(1,1,1);
	if( darkEnable )
		TextureRGBSample(In.UVSet1, darkMap, bool(false), DarkColor);
	
	// Function call #4			// baseColor
	float3 BaseDecalColor = BaseColor;

	// Function call #8			// diffuseOpacity * baseOpacity
    float OutputOpacity;		
    MultiplyFloat(DiffuseOpacity, BaseOpacity, OutputOpacity);	

	// Function call #9			// (baseColor&decalColor)*darkColor
	float3 DarkBaseColor;
	DarkColor *= lightDimmer;	// DarkColor * BrightNess
    MultiplyFloat3(DarkColor, BaseDecalColor, DarkBaseColor);
	// MultiplyFloat(DarkBaseColor, lightDimmer,DarkBaseColor);

	// Function call #10		// * diffuseColor
    float3 DiffuseBaseColor;
    MultiplyFloat3(DiffuseColor, DarkBaseColor, DiffuseBaseColor);

	// Function call #11
    float3 OutputColor = DiffuseBaseColor;
	OutputColor = lerp( OutputColor, FogColor, In.FogOut );

	// Function call #12		// color & opacity output	
    CompositeFinalRGBAColor(OutputColor, OutputOpacity, Out.Color0);

    return Out;
}

technique Default 
{
	pass Pass0
	{
		PixelShader = compile ps_2_0 pMain();
	}
}