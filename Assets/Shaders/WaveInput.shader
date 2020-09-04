Shader "Custom/WaveInput"
{
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		Cull Off ZWrite Off ZTest Always
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half value : COLOR;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.value = (v.vertex.z * 0.5) + 0.5;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed value = i.value;
				return fixed4(value, value, value, value);
			}
			ENDCG
		}
	}
}
