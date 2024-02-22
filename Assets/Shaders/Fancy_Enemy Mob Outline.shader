// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Fancy/Enemy Mob Outline"
{
  Properties
  {
    [KeywordEnum(DISABLED, ENABLED)] FANCY_DESTRO ("Features/Destro", float) = 0
    [KeywordEnum(DISABLED, ENABLED)] FANCY_REFLECTIONS ("Features/Reflections", float) = 0
    [KeywordEnum(DISABLED, ENABLED)] FANCY_EMISSION ("Features/Emission", float) = 0
    [KeywordEnum(DISABLED, ENABLED)] FANCY_OUTLINE ("Features/Outline", float) = 0
    _MainTex ("General/Base (RGB)", 2D) = "white" {}
    _MiscMap ("General/MISC1: R - reflection, G - emission, B - noise", 2D) = "white" {}
    [SliderWithLabel] Fancy_RimPower ("Rim/Power", Range(0, 10)) = 1
    _hitColor ("Hit/Hit color", Color) = (1,1,1,0)
    [SliderWithLabel] _hitBlend ("Hit/Hit blend", Range(0, 1)) = 0
    _Cube ("Reflections/Cubemap", Cube) = "_Skybox" {}
    [SliderWithLabel] Fancy_Reflectivity ("Reflections/Reflectivity", Range(0, 10)) = 0.8
    [SliderWithLabel] Fancy_MinReflectivity ("Reflections/Min Reflectivity", Range(0, 1)) = 0.2
    [SliderWithLabel] _destroPower ("Destro/Power", Range(0, 1)) = 0
    [SliderWithLabel] _destroRange ("Destro/Range", Range(0, 1)) = 0.1
    Fancy_EmissionColor ("Emission/Color", Color) = (1,1,1,1)
    [SliderWithLabel] Fancy_EmissionIntensity ("Emission/Intensity", Range(0, 1)) = 1
    [SliderWithLabel] Fancy_EmissionFogClamp ("Emission/Fog Clamp", Range(0, 1)) = 0.75
    _OutlineIntensity ("Outline/Intensity", Range(0, 1)) = 1
    _OutlineWidth ("Outline/Width", Range(0, 0.1)) = 0.005
    _OutlineMaskOffset ("Outline/Mask Offset", Range(-1, 1)) = 0
  }
  SubShader
  {
    Tags
    { 
      "QUEUE" = "Geometry"
      "RenderType" = "Opaque"
    }
    LOD 200
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "QUEUE" = "Geometry"
        "RenderType" = "Opaque"
      }
      LOD 200
      ZClip Off
      ZTest Always
      Fog
      { 
        Mode  Off
      } 
      Blend SrcAlpha OneMinusSrcAlpha
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile FANCY_OUTLINE_DISABLED
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 UNITY_MATRIX_MVP;
      struct appdata_t
      {
          float4 vertex :POSITION;
      };
      
      struct OUT_Data_Vert
      {
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 vertex :Position;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          discard;
          out_f.color = float4(0, 0, 0, 0);
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 2, name: 
    {
      Tags
      { 
        "LIGHTMODE" = "ForwardBase"
        "QUEUE" = "Geometry"
        "RenderType" = "Opaque"
        "SHADOWSUPPORT" = "false"
      }
      LOD 200
      ZClip Off
      Fog
      { 
        Mode  Off
      } 
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile DIRECTIONAL FANCY_DESTRO_DISABLED FANCY_EMISSION_DISABLED LIGHTMAP_OFF DIRLIGHTMAP_OFF DYNAMICLIGHTMAP_OFF SHADOWS_OFF FANCY_REFLECTIONS_DISABLED VERTEXLIGHT_OFF FANCY_FOG_DISABLED
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4 unity_SHAr;
      //uniform float4 unity_SHAg;
      //uniform float4 unity_SHAb;
      //uniform float4 unity_SHBr;
      //uniform float4 unity_SHBg;
      //uniform float4 unity_SHBb;
      //uniform float4 unity_SHC;
      //uniform float4x4 UNITY_MATRIX_MVP;
      //uniform float4x4 unity_ObjectToWorld;
      uniform float4 _LightColor0;
      uniform float Fancy_Reflectivity;
      uniform float Fancy_MinReflectivity;
      uniform float Fancy_lightProbesIncrease;
      uniform float Fancy_RimPower;
      uniform float4 Fancy_RimParams;
      uniform float4x4 Fancy_RimRotation;
      uniform float4 _MainTex_ST;
      uniform float4 Kamis_EnemyAmbientColor;
      uniform float4 Kamis_DirectionalLightColor;
      uniform float3 Kamis_DirectionalLightDir;
      uniform sampler2D _MainTex;
      uniform float3 _hitColor;
      uniform float _hitBlend;
      uniform float4 Fancy_ShadowColor;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float3 xlv_TEXCOORD3 :TEXCOORD3;
          float4 xlv_TEXCOORD4 :TEXCOORD4;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float3 xlv_TEXCOORD3 :TEXCOORD3;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float3 viewDir_1;
          float3 lightDir_2;
          float3 normal_3;
          float shadowMask_4;
          float4 tmpvar_5;
          float4 tmpvar_6;
          float4 tmpvar_7;
          float tmpvar_8;
          tmpvar_8 = in_v.color.x;
          shadowMask_4 = tmpvar_8;
          float3x3 tmpvar_9;
          tmpvar_9[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_9[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_9[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          float3 tmpvar_10;
          tmpvar_10 = normalize(mul(tmpvar_9, in_v.normal));
          normal_3 = tmpvar_10;
          float3 tmpvar_11;
          tmpvar_11 = _WorldSpaceLightPos0.xyz;
          float3 tmpvar_12;
          tmpvar_12 = normalize(tmpvar_11);
          lightDir_2 = tmpvar_12;
          tmpvar_5.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          float3 worldNormal_13;
          worldNormal_13 = normal_3;
          float3 lightColor_14;
          float4 tmpvar_15;
          tmpvar_15.w = 1;
          tmpvar_15.xyz = float3(worldNormal_13);
          float4 normal_16;
          normal_16 = tmpvar_15;
          float3 res_17;
          float3 x_18;
          x_18.x = dot(unity_SHAr, normal_16);
          x_18.y = dot(unity_SHAg, normal_16);
          x_18.z = dot(unity_SHAb, normal_16);
          float3 x1_19;
          float4 tmpvar_20;
          tmpvar_20 = (normal_16.xyzz * normal_16.yzzx);
          x1_19.x = dot(unity_SHBr, tmpvar_20);
          x1_19.y = dot(unity_SHBg, tmpvar_20);
          x1_19.z = dot(unity_SHBb, tmpvar_20);
          res_17 = (x_18 + (x1_19 + (unity_SHC.xyz * ((normal_16.x * normal_16.x) - (normal_16.y * normal_16.y)))));
          float3 tmpvar_21;
          float _tmp_dvx_20 = max(((1.055 * pow(max(res_17, float3(0, 0, 0)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
          tmpvar_21 = float3(_tmp_dvx_20, _tmp_dvx_20, _tmp_dvx_20);
          res_17 = tmpvar_21;
          float _tmp_dvx_21 = ((1 + Fancy_lightProbesIncrease) * tmpvar_21);
          lightColor_14 = float3(_tmp_dvx_21, _tmp_dvx_21, _tmp_dvx_21);
          tmpvar_6.xyz = (lightColor_14 + (Kamis_EnemyAmbientColor.xyz * Kamis_EnemyAmbientColor.w));
          tmpvar_6.xyz = (tmpvar_6.xyz + ((Kamis_DirectionalLightColor.xyz * max(0, dot(normal_3, Kamis_DirectionalLightDir))) * Kamis_DirectionalLightColor.w));
          tmpvar_6.xyz = (tmpvar_6.xyz + ((_LightColor0.xyz * max(0, dot(normal_3, lightDir_2))) * shadowMask_4));
          tmpvar_6.w = shadowMask_4;
          float3 tmpvar_22;
          tmpvar_22 = normalize((_WorldSpaceCameraPos - mul(unity_ObjectToWorld, in_v.vertex).xyz));
          viewDir_1 = tmpvar_22;
          float3x3 tmpvar_23;
          tmpvar_23[0] = conv_mxt4x4_0(Fancy_RimRotation).xyz;
          tmpvar_23[1] = conv_mxt4x4_1(Fancy_RimRotation).xyz;
          tmpvar_23[2] = conv_mxt4x4_2(Fancy_RimRotation).xyz;
          float3 I_24;
          I_24 = (-viewDir_1);
          tmpvar_7.xyz = float3((I_24 - (2 * (dot(normal_3, I_24) * normal_3))));
          tmpvar_7.w = (Fancy_Reflectivity + Fancy_MinReflectivity);
          tmpvar_5.z = tmpvar_5.z;
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          out_v.xlv_TEXCOORD0 = tmpvar_5;
          out_v.xlv_TEXCOORD1 = tmpvar_6;
          out_v.xlv_TEXCOORD2 = float4(1, 1, 1, 1);
          out_v.xlv_TEXCOORD3 = (((Fancy_RimParams.xyz * pow((1 - clamp(dot(mul(tmpvar_23, viewDir_1), normal_3), 0, 1)), Fancy_RimParams.w)) * Fancy_RimPower) * shadowMask_4);
          out_v.xlv_TEXCOORD4 = tmpvar_7;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float3 color_2;
          float4 tmpvar_3;
          tmpvar_3 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy);
          color_2 = (tmpvar_3.xyz * in_f.xlv_TEXCOORD1.xyz);
          float _tmp_dvx_22 = (in_f.xlv_TEXCOORD1.w + ((1 - in_f.xlv_TEXCOORD1.w) * (1 - Fancy_ShadowColor.w)));
          color_2 = lerp(Fancy_ShadowColor.xyz, color_2, float3(_tmp_dvx_22, _tmp_dvx_22, _tmp_dvx_22));
          color_2 = (color_2 + ((in_f.xlv_TEXCOORD3 * tmpvar_3.xyz) + (_hitColor * _hitBlend)));
          float4 tmpvar_4;
          tmpvar_4.xyz = float3(color_2);
          tmpvar_4.w = in_f.xlv_TEXCOORD0.z;
          tmpvar_1 = tmpvar_4;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Mobile/Diffuse"
}
