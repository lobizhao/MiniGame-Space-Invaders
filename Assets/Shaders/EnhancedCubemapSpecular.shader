Shader "Custom/EnhancedCubemapSpecular" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        [PowerSlider(5.0)] _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
        
        [Header(Custom Reflection)]
        _Cube ("Cubemap", CUBE) = "" {}
        _CubeIntensity ("Reflection Intensity", Range(1, 5)) = 1.0
    }
    
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 400
        
        CGPROGRAM

        #pragma surface surf BlinnPhong
        #pragma target 3.0

        sampler2D _MainTex;
        samplerCUBE _Cube;
        fixed4 _Color;
        half _Shininess;
        half _CubeIntensity;

        struct Input {
            float2 uv_MainTex;
            float3 worldRefl;
            INTERNAL_DATA
        };

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = tex.rgb * _Color.rgb;
        
            o.Gloss = tex.a;
            o.Specular = _Shininess;
            float3 worldRefl = WorldReflectionVector (IN, o.Normal);

            fixed4 reflcol = texCUBE (_Cube, worldRefl) * _CubeIntensity;
            o.Emission = reflcol.rgb;
            o.Alpha = tex.a * _Color.a;
        }
        ENDCG
    }

    FallBack "Legacy Shaders/Specular"
}