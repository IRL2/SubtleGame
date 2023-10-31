Shader "Custom/TransparentTexturedLine"
{
  Properties{
    _ColorMask("Texture", 2D) = "white" {}
    _Color("Color", Color) = (0.5, 0.5, 0.5, 0.5)
  }
  SubShader {
    Tags{"Queue" = "Transparent" "RenderType" = "Transparent"
         "IgnoreProjector" = "True" "PreviewType" = "Plane"}
    LOD 100
    Blend SrcAlpha OneMinusSrcAlpha
    Cull Off
    Lighting Off
    ZWrite Off

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

      struct appdata {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
      };

      struct v2f {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
      };

      sampler2D _ColorMask;
      float4 _ColorMask_ST;
      fixed4 _Color;

      v2f vert(appdata v) {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _ColorMask);
        return o;
      }

      fixed4 frag(v2f i) : SV_Target {
        fixed4 colMask = tex2D(_ColorMask, i.uv);
        return fixed4(_Color.rgb, colMask.r * _Color.a);
      }
      ENDCG
    }
  }
}
