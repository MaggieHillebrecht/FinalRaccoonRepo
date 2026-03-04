
 Shader "Custom/ChromaKeyShader" {
        Properties
        {
        _MainTex ("Video Texture", 2D) = "white" {}
        _KeyColor ("Key Color", Color) = (0,1,0,1)  // green background
        _Threshold ("Threshold", Range(0,0.2)) = 0.02
        _Smoothness ("Smoothness", Range(0,0.1)) = 0.01
        _EdgeFeather ("Edge Feather", Range(0,0.1)) = 0.01
        }

        SubShader
        {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                struct Attributes
                {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                };

                struct Varyings
                {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                };

                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);

                float4 _KeyColor;
                float _Threshold;
                float _Smoothness;
                float _EdgeFeather;

                Varyings vert(Attributes IN)
                {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                return OUT;
                }

                half4 frag(Varyings IN) : SV_Target
                {
                // Sample video
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // Compute weighted distance from key color (green)
                float dist = sqrt(
                        0.25*(col.r - _KeyColor.r)*(col.r - _KeyColor.r) +
                        0.5*(col.g - _KeyColor.g)*(col.g - _KeyColor.g) +
                        0.25*(col.b - _KeyColor.b)*(col.b - _KeyColor.b)
                );

                // Smooth alpha transition
                float alpha = smoothstep(_Threshold, _Threshold + _Smoothness, dist);

                // Optional edge feathering for softer edges
                alpha = smoothstep(_Threshold, _Threshold + _EdgeFeather + _Smoothness, dist);

                col.a = alpha; // final alpha: 1 = visible, 0 = transparent

                return col;
                }
                ENDHLSL
        }
        }
}