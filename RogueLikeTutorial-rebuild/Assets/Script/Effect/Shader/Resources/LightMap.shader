Shader "Custom/LightMap"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Light ("Light", 2D) = "white" {}
		_LightMap ("LightMap", 2D) = "white" {}
		_EmissionMap ("EmissionMap", 2D) = "white" {}
		_LastFrame ("LastFrame", 2D) = "white" {}
		_OldFrame ("OldFrame", 2D) = "white" {}
		_BlendOffSet ("BlendOffSet", Vector) = (0, 0, 0, 0)
		
		_SampleCount ("SampleCount", Int) = 3
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
			float _SampleCount;
			float _BlendRateLast;
			float _BlendRateOld;
			float4 _BlendOffSet;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 light = fixed4(0, 0, 0, 0);
				float2 gridSize = float2(_resoX / 16.0, _resoY / 10.0);
				float2 size = float2(gridSize.x /_resoX, gridSize.y / _resoY);
				for(float y = -_SampleCount; y <= _SampleCount; ++y)
				{
					for(float x = -_SampleCount; x <= _SampleCount; ++x)
					{
						fixed effective = clamp((_SampleCount + 1) - (abs(x) + abs(y)), 0, _SampleCount + 1);
						fixed2 offset = i.uv + fixed2(x * size.x, y * size.y);
						fixed4 color = tex2D(_Light, offset) * effective * effective;
						//color = offset.x > 1 ? fixed4(0, 0, 0, 0) : color;
						//color = offset.x < 0 ? fixed4(0, 0, 0, 0) : color;
						//color = offset.y > 1 ? fixed4(0, 0, 0, 0) : color;
						//color = offset.y < 0 ? fixed4(0, 0, 0, 0) : color;
						light += color;
					}
				}
				light /= (_SampleCount * 2 + 1) * (_SampleCount * 2 + 1);
				float2 blendOffset = float2(_BlendOffSet.x * size.x, _BlendOffSet.y * size.y);
				fixed4 lastFrame = tex2D(_LastFrame, i.uv + blendOffset);
				fixed4 oldFrame = tex2D(_OldFrame, i.uv + blendOffset);
				light += lastFrame * _BlendRateLast + oldFrame * _BlendRateOld;
				light *= _Brightness;
				return saturate(light);
			}
			ENDCG
		}

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
