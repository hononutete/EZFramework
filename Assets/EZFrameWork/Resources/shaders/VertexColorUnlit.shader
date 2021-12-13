Shader "VertexColorTreesSet/VertexColorUnlit" {
Properties {
	_MainTex ("Texture", 2D) = "white" {}
}
 
Category {
	Tags { "Queue"="Geometry" }
	//Lighting Off

    
	
	SubShader {
		Pass {

            ZWrite On

            CGPROGRAM
			#pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR0;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                //half3 worldNormal : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR0;
                //float4 normal : NORMAL;
                half3 worldNormal : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v) 
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.vertex);

                // ワールド空間の法線に変換（正規ベクトル）
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldNormal = worldNormal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col = fixed4(i.color.xyz, 1);

                // calc color by normal
                float diffuse = dot (i.worldNormal, _WorldSpaceLightPos0.xyz);
                diffuse = clamp(diffuse, 0.f, 1.f);
                //return fixed4(diffuse,diffuse,diffuse,1);
                

                //ambient color, this already includes the intensity of the light
                col *= fixed4(unity_AmbientSky.rgb,1);

                fixed4 diffuseRange = fixed4(1,1,1,1) - col;
                fixed4 diffuseAdd = diffuseRange * diffuse;

                col *= diffuse;

                //col *= i.color;
                return col;
            }

            ENDCG
		}
	}
}
}