Shader "Unlit/2DSkyShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        //_AtmosThicknessH("Atmosphere Thickness Horizontal", Range(0,1)) = 0

        // 光の色
        //_LightColor("Light Color", Color) = (.0, .0, .0, .0)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            ZWrite OFF
            Cull OFF

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 ltd : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                //float4 scatter : TEXCOORD1;
            };


            static const float PI = 3.14159265f;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v) 
            {
                v2f o;

                //OpenGLとDirectXの違いを考慮。OpenGLでは画面上が−１、下が1だからビルドイン変数_ProjectionParamsを使って反転。
                //OpenGLの時は.xに-1、 DirectXの時は.xに１               
                v.vertex.y *= _ProjectionParams.x;

                o.vertex = fixed4(v.vertex.xyz, 1.0f);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;

                //o.scatter = scatter;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);


                return i.color;
            }

            ENDCG
        }
    }




}
