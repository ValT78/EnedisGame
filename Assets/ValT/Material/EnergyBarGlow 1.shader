Shader "Custom/EnergyBarGlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowIntensity ("Glow Intensity", Float) = 1
        _GlowSpeed ("Glow Speed", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert alpha

        sampler2D _MainTex;
        fixed4 _GlowColor;
        float _GlowIntensity;
        float _GlowSpeed;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            
            // Ajouter un dégradé qui défile (comme un "scanner")
            float glow = sin(_GlowSpeed * _Time.y + IN.uv_MainTex.y * 10) * _GlowIntensity;
            glow = saturate(glow); // Clamp entre 0 et 1
            
            o.Albedo = c.rgb;
            o.Emission = _GlowColor.rgb * glow;
            o.Alpha = c.a;
        }
        ENDCG
    }
}
