Shader "Yulan/Leaf"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1,1,1,1)
        _GradientTop ("Gradient Top Color", Color) = (1,1,1,1)
        _GradientBot ("Gradient Bottom Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
      
        Tags {
          "Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			  }
        
        // No culling
        Cull Off ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
                float3 pos :NORMAL;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.pos = UnityObjectToViewPos(v.vertex);
                return o;
            }

            sampler2D _MainTex;
            fixed3 _Color;
            fixed3 _GradientTop;
            fixed3 _GradientBot;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb = fixed3 (1,1,1) * _Color * (i.uv.y * _GradientTop + (1-i.uv.y) * _GradientBot);
                float d = length (i.pos);
                d /= 4;
                d*=d;
                col.a /= d;
                if (col.a > 1) col.a = 1;
                if (col.a <= 0) discard;
                return col;
            }
            ENDCG
        }
    }
}
