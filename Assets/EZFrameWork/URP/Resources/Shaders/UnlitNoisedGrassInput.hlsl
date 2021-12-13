#ifndef UNIVERSAL_UNLIT_INPUT_INCLUDED
#define UNIVERSAL_UNLIT_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

TEXTURE2D(_NoiseMap); SAMPLER(sampler_NoiseMap);

CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
float4 _NoiseMap_ST;
half4 _BaseColor;
half4 _Wind;
half _Cutoff;
half _Glossiness;
half _Metallic;
CBUFFER_END

#endif
