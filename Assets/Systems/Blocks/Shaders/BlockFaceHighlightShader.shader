Shader "Custom/BlockFaceHighlight"
{
    Properties
    {
        _MainTexArray ("Texture Array", 2DArray) = "" {}
        _HighlightColor ("Highlight Color", Color) = (1, 1, 1, 0.5)
        _HighlightedFace ("Highlighted Face", Int) = -1
        _SelectedBlock ("Selected Block Position", Vector) = (0,0,0,0)
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1; // texture array index
                float4 color : COLOR; // face ID
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD3; // world position from UV3
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
            };

            UNITY_DECLARE_TEX2DARRAY(_MainTexArray);
            float4 _MainTexArray_ST;
            float4 _HighlightColor;
            int _HighlightedFace;
            float4 _SelectedBlock;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTexArray);
                o.uv2 = v.uv2;
                o.color = v.color;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = v.worldPos; // Use the world position from UV3
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = UNITY_SAMPLE_TEX2DARRAY(_MainTexArray, float3(i.uv, i.uv2.x));
                
                float3 blockPos = float3(_SelectedBlock.x, _SelectedBlock.y, _SelectedBlock.z);
                float3 facePos = ceil(i.worldPos - 0.5); // Use ceil with offset to handle floating point precision
                
                if (i.color.r * 255 == _HighlightedFace && all(facePos == blockPos))
                {
                    col = lerp(col, _HighlightColor, _HighlightColor.a);
                }
                
                return col;
            }
            ENDCG
        }
    }
} 