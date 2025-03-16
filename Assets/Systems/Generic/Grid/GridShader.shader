Shader "Custom/GridShader"
{
    Properties
    {
        _GridScale ("Grid Scale", Float) = 2
        _LineThickness ("Line Thickness", Float) = 0.1
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 1)
        _GridColor ("Grid Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off        
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0; // Use UV coordinates of the object
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            float _GridScale;
            float _LineThickness;
            fixed4 _BackgroundColor;
            fixed4 _GridColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Convert UV coordinates into scaled grid space
                float2 scaledUV = i.uv * _GridScale;

                // Calculate grid lines using absolute distance from nearest grid center
                float2 grid = abs(frac(scaledUV + 0.5) - 0.5) / fwidth(scaledUV);
                float lineGrid = min(grid.x, grid.y);

                // Apply thickness
                float alpha = step(_LineThickness, lineGrid);

                // Interpolate between background and grid color
                fixed4 resultColor = lerp(_BackgroundColor, _GridColor, alpha);

                // Ensure proper alpha blending
                resultColor.a = lerp(_BackgroundColor.a, _GridColor.a, alpha);

                return resultColor;
            }
            ENDCG
        }
    }
}