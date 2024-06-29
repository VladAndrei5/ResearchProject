Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range (0, 1)) = 0.1
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Unlit alpha

        half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
            return half4(s.Albedo, s.Alpha);
        }

        sampler2D _MainTex;
        fixed4 _OutlineColor;
        float _OutlineWidth;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;

            fixed4 pixelUp = tex2D(_MainTex, IN.uv_MainTex + fixed2(0, _OutlineWidth));
            fixed4 pixelDown = tex2D(_MainTex, IN.uv_MainTex - fixed2(0, _OutlineWidth));
            fixed4 pixelRight = tex2D(_MainTex, IN.uv_MainTex + fixed2(_OutlineWidth, 0));
            fixed4 pixelLeft = tex2D(_MainTex, IN.uv_MainTex - fixed2(_OutlineWidth, 0));

            if (c.a < 1 && (pixelUp.a == 1 || pixelDown.a == 1 || pixelRight.a == 1 || pixelLeft.a == 1))
            {
                o.Albedo = _OutlineColor.rgb;
                o.Alpha = 1;
            }
        }
        ENDCG
    }
    FallBack "Unlit/Transparent"
}