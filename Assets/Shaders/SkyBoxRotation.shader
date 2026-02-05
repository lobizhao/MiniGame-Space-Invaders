Shader "Custom/RotatingSkybox"
{
    Properties
    {
        _Tint ("Tint Color", Color) = (0.5, 0.5, 0.5, 1)
        _Tex ("Cubemap (HDR)", Cube) = "grey" {}
        _RotationSpeed ("Rotation Speed", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            samplerCUBE _Tex;
            fixed4 _Tint;
            float _RotationSpeed;

            struct appdata_t {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float3 texcoord : TEXCOORD0;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
            
                float rot = _Time.y * _RotationSpeed; 
                float s = sin(rot);
                float c = cos(rot);
                float2x2 rotationMatrix = float2x2(c, -s, s, c);
                
                float3 texcoord = v.vertex.xyz;
                texcoord.xz = mul(rotationMatrix, texcoord.xz);
                o.texcoord = texcoord;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = texCUBE(_Tex, i.texcoord);
                return tex * _Tint;
            }
            ENDCG
        }
    }
    Fallback Off
}