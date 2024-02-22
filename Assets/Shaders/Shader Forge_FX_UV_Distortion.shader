// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Forge/FX_UV_Distortion"
{
  Properties
  {
    _MainTex ("MainTex", 2D) = "white" {}
    _Masks ("Masks", 2D) = "white" {}
    _SpeedX ("Speed X", float) = 0
    _SpeedY ("Speed Y", float) = 0
    _DistortionPower ("Distortion Power", float) = 0
    _Intensity ("Intensity", float) = 0
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
      //uniform float4 _Time;
      uniform float4 _TimeEditor;
      uniform sampler2D _Masks;
      uniform float4 _Masks_ST;
      uniform float _SpeedX;
      uniform float _SpeedY;
      uniform sampler2D _MainTex;
      uniform float4 _MainTex_ST;
      uniform float _DistortionPower;
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
          float4 _MainTex_var_1;
          float4 _node_8745_2;
          float4 tmpvar_3;
          float2 P_4;
          float4 tmpvar_5;
          tmpvar_5 = (_Time + _TimeEditor);
          P_4 = ((((in_f.xlv_TEXCOORD0 + ((_SpeedX * tmpvar_5.y) * float2(1, 0))) + ((_SpeedY * tmpvar_5.y) * float2(0, 1))) * _Masks_ST.xy) + _Masks_ST.zw);
          tmpvar_3 = tex2D(_Masks, P_4);
          _node_8745_2 = tmpvar_3;
          float2 x_6;
          x_6 = ((in_f.xlv_TEXCOORD0 * 2) + (-1));
          float4 tmpvar_7;
          float2 P_8;
          P_8 = (((in_f.xlv_TEXCOORD0 + ((_DistortionPower * ((_node_8745_2.xyz * 2) + (-1))) * (1 - sqrt(dot(x_6, x_6)))).xy) * _MainTex_ST.xy) + _MainTex_ST.zw);
          tmpvar_7 = tex2D(_MainTex, P_8);
          _MainTex_var_1 = tmpvar_7;
          float4 tmpvar_9;
          tmpvar_9.w = 1;
          tmpvar_9.xyz = ((in_f.xlv_COLOR.xyz * in_f.xlv_COLOR.w) * (_MainTex_var_1.xyz * _Intensity));
          out_f.color = tmpvar_9;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
