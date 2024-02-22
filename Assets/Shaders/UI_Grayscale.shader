// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI/Grayscale"
{
  Properties
  {
    [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
    _Color ("Tint", Color) = (1,1,1,1)
    _StencilComp ("Stencil Comparison", float) = 8
    _Stencil ("Stencil ID", float) = 0
    _StencilOp ("Stencil Operation", float) = 0
    _StencilWriteMask ("Stencil Write Mask", float) = 255
    _StencilReadMask ("Stencil Read Mask", float) = 255
    _ColorMask ("Color Mask", float) = 15
    _GrayscaleAmount ("Grayscale Amount", Range(0, 1)) = 1
    [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", float) = 0
  }
  SubShader
  {
    Tags
    { 
      "CanUseSpriteAtlas" = "true"
      "IGNOREPROJECTOR" = "true"
      "PreviewType" = "Plane"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "CanUseSpriteAtlas" = "true"
        "IGNOREPROJECTOR" = "true"
        "PreviewType" = "Plane"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      ZClip Off
      ZWrite Off
      Cull Off
      Stencil
      { 
        Ref 0
        ReadMask 0
        WriteMask 0
        Pass Keep
        Fail Keep
        ZFail Keep
        PassFront Keep
        FailFront Keep
        ZFailFront Keep
        PassBack Keep
        FailBack Keep
        ZFailBack Keep
      } 
      Blend SrcAlpha OneMinusSrcAlpha
      ColorMask 0
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform float4 _Color;
      uniform float4 _TextureSampleAdd;
      uniform float4 _ClipRect;
      uniform float _GrayscaleAmount;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_COLOR :COLOR;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_COLOR :COLOR;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float2 tmpvar_1;
          tmpvar_1 = in_v.texcoord.xy;
          float4 tmpvar_2;
          float2 tmpvar_3;
          tmpvar_3 = tmpvar_1;
          tmpvar_2 = (in_v.color * _Color);
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          out_v.xlv_COLOR = tmpvar_2;
          out_v.xlv_TEXCOORD0 = tmpvar_3;
          out_v.xlv_TEXCOORD1 = in_v.vertex;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float4 color_2;
          float4 tmpvar_3;
          tmpvar_3 = ((tex2D(_MainTex, in_f.xlv_TEXCOORD0) + _TextureSampleAdd) * in_f.xlv_COLOR);
          color_2 = tmpvar_3;
          float tmpvar_4;
          float2 tmpvar_5;
          tmpvar_5.x = float((_ClipRect.z>=in_f.xlv_TEXCOORD1.x));
          tmpvar_5.y = float((_ClipRect.w>=in_f.xlv_TEXCOORD1.y));
          float2 tmpvar_6;
          tmpvar_6 = (float2(bool2(in_f.xlv_TEXCOORD1.xy >= _ClipRect.xy)) * tmpvar_5);
          tmpvar_4 = (tmpvar_6.x * tmpvar_6.y);
          color_2.w = (color_2.w * tmpvar_4);
          float tmpvar_7;
          tmpvar_7 = dot(color_2.xyz, float3(0.3, 0.59, 0.11));
          float3 tmpvar_8;
          tmpvar_8 = lerp(color_2.xyz, float3(tmpvar_7, tmpvar_7, tmpvar_7), float3(_GrayscaleAmount, _GrayscaleAmount, _GrayscaleAmount));
          color_2.xyz = float3(tmpvar_8);
          tmpvar_1 = color_2;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
