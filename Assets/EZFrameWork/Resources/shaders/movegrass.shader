Shader "Unlit/movegrass"
{
    Properties
    {
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
        _Wind("Wind", Vector) = (0,0,0,0)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
	[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
    }
    SubShader
    {
        Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
        LOD 100

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            // make fog work
            #pragma multi_compile_fog

            // インスタンシング用のpragma
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"


			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
                float2 windWeight : TEXCOORD1;
                // インスタンスIDを頂点情報として受け取る
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;

                // フラグメントシェーダにインスタンスIDを受け渡す
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _RendererColor;
			fixed2 _Flip;
			float _EnableExternalAlpha;
            fixed4 _Wind;
			fixed4 _Color;

            // メッシュごとに変えるプロパティはここに定義する
            //UNITY_INSTANCING_BUFFER_START(Props)
            //UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            //UNITY_INSTANCING_BUFFER_END(Props)

			inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
			{
				return float4(pos.xy * flip, pos.z, 1.0);
			}

			v2f SpriteVert(appdata_t IN)
			{
				v2f OUT;

                // インスタンスIDを元に位置やスケールを反映
				UNITY_SETUP_INSTANCE_ID(IN);
				// フラグメントシェーダにプロパティを受け渡す
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                //consider wind effect
                fixed4 v = IN.vertex + _Wind * IN.vertex.y;

				OUT.vertex = UnityFlipSprite(v, _Flip);
				OUT.vertex = UnityObjectToClipPos(OUT.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color * _RendererColor;

                


				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 SampleSpriteTexture(float2 uv)
			{
                // フラグメントシェーダにプロパティを受け渡す

				fixed4 color = tex2D(_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				fixed4 alpha = tex2D(_AlphaTex, uv);
				color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
#endif

				return color;
			}


			fixed4 SpriteFrag(v2f IN) : SV_Target
			{
                // インスタンスIDに応じた色を取得する
                UNITY_SETUP_INSTANCE_ID(IN);

				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
				c.rgb *= c.a;

                //ambient color, this already includes the intensity of the light
                c *= fixed4(unity_AmbientSky.rgb,1);

				return c;
			}


            ENDCG
        }
    }
}
