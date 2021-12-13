Shader "Universal Render Pipeline/Custom/URP_lightmap_floor"
{
    Properties
    {
        _BaseMap("Texture", 2D) = "white" {}
        _DirtMap("Dirt Texture", 2D) = "white" {}
        _MaskMap("Mask Texture", 2D) = "white" {}
        _FlavorMap("Flavor Texture", 2D) = "black" {}
        _SkyReflectionMap("Flavor Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1, 1, 1, 1)
        _Cutoff("AlphaCutout", Range(0.0, 1.0)) = 0.5
        //_Wind("Wind", Vector) = (0,0,0,0)

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
        [HideInInspector] _GIColor("GI Color", Color) = (1, 1, 1, 1)
        [HideInInspector] _SampleGI("SampleGI", float) = 0.0 // needed from bakedlit
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
            #pragma target 2.0 //added

            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _OCCLUSIONMAP
            #pragma shader_feature _　FLAVOR_TEX_ON
            #pragma shader_feature _MASK_LIGHTING_ON
            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON

            

            #include "UnlitFloorInput.hlsl"
            #include "../ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS       : POSITION;
                float3 normalOS      : NORMAL;
                float4 tangentOS     : TANGENT;
                float2 uv               : TEXCOORD0;
                float2 lightmapUV    : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv        : TEXCOORD0;
                float2 lightmapUV: TEXCOORD1;
                //DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1); //used texcoord1 in this macro
                float2 uvm        : TEXCOORD2;
                float fogCoord  : TEXCOORD3;
                float2 uvf        : TEXCOORD4;
                //float4 normal   : TEXCOORD5;    // xyz: normal, w: viewDir.x　if use normap use this
                float3  normal                  : TEXCOORD5;
                float2 uvs        : TEXCOORD6;
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
                output.uvm = input.uv;
                output.uvf = TRANSFORM_TEX(input.uv, _FlavorMap);
                output.uvs = TRANSFORM_TEX(input.uv, _SkyReflectionMap);
                output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
                output.normal = NormalizeNormalPerVertex(normalInput.normalWS);
                OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);

//                OUTPUT_SH(output.normal.xyz, output.vertexSH);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                half2 uv = input.uv;
                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
                half4 dirtColor = SAMPLE_TEXTURE2D(_DirtMap, sampler_DirtMap, uv);
                half4 maskColor = SAMPLE_TEXTURE2D(_MaskMap, sampler_DirtMap, input.uvm);
                half maskValue = maskColor.g;

#ifdef _MASK_LIGHTING_ON
                //light effect
                half gi = maskColor.b * 10.0f;
                gi = min(gi, 1.0h);
                maskValue *= gi;
#else
                half gi = 1.0f;
#endif
//                return half4(gi, gi, gi, 1);

                half oneminusmg = (1.0h - maskValue);




//#ifdef FLAVOR_TEX_ON
                half4 flavorColor = SAMPLE_TEXTURE2D(_FlavorMap, sampler_FlavorMap, input.uvf);
//#else
  //              half4 flavorColor = half4(0,0,0,0);
//#endif
                

                //sky refelction
                half4 skyReflectionColor = SAMPLE_TEXTURE2D(_SkyReflectionMap, sampler_SkyReflectionMap, input.uvs);
                half skyReflectionPower = 1.0h;

                //reflection
                half uvadd = (1.0h - uv.x) + uv.y;
                half width = 1.3h;
                half spd = 0.2h;
                half strength = ((uvadd + _Time.y * spd) % width) / width;
                half sub = 0.5h - strength;

                if(sub > 0){
                    strength = 0.5h - sub;
                }else{
                    strength = 0.5h + sub;
                }

                texColor.rgb += strength * 0.5f;

                half3 dirtC = dirtColor.rgb * oneminusmg * (1.0f - flavorColor.a) + flavorColor.rgb * oneminusmg * (flavorColor.a);
                half3 cleanC = (texColor.rgb * skyReflectionPower + skyReflectionColor.rgb * (1.0h - skyReflectionPower)) * maskValue;

                half3 color = dirtC + cleanC;
                color *= _BaseColor.rgb;
                half alpha = texColor.a * _BaseColor.a;
                AlphaDiscard(alpha, _Cutoff);

                //LightMap GI
                half3 normalWS = NormalizeNormalPerPixel(input.normal);
                half3 bakedGI = SampleLightmap(input.lightmapUV, normalWS);
                color *= (bakedGI);

                //apply environment light input.vertexSH => 0:dark 1:light / maskColor.b => 0:dark 1:light
                half3 intensity = _GIColor * (1.0f - gi) + 1.0h * gi;
                color *= intensity;


#ifdef _ALPHAPREMULTIPLY_ON
                color *= alpha;
#endif
                //color = MixFog(color, input.fogCoord);
                return half4(color, alpha);
                //return half4(maskColor.r, maskColor.r,maskColor.r,1);
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
    CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.UnlitFloor"
}

