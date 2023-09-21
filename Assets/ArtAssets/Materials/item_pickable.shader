Shader "Custom/item_pickable"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _OutlineScale ("Outline Scale", Float) = 0.2
        _AlphaThreshold ("Alpha threshold", Range(0.0,1.0)) = 0.2
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _OutlineColor;
            float _OutlineScale;
            float _AlphaThreshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {

                // sample the texture
                float4 col = tex2D(_MainTex, i.uv);
                clip(col.a - _AlphaThreshold);

                float2 uvOutlineOffset = (i.uv - 0.5) * _OutlineScale + 0.5;
                float4 colScaled = tex2D(_MainTex, uvOutlineOffset);

                float4 finalCol = float4(lerp(_OutlineColor.rgb, colScaled.rgb, colScaled.a), 1);

                //return finalCol;
                return finalCol;
            }
            ENDCG
        }
    }
}
