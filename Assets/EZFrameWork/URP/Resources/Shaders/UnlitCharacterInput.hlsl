#ifndef UNIVERSAL_UNLIT_INPUT_INCLUDED
#define UNIVERSAL_UNLIT_INPUT_INCLUDED

#include "../ShaderLibrary/SurfaceInput.hlsl"

TEXTURE2D(_FaceMap);
SAMPLER(sampler_FaceMap);

CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
half4 _BaseColor;
float4 _FaceMap_ST;
half4 _FaceColor;
half4 _RimLightColor;
half _RimLightPower;
half _RimLightThickness;
half _Cutoff;
half _Glossiness;
half _Metallic;
CBUFFER_END

#endif
