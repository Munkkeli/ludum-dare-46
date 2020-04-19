Shader "Unlit/Absolute Circle" {
	Properties{
		_BackgroundColor("Background Color", Color) = (0,0,0,1)
		_ForegroundMask("Foreground Mask", 2D) = "white" {}
		_ForegroundCutoff("Foreground Cutoff", Range(0,1)) = 0.5
		_BackgroundCutoff("Background Cutoff", Range(0,1)) = 0.5
		_PulseSpeed("Pulse Speed", Float) = 1
		_Pulse("Pulse", Range(0,1)) = 0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Unlit

		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
    {
         return half4(s.Albedo, s.Alpha);
    }

		sampler2D _ForegroundMask;

	struct Input {
		float2 uv_ForegroundMask;
	};

	float _PulseSpeed;
	float _Pulse;
	fixed4 _BackgroundColor;
	half _ForegroundCutoff;
	half _BackgroundCutoff;

	void surf(Input IN, inout SurfaceOutput  o) {

		fixed x = (-0.5 + IN.uv_ForegroundMask.x) * 2;
		fixed y = (-0.5 + IN.uv_ForegroundMask.y) * 2;

		// Albedo comes from a texture tinted by color
		fixed radius = 1 - sqrt(x*x + y*y);
		clip(radius - _BackgroundCutoff);
		o.Albedo = _BackgroundColor + (0.5 * sin(_PulseSpeed * _Time.y) + 0.5) * _Pulse;
		if (radius > _ForegroundCutoff) {
			clip(-1);
		}
		o.Alpha = 1;
	}
	ENDCG
	}
	FallBack "Diffuse"
}