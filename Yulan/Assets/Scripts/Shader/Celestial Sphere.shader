Shader "Unlit/Celestial Sphere"
{
  Properties
  {
    _GradientTop ("Top Color", Color) = (1,1,1,1)
    _GradientBot ("Bottom Color", Color) = (1,1,1,1)
  }
  SubShader
  {
    ZTest On Cull Front
    Pass
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      // make fog work
      #pragma multi_compile_fog

      #include "UnityCG.cginc"

      struct appdata {
        float4 vertex : POSITION;
        float4 uv : TEXCOORD0;
        float3 normal : NORMAL;
      };

      struct v2f
      {
        float2 uv : TEXCOORD0;
        UNITY_FOG_COORDS(1)
        float4 vertex : SV_POSITION;
        float3 normal : NORMAL;
      };

      float4 _MainTex_ST;
      fixed4 _GradientTop;
      fixed4 _GradientBot;

      v2f vert (appdata v)
      {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = v.uv;
        o.normal = v.normal * (-1);
        UNITY_TRANSFER_FOG(o,o.vertex);

        return o;
      }

      fixed4 frag (v2f i) : SV_Target
      {
        // sample the texture
        fixed4 col = i.uv.y * _GradientTop + (1 - i.uv.y) * _GradientBot;
        col.a = 1;
        // apply fog
        UNITY_APPLY_FOG(i.fogCoord, col);
        return col;
      }
      ENDCG
    }
  }
}
