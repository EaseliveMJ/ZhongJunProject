/* 
Shader description:
TRANSFORM = 0
OUTPUTWORLDPOS = 1
OUTPUTWORLDNBT = 1
OUTPUTWORLDVIEW = 1
OUTPUTTANGENTVIEW = 0
NORMAL = 2
SPECULAR = 0
FOGTYPE = 0
ENVMAPTYPE = 0
PROJLIGHTMAPCOUNT = 0
PROJLIGHTMAPTYPES = 0
PROJSHADOWMAPCOUNT = 0
PROJSHADOWMAPTYPES = 0
OUTPUTUVCOUNT = 3
UVSET00 = 0
UVSET00TEXOUTPUT = 0
UVSET01 = 1
UVSET01TEXOUTPUT = 0
UVSET02 = 2
UVSET02TEXOUTPUT = 1
UVSET03 = 0
UVSET03TEXOUTPUT = 0
UVSET04 = 0
UVSET04TEXOUTPUT = 0
UVSET05 = 0
UVSET05TEXOUTPUT = 0
UVSET06 = 0
UVSET06TEXOUTPUT = 0
UVSET07 = 0
UVSET07TEXOUTPUT = 0
UVSET08 = 0
UVSET08TEXOUTPUT = 0
UVSET09 = 0
UVSET09TEXOUTPUT = 0
UVSET10 = 0
UVSET10TEXOUTPUT = 0
UVSET11 = 0
UVSET11TEXOUTPUT = 0
POINTLIGHTCOUNT = 0
SPOTLIGHTCOUNT = 0
DIRLIGHTCOUNT = 0
VERTEXCOLORS = 0
VERTEXLIGHTSONLY = 0
AMBDIFFEMISSIVE = 0
LIGHTINGMODE = 1
APPLYMODE = 1
*/

//---------------------------------------------------------------------------
// Types:
//---------------------------------------------------------------------------

//---------------------------------------------------------------------------
// Constant variables:
//---------------------------------------------------------------------------

float4x4 g_World;
//float4x4 g_ViewProj;
float4x4 g_View;
float4x4 g_Proj;
float4 	 g_EyePos;

float4x4 UVSet0_TexTransform;
float4x4 UVSet1_TexTransform;
float4x4 UVSet2_TexTransform;

float  FogStart : GLOBAL;
float  FogEnd 	: GLOBAL;
//---------------------------------------------------------------------------
// Functions:
//---------------------------------------------------------------------------

/*

    This fragment is responsible for applying the view projection transform
    to the input position. Additionally, this fragment applies the world 
    transform to the input position. 
    
*/

void TransformPosition(float3 Position,
    float4x4 World,
    out float4 WorldPos)
{

    // Transform the position into world space for lighting, and projected 
    // space for display
    WorldPos = mul( float4(Position, 1.0f), World );
    
}
//---------------------------------------------------------------------------
/*

    This fragment is responsible for applying the view projection transform
    to the input world position.
    
*/

void ProjectPositionWorldToProj(float4 WorldPosition,
    float4x4 ViewProjection,
    out float4 ProjPos)
{

    ProjPos = mul(WorldPosition, ViewProjection);
    
}
//---------------------------------------------------------------------------
/*

    This fragment is responsible for applying a transform to the input set
    of texture coordinates.
    
*/

void TexTransformApply(float2 TexCoord,
    float4x4 TexTransform,
    out float2 TexCoordOut)
{

    
    TexCoordOut = mul(float4(TexCoord.x, TexCoord.y, 0.0, 1.0), TexTransform);
    
}
//---------------------------------------------------------------------------
/*

    This fragment is responsible for applying the world transform to the
    normal, binormal, and tangent.
    
*/

void TransformNBT(float3 Normal,
    float3 Binormal,
    float3 Tangent,
    float4x4 World,
    out float3 WorldNrm,
    out float3 WorldBinormal,
    out float3 WorldTangent)
{

    // Transform the normal into world space for lighting
    WorldNrm      = mul( Normal, (float3x3)World );
    WorldBinormal = mul( Binormal, (float3x3)World );
    WorldTangent  = mul( Tangent, (float3x3)World );
    
    // Should not need to normalize here since we will normalize in the pixel 
    // shader due to linear interpolation across triangle not preserving
    // normality.
    
}
//---------------------------------------------------------------------------
/*

    This fragment is responsible for calculating the camera view vector.
    
*/

void CalculateViewVector(float4 WorldPos,
    float3 CameraPos,
    out float3 WorldViewVector)
{

    WorldViewVector = CameraPos - WorldPos;
    
}
//---------------------------------------------------------------------------
//---------------------------------------------------------------------------
// Input:
//---------------------------------------------------------------------------

struct Input
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
	float3 Tangent : TANGENT0;
    float3 Binormal : BINORMAL0;
    float2 UVSet0 : TEXCOORD0;
    float2 UVSet1 : TEXCOORD1;
    float2 UVSet2 : TEXCOORD2; 
};

//---------------------------------------------------------------------------
// Output:
//---------------------------------------------------------------------------

struct Output
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
// Main():
//---------------------------------------------------------------------------

Output vMain(Input In)
{
    Output Out;
	// Function call #0
    TransformPosition(In.Position, g_World, Out.WorldPos);

	// Function call #1
	float4 ViewPos;
	ProjectPositionWorldToProj(Out.WorldPos, g_View, ViewPos);
    ProjectPositionWorldToProj(ViewPos, g_Proj, Out.PosProjected);
	float fFogOut = saturate((ViewPos.z-FogStart)/(FogEnd-FogStart));

	// Function call #2
    TexTransformApply(In.UVSet0, UVSet0_TexTransform, Out.UVSet0);
	TexTransformApply(In.UVSet1, UVSet1_TexTransform, Out.UVSet1);
	TexTransformApply(In.UVSet2, UVSet2_TexTransform, Out.UVSet2);

	// Function call #3
    TransformNBT(In.Normal, In.Binormal, In.Tangent, g_World, Out.WorldNormal, 
       Out.WorldBinormal, Out.WorldTangent);
	
	// Function call #4
    CalculateViewVector(Out.WorldPos, g_EyePos, Out.WorldView.xyz);
	Out.WorldView.w = fFogOut;
	
    return Out;
}

technique Default 
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 vMain();
	}
}