Shader "Custom/TransparentGlowingLampShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _Transparency ("Transparency", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            float4 _BaseColor;
            float4 _EmissionColor;
            float _Transparency;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                return o;
            }

            half4 frag(Varyings i) : SV_TARGET
            {
                // Combine base color and emission
                half3 finalColor = _BaseColor.rgb + _EmissionColor.rgb;
                return half4(finalColor, _Transparency); // Use transparency for alpha
            }
            ENDHLSL
        }
    }
    FallBack "UniversalRenderPipeline/Lit"
}
