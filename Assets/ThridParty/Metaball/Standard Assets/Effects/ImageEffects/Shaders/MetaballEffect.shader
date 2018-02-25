// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/MetaballEffect"
{
	Properties
	{
		_MainTex ("", 2D) = "" {}
		_MetaballTex ("", 2D) = "" {}
		_Color ("", Color) = (1, 1, 1, 1)
	}
	CGINCLUDE
	#include "UnityCG.cginc"
	struct v2f {
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		half2 taps[4] : TEXCOORD1; 
	};
	sampler2D _MetaballTex;
	sampler2D _MainTex;
	half4 _MainTex_TexelSize;
	half4 _BlurOffsets;
	half4 _Color;
	v2f vert( appdata_img v ) {
		v2f o; 
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord - _BlurOffsets.xy * _MainTex_TexelSize.xy; // hack, see BlurEffect.cs for the reason for this. let's make a new blur effect soon
		o.taps[0] = o.uv + _MainTex_TexelSize * _BlurOffsets.xy;
		o.taps[1] = o.uv - _MainTex_TexelSize * _BlurOffsets.xy;
		o.taps[2] = o.uv + _MainTex_TexelSize * _BlurOffsets.xy * half2(1,-1);
		o.taps[3] = o.uv - _MainTex_TexelSize * _BlurOffsets.xy * half2(1,-1);
		return o;
	}
	half4 frag(v2f i) : SV_Target {
		half4 original = tex2D(_MainTex, i.uv);
		half4 color = tex2D(_MetaballTex, i.taps[0]);
		color += tex2D(_MetaballTex, i.taps[1]);
		color += tex2D(_MetaballTex, i.taps[2]);
		color += tex2D(_MetaballTex, i.taps[3]); 

		if (color.a > .15f)
		{
			float ca = color.a;
			color = _Color;
			color.a = _Color.a + min(.0, ca - .8) + .2;
		}

		//color blending:
		half4 finalColor = (1 - color.a) * original * original.a + color * color.a;

		return finalColor;
	}
	ENDCG
	SubShader {
		 Pass {
			  ZTest Always Cull Off ZWrite Off

			  CGPROGRAM
			  #pragma vertex vert
			  #pragma fragment frag
			  ENDCG
		  }
	}
	Fallback off
}
