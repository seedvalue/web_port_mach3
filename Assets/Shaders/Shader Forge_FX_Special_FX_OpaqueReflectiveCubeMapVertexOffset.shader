// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Forge/FX/Special/FX_OpaqueReflectiveCubeMapVertexOffset"
{
  Properties
  {
    _Texture ("Texture", 2D) = "white" {}
    _Cubemap ("Cubemap", Cube) = "_Skybox" {}
    _Emission ("Emission", 2D) = "white" {}
    _VertexOffsetPower ("Vertex Offset Power", float) = 0
    _VertexOffsetSpeed ("Vertex Offset Speed", float) = 0
    [MaterialToggle] _SettingsBelowWillWorkEmissionMultiplyWithMainTexture ("Settings Below Will Work (Emission Multiply With Main Texture)", float) = 0
    _Color ("Color", Color) = (0.5,0.5,0.5,1)
    _EmissionSpeed ("Emission Speed", float) = 1
  }
  SubShader
  {
    Tags
    { 
      "RenderType" = "Opaque"
    }
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "LIGHTMODE" = "ForwardBase"
        "RenderType" = "Opaque"
        "SHADOWSUPPORT" = "true"
      }
      ZClip Off
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
      //uniform float4 _Time;
      //uniform float4 unity_4LightPosX0;
      //uniform float4 unity_4LightPosY0;
      //uniform float4 unity_4LightPosZ0;
      //uniform float4 unity_4LightAtten0;
      //uniform float4 unity_LightColor[8];
      //uniform float4x4 UNITY_MATRIX_MVP;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      uniform float4 _TimeEditor;
      uniform float _VertexOffsetPower;
      uniform float _VertexOffsetSpeed;
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _WorldSpaceLightPos0;
      uniform float4 _LightColor0;
      uniform sampler2D _Texture;
      uniform float4 _Texture_ST;
      uniform samplerCUBE _Cubemap;
      uniform float _EmissionSpeed;
      uniform sampler2D _Emission;
      uniform float4 _Emission_ST;
      uniform float4 _Color;
      uniform float _SettingsBelowWillWorkEmissionMultiplyWithMainTexture;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float3 xlv_TEXCOORD3 :TEXCOORD3;
          float3 xlv_TEXCOORD6 :TEXCOORD6;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float3 xlv_TEXCOORD3 :TEXCOORD3;
          float3 xlv_TEXCOORD6 :TEXCOORD6;
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
          float4 tmpvar_2;
          float3x3 tmpvar_3;
          tmpvar_3[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_3[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_3[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float3 tmpvar_4;
          tmpvar_4 = normalize(mul(in_v.normal, tmpvar_3));
          float tmpvar_5;
          tmpvar_5 = (abs((frac(((3 * in_v.texcoord1.xy) + ((_VertexOffsetSpeed * (_Time + _TimeEditor).y) * float2(0, (-1)))).y) - 0.5)) * 2);
          tmpvar_1.xyz = (in_v.vertex.xyz + ((((tmpvar_5 * tmpvar_5) * in_v.normal) * _VertexOffsetPower) * in_v.color.xyz));
          tmpvar_2 = mul(unity_ObjectToWorld, tmpvar_1);
          float3 lightColor0_6;
          lightColor0_6 = unity_LightColor[0].xyz;
          float3 lightColor1_7;
          lightColor1_7 = unity_LightColor[1].xyz;
          float3 lightColor2_8;
          lightColor2_8 = unity_LightColor[2].xyz;
          float3 lightColor3_9;
          lightColor3_9 = unity_LightColor[3].xyz;
          float4 lightAttenSq_10;
          lightAttenSq_10 = unity_4LightAtten0;
          float3 col_11;
          float4 ndotl_12;
          float4 lengthSq_13;
          float4 tmpvar_14;
          tmpvar_14 = (unity_4LightPosX0 - tmpvar_2.x);
          float4 tmpvar_15;
          tmpvar_15 = (unity_4LightPosY0 - tmpvar_2.y);
          float4 tmpvar_16;
          tmpvar_16 = (unity_4LightPosZ0 - tmpvar_2.z);
          lengthSq_13 = (tmpvar_14 * tmpvar_14);
          lengthSq_13 = (lengthSq_13 + (tmpvar_15 * tmpvar_15));
          lengthSq_13 = (lengthSq_13 + (tmpvar_16 * tmpvar_16));
          float4 tmpvar_17;
          tmpvar_17 = max(lengthSq_13, float4(1E-06, 1E-06, 1E-06, 1E-06));
          lengthSq_13 = tmpvar_17;
          ndotl_12 = (tmpvar_14 * tmpvar_4.x);
          ndotl_12 = (ndotl_12 + (tmpvar_15 * tmpvar_4.y));
          ndotl_12 = (ndotl_12 + (tmpvar_16 * tmpvar_4.z));
          float4 tmpvar_18;
          tmpvar_18 = max(float4(0, 0, 0, 0), (ndotl_12 * rsqrt(tmpvar_17)));
          ndotl_12 = tmpvar_18;
          float4 tmpvar_19;
          tmpvar_19 = (tmpvar_18 * (1 / (1 + (tmpvar_17 * lightAttenSq_10))));
          col_11 = (lightColor0_6 * tmpvar_19.x);
          col_11 = (col_11 + (lightColor1_7 * tmpvar_19.y));
          col_11 = (col_11 + (lightColor2_8 * tmpvar_19.z));
          col_11 = (col_11 + (lightColor3_9 * tmpvar_19.w));
          out_v.vertex = UnityObjectToClipPos(tmpvar_1);
          out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
          out_v.xlv_TEXCOORD1 = in_v.texcoord1.xy;
          out_v.xlv_TEXCOORD2 = tmpvar_2;
          out_v.xlv_TEXCOORD3 = tmpvar_4;
          out_v.xlv_TEXCOORD6 = col_11;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 _Emission_var_1;
          float4 _Texture_var_2;
          float3 lightDirection_3;
          float3 tmpvar_4;
          tmpvar_4 = normalize(in_f.xlv_TEXCOORD3);
          float3 tmpvar_5;
          tmpvar_5 = normalize((_WorldSpaceCameraPos - in_f.xlv_TEXCOORD2.xyz));
          float3 tmpvar_6;
          float3 I_7;
          I_7 = (-tmpvar_5);
          tmpvar_6 = (I_7 - (2 * (dot(tmpvar_4, I_7) * tmpvar_4)));
          float3 tmpvar_8;
          tmpvar_8 = normalize(_WorldSpaceLightPos0.xyz);
          lightDirection_3 = tmpvar_8;
          float4 tmpvar_9;
          float _tmp_dvx_15 = texCUBE(_Cubemap, tmpvar_6);
          tmpvar_9 = float4(_tmp_dvx_15, _tmp_dvx_15, _tmp_dvx_15, _tmp_dvx_15);
          float4 tmpvar_10;
          float2 P_11;
          P_11 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _Texture);
          tmpvar_10 = tex2D(_Texture, P_11);
          _Texture_var_2 = tmpvar_10;
          float4 tmpvar_12;
          float2 P_13;
          P_13 = (((in_f.xlv_TEXCOORD1 + ((_EmissionSpeed * (_Time + _TimeEditor).y) * float2(0, (-1)))) * _Emission_ST.xy) + _Emission_ST.zw);
          tmpvar_12 = tex2D(_Emission, P_13);
          _Emission_var_1 = tmpvar_12;
          float4 tmpvar_14;
          tmpvar_14.w = 1;
          tmpvar_14.xyz = (((((_LightColor0.xyz * pow(max(0, dot(normalize((tmpvar_5 + lightDirection_3)), tmpvar_4)), 64)) * float3(0.75, 0.75, 0.75)) + (float3(0.375, 0.375, 0.375) * tmpvar_9.xyz)) + clamp(lerp(_Texture_var_2.xyz, (_Texture_var_2.xyz + (_Color.xyz * _Emission_var_1.xyz)), float3(_SettingsBelowWillWorkEmissionMultiplyWithMainTexture, _SettingsBelowWillWorkEmissionMultiplyWithMainTexture, _SettingsBelowWillWorkEmissionMultiplyWithMainTexture)), 0, 1)) + (in_f.xlv_TEXCOORD6 * _Texture_var_2.xyz));
          out_f.color = tmpvar_14;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Mobile/Diffuse"
}
