// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Forge/Better/FX_StepDispertion"
{
  Properties
  {
    _Texture ("Texture", 2D) = "white" {}
    _Alpha ("Alpha", 2D) = "white" {}
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
      Blend One One
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile DIRECTIONAL
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform float4 _Texture_ST;
      uniform sampler2D _Texture;
      uniform sampler2D _Alpha;
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
          out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _Texture);
          out_v.xlv_COLOR = in_v.color;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float3 alpha_1;
          float3 _Texture_var_2;
          float3 tmpvar_3;
          tmpvar_3 = tex2D(_Texture, in_f.xlv_TEXCOORD0).xyz;
          _Texture_var_2 = tmpvar_3;
          float3 tmpvar_4;
          tmpvar_4 = tex2D(_Alpha, in_f.xlv_TEXCOORD0).xxx;
          alpha_1 = tmpvar_4;
          float4 tmpvar_5;
          tmpvar_5.w = 1;
          float _tmp_dvx_16 = (1 - in_f.xlv_COLOR.w);
          tmpvar_5.xyz = ((in_f.xlv_COLOR.xyz * _Texture_var_2) * (1 - float3(bool3(float3(_tmp_dvx_16, _tmp_dvx_16, _tmp_dvx_16) >= alpha_1))));
          out_f.color = tmpvar_5;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Mobile/Diffuse"
}
