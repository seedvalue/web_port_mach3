// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Fancy/Shadow"
{
  Properties
  {
    _AlphaTex ("General/MISC: R - alpha", 2D) = "white" {}
    [Slider] Kamis_ShadowColorRatio ("Shadow/Color Ratio", Range(0, 1)) = 0
    [Slider] Kamis_ShadowPower ("Shadow/Power", Range(0, 1)) = 1
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
      "SHADOWSUPPORT" = "false"
    }
    LOD 200
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "ForwardBase"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
        "SHADOWSUPPORT" = "false"
      }
      LOD 200
      ZClip Off
      ZWrite Off
      Fog
      { 
        Mode  Off
      } 
      Blend SrcAlpha OneMinusSrcAlpha
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform float4 _AlphaTex_ST;
      uniform sampler2D _AlphaTex;
      uniform float Kamis_ShadowColorRatio;
      uniform float Kamis_ShadowPower;
      uniform float4 Kamis_ShadowColor0;
      uniform float4 Kamis_ShadowColor1;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _AlphaTex);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float4 color_2;
          float4 tmpvar_3;
          tmpvar_3 = lerp(Kamis_ShadowColor0, Kamis_ShadowColor1, float4(Kamis_ShadowColorRatio, Kamis_ShadowColorRatio, Kamis_ShadowColorRatio, Kamis_ShadowColorRatio));
          color_2.xyz = tmpvar_3.xyz;
          color_2.w = (tmpvar_3.w * Kamis_ShadowPower);
          color_2.w = (color_2.w * tex2D(_AlphaTex, in_f.xlv_TEXCOORD0).x);
          tmpvar_1 = color_2;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Mobile/Diffuse"
}
