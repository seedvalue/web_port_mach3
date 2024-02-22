// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Forge/FX_Sweep"
{
  Properties
  {
    _TextureBlend ("TextureBlend", 2D) = "white" {}
    _Texture ("Texture", 2D) = "white" {}
    _Power ("Power", float) = 2
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
      uniform sampler2D _Texture;
      uniform float4 _Texture_ST;
      uniform sampler2D _TextureBlend;
      uniform float4 _TextureBlend_ST;
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
          float4 _TextureBlend_var_1;
          float4 _Texture_var_2;
          float4 tmpvar_3;
          float2 P_4;
          P_4 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _Texture);
          tmpvar_3 = tex2D(_Texture, P_4);
          _Texture_var_2 = tmpvar_3;
          float tmpvar_5;
          float tmpvar_6;
          tmpvar_6 = (in_f.xlv_COLOR.w * 4);
          tmpvar_5 = (1 - ((in_f.xlv_TEXCOORD0.x - (tmpvar_6 + (-2))) * (in_f.xlv_TEXCOORD0.x - (tmpvar_6 + (-2)))));
          float4 tmpvar_7;
          float2 P_8;
          P_8 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _TextureBlend);
          tmpvar_7 = tex2D(_TextureBlend, P_8);
          _TextureBlend_var_1 = tmpvar_7;
          float4 tmpvar_9;
          tmpvar_9.xyz = ((in_f.xlv_COLOR.xyz * in_f.xlv_COLOR.w) * ((_Texture_var_2.xyz * tmpvar_5) * _Power));
          tmpvar_9.w = (tmpvar_5 * _TextureBlend_var_1.x);
          out_f.color = tmpvar_9;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
