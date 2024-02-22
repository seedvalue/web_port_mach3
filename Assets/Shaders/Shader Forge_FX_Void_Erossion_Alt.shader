// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Forge/FX_Void_Erossion_Alt"
{
  Properties
  {
    _Speed1X ("Speed 1 X", float) = 0
    _Speed1Y ("Speed 1 Y", float) = 0
    _Speed2X ("Speed 2 X", float) = 0
    _Speed2Y ("Speed 2 Y", float) = 0
    [NoScaleOffset] _MainTex ("MainTex", 2D) = "white" {}
    _UVScale ("UV Scale", float) = 1
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
      Cull Off
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
      uniform float _Speed1X;
      uniform float _Speed1Y;
      uniform float _Speed2X;
      uniform float _Speed2Y;
      uniform sampler2D _MainTex;
      uniform float _UVScale;
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
          float4 _Texture2_1;
          float4 _Texture_2;
          float4 tmpvar_3;
          tmpvar_3 = (_Time + _TimeEditor);
          float2 tmpvar_4;
          float2 tmpvar_5;
          tmpvar_5 = (in_f.xlv_TEXCOORD0 + in_f.xlv_TEXCOORD1.x);
          tmpvar_4 = (((_UVScale * tmpvar_5) + ((_Speed1X * tmpvar_3.y) * float2(1, 0))) + ((tmpvar_3.y * _Speed1Y) * float2(0, 1)));
          float4 tmpvar_6;
          tmpvar_6 = tex2D(_MainTex, tmpvar_4);
          _Texture_2 = tmpvar_6;
          float2 tmpvar_7;
          tmpvar_7 = (((_UVScale * tmpvar_5) + ((_Speed2X * tmpvar_3.y) * float2(1, 0))) + ((tmpvar_3.y * _Speed2Y) * float2(0, 1)));
          float4 tmpvar_8;
          tmpvar_8 = tex2D(_MainTex, tmpvar_7);
          _Texture2_1 = tmpvar_8;
          float x_9;
          x_9 = (((_Texture_2.x + _Texture2_1.y) - (1 - in_f.xlv_COLOR.w)) - 0.5);
          if((x_9<0))
          {
              discard;
          }
          float4 tmpvar_10;
          tmpvar_10.w = 1;
          tmpvar_10.xyz = in_f.xlv_COLOR.xyz;
          out_f.color = tmpvar_10;
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
      //uniform float4 _Time;
      uniform float4 _TimeEditor;
      uniform float _Speed1X;
      uniform float _Speed1Y;
      uniform float _Speed2X;
      uniform float _Speed2Y;
      uniform sampler2D _MainTex;
      uniform float _UVScale;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_COLOR :COLOR;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
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
          out_v.xlv_TEXCOORD2 = in_v.texcoord1;
          out_v.xlv_COLOR = in_v.color;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 _Texture2_1;
          float4 _Texture_2;
          float4 tmpvar_3;
          tmpvar_3 = (_Time + _TimeEditor);
          float2 tmpvar_4;
          float2 tmpvar_5;
          tmpvar_5 = (in_f.xlv_TEXCOORD1 + in_f.xlv_TEXCOORD2.x);
          tmpvar_4 = (((_UVScale * tmpvar_5) + ((_Speed1X * tmpvar_3.y) * float2(1, 0))) + ((tmpvar_3.y * _Speed1Y) * float2(0, 1)));
          float4 tmpvar_6;
          tmpvar_6 = tex2D(_MainTex, tmpvar_4);
          _Texture_2 = tmpvar_6;
          float2 tmpvar_7;
          tmpvar_7 = (((_UVScale * tmpvar_5) + ((_Speed2X * tmpvar_3.y) * float2(1, 0))) + ((tmpvar_3.y * _Speed2Y) * float2(0, 1)));
          float4 tmpvar_8;
          tmpvar_8 = tex2D(_MainTex, tmpvar_7);
          _Texture2_1 = tmpvar_8;
          float x_9;
          x_9 = (((_Texture_2.x + _Texture2_1.y) - (1 - in_f.xlv_COLOR.w)) - 0.5);
          if((x_9<0))
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
