Shader "Custom/LghtMap"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LightMap ("LightMap", 2D) = "white" {}
		
		_SampleCount ("SampleCount", Int) = 3
		_Brightness ("Brightness", Float) = 1.0
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
			half _Brightness;
			float _resoX;
			float _resoY;
			half _SampleCount;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 light = 0;
				float2 gridSize = float2(_resoX / 16.0, _resoY / 10.0);

				float2 size = float2(gridSize.x /_resoX, gridSize.y / _resoY);
				for(half y = -_SampleCount; y <= _SampleCount; ++y)
				{
					for(half x = -_SampleCount; x <= _SampleCount; ++x)
					{
						fixed effective = clamp((_SampleCount + 1) - (abs(x) + abs(y)), 0, _SampleCount + 1);
						fixed2 offset = fixed2(x * size.x, y * size.y);
						light += tex2D(_LightMap, i.uv + offset) * effective;
					}
				}
				light /= (_SampleCount * 2 + 1) * (_SampleCount * 2 + 1);
				light *= _Brightness;
				float finalVal = saturate(light.r);

				col = lerp(fixed4(0, 0, 0, 0), col, finalVal) ;
				return col;
			}
			ENDCG
		}
	}
}
