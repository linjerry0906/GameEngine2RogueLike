Shader "Custom/LightMap"
{
	Properties
	{
		_Light ("Light", 2D) = "white" {}
		_LastFrame ("LastFrame", 2D) = "white" {}
		_OldFrame ("OldFrame", 2D) = "white" {}
		_BlendOffSet ("BlendOffSet", Vector) = (0, 0, 0, 0)
		
		_Brightness ("Brightness", Float) = 1.0
		_BlendRateLast ("BlendRateLast", Range(0, 1)) = 0.5
		_BlendRateOld ("BlendRateOld", Range(0, 1)) = 0.3
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
			
			sampler2D _Light;
			sampler2D _LastFrame;
			sampler2D _OldFrame;
			float _Brightness;
			float _resoX;
			float _resoY;
			static const float _SampleCount = 6.0;
			static const float _TotalSample = 13.0;
			float _BlendRateLast;
			float _BlendRateOld;
			float4 _BlendOffSet;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 light = fixed4(0, 0, 0, 0);
				float2 gridSize = float2(_resoX / 16.0, _resoY / 10.0);
				float2 size = float2(gridSize.x /_resoX, gridSize.y / _resoY);
				for(fixed y = 0; y <= _TotalSample; ++y)
				{
					for(fixed x = 0; x <= _TotalSample; ++x)
					{
						fixed sampleX = x - _SampleCount;
						fixed sampleY = y - _SampleCount;
						fixed effective = clamp((_SampleCount + 1) - (abs(sampleX) + abs(sampleY)), 0, _SampleCount + 1);
						fixed2 offset = i.uv + fixed2(sampleX * size.x, sampleY * size.y);
						fixed4 color = tex2D(_Light, offset) * effective * effective;
						//color = offset.x > 1 ? fixed4(0, 0, 0, 0) : color;
						//color = offset.x < 0 ? fixed4(0, 0, 0, 0) : color;
						//color = offset.y > 1 ? fixed4(0, 0, 0, 0) : color;
						//color = offset.y < 0 ? fixed4(0, 0, 0, 0) : color;
						light += color;
					}
				}
				light /= _TotalSample * _TotalSample;
				float2 blendOffset = float2(_BlendOffSet.x * size.x, _BlendOffSet.y * size.y);
				fixed4 lastFrame = tex2D(_LastFrame, i.uv + blendOffset);
				fixed4 oldFrame = tex2D(_OldFrame, i.uv + blendOffset);
				light += lastFrame * _BlendRateLast + oldFrame * _BlendRateOld;
				light *= _Brightness;
				return saturate(light);
			}
			ENDCG
		}
	}
}
