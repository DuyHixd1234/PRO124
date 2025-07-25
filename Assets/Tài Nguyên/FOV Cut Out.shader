Shader "Custom/FOVCutoutShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _MaskTex ("Mask (RGB)", 2D) = "white" {}
        _Color ("Overlay Color", Color) = (0,0,0,0.7)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _MaskTex;
            fixed4 _Color;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Lấy giá trị mask tại pixel hiện tại
                fixed4 mask = tex2D(_MaskTex, i.uv);

                // Nếu mask trắng (1.0), thì alpha = 0 => vùng trong tầm nhìn
                // Nếu mask đen (0.0), thì alpha giữ nguyên = tối mờ
                fixed alpha = _Color.a * (1.0 - mask.r);

                return fixed4(_Color.rgb, alpha);
            }
            ENDCG
        }
    }
}
