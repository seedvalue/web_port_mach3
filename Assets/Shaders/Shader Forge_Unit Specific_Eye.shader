// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Forge/Unit Specific/Eye"
{
  Properties
  {
    _Eyelid ("Eyelid", 2D) = "white" {}
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
      uniform sampler2D _Eyelid;
      uniform float4 _Eyelid_ST;
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
          float4 _Eyelid_var_1;
          float tmpvar_2;
          tmpvar_2 = (in_f.xlv_COLOR.w * 0.5);
          float tmpvar_3;
          tmpvar_3 = (float((tmpvar_2>=(1 - in_f.xlv_TEXCOORD0.x))) + float((tmpvar_2>=in_f.xlv_TEXCOORD0.x)));
          float x_4;
          x_4 = (tmpvar_3 - 0.5);
          if((x_4<0))
          {
              discard;
          }
          float4 tmpvar_5;
          float2 P_6;
          P_6 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _Eyelid);
          tmpvar_5 = tex2D(_Eyelid, P_6);
          _Eyelid_var_1 = tmpvar_5;
          float4 tmpvar_7;
          tmpvar_7.w = 1;
          tmpvar_7.xyz = ((_Eyelid_var_1.xyz * tmpvar_3) * in_f.xlv_COLOR.xyz);
          out_f.color = tmpvar_7;
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
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_COLOR :COLOR;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_COLOR :COLOR;
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
          out_v.xlv_COLOR = in_v.color;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float tmpvar_1;
          tmpvar_1 = (in_f.xlv_COLOR.w * 0.5);
          float x_2;
          x_2 = ((float((tmpvar_1>=(1 - in_f.xlv_TEXCOORD1.x))) + float((tmpvar_1>=in_f.xlv_TEXCOORD1.x))) - 0.5);
          if((x_2<0))
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
