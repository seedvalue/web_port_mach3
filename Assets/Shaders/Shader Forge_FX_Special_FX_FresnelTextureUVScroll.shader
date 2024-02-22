// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Forge/FX/Special/FX_FresnelTextureUVScroll"
{
  Properties
  {
    _node_5572 ("node_5572", 2D) = "white" {}
    _node_3042 ("node_3042", 2D) = "white" {}
    _node_7779 ("node_7779", 2D) = "white" {}
    _node_1780 ("node_1780", float) = 0
    _node_9786 ("node_9786", float) = 0
    _node_5748 ("node_5748", float) = 0
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
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 UNITY_MATRIX_MVP;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4 _Time;
      //uniform float3 _WorldSpaceCameraPos;
      uniform float4 _TimeEditor;
      uniform sampler2D _node_5572;
      uniform float4 _node_5572_ST;
      uniform float _node_1780;
      uniform float _node_9786;
      uniform sampler2D _node_3042;
      uniform float4 _node_3042_ST;
      uniform float _node_5748;
      uniform sampler2D _node_7779;
      uniform float4 _node_7779_ST;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float3 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_COLOR :COLOR;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float3 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_COLOR :COLOR;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float3x3 tmpvar_1;
          tmpvar_1[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_1[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_1[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
          out_v.xlv_TEXCOORD1 = mul(unity_ObjectToWorld, in_v.vertex);
          out_v.xlv_TEXCOORD2 = normalize(mul(in_v.normal, tmpvar_1));
          out_v.xlv_COLOR = in_v.color;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 _node_5572_var_1;
          float4 _node_3042_var_2;
          float4 _node_7779_var_3;
          float3 tmpvar_4;
          tmpvar_4 = normalize(in_f.xlv_TEXCOORD2);
          float3 tmpvar_5;
          tmpvar_5 = normalize((_WorldSpaceCameraPos - in_f.xlv_TEXCOORD1.xyz));
          float4 tmpvar_6;
          float2 P_7;
          P_7 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _node_7779);
          tmpvar_6 = tex2D(_node_7779, P_7);
          _node_7779_var_3 = tmpvar_6;
          float4 tmpvar_8;
          float2 P_9;
          P_9 = (((in_f.xlv_TEXCOORD0 + ((_node_5748 * (_Time + _TimeEditor).y) * float2(1, 0))) * _node_3042_ST.xy) + _node_3042_ST.zw);
          tmpvar_8 = tex2D(_node_3042, P_9);
          _node_3042_var_2 = tmpvar_8;
          float4 tmpvar_10;
          float2 P_11;
          P_11 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _node_5572);
          tmpvar_10 = tex2D(_node_5572, P_11);
          _node_5572_var_1 = tmpvar_10;
          float4 tmpvar_12;
          tmpvar_12.w = 1;
          tmpvar_12.xyz = (in_f.xlv_COLOR.xyz * ((((10 * _node_7779_var_3.xyz) + (_node_3042_var_2.xyz + _node_5572_var_1.xyz)) * pow((1 - max(0, dot((tmpvar_4 * sign(dot(tmpvar_5, tmpvar_4))), tmpvar_5))), _node_1780)) * _node_9786));
          out_f.color = tmpvar_12;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Mobile/Diffuse"
}
