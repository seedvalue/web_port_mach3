// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Forge/OrbWaste"
{
  Properties
  {
    _Main ("Main", 2D) = "white" {}
    _Dissolve ("Dissolve", Range(0, 1)) = 0
    _Noise ("Noise", 2D) = "white" {}
    _Ramp ("Ramp", 2D) = "white" {}
    _node_9091 ("node_9091", Range(0, 1)) = 0
    [HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
  }
  SubShader
  {
    Tags
    { 
      "QUEUE" = "AlphaTest"
      "RenderType" = "TransparentCutout"
    }
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "LIGHTMODE" = "ForwardBase"
        "QUEUE" = "AlphaTest"
        "RenderType" = "TransparentCutout"
        "SHADOWSUPPORT" = "true"
      }
      ZClip Off
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile DIRECTIONAL
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform sampler2D _Main;
      uniform float4 _Main_ST;
      uniform sampler2D _Noise;
      uniform float4 _Noise_ST;
      uniform sampler2D _Ramp;
      uniform float4 _Ramp_ST;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_COLOR :COLOR;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
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
          out_v.xlv_TEXCOORD1 = in_v.texcoord1;
          out_v.xlv_COLOR = in_v.color;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 _node_4214_1;
          float4 _node_6805_2;
          float4 _Main_var_3;
          float4 tmpvar_4;
          float2 P_5;
          P_5 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _Main);
          tmpvar_4 = tex2D(_Main, P_5);
          _Main_var_3 = tmpvar_4;
          float4 tmpvar_6;
          float2 P_7;
          P_7 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _Noise);
          tmpvar_6 = tex2D(_Noise, P_7);
          _node_6805_2 = tmpvar_6;
          float tmpvar_8;
          tmpvar_8 = ((((1 - in_f.xlv_TEXCOORD1.x) * 1.6) + (-0.6)) + _node_6805_2.x);
          float x_9;
          x_9 = ((_Main_var_3.w * tmpvar_8) - 0.5);
          if((x_9<0))
          {
              discard;
          }
          float2 tmpvar_10;
          tmpvar_10.y = 0;
          tmpvar_10.x = (1 - clamp(((tmpvar_8 * 16) + (-8)), 0, 1));
          float4 tmpvar_11;
          float2 P_12;
          P_12 = TRANSFORM_TEX(tmpvar_10, _Ramp);
          tmpvar_11 = tex2D(_Ramp, P_12);
          _node_4214_1 = tmpvar_11;
          float4 tmpvar_13;
          tmpvar_13.w = 1;
          tmpvar_13.xyz = (dot(_Main_var_3.xyz, float3(0.3, 0.59, 0.11)) + (_node_4214_1.xyz * in_f.xlv_COLOR.xyz));
          out_f.color = tmpvar_13;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 2, name: SHADOWCASTER
    {
      Name "SHADOWCASTER"
      Tags
      { 
        "LIGHTMODE" = "SHADOWCASTER"
        "QUEUE" = "AlphaTest"
        "RenderType" = "TransparentCutout"
        "SHADOWSUPPORT" = "true"
      }
      ZClip Off
      Offset 1, 1
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile SHADOWS_DEPTH
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 unity_LightShadowBias;
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform sampler2D _Main;
      uniform float4 _Main_ST;
      uniform sampler2D _Noise;
      uniform float4 _Noise_ST;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4 tmpvar_1;
          float4 tmpvar_2;
          tmpvar_2.w = 1;
          tmpvar_2.xyz = in_v.vertex.xyz;
          tmpvar_1 = UnityObjectToClipPos(tmpvar_2);
          float4 clipPos_3;
          clipPos_3.xyw = tmpvar_1.xyw;
          clipPos_3.z = (tmpvar_1.z + clamp((unity_LightShadowBias.x / tmpvar_1.w), 0, 1));
          clipPos_3.z = lerp(clipPos_3.z, max(clipPos_3.z, (-tmpvar_1.w)), unity_LightShadowBias.y);
          out_v.vertex = clipPos_3;
          out_v.xlv_TEXCOORD1 = in_v.texcoord.xy;
          out_v.xlv_TEXCOORD2 = in_v.texcoord1;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 _node_6805_1;
          float4 _Main_var_2;
          float4 tmpvar_3;
          float2 P_4;
          P_4 = TRANSFORM_TEX(in_f.xlv_TEXCOORD1, _Main);
          tmpvar_3 = tex2D(_Main, P_4);
          _Main_var_2 = tmpvar_3;
          float4 tmpvar_5;
          float2 P_6;
          P_6 = TRANSFORM_TEX(in_f.xlv_TEXCOORD1, _Noise);
          tmpvar_5 = tex2D(_Noise, P_6);
          _node_6805_1 = tmpvar_5;
          float x_7;
          x_7 = ((_Main_var_2.w * ((((1 - in_f.xlv_TEXCOORD2.x) * 1.6) + (-0.6)) + _node_6805_1.x)) - 0.5);
          if((x_7<0))
          {
              discard;
          }
          out_f.color = float4(0, 0, 0, 0);
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
