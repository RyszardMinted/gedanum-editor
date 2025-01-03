Shader "Custom/TextureArrayShader" {
    Properties {
        _MainTexArray("Texture Array", 2DArray) = "" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            UNITY_DECLARE_TEX2DARRAY(_MainTexArray);

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float faceIndex : TEXCOORD1; // Face index (0-5 for 6 faces)
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float faceIndex : TEXCOORD1;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.faceIndex = v.faceIndex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                return UNITY_SAMPLE_TEX2DARRAY(_MainTexArray, float3(i.uv, i.faceIndex));
            }
            ENDCG
        }
    }
}