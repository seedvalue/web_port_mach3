// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Forge/FX/FX_DispertionShaderAlphaBlendDoubleSided"
{
  Properties
  {
    _Tex ("Tex", 2D) = "white" {}
    _Alpha ("Alpha", 2D) = "white" {}
    _X ("X", float) = 0
    _Y ("Y", float) = 0
    [HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
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
      Blend SrcAlpha OneMinusSrcAlpha
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile DIRECTIONAL
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 UNITY_MATRIX_MVP;
      //uniform float4 _Time;
      uniform float4 _TimeEditor;
      uniform sampler2D _Tex;
      uniform float4 _Tex_ST;
      uniform sampler2D _Alpha;
      uniform float4 _Alpha_ST;
      uniform float _X;
      uniform float _Y;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_COLOR :COLOR;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_COLOR :COLOR;
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
          out_v.xlv_COLOR = in_v.color;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 _Alpha_var_1;
          float4 _Tex_var_2;
          float4 tmpvar_3;
          float2 P_4;
          P_4 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _Tex);
          tmpvar_3 = tex2D(_Tex, P_4);
          _Tex_var_2 = tmpvar_3;
          float4 tmpvar_5;
          tmpvar_5 = (_Time + _TimeEditor);
          float4 tmpvar_6;
          float2 P_7;
          P_7 = ((((in_f.xlv_TEXCOORD0 + ((_X * tmpvar_5.y) * float2(1, 0))) + ((tmpvar_5.y * _Y) * float2(0, 1))) * _Alpha_ST.xy) + _Alpha_ST.zw);
          tmpvar_6 = tex2D(_Alpha, P_7);
          _Alpha_var_1 = tmpvar_6;
          float4 tmpvar_8;
          tmpvar_8.xyz = (in_f.xlv_COLOR.xyz * _Tex_var_2.xyz);
          tmpvar_8.w = float((_Alpha_var_1.x>=(1 - in_f.xlv_COLOR.w)));
          out_f.color = tmpvar_8;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
