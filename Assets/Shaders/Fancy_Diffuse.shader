// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Fancy/Diffuse"
{
  Properties
  {
    [KeywordEnum(DISABLED, ENABLED)] FANCY_COLOR_MASK ("Features/Color mask", float) = 0
    [KeywordEnum(DISABLED, ENABLED, ENABLED_WITH_ROTATION)] FANCY_REFLECTIONS ("Features/Reflections", float) = 0
    [KeywordEnum(DISABLED, ENABLED)] FANCY_BUMP_MAP ("Features/Shitty Bump", float) = 0
    [KeywordEnum(DISABLED, ENABLED)] FANCY_DESTRO ("Features/Destro", float) = 0
    [KeywordEnum(ALLOWED, FORBIDDEN)] FANCY_FOG ("Features/Fog", float) = 0
    _MainTex ("General/Base (RGB)", 2D) = "white" {}
    [NeedsKeyword(FANCY_REFLECTIONS_ENABLED, FANCY_DESTRO_ENABLED)] _AlphaTex ("General/MISC: G - reflection, B - noise", 2D) = "white" {}
    [NeedsKeyword(FANCY_COLOR_MASK_ENABLED)] _Color ("General/Color mask", Color) = (1,1,1,1)
    [NeedsKeyword(FANCY_BUMP_MAP_ENABLED)] _BumpMap ("General/Normalmap (for reflection and hero light)", 2D) = "bump" {}
    [NeedsKeyword(FANCY_REFLECTIONS_ENABLED)] _Cube ("Reflections/Cubemap", Cube) = "_Skybox" {}
    [SliderWithLabel(FANCY_REFLECTIONS_ENABLED)] Fancy_Reflectivity ("Reflections/Reflectivity", Range(0, 10)) = 0.8
    [SliderWithLabel(FANCY_REFLECTIONS_ENABLED)] Fancy_MinReflectivity ("Reflections/Min Reflectivity", Range(0, 1)) = 0.2
    [SliderWithLabel(FANCY_FOG_KAMIS_ENABLED)] Kamis_FogPitIntensity ("Kamis/Pit Fog Intesity", Range(0, 1)) = 1
    [SliderWithLabel(FANCY_DESTRO_ENABLED)] _destroPower ("Destro/Power", Range(0, 1)) = 0
    [SliderWithLabel(FANCY_DESTRO_ENABLED)] _destroRange ("Destro/Range", Range(0, 1)) = 0.1
    [KeywordEnum(DISABLED, ENABLED)] FANCY_FOG_SCALING2 ("Features/Fog Scaing", float) = 0
    [SliderWithLabel(FANCY_FOG_SCALING2_ENABLED)] Fancy_FogScale ("General/Fog Scale", Range(0, 2)) = 1
    [RotationMatrix(FANCY_REFLECTIONS_ENABLED_WITH_ROTATION)] Fancy_ReflectionRotation ("Reflections/Rotation", Vector) = (0,0,0,1)
  }
  SubShader
  {
    Tags
    { 
      "QUEUE" = "Geometry"
      "RenderType" = "Opaque"
    }
    LOD 200
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "LIGHTMODE" = "ForwardBase"
        "QUEUE" = "Geometry"
        "RenderType" = "Opaque"
      }
      LOD 200
      ZClip Off
      Fog
      { 
        Mode  Off
      } 
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile DIRECTIONAL FANCY_BUMP_MAP_DISABLED FANCY_COLOR_MASK_DISABLED FANCY_DESTRO_DISABLED FANCY_FOG_SCALING2_DISABLED LIGHTMAP_OFF DIRLIGHTMAP_OFF DYNAMICLIGHTMAP_OFF SHADOWS_OFF FANCY_REFLECTIONS_DISABLED VERTEXLIGHT_OFF FANCY_FOG_DISABLED FANCY_FORCED_VERTEXLIGHT_OFF FANCY_FOG_ALLOWED
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      
      
      #define CODE_BLOCK_VERTEX
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
      uniform float4 _MainTex_ST;
      uniform sampler2D _MainTex;
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
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4 tmpvar_1;
          tmpvar_1 = in_v.color;
          float4 tmpvar_2;
          float4 tmpvar_3;
          float4 tmpvar_4;
          tmpvar_2.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          tmpvar_3.z = tmpvar_1.w;
          float3x3 tmpvar_5;
          tmpvar_5[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_5[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_5[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          float3 tmpvar_6;
          tmpvar_6 = normalize(mul(tmpvar_5, in_v.normal));
          float3 tmpvar_7;
          tmpvar_7 = _WorldSpaceLightPos0.xyz;
          float3 tmpvar_8;
          tmpvar_8 = normalize(tmpvar_7);
          float4 tmpvar_9;
          tmpvar_9.w = 1;
          tmpvar_9.xyz = float3(tmpvar_6);
          float4 normal_10;
          normal_10 = tmpvar_9;
          float3 res_11;
          float3 x_12;
          x_12.x = dot(unity_SHAr, normal_10);
          x_12.y = dot(unity_SHAg, normal_10);
          x_12.z = dot(unity_SHAb, normal_10);
          float3 x1_13;
          float4 tmpvar_14;
          tmpvar_14 = (normal_10.xyzz * normal_10.yzzx);
          x1_13.x = dot(unity_SHBr, tmpvar_14);
          x1_13.y = dot(unity_SHBg, tmpvar_14);
          x1_13.z = dot(unity_SHBb, tmpvar_14);
          res_11 = (x_12 + (x1_13 + (unity_SHC.xyz * ((normal_10.x * normal_10.x) - (normal_10.y * normal_10.y)))));
          float3 tmpvar_15;
          float _tmp_dvx_24 = max(((1.055 * pow(max(res_11, float3(0, 0, 0)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
          tmpvar_15 = float3(_tmp_dvx_24, _tmp_dvx_24, _tmp_dvx_24);
          res_11 = tmpvar_15;
          tmpvar_4.xyz = float3(tmpvar_15);
          float3 tmpvar_16;
          float3 lightDir_17;
          lightDir_17 = tmpvar_8;
          float3 normal_18;
          normal_18 = tmpvar_6;
          tmpvar_16 = (_LightColor0.xyz * max(0, dot(normal_18, lightDir_17)));
          tmpvar_4.xyz = (tmpvar_4.xyz + tmpvar_16);
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          out_v.xlv_TEXCOORD0 = tmpvar_2;
          out_v.xlv_TEXCOORD1 = tmpvar_3;
          out_v.xlv_TEXCOORD2 = tmpvar_4;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 color_1;
          float3 albedo_2;
          float3 tmpvar_3;
          tmpvar_3 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy).xyz;
          albedo_2 = tmpvar_3;
          color_1.w = 1;
          color_1.xyz = (albedo_2 * in_f.xlv_TEXCOORD2.xyz);
          color_1.w = color_1.w;
          out_f.color = color_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Mobile/Diffuse"
}
