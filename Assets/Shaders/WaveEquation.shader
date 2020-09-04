Shader "Custom/WaveEquation"
{
	Properties
	{
		_InputTex("Input", 2D) = "black" {}
		_PrevTex("Prev", 2D) = "black" {}
		_PrevPrevTex("PrevPrev", 2D) = "black" {}
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D_half _InputTex;
			sampler2D_half _PrevTex;
			sampler2D_half _PrevPrevTex;
			float2 _Stride;

			v2f vert(appdata_img v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 stride = _Stride;
				half4 prev = (tex2D(_PrevTex, i.uv) * 2) - 1;
				half value =
					(prev.r * 2 -
						(tex2D(_PrevPrevTex, i.uv).r * 2 - 1) +
						((tex2D(_PrevTex, half2(i.uv.x + stride.x, i.uv.y)).r * 2 - 1) +
						 (tex2D(_PrevTex, half2(i.uv.x - stride.x, i.uv.y)).r * 2 - 1) +
						 (tex2D(_PrevTex, half2(i.uv.x, i.uv.y + stride.y)).r * 2 - 1) +
						 (tex2D(_PrevTex, half2(i.uv.x, i.uv.y - stride.y)).r * 2 - 1) -
						 prev.r * 4) * (0.1));
				value += (tex2D(_InputTex, i.uv).r * 2) - 1;
				value *= 0.992;
				value = (value + 1) * 0.5;
				return fixed4(value, value, value, value);
			}
			
			//fixed4 frag(v2f i) : SV_Target
			//{
			//	float2 stride = _Stride;
			//	float4 prev = tex2D(_PrevTex, i.uv);
			//	float value =
			//		(prev.r * 2 -
			//			tex2D(_PrevPrevTex, i.uv).r +
			//			(tex2D(_PrevTex, float2(i.uv.x + stride.x, i.uv.y)).r +
			//			 tex2D(_PrevTex, float2(i.uv.x - stride.x, i.uv.y)).r +
			//			 tex2D(_PrevTex, float2(i.uv.x, i.uv.y + stride.y)).r +
			//			 tex2D(_PrevTex, float2(i.uv.x, i.uv.y - stride.y)).r -
			//			 prev.r * 4) * (0.001));
			//	value += tex2D(_InputTex, i.uv).r;
			//	value *= 0.992;
			//	return fixed4(value, value, value, value);
			//}
			ENDCG
		}
	}
}
