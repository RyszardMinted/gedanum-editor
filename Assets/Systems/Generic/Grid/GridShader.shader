Shader "Custom/GridShader"
{
    Properties
    {
        _GridColor("Grid Color", Color) = (0.5, 0.5, 0.5, 1)
        _BackgroundColor("Background Color", Color) = (0, 0, 0, 0) // Transparent background
        _Scale("Grid Scale", Float) = 1
        _LineThickness("Line Thickness", Float) = 0.05
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            AlphaTest Greater 0.1 // Ensures alpha cutoff

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _GridColor;
            fixed4 _BackgroundColor;
            float _Scale;
            float _LineThickness;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xz * _Scale;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 grid = abs(frac(i.uv - 0.5) - 0.5) / fwidth(i.uv); // Line width calculations
                float lineGrid = min(grid.x, grid.y);

                // Make lines opaque and background transparent
                float alpha = step(_LineThickness, lineGrid);
                return lerp(_BackgroundColor, _GridColor, alpha);
            }
            ENDCG
        }
    }
}