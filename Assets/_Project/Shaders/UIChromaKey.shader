// UI shader that removes a green-screen background at runtime and removes green spill from edges.
// Used on the victory banner RawImage so the video's green becomes transparent.
Shader "SWE402/UI Chroma Key"
{
    Properties
    {
        [PerRendererData] _MainTex ("Video Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _KeyLow ("Key Start (how green before fading)", Range(0, 1)) = 0.08
        _KeyHigh ("Key End (fully transparent)", Range(0, 1)) = 0.40
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _KeyLow;
            float _KeyHigh;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                // how much greener than red/blue: ~1 on the green screen, <= 0 on banner pixels
                float key = c.g - max(c.r, c.b);
                float alpha = 1.0 - smoothstep(_KeyLow, _KeyHigh, key);
                // despill: pull leftover green out of edge pixels
                c.g = min(c.g, (c.r + c.b) * 0.5);
                c.rgb *= i.color.rgb;
                c.a = alpha * i.color.a;
                return c;
            }
            ENDCG
        }
    }
}
