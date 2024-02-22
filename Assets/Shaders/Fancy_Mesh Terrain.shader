// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Fancy/Mesh Terrain"
{
  Properties
  {
    [KeywordEnum(ENABLED, DISABLED)] DEBUG_LIGHTMAP ("Debug/Lightmap", float) = 0
    [KeywordEnum(ENABLED, DISABLED)] FANCY_HERO_LIGHT ("Features/Hero Light", float) = 0
    [KeywordEnum(LIMIT_3, LIMIT_2)] FANCY_TERRAIN_SPLATS ("Features/Splats", float) = 0
    [KeywordEnum(DISABLED, ENABLED)] FANCY_TEXTURE_ROTATION ("Features/Rotation", float) = 0
    _Control ("General/Control texture", 2D) = "white" {}
    _Splat0 ("General/Layer 0 (R)", 2D) = "black" {}
    _Splat1 ("General/Layer 1 (G)", 2D) = "black" {}
    _Splat2 ("General/Layer 2 (B)", 2D) = "black" {}
    [HideInInspector] _Brush ("Brush properties (u,v - position, z - size in texcoords system)", Vector) = (0,0,0.1,0)
    [HideInInspector] _BrushColor ("Brush color", Color) = (0,0,1,1)
    [Rotation2DMatrix(FANCY_TEXTURE_ROTATION_ENABLED)] Fancy_TextureRotation ("General/Texture Rotation", Vector) = (1,0,0,1)
  }
  SubShader
  {
    Tags
    { 
      "QUEUE" = "Geometry-99"
      "RenderType" = "Opaque"
    }
    LOD 200
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "LIGHTMODE" = "ForwardBase"
        "QUEUE" = "Geometry-99"
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
      #pragma multi_compile DIRECTIONAL LIGHTMAP_ON DIRLIGHTMAP_OFF DYNAMICLIGHTMAP_OFF SHADOWS_OFF FANCY_HERO_LIGHT_ENABLED VERTEXLIGHT_OFF FANCY_FOG_DISABLED FANCY_FORCED_VERTEXLIGHT_OFF DEBUG_LIGHTMAP_ENABLED FANCY_TERRAIN_SPLATS_LIMIT_3 FANCY_TEXTURE_ROTATION_DISABLED EDITOR_OFF
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
      // uniform float4 unity_LightmapST;
      uniform float4 _HeroLightPos;
      uniform float4 Fancy_HeroLightParams;
      uniform float4 _Control_ST;
      uniform float4 _Splat0_ST;
      uniform float4 _Splat1_ST;
      uniform float4 _Splat2_ST;
      // uniform sampler2D unity_Lightmap;
      uniform sampler2D _Control;
      uniform sampler2D _Splat0;
      uniform sampler2D _Splat1;
      uniform sampler2D _Splat2;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_TEXCOORD3 :TEXCOORD3;
          float4 xlv_TEXCOORD4 :TEXCOORD4;
          float4 xlv_TEXCOORD7 :TEXCOORD7;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_TEXCOORD3 :TEXCOORD3;
          float4 xlv_TEXCOORD4 :TEXCOORD4;
          float4 xlv_TEXCOORD7 :TEXCOORD7;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4 tmpvar_1;
          float4 tmpvar_2;
          float4 tmpvar_3;
          float4 tmpvar_4;
          float4 tmpvar_5;
          tmpvar_1.xy = TRANSFORM_TEX(in_v.texcoord.xy, _Control);
          tmpvar_2.xy = TRANSFORM_TEX(in_v.texcoord.xy, _Splat0);
          tmpvar_3.xy = TRANSFORM_TEX(in_v.texcoord.xy, _Splat1);
          tmpvar_4.xy = TRANSFORM_TEX(in_v.texcoord.xy, _Splat2);
          float3 tmpvar_6;
          tmpvar_6 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
          float3x3 tmpvar_7;
          tmpvar_7[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_7[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_7[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          float3 tmpvar_8;
          tmpvar_8 = normalize(mul(tmpvar_7, in_v.normal));
          tmpvar_5.xy = ((in_v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
          float3 normal_9;
          normal_9 = tmpvar_8;
          float3 position_10;
          position_10 = tmpvar_6;
          float3 light_11;
          float3 tmpvar_12;
          tmpvar_12 = (_HeroLightPos.xyz - position_10);
          light_11 = tmpvar_12;
          float tmpvar_13;
          tmpvar_13 = sqrt(dot(light_11, light_11));
          float tmpvar_14;
          tmpvar_14 = (tmpvar_13 * Fancy_HeroLightParams.w);
          float tmpvar_15;
          tmpvar_15 = max(0, (1 - (tmpvar_14 * tmpvar_14)));
          float4 tmpvar_16;
          tmpvar_16.xyz = ((max(0, dot(normal_9, (light_11 / tmpvar_13))) * tmpvar_15) * Fancy_HeroLightParams.xyz);
          tmpvar_16.w = tmpvar_15;
          tmpvar_5.z = tmpvar_5.z;
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          out_v.xlv_TEXCOORD0 = tmpvar_1;
          out_v.xlv_TEXCOORD1 = tmpvar_2;
          out_v.xlv_TEXCOORD2 = tmpvar_3;
          out_v.xlv_TEXCOORD3 = tmpvar_4;
          out_v.xlv_TEXCOORD4 = tmpvar_5;
          out_v.xlv_TEXCOORD7 = tmpvar_16;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 color_1;
          float4 sc_2;
          float4 t2_3;
          float4 t1_4;
          float4 t0_5;
          float4 tmpvar_6;
          tmpvar_6 = tex2D(_Splat0, in_f.xlv_TEXCOORD1.xy);
          t0_5 = tmpvar_6;
          float4 tmpvar_7;
          tmpvar_7 = tex2D(_Splat1, in_f.xlv_TEXCOORD2.xy);
          t1_4 = tmpvar_7;
          float4 tmpvar_8;
          tmpvar_8 = tex2D(_Splat2, in_f.xlv_TEXCOORD3.xy);
          t2_3 = tmpvar_8;
          float4 tmpvar_9;
          tmpvar_9 = tex2D(_Control, in_f.xlv_TEXCOORD0.xy);
          sc_2 = tmpvar_9;
          float3 tmpvar_10;
          tmpvar_10 = (((t0_5.xyz * sc_2.x) + (t1_4.xyz * sc_2.y)) + (t2_3.xyz * sc_2.z));
          color_1.w = 0;
          float4 tmpvar_11;
          tmpvar_11 = UNITY_SAMPLE_TEX2D(unity_Lightmap, in_f.xlv_TEXCOORD4.xy);
          float3 tmpvar_12;
          tmpvar_12 = (2 * tmpvar_11.xyz);
          color_1.xyz = float3((tmpvar_10 * tmpvar_12));
          float3 tmpvar_13;
          tmpvar_13 = clamp(color_1.xyz, 0, 1);
          float3 tmpvar_14;
          tmpvar_14 = (tmpvar_13 * in_f.xlv_TEXCOORD7.xyz);
          color_1.xyz = (tmpvar_13 + (2 * lerp(tmpvar_14, ((-tmpvar_14) + in_f.xlv_TEXCOORD7.xyz), float3(bool3(tmpvar_13 >= float3(0.5, 0.5, 0.5))))));
          color_1.w = 1;
          color_1.xyz = color_1.xyz;
          color_1.w = color_1.w;
          out_f.color = color_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Mobile/Diffuse"
}
