Shader "Custom/LegacyReflectiveSpecularWithEmission" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        [PowerSlider(5.0)] _Shininess ("Shininess", Range (0.01, 1)) = 0.078125
        _ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
        _MainTex ("Base (RGB) RefStrGloss (A)", 2D) = "white" {}
        _Cube ("Reflection Cubemap", Cube) = "_Skybox" {}
        
        [Header(Emission Settings)]
        _EmissionMap ("Emission Map", 2D) = "black" {}
        _EmissionIntensity ("Emission Intensity", Range(0, 3)) = 1.0
    }

    SubShader {
        LOD 300
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf BlinnPhong
        #pragma target 3.0

        sampler2D _MainTex;
        samplerCUBE _Cube;
        fixed4 _Color;
        fixed4 _ReflectColor;
        half _Shininess;
        
        sampler2D _EmissionMap;
        half _EmissionIntensity;

        struct Input {
            float2 uv_MainTex;
            float3 worldRefl;
        };

        void surf (Input IN, inout SurfaceOutput o) {

            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 c = tex * _Color;
            o.Albedo = c.rgb;
            
            o.Gloss = tex.a;
            o.Specular = _Shininess;
            

            fixed4 reflcol = texCUBE (_Cube, IN.worldRefl);
            reflcol *= tex.a;
            reflcol *= _ReflectColor;
            
            fixed4 emissionCol = tex2D(_EmissionMap, IN.uv_MainTex) * _EmissionIntensity;
            
            o.Emission = reflcol.rgb + emissionCol.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Legacy Shaders/Reflective/VertexLit"
}