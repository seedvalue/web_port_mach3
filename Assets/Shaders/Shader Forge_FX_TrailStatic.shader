// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Forge/FX_TrailStatic"
{
  Properties
  {
    _Texture ("Texture", 2D) = "white" {}
    _Mask ("Mask", 2D) = "white" {}
    _Color ("Color", Color) = (0.5,0.5,0.5,1)
    _TextureSpeed ("Texture Speed", float) = 0
    _DeformationValue ("DeformationValue", float) = 4
    _DeformationIntensity ("DeformationIntensity", float) = 0
    _DeformationDirection ("DeformationDirection", Vector) = (0,0,0,0)
    _DeformationSpeed ("DeformationSpeed", float) = 0
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
      //uniform float4 _Time;
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform float4 _TimeEditor;
      uniform float _DeformationValue;
      uniform float _DeformationIntensity;
      uniform float4 _DeformationDirection;
      uniform float _DeformationSpeed;
      uniform sampler2D _Texture;
      uniform float4 _Texture_ST;
      uniform float _TextureSpeed;
      uniform float4 _Color;
      uniform sampler2D _Mask;
      uniform float4 _Mask_ST;
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
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4 tmpvar_1;
          tmpvar_1.w = in_v.vertex.w;
          tmpvar_1.xyz = (in_v.vertex.xyz + (((sin(((_DeformationSpeed * (_Time + _TimeEditor).y) + (in_v.texcoord.x * _DeformationValue))) * _DeformationDirection.xyz) * _DeformationIntensity) * in_v.color.xyz));
          out_v.vertex = UnityObjectToClipPos(tmpvar_1);
          out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
          out_v.xlv_COLOR = in_v.color;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 _Mask_var_1;
          float4 _Texture_var_2;
          float4 tmpvar_3;
          float2 P_4;
          P_4 = (((in_f.xlv_TEXCOORD0 + (((_Time + _TimeEditor).y * _TextureSpeed) * float2(1, 0))) * _Texture_ST.xy) + _Texture_ST.zw);
          tmpvar_3 = tex2D(_Texture, P_4);
          _Texture_var_2 = tmpvar_3;
          float4 tmpvar_5;
          float2 P_6;
          P_6 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _Mask);
          tmpvar_5 = tex2D(_Mask, P_6);
          _Mask_var_1 = tmpvar_5;
          float4 tmpvar_7;
          tmpvar_7.w = 1;
          tmpvar_7.xyz = (_Color.xyz * (_Texture_var_2.xyz * _Mask_var_1.xyz));
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
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "SHADOWCASTER"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
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
      //uniform float4 _Time;
      //uniform float4 unity_LightShadowBias;
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform float4 _TimeEditor;
      uniform float _DeformationValue;
      uniform float _DeformationIntensity;
      uniform float4 _DeformationDirection;
      uniform float _DeformationSpeed;
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
          float4 vertex :Position;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4 tmpvar_1;
          tmpvar_1.w = in_v.vertex.w;
          tmpvar_1.xyz = (in_v.vertex.xyz + (((sin(((_DeformationSpeed * (_Time + _TimeEditor).y) + (in_v.texcoord.x * _DeformationValue))) * _DeformationDirection.xyz) * _DeformationIntensity) * in_v.color.xyz));
          float4 tmpvar_2;
          float4 tmpvar_3;
          tmpvar_3.w = 1;
          tmpvar_3.xyz = tmpvar_1.xyz;
          tmpvar_2 = UnityObjectToClipPos(tmpvar_3);
          float4 clipPos_4;
          clipPos_4.xyw = tmpvar_2.xyw;
          clipPos_4.z = (tmpvar_2.z + clamp((unity_LightShadowBias.x / tmpvar_2.w), 0, 1));
          clipPos_4.z = lerp(clipPos_4.z, max(clipPos_4.z, (-tmpvar_2.w)), unity_LightShadowBias.y);
          out_v.vertex = clipPos_4;
          out_v.xlv_TEXCOORD1 = in_v.texcoord.xy;
          out_v.xlv_COLOR = in_v.color;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          out_f.color = float4(0, 0, 0, 0);
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
