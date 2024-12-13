Shader "Custom/EmissiveOutline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1, 1, 0, 1)
        _OutlineWidth ("Outline Width", Float) = 0.03
        _EmissionIntensity ("Emission Intensity", Float) = 2.0
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Pass
        {
            Cull Front
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            float _OutlineWidth;
            float4 _OutlineColor;
            float _EmissionIntensity;

            v2f vert(appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal) * _OutlineWidth;
                v.vertex.xyz += norm;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = _OutlineColor * _EmissionIntensity;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
