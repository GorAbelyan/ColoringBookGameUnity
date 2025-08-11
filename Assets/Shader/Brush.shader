Shader "Custom/Brush"
{
    Properties
    {
        _BrushTex ("Brush Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _BrushSize ("Brush Size", Float) = 0.1
        _BrushPos ("Brush Pos", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {

            ZWrite Off
           Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            sampler2D _BrushTex;
            fixed4 _Color;
            float _BrushSize;
            float4 _BrushPos; // UV

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            
   fixed4 frag (v2f i) : SV_Target
{
    float alpha = tex2D(_BrushTex, i.uv).a;
    if (alpha < 0.01) discard;
    return float4(_Color.rgb, 1);
}



            ENDCG
        }
    }
}
