// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Forge/FX_Sweep_Additive"
{
  Properties
  {
    _Texture ("Texture", 2D) = "white" {}
    _Power ("Power", float) = 2
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
      uniform sampler2D _Texture;
      uniform float4 _Texture_ST;
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
          float4 _Texture_var_1;
          float4 tmpvar_2;
          float2 P_3;
          P_3 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _Texture);
          tmpvar_2 = tex2D(_Texture, P_3);
          _Texture_var_1 = tmpvar_2;
          float4 tmpvar_4;
          tmpvar_4.w = 1;
          float tmpvar_5;
          tmpvar_5 = (in_f.xlv_COLOR.w * 4);
          tmpvar_4.xyz = ((((_Texture_var_1.xyz * (1 - ((in_f.xlv_TEXCOORD0.x - (tmpvar_5 + (-2))) * (in_f.xlv_TEXCOORD0.x - (tmpvar_5 + (-2)))))) * _Power) * in_f.xlv_COLOR.xyz) * in_f.xlv_COLOR.w);
          out_f.color = tmpvar_4;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
