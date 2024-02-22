// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Forge/FX_AddUVScrollTint_03"
{
  Properties
  {
    _Mask ("Mask", 2D) = "white" {}
    _TintColor ("Color", Color) = (1,1,1,1)
    _Intensity ("Intensity", Range(0, 5)) = 1
    _U_Offset ("U_Offset", float) = 0
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "ForwardBase"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
        "SHADOWSUPPORT" = "true"
      }
      ZClip Off
      ZWrite Off
      Cull Off
      Blend One One
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile DIRECTIONAL
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform sampler2D _Mask;
      uniform float4 _Mask_ST;
      uniform float4 _TintColor;
      uniform float _Intensity;
      uniform float _U_Offset;
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
          out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float4 finalRGBA_2;
          float4 _Mask_var_3;
          float2 tmpvar_4;
          tmpvar_4.x = (_U_Offset + in_f.xlv_TEXCOORD0.x);
          tmpvar_4.y = in_f.xlv_TEXCOORD0.y;
          float4 tmpvar_5;
          float2 P_6;
          P_6 = TRANSFORM_TEX(tmpvar_4, _Mask);
          tmpvar_5 = tex2D(_Mask, P_6);
          _Mask_var_3 = tmpvar_5;
          float4 tmpvar_7;
          tmpvar_7.w = 1;
          tmpvar_7.xyz = ((_Mask_var_3.xyz * _TintColor.xyz) * _Intensity);
          finalRGBA_2 = tmpvar_7;
          tmpvar_1 = finalRGBA_2;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
