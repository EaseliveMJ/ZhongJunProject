/* 
Shader description:
APPLYMODE = 1
WORLDPOSITION = 1
WORLDNORMAL = 1
WORLDNBT = 1
WORLDVIEW = 1
NORMALMAPTYPE = 0
PARALLAXMAPCOUNT = 0
BASEMAPCOUNT = 1
NORMALMAPCOUNT = 1
DARKMAPCOUNT = 1
DETAILMAPCOUNT = 1
BUMPMAPCOUNT = 0
GLOSSMAPCOUNT = 0
GLOWMAPCOUNT = 1
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
PERVERTEXLIGHTING = 0
UVSETFORMAP00 = 0
UVSETFORMAP01 = 0
UVSETFORMAP02 = 1
UVSETFORMAP03 = 0
UVSETFORMAP04 = 2
UVSETFORMAP05 = 0
UVSETFORMAP06 = 0
UVSETFORMAP07 = 0
UVSETFORMAP08 = 0
UVSETFORMAP09 = 0
UVSETFORMAP10 = 0
UVSETFORMAP11 = 0
POINTLIGHTCOUNT = 0
SPOTLIGHTCOUNT = 1
DIRLIGHTCOUNT = 1
SHADOWMAPFORLIGHT = 0
SPECULAR = 0
AMBDIFFEMISSIVE = 0
LIGHTINGMODE = 1
APPLYAMBIENT = 1
BASEMAPALPHAONLY = 0
APPLYEMISSIVE = 1
SHADOWTECHNIQUE = 0
ALPHATEST = 0
*/
//---------------------------------------------------------------------------
// Types:
//---------------------------------------------------------------------------

//---------------------------------------------------------------------------
// Constant variables:
//---------------------------------------------------------------------------
sampler2D baseMap 	: register(s0);
sampler2D darkMap	: register(s1);
sampler2D decalMap	: register(s2);
sampler2D glossMap	: register(s3);
sampler2D envMap 	: register(s4);
sampler2D normalMap : register(s5);

float4 g_AmbientLight;

float4 g_DirAmbient0;
float4 g_DirDiffuse0;
float4 g_DirWorldDirection0;

float4x4 g_EnvironmentMapWorldProjectionTransform0;

bool	darkEnable;
bool	decalEnable;
bool	glossEnable;
bool	envEnable;
bool	normalEnable;

float	lightDimmer;
float3	FogColor : GLOBAL;
//---------------------------------------------------------------------------
// Functions:
//---------------------------------------------------------------------------

/*

    This fragment is responsible for normalizing a float3.
    
*/

void NormalizeFloat3(float3 VectorIn,
    out float3 VectorOut)
{

    VectorOut = normalize(VectorIn);
    
}
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
    
    This fragment is responsible for scaling a float3 by a constant. 
    
*/

void ScaleFloat3(float3 V1,
    float Scale,
    out float3 Output)
{

    Output = Scale * V1;
    
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

    This fragment is responsible for sampling a normal map to generate the
    new world-space normal.
    
    The normal map type is an enumerated value that indicates the following:
        0 - Standard (rgb = normal/binormal/tangent)
        1 - DXN (rg = normal.xy need to calculate z)
        2 - DXT5 (ag = normal.xy need to calculate z)
    
*/

void CalculateNormalFromColor(float4 NormalMap,
    float3 WorldNormalIn,
    float3 WorldBinormalIn,
    float3 WorldTangentIn,
    int NormalMapType,
    out float3 WorldNormalOut)
{

    
    NormalMap = NormalMap * 2.0 - 1.0;
    
    // Do nothing extra for Standard
    // Handle compressed types:
    if (NormalMapType == 1) // DXN
    {
        NormalMap.rgb = float3(NormalMap.r, NormalMap.g, 
            sqrt(1 - NormalMap.r * NormalMap.r - NormalMap.g * NormalMap.g));
    }
    else if (NormalMapType == 2) // DXT5
    {
        NormalMap.rg = NormalMap.ag;
        NormalMap.b = sqrt(1 - NormalMap.r*NormalMap.r -  
            NormalMap.g * NormalMap.g);
    }
       
    float3x3 xForm = float3x3(WorldTangentIn, WorldBinormalIn, WorldNormalIn);
    xForm = transpose(xForm);
    WorldNormalOut = mul(xForm, NormalMap.rgb);
    
    WorldNormalOut = normalize(WorldNormalOut);
    
}

//---------------------------------------------------------------------------
/*

    This fragment is responsible for accumulating the effect of a light
    on the current pixel.
    
    LightType can be one of three values:
        0 - Directional
        1 - Point 
        2 - Spot
        
    Note that the LightType must be a compile-time variable,
    not a runtime constant/uniform variable on most Shader Model 2.0 cards.
    
    The compiler will optimize out any constants that aren't used.
    
    Attenuation is defined as (const, linear, quad, range).
    Range is not implemented at this time.
    
    SpotAttenuation is stored as (cos(theta/2), cos(phi/2), falloff)
    theta is the angle of the inner cone and phi is the angle of the outer
    cone in the traditional DX manner. Gamebryo only allows setting of
    phi, so cos(theta/2) will typically be cos(0) or 1. To disable spot
    effects entirely, set cos(theta/2) and cos(phi/2) to -1 or lower.
    
*/

void Light(float4 WorldPos,
    float3 WorldNrm,
    int LightType,
    bool SpecularEnable,
    float Shadow,
    float3 WorldViewVector,
    float4 LightPos,
    float3 LightAmbient,
    float3 LightDiffuse,
    float3 LightSpecular,
    float3 LightAttenuation,
    float3 LightSpotAttenuation,
    float3 LightDirection,
    float4 SpecularPower,
    float3 AmbientAccum,
    float3 DiffuseAccum,
    float3 SpecularAccum,
    out float3 AmbientAccumOut,
    out float3 DiffuseAccumOut,
    out float3 SpecularAccumOut)
{
   
    // Get the world space light vector.
    float3 LightVector;
    float DistanceToLight;
    float DistanceToLightSquared;
        
    if (LightType == 0)
    {
        LightVector = -LightDirection;
    }
    else
    {
        LightVector = LightPos - WorldPos;
        DistanceToLightSquared = dot(LightVector, LightVector);
        DistanceToLight = length(LightVector);
        LightVector = normalize(LightVector);
    }
    
    // Take N dot L as intensity.
    float LightNDotL = dot(LightVector, WorldNrm);
    float LightIntensity = max(0, LightNDotL);

    float Attenuate = Shadow;
    
    if (LightType != 0)
    {
        // Attenuate Here
        Attenuate = LightAttenuation.x +
            LightAttenuation.y * DistanceToLight +
            LightAttenuation.z * DistanceToLightSquared;
        Attenuate = max(1.0, Attenuate);
        Attenuate = 1.0 / Attenuate;
        Attenuate *= Shadow;

        if (LightType == 2)
        {
            // Get intensity as cosine of light vector and direction.
            float CosAlpha = dot(-LightVector, LightDirection);

            // Factor in inner and outer cone angles.
            float AttenDiff = LightSpotAttenuation.x - LightSpotAttenuation.y;
            CosAlpha = saturate((CosAlpha - LightSpotAttenuation.y) / 
                AttenDiff);

            // Power to falloff.
            // The pow() here can create a NaN if CosAlpha is 0 or less.
            // On some cards (GeForce 6800), the NaN will propagate through
            // a ternary instruction, so we need two to be safe.
            float origCosAlpha = CosAlpha;
            CosAlpha = origCosAlpha <= 0.0 ? 1.0 : CosAlpha;
            CosAlpha = pow(CosAlpha, LightSpotAttenuation.z);
            CosAlpha = origCosAlpha <= 0.0 ? 0.0 : CosAlpha;

            // Multiply the spot attenuation into the overall attenuation.
            Attenuate *= CosAlpha;
        }
    }
    // Determine the interaction of diffuse color of light and material.
    // Scale by the attenuated intensity.
    DiffuseAccumOut = DiffuseAccum;
    DiffuseAccumOut.rgb += LightDiffuse.rgb * LightIntensity * Attenuate;

    // Determine ambient contribution - Is affected by shadow
    AmbientAccumOut = AmbientAccum;
    AmbientAccumOut.rgb += LightAmbient.rgb * Attenuate;

    SpecularAccumOut = SpecularAccum;
    if (SpecularEnable)
    {
        // Get the half vector.
        float3 LightHalfVector = LightVector + WorldViewVector;
        LightHalfVector = normalize(LightHalfVector);

        // Determine specular intensity.
        float LightNDotH = max(0, dot(WorldNrm, LightHalfVector));
        float LightSpecIntensity = pow(LightNDotH, SpecularPower.x);
        
        //if (LightNDotL < 0.0)
        //    LightSpecIntensity = 0.0;
        // Must use the code below rather than code above.
        // Using previous lines will cause the compiler to generate incorrect
        // output.
        float SpecularMultiplier = LightNDotL > 0.0 ? 1.0 : 0.0;
        
        // Attenuate Here
        LightSpecIntensity = LightSpecIntensity * Attenuate * 
            SpecularMultiplier;
        
        // Determine the interaction of specular color of light and material.
        // Scale by the attenuated intensity.
        SpecularAccumOut.rgb += LightSpecIntensity * LightSpecular;
    }       

    
    
}
//---------------------------------------------------------------------------
/*

    This fragment is responsible for computing the reflection vector.
    The WorldViewVector is negated because the HLSL "reflect" function
    expects a world-to-camera vector, rather than a camera-to-world vector.
    
*/

void WorldReflect(float3 WorldNrm,
    float3 WorldViewVector,
    bool NormalizeNormal,
    out float3 WorldReflect)
{

    if (NormalizeNormal)
        WorldNrm = normalize(WorldNrm);
    WorldReflect = reflect(-WorldViewVector, WorldNrm);
    
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

    This fragment is responsible for applying a projection to the input set
    of texture coordinates.
    
*/

void ProjectTextureCoordinates(float3 TexCoord,
    float4x4 TexTransform,
    out float4 TexCoordOut)
{
    TexCoordOut = mul(float4(TexCoord, 1.0), TexTransform);
}
//---------------------------------------------------------------------------
/*

    This fragment is responsible for computing the coefficients for the 
    following equations:
    
    Kdiffuse = MatEmissive + 
        MatAmbient * Summation(0...N){LightAmbientContribution[N]} + 
        MatDiffuse * Summation(0..N){LightDiffuseContribution[N]}
        
    Kspecular = MatSpecular * Summation(0..N){LightSpecularContribution[N]}
    
    
*/

void ComputeShadingCoefficients(float3 MatEmissive,
    float3 MatDiffuse,
    float3 MatAmbient,
    float3 MatSpecular,
    float3 LightSpecularAccum,
    float3 LightDiffuseAccum,
    float3 LightAmbientAccum,
    bool Saturate,
    out float3 Diffuse,
    out float3 Specular)
{

    Diffuse = MatEmissive + MatAmbient * LightAmbientAccum + 
        MatDiffuse * LightDiffuseAccum;
    Specular = MatSpecular * LightSpecularAccum;
    
    if (Saturate)
    {
        Diffuse = saturate(Diffuse);
        Specular = saturate(Specular);
    }
	
	Diffuse = MatEmissive + MatAmbient * LightAmbientAccum + 
		MatDiffuse * LightDiffuseAccum;
	
}
//---------------------------------------------------------------------------
/*

    This fragment is responsible for adding two float3's. 
    
*/

void AddFloat3(float3 V1,
    float3 V2,
    out float3 Output)
{

    Output = V1 + V2;
    
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
    float4 WorldPos : TEXCOORD0;
    float3 WorldNormal : TEXCOORD1;
    float3 WorldBinormal : TEXCOORD2;
    float3 WorldTangent : TEXCOORD3;
    float4 WorldView : TEXCOORD4;
    float2 UVSet0 : TEXCOORD5;
    float2 UVSet1 : TEXCOORD6;
    float2 UVSet2 : TEXCOORD7;
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
	// Function call #0				//单位化 N
    float3 VectorOut_CallOut0;
    NormalizeFloat3(In.WorldNormal, VectorOut_CallOut0);	

	// Function call #3				//材质Color与Opacity切割
    //float3 Color_CallOut3;
    //float Opacity_CallOut3;
    //SplitColorAndOpacity(g_MaterialDiffuse, Color_CallOut3, Opacity_CallOut3);

	// Function call #4				//DarkMap采样
    float3 ColorOut_CallOut4 = float3(1.0,1.0,1.0);
	if( darkEnable )
	    TextureRGBSample(In.UVSet1, darkMap, bool(false), ColorOut_CallOut4);
	ScaleFloat3( ColorOut_CallOut4, lightDimmer, ColorOut_CallOut4);

	// Function call #5				//DecalMap采样
    float3 ColorOut_CallOut5 = float3(1.0, 1.0, 1.0);		
	if( decalEnable )
		TextureRGBSample(In.UVSet2, decalMap, bool(false), ColorOut_CallOut5);

	// Function call #6				//GlossMap采样
    float3 ColorOut_CallOut6 = float3(1,1,1);
	if( glossEnable )
		TextureRGBSample(In.UVSet0, glossMap, bool(false), ColorOut_CallOut6);

	// Function call #8				//BaseMap采样
    float4 ColorOut_CallOut8;
    TextureRGBASample(In.UVSet0, baseMap, bool(false), ColorOut_CallOut8);

	// Function call #9
    //float3 Output_CallOut9;		//DecalColor
    //ScaleFloat3(ColorOut_CallOut5, float(2.0), Output_CallOut9);
	
	// Function call #10			//NormalMap采样
    float4 ColorOut_CallOut10;		
	// Function call #13			//WorldNormal
    float3 WorldNormalOut_CallOut13 = VectorOut_CallOut0;	
	if( normalEnable )
	{
		// Function call #1				//单位化 T
		float3 VectorOut_CallOut1;
		NormalizeFloat3(In.WorldTangent, VectorOut_CallOut1);	

		// Function call #2				//单位化 B
		float3 VectorOut_CallOut2;
		NormalizeFloat3(In.WorldBinormal, VectorOut_CallOut2);
	
		TextureRGBASample(In.UVSet0, normalMap, bool(false), ColorOut_CallOut10);//NormalMap采样		
		CalculateNormalFromColor(ColorOut_CallOut10, VectorOut_CallOut0, 		 //Normal*NBT
		VectorOut_CallOut2, VectorOut_CallOut1, int(0),
		WorldNormalOut_CallOut13);
	}	
	
	// Function call #11
    float3 Color_CallOut11;
    float Opacity_CallOut11;		//BaseColor Color和Opacity切分
    SplitColorAndOpacity(ColorOut_CallOut8, Color_CallOut11, Opacity_CallOut11);

	// Function call #12
    //float Output_CallOut12;			//计算透明度
    //MultiplyFloat(Opacity_CallOut3, Opacity_CallOut11, Output_CallOut12);
		
	// Function call #14			//计算平行光
    float3 AmbientAccumOut_CallOut14;
    float3 DiffuseAccumOut_CallOut14;
    float3 SpecularAccumOut_CallOut14;
    Light(In.WorldPos, WorldNormalOut_CallOut13, int(0), bool(false), 
        float(1.0), float3(0.0, 0.0, 0.0), float4(0,0,0,0), g_DirAmbient0, 
        g_DirDiffuse0, float3(1,1,1), float3(0.0, 1.0, 0.0), 
        float3(-1.0, -1.0, 0.0), g_DirWorldDirection0, 
        float4(1.0, 1.0, 1.0, 1.0), g_AmbientLight, float3(0.0, 0.0, 0.0), 
        float3(0.0, 0.0, 0.0), AmbientAccumOut_CallOut14, 
        DiffuseAccumOut_CallOut14, SpecularAccumOut_CallOut14);

	// Function call #16		//BaseColor*DarkColor
    //float3 Output_CallOut16 = Color_CallOut11;	
    //MultiplyFloat3(ColorOut_CallOut4, Color_CallOut11, Output_CallOut16);

	// Function call #17		//计算PointLight
    //float3 AmbientAccumOut_CallOut17;
    //float3 DiffuseAccumOut_CallOut17;
    //float3 SpecularAccumOut_CallOut17;	    
    //Light(In.WorldPos, WorldNormalOut_CallOut13, int(1), bool(false), 
    //    float(1.0), float3(0.0, 0.0, 0.0), g_PointWorldPosition0, 
    //    g_PointAmbient0, g_PointDiffuse0, g_PointSpecular0, g_PointAttenuation0, 
    //    float3(-1.0, -1.0, 0.0), float3(1.0, 0.0, 0.0), 
    //    float4(1.0, 1.0, 1.0, 1.0), AmbientAccumOut_CallOut14, DiffuseAccumOut_CallOut14, 
    //    SpecularAccumOut_CallOut14, AmbientAccumOut_CallOut17, 
    //    DiffuseAccumOut_CallOut17, SpecularAccumOut_CallOut17);

	// Function call #19
    float3 Output_CallOut19 = Color_CallOut11;	//BaseColor*DarkColor*DetailColor
    //MultiplyFloat3(Output_CallOut11, Output_CallOut9, Output_CallOut19);
	
	float4 Output_CallOut16 = float4(0,0,0,0);
	if( decalEnable )
	{
		Output_CallOut16 = tex2D( decalMap, In.UVSet2 );
		Output_CallOut19 = lerp( Color_CallOut11, Output_CallOut16.xyz, Output_CallOut16.w );
	}
	
	// lerp(baseColor, decalColor)*DarkColor	
	MultiplyFloat3(ColorOut_CallOut4, Output_CallOut19, Output_CallOut19);

	// Function call #21			//Material*Light
    float3 Diffuse_CallOut21;
    float3 Specular_CallOut21;
    ComputeShadingCoefficients( float3(0,0,0), float3(1,1,1), float3(1,1,1), float3(1,1,1), 
		SpecularAccumOut_CallOut14, DiffuseAccumOut_CallOut14, AmbientAccumOut_CallOut14,
		bool(false), Diffuse_CallOut21, Specular_CallOut21);

	// Function call #22			//Environment Map reflect
    float3 ColorOut_CallOut22 = float3(0,0,0);
	if( envEnable )
	{
		// Function call #7			//观察方向单位化
		float3 VectorOut_CallOut7;
		NormalizeFloat3(In.WorldView, VectorOut_CallOut7);
	
		// Function call #15		//计算反射向量
		float3 WorldReflect_CallOut15;
		WorldReflect(WorldNormalOut_CallOut13, VectorOut_CallOut7, bool(false), 
			WorldReflect_CallOut15);
			
		// Function call #18		//单位化反射向量
		float3 VectorOut_CallOut18;
		NormalizeFloat3(WorldReflect_CallOut15, VectorOut_CallOut18);
		
		// Function call #20
		float4 TexCoordOut_CallOut20;	//EnvMap采样
		ProjectTextureCoordinates(VectorOut_CallOut18, 
			g_EnvironmentMapWorldProjectionTransform0, TexCoordOut_CallOut20);
		
		TextureRGBSample(TexCoordOut_CallOut20, envMap, bool(false), ColorOut_CallOut22);
	}

	// Function call #23			// * diffuseMaterial
    float3 Output_CallOut23;	
    MultiplyFloat3(Diffuse_CallOut21, Output_CallOut19, Output_CallOut23);
	
	// Function call #24			//+Glow
    //float3 Output_CallOut24;
    //AddFloat3(Output_CallOut23, ColorOut_CallOut6, Output_CallOut24);

	// Function call #25
    float3 Output_CallOut25;		// specular*gloss
    MultiplyFloat3(Specular_CallOut21, ColorOut_CallOut6, Output_CallOut25);
	
	// Function call #26
    float3 Output_CallOut26;		// environment*gloss
    MultiplyFloat3(ColorOut_CallOut22, ColorOut_CallOut6,  Output_CallOut26);

	// Function call #27			// +specular+environment
    float3 Output_CallOut27;
    AddFloat3(Output_CallOut25, Output_CallOut26, Output_CallOut27);

	// Function call #26			// output color
    float3 OutputColor_CallOut28;
    CompositeFinalRGBColor(Output_CallOut23, Output_CallOut27, OutputColor_CallOut28);
	OutputColor_CallOut28 = lerp( OutputColor_CallOut28, FogColor, In.WorldView.w );

	// Function call #27			// output color + alpha
    CompositeFinalRGBAColor(OutputColor_CallOut28, Opacity_CallOut11, Out.Color0);

    return Out;
}

technique Default 
{
	pass Pass0
	{
		PixelShader = compile ps_2_0 pMain();
	}
}