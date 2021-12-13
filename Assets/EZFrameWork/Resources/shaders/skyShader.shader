Shader "Unlit/skyShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        // 大気の厚さ （真上）
        _AtmosThicknessV("Atmosphere Thickness Vertical", Range(0,1)) = 0

        // 大気の厚さ （真横）
        _WaveLength("WaveLength", Vector) = (0,0,0,0)

        //_GReduction("g reduction", Range(0,1)) = 0

        //_BReduction("b reduction", Range(0,1)) = 0

        _AtmosThicknessH("Atmosphere Thickness Horizontal", Range(0,1)) = 0


        // 光の色
        _LightColor("Light Color", Color) = (.0, .0, .0, .0)

        //波長
        _Lamda("Lamda", Range(0,1)) = 0

        // 波長
        // 波長
        // 波長


    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            ZWrite OFF
            //ZTest Always
            Cull OFF

            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct appdata members scatter)
//#pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
          //  #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 ltd : TEXCOORD1;
                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 scatter : TEXCOORD1;
            };


            static const float PI = 3.14159265f;

            sampler2D _MainTex;
            float4 _MainTex_ST;
            uniform float4 _LightColor;
            uniform float4 _AtmosThicknessV;
            uniform float4 _AtmosThicknessH;
            uniform float4 _WaveLength;

                float Scatter(float Io, float lamda, float z, float H)
                {
                    //float Io = 1.0f;

                    //空気の屈折率 
                    float n = 1.000292f;

                    //空気の分子数 No
                    float No = 2.54E+25f;

                    //標準状態での空気密度Po
                    float Po = 1.22f;

                    //大気の最高硬度をＨとした場合の、高度zにおける空気密度P
                    float P = Po * exp(-z / H);

                    float s = 32.f * pow(PI, 3.f) * pow((n - 1.f), 2.f);

                    float g = (3.f * No * Po * pow(lamda, 4.f));

                    //レイリー散乱の波長によって決まる散乱係数
                    float cof = s / g;
                    //float cof = 32.f * pow(PI, 3.f) * pow((n - 1.f), 2.f)　/ (3.f * No * Po * pow(lamda, 4.f));

                    //減光率
                    float I = Io * exp(-cof * Po * H); 

                    //距離が増えるほど、exp(-ここは増えていく)つまり、より小さい数になっていく
                    //float reduce = 1f - exp(-cof * Po * H);

                    return I;
                }


            v2f vert (appdata v) 
            {
                v2f o;

                //local coord
                o.vertex = fixed4(v.vertex.xyz, 1.0f);
                o.vertex.w = 1;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                //float r = Scatter(_LightColor.r, _WaveLength.x, 0.f, 8000.f);
                //float g = Scatter(_LightColor.g, _WaveLength.y, 0.f, 8000.f);
                //float b = Scatter(_LightColor.b, _WaveLength.z, 0.f, 8000.f);
                float r = Scatter(_LightColor.r, _WaveLength.x, 0.f, v.ltd.x);
                float g = Scatter(_LightColor.g, _WaveLength.y, 0.f, v.ltd.x);
                float b = Scatter(_LightColor.b, _WaveLength.z, 0.f, v.ltd.x);

                //RGBそれぞれの波長の光について散乱率を求める
                float4 scatter = float4(r,g,b,1.f);

                o.scatter = scatter;

                //UNITY_TRANSFER_FOG(o,o.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //return fixed4(0.4f, 0.4f, 0.9f, 1.0f);
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                 //r channel
                //_LightColor.r *= i.scatter.x;

                //g channel
                //_LightColor.g *= i.scatter.y;

                //g channel
                //_LightColor.b *= i.scatter.z;

                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);

                return i.scatter;
            }

            ENDCG
        }
    }




}
