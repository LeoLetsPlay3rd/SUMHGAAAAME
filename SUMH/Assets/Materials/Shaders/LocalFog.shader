Shader "Custom/LocalFog"
{
    Properties
    {
        _FogColor ("Fog Color", Color) = (1,1,1,0.5)
        _FogDensity ("Fog Density", Range(0,1)) = 0.2
        _Size ("Size", Vector) = (1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _FogColor;
            float _FogDensity;
            float3 _Size;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Calculate fog intensity based on distance from the center
                float3 center = float3(0, 0, 0); // Center of the volume
                float distance = length((i.worldPos - center) / _Size);
                float fog = exp(-_FogDensity * distance);

                // Combine fog with color
                half4 color = _FogColor;
                color.a *= fog;
                return color;
            }
            ENDHLSL
        }
    }
}
