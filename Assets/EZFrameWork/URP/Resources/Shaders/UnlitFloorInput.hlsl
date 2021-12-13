#ifndef UNIVERSAL_UNLIT_INPUT_INCLUDED
#define UNIVERSAL_UNLIT_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

TEXTURE2D(_DirtMap);
SAMPLER(sampler_DirtMap);

TEXTURE2D(_MaskMap);
SAMPLER(sampler_MaskMap);

TEXTURE2D(_FlavorMap);
SAMPLER(sampler_FlavorMap);

TEXTURE2D(_SkyReflectionMap);
SAMPLER(sampler_SkyReflectionMap);

CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
float4 _DirtMap_ST;
float4 _MaskMap_ST;
float4 _FlavorMap_ST;
float4 _SkyReflectionMap_ST;
half4 _BaseColor;
half4 _GIColor;
half4 _Wind;
half _Cutoff;
half _Glossiness;
half _Metallic;
CBUFFER_END

#endif
