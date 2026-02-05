Shader "Hidden/SimpleBloom" {
    Properties { _MainTex ("", 2D) = "white" {} }
    CGINCLUDE
    #include "UnityCG.cginc"
    sampler2D _MainTex;
    float4 _MainTex_TexelSize;
    half _Threshold;
    half _Intensity;

    half4 frag_blur(v2f_img i) : SV_Target {
        half4 d = _MainTex_TexelSize.xyxy * half4(-1, -1, 1, 1);
        half4 s = tex2D(_MainTex, i.uv + d.xy);
        s += tex2D(_MainTex, i.uv + d.zy);
        s += tex2D(_MainTex, i.uv + d.xw);
        s += tex2D(_MainTex, i.uv + d.zw);
        return s * 0.25h;
    }

    half4 frag_threshold(v2f_img i) : SV_Target {
        half4 color = tex2D(_MainTex, i.uv);

        float brightness = max(color.r, max(color.g, color.b));
        float contribution = max(0, brightness - _Threshold);
        contribution /= max(brightness, 0.00001);
        return color * contribution;
    }
    

    half4 frag_composite(v2f_img i) : SV_Target {
        half4 original = tex2D(_MainTex, i.uv);
        half4 bloom = tex2D(sampler2D(_BloomTex), i.uv); 
        return original + bloom * _Intensity;
    }

    ENDCG
    SubShader {
        ZTest Always Cull Off ZWrite Off
        Pass { CGPROGRAM #pragma vertex vert_img #pragma fragment frag_threshold ENDCG }
        Pass { CGPROGRAM #pragma vertex vert_img #pragma fragment frag_blur ENDCG }
        Pass { CGPROGRAM #pragma vertex vert_img #pragma fragment frag_composite ENDCG }
    }
    Fallback off
}