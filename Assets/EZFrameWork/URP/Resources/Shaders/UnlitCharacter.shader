﻿Shader "Universal Render Pipeline/Custom/UnlitCharacter"
{
    Properties
    {
        _BaseMap("Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1, 1, 1, 1)
        _FaceMap("Texture", 2D) = "white" {}
        _FaceColor("Color", Color) = (1, 1, 1, 1)
        _Cutoff("AlphaCutout", Range(0.0, 1.0)) = 0.5
        

        // BlendMode
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("Src", Float) = 1.0
        [HideInInspector] _DstBlend("Dst", Float) = 0.0
        [HideInInspector] _ZWrite("ZWrite", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0

        // Editmode props
        [HideInInspector] _QueueOffset("Queue offset", Float) = 0.0

        // ObsoleteProperties
        [HideInInspector] _MainTex("BaseMap", 2D) = "white" {}
        [HideInInspector] _Color("Base Color", Color) = (0.5, 0.5, 0.5, 1)
        [HideInInspector] _SampleGI("SampleGI", float) = 0.0 // needed from bakedlit

         _RimLightColor("RimLight Color", Color) = (1, 1, 1, 1)
         _RimLightPower("RimLight Power", Range(0.0, 1.0)) = 0.5
        _RimLightThickness("RimLight Thickness", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Blend [_SrcBlend][_DstBlend]
        ZWrite [_ZWrite]
        Cull [_Cull]

        Pass
        {
            Name "Unlit"
            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _SIMPLE_GI_ON
            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "./UnlitCharacterInput.hlsl"
            #include "../ShaderLibrary/Lighting.hlsl"


            struct Attributes
            {
                float4 positionOS       : POSITION;
                float3 normalOS      : NORMAL;
                float4 tangentOS     : TANGENT;
                float2 uv               : TEXCOORD0;
                //float2 uv2               : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv        : TEXCOORD0;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1); //used texcoord1 in this macro
                float fogCoord  : TEXCOORD2;
                float3 normal   : TEXCOORD3;    // xyz: normal, w: viewDir.x
                float3 viewDir  : TEXCOORD4;
                float2 uv2  : TEXCOORD5;
                float4 vertex : SV_POSITION;
                

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.vertex = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                //output.uv2 = TRANSFORM_TEX(input.uv2, _FaceMap);
                output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
                output.normal = NormalizeNormalPerVertex(normalInput.normalWS);
                output.viewDir = normalize(GetCameraPositionWS() - vertexInput.positionWS);
                OUTPUT_SH(output.normal.xyz, output.vertexSH);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half2 uv = input.uv;
                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
                //half4 faceColor = SAMPLE_TEXTURE2D(_FaceMap, sampler_FaceMap, input.uv2);
                //texColor += faceColor;

                half3 color = texColor.rgb * _BaseColor.rgb;
                half alpha = texColor.a * _BaseColor.a;
                AlphaDiscard(alpha, _Cutoff);

                //LimLight
                half nvDot = dot(input.normal, input.viewDir);
                if(nvDot < _RimLightThickness)
                    nvDot = nvDot / _RimLightThickness;
                else 
                    nvDot = 1;

                half rmRate = (1.0h - nvDot) * _RimLightPower;
                color = _RimLightColor * rmRate + color * (1.0h - rmRate);

#ifdef _SIMPLE_GI_ON
                //apply environment light
                half intensity = input.vertexSH / 0.5h;
                color *= intensity;
#endif

#ifdef _ALPHAPREMULTIPLY_ON
                color *= alpha;
#endif

                color = MixFog(color, input.fogCoord);

                return half4(color, alpha);
            }
            ENDHLSL
        }
        Pass
        {
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMetaUnlit

            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitMetaPass.hlsl"

            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    //CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.UnlitShader"
    CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.UnlitCharacterShader"
}
