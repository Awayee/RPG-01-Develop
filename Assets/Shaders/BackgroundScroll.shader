Shader "Custom/Background Scroll"{
	Properties{
		_MainTex("Main Texture", 2D) = "white"{}
		_ScrollSpeed("Scroll Speed", Range(0, 1)) = 1
		_Multiplier("Multiplier", float) = 1
	}
	SubShader{
		Pass{
			Tags{"LightMode" = "ForwardBase"}
			CGPROGRAM
			#pragma vertex vert 
			#pragma fragment frag 
			#include "Lighting.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _ScrollSpeed;
			float _Multiplier;
			struct a2v{
				float4 vertex:POSITION;
				float4 texcoord:TEXCOORD0;
			};
			struct v2f{
				float4 pos:SV_POSITION;
				float2 uv:TEXCOORD0;
			};
			void vert(in a2v v, out v2f o){
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex) - frac(float2(_ScrollSpeed, 0) * _Time.y);
			}
			fixed4 frag(v2f i):SV_TARGET{
				fixed4 baseColor = tex2D(_MainTex, i.uv);
				baseColor *= _Multiplier;
				return baseColor;
			}
			ENDCG
		}
	}
}