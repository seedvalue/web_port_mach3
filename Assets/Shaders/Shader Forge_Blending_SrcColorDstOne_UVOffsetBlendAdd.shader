// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Forge/Blending/SrcColorDstOne_UVOffsetBlendAdd"
{
  Properties
  {
    _node_8525 ("node_8525", 2D) = "white" {}
    _node_5083 ("node_5083", 2D) = "white" {}
    _Speed ("Speed", float) = 1
    _node_4059 ("node_4059", Color) = (0.5,0.5,0.5,1)
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
      Cull Off
      Blend SrcColor One
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
      uniform sampler2D _node_8525;
      uniform float4 _node_8525_ST;
      uniform sampler2D _node_5083;
      uniform float4 _node_5083_ST;
      uniform float _Speed;
      uniform float4 _node_4059;
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
          float4 _node_5083_var_1;
          float4 _node_8525_var_2;
          float2 tmpvar_3;
          tmpvar_3.x = (in_f.xlv_COLOR.x + in_f.xlv_TEXCOORD0.x);
          tmpvar_3.y = in_f.xlv_TEXCOORD0.y;
          float4 tmpvar_4;
          float2 P_5;
          P_5 = (((tmpvar_3 + ((_Speed * (_Time + _TimeEditor).y) * float2(1, 0))) * _node_8525_ST.xy) + _node_8525_ST.zw);
          tmpvar_4 = tex2D(_node_8525, P_5);
          _node_8525_var_2 = tmpvar_4;
          float4 tmpvar_6;
          float2 P_7;
          P_7 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _node_5083);
          tmpvar_6 = tex2D(_node_5083, P_7);
          _node_5083_var_1 = tmpvar_6;
          float4 tmpvar_8;
          tmpvar_8.xyz = ((_node_4059.xyz * in_f.xlv_COLOR.w) * ((_node_8525_var_2.xyz * _node_5083_var_1.x) * 2));
          tmpvar_8.w = (in_f.xlv_COLOR.w * _node_5083_var_1.x);
          out_f.color = tmpvar_8;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
