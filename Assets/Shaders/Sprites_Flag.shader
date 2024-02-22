// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/Flag"
{
  Properties
  {
    _MainTex ("Texture", 2D) = "white" {}
    _Curves ("Cureves", Range(0, 10)) = 0.5
    _gravity ("Gravity Fall", Range(0, 1)) = 0.5
    _windSpeed ("Wind Speed", Range(0, 25)) = 4
  }
  SubShader
  {
    Tags
    { 
      "QUEUE" = "Transparent"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "QUEUE" = "Transparent"
      }
      ZClip Off
      Cull Off
      Blend SrcAlpha OneMinusSrcAlpha
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform float4 _MainTex_ST;
      //uniform float4 _Time;
      uniform sampler2D _MainTex;
      uniform float _Curves;
      uniform float _gravity;
      uniform float _windSpeed;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
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
          out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 col_1;
          float yoffset_2;
          yoffset_2 = (((in_f.xlv_TEXCOORD0.x - (_Time.x * _windSpeed)) * 2) - 1);
          yoffset_2 = ((yoffset_2 * 1.570796) * _Curves);
          yoffset_2 = (((cos((yoffset_2 * 2)) * in_f.xlv_TEXCOORD0.x) * 0.05) + ((_gravity * in_f.xlv_TEXCOORD0.x) * in_f.xlv_TEXCOORD0.x));
          float2 tmpvar_3;
          tmpvar_3.x = in_f.xlv_TEXCOORD0.x;
          tmpvar_3.y = (in_f.xlv_TEXCOORD0.y + yoffset_2);
          float4 tmpvar_4;
          tmpvar_4 = tex2D(_MainTex, tmpvar_3);
          col_1.w = tmpvar_4.w;
          col_1.xyz = (tmpvar_4.xyz * tmpvar_4.w);
          out_f.color = col_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
