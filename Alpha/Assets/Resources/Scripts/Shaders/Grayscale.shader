Shader "Custom/Gray" {
	Properties{
		_Color("Main Color (A=Opacity)", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "Grey" {}
	}
	SubShader{
		Tags{ "RenderType" = "Transparent" }
		LOD 200
		CGPROGRAM
		#pragma surface surf Lambert alpha
			sampler2D _MainTex;
			float4 _Color;

			struct Input {
				float2 uv_MainTex;
			};

			void surf(Input IN, inout SurfaceOutput o) {

				half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				
				float alpha = c.a;

				float grayScale = Luminance(c.rgb);

				float4 saturated;
				for (int i = 0; i < 4; i += 1)
				{
					saturated = ((c+1)*(c+1))*(grayScale);
					c = saturated;
				}

				o.Albedo = c.rgb;
				o.Alpha = alpha;
			}
		ENDCG
	}
	Fallback "Diffuse"
}