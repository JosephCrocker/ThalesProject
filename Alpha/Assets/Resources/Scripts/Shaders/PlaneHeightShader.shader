Shader "Custom/PlaneHeightShader" {
	Properties{
		_HighColor("High Color", Color) = (1,0,0,1)
		_MediumColor("Medium Color", Color) = (0,0,1,1)
		_LowColor("Low Color", Color) = (0,1,0,1)


		_HighHeight("High Height", Float) = 18
		_MediumHeight("Medium Height", Float) = 10
		_LowHeight("Low Height", Float) = 0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		
		fixed4 _LowColor;
		fixed4 _MediumColor;
		fixed4 _HighColor;

		fixed _HighHeight;
		fixed _MediumHeight;
		fixed _LowHeight;

		struct Input {
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			if (IN.worldPos.y > _LowHeight)
			{
				o.Albedo = _LowColor.rgb;
				o.Alpha = _LowColor.a;
			}
			if (IN.worldPos.y > _MediumHeight)
			{
				o.Albedo = _MediumColor.rgb;
				o.Alpha = _MediumColor.a;
			}
			if (IN.worldPos.y > _HighHeight)
			{
				o.Albedo = _HighColor.rgb;
				o.Alpha = _HighColor.a;
			}
			
		}
		ENDCG
	}
	FallBack "Diffuse"
}
