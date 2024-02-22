// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Forge/Blending/BlendAdd_MultiplyTextures"
{
  Properties
  {
    _Texture ("Texture", 2D) = "white" {}
    _Texture2 ("Texture2", 2D) = "white" {}
    _Intensity ("Intensity", float) = 1
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
      uniform sampler2D _Texture;
      uniform float4 _Texture_ST;
      uniform sampler2D _Texture2;
      uniform float4 _Texture2_ST;
      uniform float _Intensity;
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
          float4 _Texture2_var_1;
          float4 _Texture_var_2;
          float4 tmpvar_3;
          float2 P_4;
          P_4 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _Texture);
          tmpvar_3 = tex2D(_Texture, P_4);
          _Texture_var_2 = tmpvar_3;
          float4 tmpvar_5;
          float2 P_6;
          P_6 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _Texture2);
          tmpvar_5 = tex2D(_Texture2, P_6);
          _Texture2_var_1 = tmpvar_5;
          float4 tmpvar_7;
          tmpvar_7.xyz = ((in_f.xlv_COLOR.xyz * in_f.xlv_COLOR.w) * ((_Texture_var_2.xyz * _Texture2_var_1.x) * _Intensity));
          tmpvar_7.w = (in_f.xlv_COLOR.w * _Texture2_var_1.x);
          out_f.color = tmpvar_7;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
