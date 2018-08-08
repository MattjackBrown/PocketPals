// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/PPalFirstShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NoiseTex("NoiseTexture", 2D) = "white" {}
		_BlendTex("BlendTexture", 2D) = "white" {}
		_Tint("NightMultiplier", Color) = (1,1,1,1)
	    playerPos("playerPos", Vector) = (0, 0, 0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 pos : TEXCOORD3;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _NoiseTex;
			sampler2D _BlendTex;
			float4 _MainTex_ST;
			float4 playerPos;
			fixed4 _Tint;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.pos = mul(unity_ObjectToWorld, v.vertex);


				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col1 = tex2D(_NoiseTex, i.uv);
				fixed4 col2 = tex2D(_BlendTex, i.uv);
				
				float dist = clamp(distance(i.pos, playerPos), 0.1, 12);

				col1 = col1 * (dist / 10);
				col1 = 1-clamp(col1, 0.1, 7);
				//col2 = col2 *  dist;
				//col = (col * col1*0.9)+ (col2*0.05);

				col = ((col*2.5) +(col1*0.45)+(col2*0.05))/3;

				col = col * _Tint;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				
				return col;
			}
			ENDCG
		}
	}
}
