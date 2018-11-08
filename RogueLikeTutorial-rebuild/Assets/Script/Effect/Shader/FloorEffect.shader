﻿Shader "Custom/FloorEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_GridTex ("GridTex", 2D) = "white" {}
		_EffectValue ("EffectValue", Range(0, 1)) = 1
		_Color1 ("Color1", Color) = (0, 0, 0, 0)
		_Color2 ("Color2", Color) = (0, 0, 0, 0)

		[Space(20)]
		_CurrentLightUp("CurrentLightUp", Float) = 0
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
			#pragma multi_compile_instancing
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(int, _ColorIndex)
            UNITY_INSTANCING_BUFFER_END(Props)

			v2f vert (appdata v)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _GridTex;
			fixed4 _Color1;
			fixed4 _Color2;
			fixed _EffectValue;
			fixed _CurrentLightUp;

			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				int index = UNITY_ACCESS_INSTANCED_PROP(Props, _ColorIndex);
				fixed4 colors[2];
				colors[0] = _Color1;
				colors[1] = _Color2;
				fixed lerpVal = index % 2.0;
				_CurrentLightUp %= 2.0;
				lerpVal = abs(_CurrentLightUp - lerpVal);
				colors[index] = lerp(colors[index], fixed4(1, 1, 1, 1), lerpVal);

				fixed4 texOrign = tex2D(_MainTex, i.uv);
				fixed4 grid = tex2D(_GridTex, i.uv);
				fixed4 finalGrid = texOrign * colors[index];
				finalGrid = lerp(finalGrid, grid * colors[index], grid.a * (1 - lerpVal));
				fixed4 col = lerp(texOrign, finalGrid, _EffectValue);
				return col;
			}
			ENDCG
		}
	}
}
