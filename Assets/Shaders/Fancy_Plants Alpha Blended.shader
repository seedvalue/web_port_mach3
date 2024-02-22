// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Fancy/Plants Alpha Blended"
{
  Properties
  {
    [KeywordEnum(ENABLED, DISABLED)] FANCY_HERO_LIGHT ("Features/Hero Light", float) = 0
    [Enum(ON, 1, OFF, 0)] _ZWrite ("Features/ZWrite", float) = 1
    _MainTex ("General/Base (RGB)", 2D) = "white" {}
    _AlphaTex ("General/Alpha", 2D) = "white" {}
    [SliderWithLabel] _Cutoff ("General/Cutoff", Range(0, 1)) = 0.5
    [SliderWithLabel] _Disp ("General/Displacement", Range(0, 1)) = 0.5
    _TransparencyLM ("Lightmapper/Alpha Inverted", 2D) = "black" {}
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
      "SHADOWSUPPORT" = "false"
    }
    LOD 200
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "ForwardBase"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
        "SHADOWSUPPORT" = "false"
      }
      LOD 200
      ZClip Off
      ZWrite Off
      Fog
      { 
        Mode  Off
      } 
      Blend SrcAlpha OneMinusSrcAlpha
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile DIRECTIONAL LIGHTMAP_OFF DIRLIGHTMAP_OFF DYNAMICLIGHTMAP_OFF SHADOWS_OFF FANCY_HERO_LIGHT_ENABLED VERTEXLIGHT_OFF FANCY_FOG_DISABLED FANCY_FORCED_VERTEXLIGHT_OFF
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 _Time;
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
      uniform float4 _HeroLightPos;
      uniform float4 Fancy_HeroLightParams;
      uniform float4 _MainTex_ST;
      uniform float _Disp;
      uniform sampler2D _MainTex;
      uniform sampler2D _AlphaTex;
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
          float4 xlv_TEXCOORD3 :TEXCOORD3;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_TEXCOORD3 :TEXCOORD3;
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
          tmpvar_2.w = in_v.vertex.w;
          tmpvar_2.xyz = (in_v.vertex.xyz + ((in_v.color.xyz * _Disp) * cos(((frac((sin(dot(floor(mul(unity_ObjectToWorld, in_v.vertex).xz), float2(12.9898, 78.233))) * 43758.55)) * 100) + _Time.z))));
          float4 tmpvar_3;
          float4 tmpvar_4;
          float4 tmpvar_5;
          float4 tmpvar_6;
          tmpvar_3.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          tmpvar_4.z = tmpvar_1.w;
          float3 tmpvar_7;
          tmpvar_7 = mul(unity_ObjectToWorld, tmpvar_2).xyz;
          float3x3 tmpvar_8;
          tmpvar_8[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_8[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_8[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          float3 tmpvar_9;
          tmpvar_9 = normalize(mul(tmpvar_8, in_v.normal));
          float3 tmpvar_10;
          tmpvar_10 = _WorldSpaceLightPos0.xyz;
          float3 tmpvar_11;
          tmpvar_11 = normalize(tmpvar_10);
          float4 tmpvar_12;
          tmpvar_12.w = 1;
          tmpvar_12.xyz = float3(tmpvar_9);
          float4 normal_13;
          normal_13 = tmpvar_12;
          float3 res_14;
          float3 x_15;
          x_15.x = dot(unity_SHAr, normal_13);
          x_15.y = dot(unity_SHAg, normal_13);
          x_15.z = dot(unity_SHAb, normal_13);
          float3 x1_16;
          float4 tmpvar_17;
          tmpvar_17 = (normal_13.xyzz * normal_13.yzzx);
          x1_16.x = dot(unity_SHBr, tmpvar_17);
          x1_16.y = dot(unity_SHBg, tmpvar_17);
          x1_16.z = dot(unity_SHBb, tmpvar_17);
          res_14 = (x_15 + (x1_16 + (unity_SHC.xyz * ((normal_13.x * normal_13.x) - (normal_13.y * normal_13.y)))));
          float3 tmpvar_18;
          float _tmp_dvx_23 = max(((1.055 * pow(max(res_14, float3(0, 0, 0)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
          tmpvar_18 = float3(_tmp_dvx_23, _tmp_dvx_23, _tmp_dvx_23);
          res_14 = tmpvar_18;
          tmpvar_5.xyz = float3(tmpvar_18);
          float3 tmpvar_19;
          float3 lightDir_20;
          lightDir_20 = tmpvar_11;
          float3 normal_21;
          normal_21 = tmpvar_9;
          tmpvar_19 = (_LightColor0.xyz * max(0, dot(normal_21, lightDir_20)));
          tmpvar_5.xyz = (tmpvar_5.xyz + tmpvar_19);
          float3 normal_22;
          normal_22 = tmpvar_9;
          float3 position_23;
          position_23 = tmpvar_7;
          float3 light_24;
          float3 tmpvar_25;
          tmpvar_25 = (_HeroLightPos.xyz - position_23);
          light_24 = tmpvar_25;
          float tmpvar_26;
          tmpvar_26 = sqrt(dot(light_24, light_24));
          float tmpvar_27;
          tmpvar_27 = (tmpvar_26 * Fancy_HeroLightParams.w);
          float tmpvar_28;
          tmpvar_28 = max(0, (1 - (tmpvar_27 * tmpvar_27)));
          float4 tmpvar_29;
          tmpvar_29.xyz = ((max(0, dot(normal_22, (light_24 / tmpvar_26))) * tmpvar_28) * Fancy_HeroLightParams.xyz);
          tmpvar_29.w = tmpvar_28;
          tmpvar_6 = tmpvar_29;
          out_v.vertex = UnityObjectToClipPos(tmpvar_2);
          out_v.xlv_TEXCOORD0 = tmpvar_3;
          out_v.xlv_TEXCOORD1 = tmpvar_4;
          out_v.xlv_TEXCOORD2 = tmpvar_5;
          out_v.xlv_TEXCOORD3 = tmpvar_6;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float3 heroLightColor_1;
          float4 color_2;
          float alpha_3;
          float3 misc_4;
          float3 albedo_5;
          float3 tmpvar_6;
          tmpvar_6 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy).xyz;
          albedo_5 = tmpvar_6;
          float3 tmpvar_7;
          tmpvar_7 = tex2D(_AlphaTex, in_f.xlv_TEXCOORD0.xy).xyz;
          misc_4 = tmpvar_7;
          alpha_3 = misc_4.x;
          color_2.w = alpha_3;
          color_2.xyz = (albedo_5 * in_f.xlv_TEXCOORD2.xyz);
          heroLightColor_1 = in_f.xlv_TEXCOORD3.xyz;
          float3 tmpvar_8;
          tmpvar_8 = clamp(color_2.xyz, 0, 1);
          float3 tmpvar_9;
          tmpvar_9 = (tmpvar_8 * heroLightColor_1);
          color_2.xyz = float3((tmpvar_8 + (2 * lerp(tmpvar_9, ((-tmpvar_9) + heroLightColor_1), float3(bool3(tmpvar_8 >= float3(0.5, 0.5, 0.5)))))));
          out_f.color = color_2;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Mobile/Diffuse"
}
