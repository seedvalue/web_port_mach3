// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Forge/FX_SunRays"
{
  Properties
  {
    _Texture ("Texture", 2D) = "white" {}
    _Alpha ("Alpha", 2D) = "white" {}
    _Speed ("Speed", float) = 1
    _Power ("Power", float) = 1
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
      Blend One OneMinusSrcAlpha
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
      uniform sampler2D _Texture;
      uniform float4 _Texture_ST;
      uniform sampler2D _Alpha;
      uniform float4 _Alpha_ST;
      uniform float _Speed;
      uniform float _Power;
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
          float4 _Texture_var_2;
          float4 tmpvar_3;
          tmpvar_3 = (_Time + _TimeEditor);
          float tmpvar_4;
          tmpvar_4 = cos((_Speed * tmpvar_3.y));
          float tmpvar_5;
          tmpvar_5 = sin((_Speed * tmpvar_3.y));
          float2x2 tmpvar_6;
          conv_mxt2x2_0(tmpvar_6).x = tmpvar_4;
          conv_mxt2x2_0(tmpvar_6).y = tmpvar_5;
          conv_mxt2x2_1(tmpvar_6).x = (-tmpvar_5);
          conv_mxt2x2_1(tmpvar_6).y = tmpvar_4;
          float4 tmpvar_7;
          float2 P_8;
          P_8 = (((mul((in_f.xlv_TEXCOORD0 - float2(0.5, 0.5)), tmpvar_6) + float2(0.5, 0.5)) * _Texture_ST.xy) + _Texture_ST.zw);
          tmpvar_7 = tex2D(_Texture, P_8);
          _Texture_var_2 = tmpvar_7;
          float4 tmpvar_9;
          float2 P_10;
          P_10 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _Alpha);
          tmpvar_9 = tex2D(_Alpha, P_10);
          _Alpha_var_1 = tmpvar_9;
          float4 tmpvar_11;
          tmpvar_11.xyz = ((_Texture_var_2.xyz * _Alpha_var_1.x) * (_Power * in_f.xlv_COLOR.w));
          tmpvar_11.w = ((_Texture_var_2.x * _Alpha_var_1.x) * in_f.xlv_COLOR.w);
          out_f.color = tmpvar_11;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
