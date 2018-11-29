Shader "Custom/BlendPostProcess"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LightMap ("LightMap", 2D) = "white" {}
		_EmissionMap ("EmissionMap", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _LightMap;
			sampler2D _EmissionMap;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 light = tex2D(_LightMap, i.uv);
				fixed4 emission = tex2D(_EmissionMap, i.uv);

				light += emission;
				float finalVal = saturate(light.a);

				col = lerp(fixed4(0, 0, 0, 0), col, finalVal) ;
				return col;
			}
			ENDCG
		}
	}
}
