// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Fancy/Hero"
{
  Properties
  {
    [KeywordEnum(DISABLED, ENABLED, ENABLED_WITH_ROTATION)] FANCY_REFLECTIONS ("Features/Reflections", float) = 0
    [KeywordEnum(DISABLED, ENABLED)] FANCY_EMISSION ("Features/Emission", float) = 0
    [KeywordEnum(DISABLED, ENABLED)] FANCY_SELECTION ("Features/Selection", float) = 0
    _MainTex ("General/Base (RGB)", 2D) = "white" {}
    _MiscMap ("General/MISC: R - reflection, B - hit mask", 2D) = "white" {}
    _MiscMap2 ("General/MISC2: RGB - emission", 2D) = "white" {}
    [SliderWithLabel] Fancy_RimPower ("Rim/Power", Range(0, 10)) = 1
    _Cube ("Reflections/Cubemap", Cube) = "_Skybox" {}
    [SliderWithLabel] Fancy_Reflectivity ("Reflections/Reflection Scale", Range(0, 10)) = 0.8
    [SliderWithLabel] Fancy_MinReflectivity ("Reflections/Min Reflection Strength", Range(0, 1)) = 0.2
    [RotationMatrix(FANCY_REFLECTIONS_ENABLED_WITH_ROTATION)] Fancy_ReflectionRotation ("Reflections/Rotation", Vector) = (0,0,0,1)
    Fancy_EmissionColor ("Emission/Color", Color) = (1,1,1,1)
    [SliderWithLabel] Fancy_EmissionIntensity ("Emission/Intensity", Range(0, 1)) = 1
    [SliderWithLabel] _selectionSelected ("Selection/Selected", Range(0, 1)) = 0
    [SliderWithLabel] _selectionFaded ("Selection/Faded", Range(0, 1)) = 0
    [SliderWithLabel] _rzuk2 ("General/Min Shadow Value", Range(0, 1)) = 0
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
      #pragma multi_compile DIRECTIONAL VERTEXLIGHT_ON FANCY_EMISSION_DISABLED FANCY_SELECTION_ENABLED LIGHTMAP_OFF DIRLIGHTMAP_OFF DYNAMICLIGHTMAP_OFF SHADOWS_OFF FANCY_REFLECTIONS_DISABLED
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
      //uniform float4 unity_4LightPosX0;
      //uniform float4 unity_4LightPosY0;
      //uniform float4 unity_4LightPosZ0;
      //uniform float4 unity_4LightAtten0;
      //uniform float4 unity_LightColor[8];
      //uniform float4 unity_SHAr;
      //uniform float4 unity_SHAg;
      //uniform float4 unity_SHAb;
      //uniform float4 unity_SHBr;
      //uniform float4 unity_SHBg;
      //uniform float4 unity_SHBb;
      //uniform float4 unity_SHC;
      //uniform float4x4 UNITY_MATRIX_MVP;
      //uniform float4x4 unity_ObjectToWorld;
      uniform float Fancy_pointLightIncrease;
      uniform float Fancy_lightProbesIncrease;
      uniform float4x4 Fancy_RimRotation;
      uniform float4 _MainTex_ST;
      uniform float3 Fancy_HeroAmbientLight;
      uniform float _rzuk2;
      uniform float4 _LightColor0;
      uniform sampler2D _MainTex;
      uniform float4 Fancy_ShadowColor;
      uniform float Fancy_RimPower;
      uniform float4 Fancy_RimParams;
      uniform float4 Fancy_HeroStaticFog;
      uniform float4 Fancy_SelectionSelected;
      uniform float4 Fancy_SelectionFaded;
      uniform float _selectionSelected;
      uniform float _selectionFaded;
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
          float3 xlv_TEXCOORD2 :TEXCOORD2;
          float3 xlv_TEXCOORD3 :TEXCOORD3;
          float4 xlv_TEXCOORD4 :TEXCOORD4;
          float3 xlv_TEXCOORD5 :TEXCOORD5;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float3 xlv_TEXCOORD2 :TEXCOORD2;
          float3 xlv_TEXCOORD3 :TEXCOORD3;
          float3 xlv_TEXCOORD5 :TEXCOORD5;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float3 viewDir_1;
          float3 normal_2;
          float3 worldPos_3;
          float shadowMask_4;
          float4 tmpvar_5;
          float4 tmpvar_6;
          float3 tmpvar_7;
          float3 tmpvar_8;
          float4 tmpvar_9;
          float tmpvar_10;
          tmpvar_10 = in_v.color.x;
          shadowMask_4 = tmpvar_10;
          float3 tmpvar_11;
          float4 tmpvar_12;
          tmpvar_12 = mul(unity_ObjectToWorld, in_v.vertex);
          tmpvar_11 = tmpvar_12.xyz;
          worldPos_3 = tmpvar_11;
          float3x3 tmpvar_13;
          tmpvar_13[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_13[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_13[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          float3 tmpvar_14;
          tmpvar_14 = normalize(mul(tmpvar_13, in_v.normal));
          normal_2 = tmpvar_14;
          float3 tmpvar_15;
          tmpvar_15 = normalize((_WorldSpaceCameraPos - tmpvar_12.xyz));
          viewDir_1 = tmpvar_15;
          tmpvar_5.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          float3 worldPos_16;
          worldPos_16 = worldPos_3;
          float3 worldNormal_17;
          worldNormal_17 = normal_2;
          float3 lightColor_18;
          float3 lightColor0_19;
          lightColor0_19 = unity_LightColor[0].xyz;
          float3 lightColor1_20;
          lightColor1_20 = unity_LightColor[1].xyz;
          float3 lightColor2_21;
          lightColor2_21 = unity_LightColor[2].xyz;
          float3 lightColor3_22;
          lightColor3_22 = unity_LightColor[3].xyz;
          float4 lightAttenSq_23;
          lightAttenSq_23 = unity_4LightAtten0;
          float3 col_24;
          float4 ndotl_25;
          float4 lengthSq_26;
          float4 tmpvar_27;
          tmpvar_27 = (unity_4LightPosX0 - worldPos_16.x);
          float4 tmpvar_28;
          tmpvar_28 = (unity_4LightPosY0 - worldPos_16.y);
          float4 tmpvar_29;
          tmpvar_29 = (unity_4LightPosZ0 - worldPos_16.z);
          lengthSq_26 = (tmpvar_27 * tmpvar_27);
          lengthSq_26 = (lengthSq_26 + (tmpvar_28 * tmpvar_28));
          lengthSq_26 = (lengthSq_26 + (tmpvar_29 * tmpvar_29));
          float4 tmpvar_30;
          tmpvar_30 = max(lengthSq_26, float4(1E-06, 1E-06, 1E-06, 1E-06));
          lengthSq_26 = tmpvar_30;
          ndotl_25 = (tmpvar_27 * worldNormal_17.x);
          ndotl_25 = (ndotl_25 + (tmpvar_28 * worldNormal_17.y));
          ndotl_25 = (ndotl_25 + (tmpvar_29 * worldNormal_17.z));
          float4 tmpvar_31;
          tmpvar_31 = max(float4(0, 0, 0, 0), (ndotl_25 * rsqrt(tmpvar_30)));
          ndotl_25 = tmpvar_31;
          float4 tmpvar_32;
          tmpvar_32 = (tmpvar_31 * (1 / (1 + (tmpvar_30 * lightAttenSq_23))));
          col_24 = (lightColor0_19 * tmpvar_32.x);
          col_24 = (col_24 + (lightColor1_20 * tmpvar_32.y));
          col_24 = (col_24 + (lightColor2_21 * tmpvar_32.z));
          col_24 = (col_24 + (lightColor3_22 * tmpvar_32.w));
          float _tmp_dvx_17 = ((1 + Fancy_pointLightIncrease) * col_24);
          lightColor_18 = float3(_tmp_dvx_17, _tmp_dvx_17, _tmp_dvx_17);
          float4 tmpvar_33;
          tmpvar_33.w = 1;
          tmpvar_33.xyz = float3(worldNormal_17);
          float4 normal_34;
          normal_34 = tmpvar_33;
          float3 res_35;
          float3 x_36;
          x_36.x = dot(unity_SHAr, normal_34);
          x_36.y = dot(unity_SHAg, normal_34);
          x_36.z = dot(unity_SHAb, normal_34);
          float3 x1_37;
          float4 tmpvar_38;
          tmpvar_38 = (normal_34.xyzz * normal_34.yzzx);
          x1_37.x = dot(unity_SHBr, tmpvar_38);
          x1_37.y = dot(unity_SHBg, tmpvar_38);
          x1_37.z = dot(unity_SHBb, tmpvar_38);
          res_35 = (x_36 + (x1_37 + (unity_SHC.xyz * ((normal_34.x * normal_34.x) - (normal_34.y * normal_34.y)))));
          float3 tmpvar_39;
          float _tmp_dvx_18 = max(((1.055 * pow(max(res_35, float3(0, 0, 0)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
          tmpvar_39 = float3(_tmp_dvx_18, _tmp_dvx_18, _tmp_dvx_18);
          res_35 = tmpvar_39;
          lightColor_18 = (lightColor_18 + ((1 + Fancy_lightProbesIncrease) * tmpvar_39));
          tmpvar_6.xyz = float3((lightColor_18 + Fancy_HeroAmbientLight));
          tmpvar_6.w = max(shadowMask_4, _rzuk2);
          tmpvar_7 = normal_2;
          tmpvar_9.xyz = float3(viewDir_1);
          float3 tmpvar_40;
          float3x3 tmpvar_41;
          tmpvar_41[0] = conv_mxt4x4_0(Fancy_RimRotation).xyz;
          tmpvar_41[1] = conv_mxt4x4_1(Fancy_RimRotation).xyz;
          tmpvar_41[2] = conv_mxt4x4_2(Fancy_RimRotation).xyz;
          tmpvar_40 = mul(tmpvar_41, viewDir_1);
          tmpvar_8 = tmpvar_40;
          float3 tmpvar_42;
          tmpvar_42 = _WorldSpaceLightPos0.xyz;
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          out_v.xlv_TEXCOORD0 = tmpvar_5;
          out_v.xlv_TEXCOORD1 = tmpvar_6;
          out_v.xlv_TEXCOORD2 = tmpvar_7;
          out_v.xlv_TEXCOORD3 = tmpvar_8;
          out_v.xlv_TEXCOORD4 = tmpvar_9;
          out_v.xlv_TEXCOORD5 = tmpvar_42;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float3 color_1;
          float3 emission_2;
          float3 rimDir_3;
          float3 lightDir_4;
          float3 normal_5;
          float shadowMask_6;
          float4 tmpvar_7;
          tmpvar_7 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy);
          float tmpvar_8;
          tmpvar_8 = in_f.xlv_TEXCOORD1.w;
          shadowMask_6 = tmpvar_8;
          float3 tmpvar_9;
          tmpvar_9 = normalize(in_f.xlv_TEXCOORD2);
          normal_5 = tmpvar_9;
          lightDir_4 = in_f.xlv_TEXCOORD5;
          rimDir_3 = in_f.xlv_TEXCOORD3;
          float3 tmpvar_10;
          tmpvar_10 = in_f.xlv_TEXCOORD1.xyz;
          emission_2 = tmpvar_10;
          color_1 = (((tmpvar_7.xyz * (_LightColor0.xyz * max(0, dot(normal_5, lightDir_4)))) * shadowMask_6) + (tmpvar_7.xyz * emission_2));
          float _tmp_dvx_19 = (shadowMask_6 + ((1 - shadowMask_6) * (1 - Fancy_ShadowColor.w)));
          color_1 = (Fancy_HeroStaticFog.xyz + (lerp(Fancy_ShadowColor.xyz, color_1, float3(_tmp_dvx_19, _tmp_dvx_19, _tmp_dvx_19)) * (1 - Fancy_HeroStaticFog.w)));
          color_1 = (color_1 + ((((Fancy_RimParams.xyz * pow((1 - clamp(dot(rimDir_3, normal_5), 0, 1)), Fancy_RimParams.w)) * Fancy_RimPower) * shadowMask_6) * tmpvar_7.xyz));
          color_1 = ((color_1 * (1 - (_selectionFaded * Fancy_SelectionFaded.w))) + ((Fancy_SelectionFaded.xyz * _selectionFaded) * Fancy_SelectionFaded.w));
          color_1 = ((color_1 * (1 - (_selectionSelected * Fancy_SelectionSelected.w))) + ((Fancy_SelectionSelected.xyz * _selectionSelected) * Fancy_SelectionSelected.w));
          float4 tmpvar_11;
          tmpvar_11.xyz = float3(color_1);
          tmpvar_11.w = 1;
          out_f.color = tmpvar_11;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Mobile/Diffuse"
}
