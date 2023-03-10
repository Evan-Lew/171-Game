// Automatically patched for procedural instancing with Nature Renderer: https://visualdesigncafe.com/nature-renderer
// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Polyart/Dreamscape/URP/Tree Wind (Nature Renderer)"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin][Enum(Back,2,Front,1,Off,0)]_Culling("Culling", Int) = 2
		[Header(Foliage)][Header(.)][SingleLineTexture]_FoliageColorMap("Foliage Color Map", 2D) = "white" {}
		[Normal][SingleLineTexture]_FoliageNormalMap("Foliage Normal Map", 2D) = "bump" {}
		_FoliageSize("Foliage Size", Float) = 100
		_FoliageColorTop("Foliage Color Top", Color) = (1,1,1,0)
		_FoliageColorBottom("Foliage Color Bottom", Color) = (0,0,0,0)
		_GradientOffset("Gradient Offset", Float) = 0
		_GradientFallout("Gradient Fallout", Float) = 0
		_FoliageEmissiveColor("Foliage Emissive Color", Color) = (0,0,0,0)
		_FoliageNormalIntensity("Foliage Normal Intensity", Range( 0 , 1)) = 1
		_FoliageRoughness("Foliage Roughness", Range( 0 , 1)) = 0.85
		_FoliageEmissiveIntensity("Foliage Emissive Intensity", Range( 0 , 20)) = 0
		_AlphaClip("Alpha Clip", Range( 0 , 1)) = 0.5
		[Header (Trunk)][Toggle(_USEDFORTRUNK_ON)] _UsedforTrunk("Used for Trunk?", Float) = 0
		[SingleLineTexture]_ColorMap("Color Map", 2D) = "white" {}
		[SingleLineTexture]_NormalMap("Normal Map", 2D) = "white" {}
		[SingleLineTexture]_ORMMap("ORM Map", 2D) = "white" {}
		[SingleLineTexture]_EmissiveMap("Emissive Map", 2D) = "white" {}
		_TextureSize("Texture Size", Float) = 100
		_ColorTint("Color Tint", Color) = (1,1,1,0)
		_NormalIntensity("Normal Intensity", Range( 0 , 1)) = 1
		_RoughnessIntensity("Roughness Intensity", Range( 0 , 2)) = 1
		_EmissiveColor("Emissive Color", Color) = (0,0,0,0)
		_EmissiveIntensity("Emissive Intensity", Range( 0 , 20)) = 0
		[Header(WIND RUSTLE)][Toggle(_USEGLOBALWINDSETTINGS_ON)] _UseGlobalWindSettings("Use Global Wind Settings?", Float) = 0
		[HideInInspector][SingleLineTexture]_NoiseTexture("NoiseTexture", 2D) = "white" {}
		_WindScrollSpeed("Wind Scroll Speed", Range( 0 , 0.5)) = 0.05
		_WindJitterSpeed("Wind Jitter Speed", Range( 0 , 0.5)) = 0.05
		_WindOffsetIntensity("Wind Offset Intensity", Range( 0 , 1)) = 1
		_WindRustleSize("Wind Rustle Size", Range( 0 , 0.2)) = 0.035
		[Header(WIND SWAY)][Toggle(_USESGLOBALWINDSETTINGS_ON)] _UsesGlobalWindSettings("Uses Global Wind Settings?", Float) = 0
		_WindSwayDirection("Wind Sway Direction", Vector) = (1,0,0,0)
		_WIndSwayIntensity("WInd Sway Intensity", Float) = 1
		[ASEEnd]_WIndSwayFrequency("WInd Sway Frequency", Float) = 1

		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		_TransStrength( "Strength", Range( 0, 50 ) ) = 1
		_TransNormal( "Normal Distortion", Range( 0, 1 ) ) = 0.5
		_TransScattering( "Scattering", Range( 1, 50 ) ) = 2
		_TransDirect( "Direct", Range( 0, 1 ) ) = 0.9
		_TransAmbient( "Ambient", Range( 0, 1 ) ) = 0.1
		_TransShadow( "Shadow", Range( 0, 1 ) ) = 0.5
		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry"  "NatureRendererInstancing"="True" }
		Cull Off
		AlphaToMask Off
		
		HLSLINCLUDE
		#pragma target 2.0

		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x 

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS

		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward"  "NatureRendererInstancing"="True" }
			
			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			

			#define _NORMAL_DROPOFF_TS 1

			#pragma multi_compile_instancing

			#pragma multi_compile _ LOD_FADE_CROSSFADE

			#pragma multi_compile_fog

			#define ASE_FOG 1

			#define _TRANSLUCENCY_ASE 1

			#define _EMISSION

			#define _ALPHATEST_ON 1

			#define _NORMALMAP 1

			#define ASE_SRP_VERSION 999999



			

			#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF

			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

			#pragma multi_compile _ _SHADOWS_SOFT

			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

			

			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING

			#pragma multi_compile _ SHADOWS_SHADOWMASK



			#pragma multi_compile _ DIRLIGHTMAP_COMBINED

			#pragma multi_compile _ LIGHTMAP_ON



			#pragma vertex vert

			#pragma fragment frag



			#define SHADERPASS_FORWARD



			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			

			#if ASE_SRP_VERSION <= 70108

			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR

			#endif



			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)

			    #define ENABLE_TERRAIN_PERPIXEL_NORMAL

			#endif



			#define ASE_NEEDS_VERT_NORMAL

			#define ASE_NEEDS_VERT_POSITION

			#pragma shader_feature_local _USEDFORTRUNK_ON

			#pragma shader_feature_local _USEGLOBALWINDSETTINGS_ON

			#pragma shader_feature_local _USESGLOBALWINDSETTINGS_ON
			#include "Assets/Visual Design Cafe/Nature Renderer/Shader Includes/Nature Renderer.templatex"
			#pragma instancing_options procedural:SetupNatureRenderer forwardadd renderinglayer





			struct VertexInput

			{

				float4 vertex : POSITION;

				float3 ase_normal : NORMAL;

				float4 ase_tangent : TANGENT;

				float4 texcoord1 : TEXCOORD1;

				float4 texcoord : TEXCOORD0;

				

				UNITY_VERTEX_INPUT_INSTANCE_ID

			};



			struct VertexOutput

			{

				float4 clipPos : SV_POSITION;

				float4 lightmapUVOrVertexSH : TEXCOORD0;

				half4 fogFactorAndVertexLight : TEXCOORD1;

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)

				float4 shadowCoord : TEXCOORD2;

				#endif

				float4 tSpace0 : TEXCOORD3;

				float4 tSpace1 : TEXCOORD4;

				float4 tSpace2 : TEXCOORD5;

				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)

				float4 screenPos : TEXCOORD6;

				#endif

				float4 ase_texcoord7 : TEXCOORD7;

				float4 ase_texcoord8 : TEXCOORD8;

				UNITY_VERTEX_INPUT_INSTANCE_ID

				UNITY_VERTEX_OUTPUT_STEREO

			};



			CBUFFER_START(UnityPerMaterial)

			float4 _EmissiveColor;

			float4 _FoliageEmissiveColor;

			float4 _FoliageColorBottom;

			float4 _FoliageColorTop;

			float4 _ColorTint;

			float2 _WindSwayDirection;

			int _Culling;

			float _FoliageRoughness;

			float _EmissiveIntensity;

			float _FoliageEmissiveIntensity;

			float _NormalIntensity;

			float _FoliageNormalIntensity;

			float _TextureSize;

			float _GradientFallout;

			float _RoughnessIntensity;

			float _GradientOffset;

			float _WIndSwayFrequency;

			float _WIndSwayIntensity;

			float _WindOffsetIntensity;

			float _WindJitterSpeed;

			float _WindRustleSize;

			float _WindScrollSpeed;

			float _FoliageSize;

			float _AlphaClip;

			#ifdef _TRANSMISSION_ASE

				float _TransmissionShadow;

			#endif

			#ifdef _TRANSLUCENCY_ASE

				float _TransStrength;

				float _TransNormal;

				float _TransScattering;

				float _TransDirect;

				float _TransAmbient;

				float _TransShadow;

			#endif

			#ifdef TESSELLATION_ON

				float _TessPhongStrength;

				float _TessValue;

				float _TessMin;

				float _TessMax;

				float _TessEdgeLength;

				float _TessMaxDisp;

			#endif

			CBUFFER_END

			sampler2D _NoiseTexture;

			float varWindRustleScrollSpeed;

			float Float0;

			float varWindSwayIntensity;

			float2 varWindDirection;

			float varWindSwayFrequency;

			sampler2D _FoliageColorMap;

			sampler2D _ColorMap;

			sampler2D _FoliageNormalMap;

			sampler2D _NormalMap;

			sampler2D _EmissiveMap;

			sampler2D _ORMMap;





			

			VertexOutput VertexFunction( VertexInput v  )

			{

				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);



				float temp_output_18_0_g87 = _WindScrollSpeed;

				#ifdef _USEGLOBALWINDSETTINGS_ON

				float staticSwitch25_g87 = ( temp_output_18_0_g87 * varWindRustleScrollSpeed );

				#else

				float staticSwitch25_g87 = temp_output_18_0_g87;

				#endif

				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;

				float2 appendResult4_g87 = (float2(ase_worldPos.x , ase_worldPos.z));

				float2 temp_output_7_0_g87 = ( appendResult4_g87 * _WindRustleSize );

				float2 panner9_g87 = ( ( staticSwitch25_g87 * _TimeParameters.x ) * float2( 1,1 ) + temp_output_7_0_g87);

				float temp_output_19_0_g87 = _WindJitterSpeed;

				#ifdef _USEGLOBALWINDSETTINGS_ON

				float staticSwitch26_g87 = ( temp_output_19_0_g87 * Float0 );

				#else

				float staticSwitch26_g87 = temp_output_19_0_g87;

				#endif

				float2 panner13_g87 = ( ( _TimeParameters.x * staticSwitch26_g87 ) * float2( 1,1 ) + ( temp_output_7_0_g87 * float2( 2,2 ) ));

				float4 lerpResult30_g87 = lerp( float4( float3(0,0,0) , 0.0 ) , ( pow( tex2Dlod( _NoiseTexture, float4( panner9_g87, 0, 0.0) ) , 1.0 ) * tex2Dlod( _NoiseTexture, float4( panner13_g87, 0, 0.0) ) ) , _WindOffsetIntensity);

				float temp_output_27_0_g83 = _WIndSwayIntensity;

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float staticSwitch33_g83 = ( temp_output_27_0_g83 * varWindSwayIntensity );

				#else

				float staticSwitch33_g83 = temp_output_27_0_g83;

				#endif

				float temp_output_14_0_g83 = ( ( v.vertex.xyz.y * ( staticSwitch33_g83 / 100.0 ) ) + 1.0 );

				float temp_output_16_0_g83 = ( temp_output_14_0_g83 * temp_output_14_0_g83 );

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float2 staticSwitch41_g83 = varWindDirection;

				#else

				float2 staticSwitch41_g83 = _WindSwayDirection;

				#endif

				float2 clampResult10_g83 = clamp( staticSwitch41_g83 , float2( -1,-1 ) , float2( 1,1 ) );

				float4 transform1_g83 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));

				float4 appendResult3_g83 = (float4(transform1_g83.x , 0.0 , transform1_g83.z , 0.0));

				float dotResult4_g84 = dot( appendResult3_g83.xy , float2( 12.9898,78.233 ) );

				float lerpResult10_g84 = lerp( 1.0 , 1.01 , frac( ( sin( dotResult4_g84 ) * 43758.55 ) ));

				float mulTime9_g83 = _TimeParameters.x * lerpResult10_g84;

				float temp_output_29_0_g83 = _WIndSwayFrequency;

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float staticSwitch34_g83 = ( temp_output_29_0_g83 * varWindSwayFrequency );

				#else

				float staticSwitch34_g83 = temp_output_29_0_g83;

				#endif

				float2 break26_g83 = ( ( ( temp_output_16_0_g83 * temp_output_16_0_g83 ) - temp_output_16_0_g83 ) * ( ( ( staticSwitch41_g83 * float2( 4,4 ) ) + sin( ( ( clampResult10_g83 * mulTime9_g83 ) * staticSwitch34_g83 ) ) ) / float2( 4,4 ) ) );

				float4 appendResult25_g83 = (float4(break26_g83.x , 0.0 , break26_g83.y , 0.0));

				float4 temp_output_246_0 = appendResult25_g83;

				#ifdef _USEDFORTRUNK_ON

				float4 staticSwitch100 = temp_output_246_0;

				#else

				float4 staticSwitch100 = ( ( float4( v.ase_normal , 0.0 ) * lerpResult30_g87 ) + temp_output_246_0 );

				#endif

				float4 vWind116 = staticSwitch100;

				

				o.ase_texcoord7 = v.vertex;

				o.ase_texcoord8.xy = v.texcoord.xy;

				

				//setting value to unused interpolator channels and avoid initialization warnings

				o.ase_texcoord8.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					float3 defaultVertexValue = v.vertex.xyz;

				#else

					float3 defaultVertexValue = float3(0, 0, 0);

				#endif

				float3 vertexValue = vWind116.rgb;

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					v.vertex.xyz = vertexValue;

				#else

					v.vertex.xyz += vertexValue;

				#endif

				v.ase_normal = v.ase_normal;



				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				float3 positionVS = TransformWorldToView( positionWS );

				float4 positionCS = TransformWorldToHClip( positionWS );



				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );



				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);

				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);

				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);



				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );

				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );



				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)

					o.lightmapUVOrVertexSH.zw = v.texcoord;

					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;

				#endif



				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );

				#ifdef ASE_FOG

					half fogFactor = ComputeFogFactor( positionCS.z );

				#else

					half fogFactor = 0;

				#endif

				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

				

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)

				VertexPositionInputs vertexInput = (VertexPositionInputs)0;

				vertexInput.positionWS = positionWS;

				vertexInput.positionCS = positionCS;

				o.shadowCoord = GetShadowCoord( vertexInput );

				#endif

				

				o.clipPos = positionCS;

				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)

				o.screenPos = ComputeScreenPos(positionCS);

				#endif

				return o;

			}

			

			#if defined(TESSELLATION_ON)

			struct VertexControl

			{

				float4 vertex : INTERNALTESSPOS;

				float3 ase_normal : NORMAL;

				float4 ase_tangent : TANGENT;

				float4 texcoord : TEXCOORD0;

				float4 texcoord1 : TEXCOORD1;

				

				UNITY_VERTEX_INPUT_INSTANCE_ID

			};



			struct TessellationFactors

			{

				float edge[3] : SV_TessFactor;

				float inside : SV_InsideTessFactor;

			};



			VertexControl vert ( VertexInput v )

			{

				VertexControl o;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.vertex = v.vertex;

				o.ase_normal = v.ase_normal;

				o.ase_tangent = v.ase_tangent;

				o.texcoord = v.texcoord;

				o.texcoord1 = v.texcoord1;

				

				return o;

			}



			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)

			{

				TessellationFactors o;

				float4 tf = 1;

				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;

				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;

				#if defined(ASE_FIXED_TESSELLATION)

				tf = FixedTess( tessValue );

				#elif defined(ASE_DISTANCE_TESSELLATION)

				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );

				#elif defined(ASE_LENGTH_TESSELLATION)

				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );

				#elif defined(ASE_LENGTH_CULL_TESSELLATION)

				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );

				#endif

				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;

				return o;

			}



			[domain("tri")]

			[partitioning("fractional_odd")]

			[outputtopology("triangle_cw")]

			[patchconstantfunc("TessellationFunction")]

			[outputcontrolpoints(3)]

			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)

			{

			   return patch[id];

			}



			[domain("tri")]

			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)

			{

				VertexInput o = (VertexInput) 0;

				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;

				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;

				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;

				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;

				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;

				

				#if defined(ASE_PHONG_TESSELLATION)

				float3 pp[3];

				for (int i = 0; i < 3; ++i)

					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));

				float phongStrength = _TessPhongStrength;

				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;

				#endif

				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);

				return VertexFunction(o);

			}

			#else

			VertexOutput vert ( VertexInput v )

			{

				return VertexFunction( v );

			}

			#endif



			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)

				#define ASE_SV_DEPTH SV_DepthLessEqual  

			#else

				#define ASE_SV_DEPTH SV_Depth

			#endif



			half4 frag ( VertexOutput IN 

						#ifdef ASE_DEPTH_WRITE_ON

						,out float outputDepth : ASE_SV_DEPTH

						#endif

						 ) : SV_Target

			{

				UNITY_SETUP_INSTANCE_ID(IN);

				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);



				#ifdef LOD_FADE_CROSSFADE

					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );

				#endif



				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)

					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;

					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));

					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);

					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);

				#else

					float3 WorldNormal = normalize( IN.tSpace0.xyz );

					float3 WorldTangent = IN.tSpace1.xyz;

					float3 WorldBiTangent = IN.tSpace2.xyz;

				#endif

				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);

				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)

				float4 ScreenPos = IN.screenPos;

				#endif



				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)

					ShadowCoords = IN.shadowCoord;

				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)

					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );

				#endif

	

				WorldViewDirection = SafeNormalize( WorldViewDirection );



				float4 lerpResult115 = lerp( _FoliageColorBottom , _FoliageColorTop , saturate( ( ( IN.ase_texcoord7.xyz.y + _GradientOffset ) * ( _GradientFallout * 2 ) ) ));

				float temp_output_235_0 = ( _FoliageSize / 100.0 );

				float2 appendResult234 = (float2(temp_output_235_0 , temp_output_235_0));

				float2 texCoord236 = IN.ase_texcoord8.xy * appendResult234 + float2( 0,0 );

				float4 tex2DNode124 = tex2D( _FoliageColorMap, texCoord236 );

				float4 vFoliageColor172 = ( lerpResult115 * tex2DNode124 );

				float2 texCoord18_g88 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );

				float2 temp_output_19_0_g88 = ( texCoord18_g88 / ( _TextureSize / 100.0 ) );

				#ifdef _USEDFORTRUNK_ON

				float4 staticSwitch82 = ( _ColorTint * tex2D( _ColorMap, temp_output_19_0_g88 ) );

				#else

				float4 staticSwitch82 = vFoliageColor172;

				#endif

				

				float3 unpack176 = UnpackNormalScale( tex2D( _FoliageNormalMap, texCoord236 ), _FoliageNormalIntensity );

				unpack176.z = lerp( 1, unpack176.z, saturate(_FoliageNormalIntensity) );

				float3 vFoliageNormal226 = unpack176;

				float3 unpack26_g88 = UnpackNormalScale( tex2D( _NormalMap, temp_output_19_0_g88 ), _NormalIntensity );

				unpack26_g88.z = lerp( 1, unpack26_g88.z, saturate(_NormalIntensity) );

				#ifdef _USEDFORTRUNK_ON

				float3 staticSwitch97 = unpack26_g88;

				#else

				float3 staticSwitch97 = vFoliageNormal226;

				#endif

				

				float4 vFoliageEmissive225 = ( _FoliageEmissiveColor * _FoliageEmissiveIntensity );

				#ifdef _USEDFORTRUNK_ON

				float4 staticSwitch228 = ( ( tex2D( _EmissiveMap, temp_output_19_0_g88 ) * _EmissiveColor ) * _EmissiveIntensity );

				#else

				float4 staticSwitch228 = vFoliageEmissive225;

				#endif

				

				float4 tex2DNode9_g89 = tex2D( _ORMMap, temp_output_19_0_g88 );

				#ifdef _USEDFORTRUNK_ON

				float staticSwitch98 = ( 1.0 - ( tex2DNode9_g89.g * _RoughnessIntensity ) );

				#else

				float staticSwitch98 = ( 1.0 - _FoliageRoughness );

				#endif

				

				float vFoliageOpacity173 = tex2DNode124.a;

				#ifdef _USEDFORTRUNK_ON

				float staticSwitch182 = 1.0;

				#else

				float staticSwitch182 = vFoliageOpacity173;

				#endif

				

				float3 Albedo = staticSwitch82.rgb;

				float3 Normal = staticSwitch97;

				float3 Emission = staticSwitch228.rgb;

				float3 Specular = 0.5;

				float Metallic = 0;

				float Smoothness = staticSwitch98;

				float Occlusion = 1;

				float Alpha = staticSwitch182;

				float AlphaClipThreshold = _AlphaClip;

				float AlphaClipThresholdShadow = 0.5;

				float3 BakedGI = 0;

				float3 RefractionColor = 1;

				float RefractionIndex = 1;

				float3 Transmission = 1;

				float3 Translucency = 1;

				#ifdef ASE_DEPTH_WRITE_ON

				float DepthValue = 0;

				#endif



				#ifdef _ALPHATEST_ON

					clip(Alpha - AlphaClipThreshold);

				#endif



				InputData inputData;

				inputData.positionWS = WorldPosition;

				inputData.viewDirectionWS = WorldViewDirection;

				inputData.shadowCoord = ShadowCoords;



				#ifdef _NORMALMAP

					#if _NORMAL_DROPOFF_TS

					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));

					#elif _NORMAL_DROPOFF_OS

					inputData.normalWS = TransformObjectToWorldNormal(Normal);

					#elif _NORMAL_DROPOFF_WS

					inputData.normalWS = Normal;

					#endif

					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);

				#else

					inputData.normalWS = WorldNormal;

				#endif



				#ifdef ASE_FOG

					inputData.fogCoord = IN.fogFactorAndVertexLight.x;

				#endif



				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)

					float3 SH = SampleSH(inputData.normalWS.xyz);

				#else

					float3 SH = IN.lightmapUVOrVertexSH.xyz;

				#endif



				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );

				#ifdef _ASE_BAKEDGI

					inputData.bakedGI = BakedGI;

				#endif

				

				inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.clipPos);

				inputData.shadowMask = SAMPLE_SHADOWMASK(IN.lightmapUVOrVertexSH.xy);



				half4 color = UniversalFragmentPBR(

					inputData, 

					Albedo, 

					Metallic, 

					Specular, 

					Smoothness, 

					Occlusion, 

					Emission, 

					Alpha);



				#ifdef _TRANSMISSION_ASE

				{

					float shadow = _TransmissionShadow;



					Light mainLight = GetMainLight( inputData.shadowCoord );

					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;

					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );

					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;

					color.rgb += Albedo * mainTransmission;



					#ifdef _ADDITIONAL_LIGHTS

						int transPixelLightCount = GetAdditionalLightsCount();

						for (int i = 0; i < transPixelLightCount; ++i)

						{

							Light light = GetAdditionalLight(i, inputData.positionWS);

							float3 atten = light.color * light.distanceAttenuation;

							atten = lerp( atten, atten * light.shadowAttenuation, shadow );



							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;

							color.rgb += Albedo * transmission;

						}

					#endif

				}

				#endif



				#ifdef _TRANSLUCENCY_ASE

				{

					float shadow = _TransShadow;

					float normal = _TransNormal;

					float scattering = _TransScattering;

					float direct = _TransDirect;

					float ambient = _TransAmbient;

					float strength = _TransStrength;



					Light mainLight = GetMainLight( inputData.shadowCoord );

					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;

					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );



					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;

					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );

					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;

					color.rgb += Albedo * mainTranslucency * strength;



					#ifdef _ADDITIONAL_LIGHTS

						int transPixelLightCount = GetAdditionalLightsCount();

						for (int i = 0; i < transPixelLightCount; ++i)

						{

							Light light = GetAdditionalLight(i, inputData.positionWS);

							float3 atten = light.color * light.distanceAttenuation;

							atten = lerp( atten, atten * light.shadowAttenuation, shadow );



							half3 lightDir = light.direction + inputData.normalWS * normal;

							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );

							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;

							color.rgb += Albedo * translucency * strength;

						}

					#endif

				}

				#endif



				#ifdef _REFRACTION_ASE

					float4 projScreenPos = ScreenPos / ScreenPos.w;

					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal,0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );

					projScreenPos.xy += refractionOffset.xy;

					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;

					color.rgb = lerp( refraction, color.rgb, color.a );

					color.a = 1;

				#endif



				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY

					color.rgb *= color.a;

				#endif



				#ifdef ASE_FOG

					#ifdef TERRAIN_SPLAT_ADDPASS

						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );

					#else

						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);

					#endif

				#endif



				#ifdef ASE_DEPTH_WRITE_ON

					outputDepth = DepthValue;

				#endif



				return color;

			}



			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster"  "NatureRendererInstancing"="True" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM

			

			#define _NORMAL_DROPOFF_TS 1

			#pragma multi_compile_instancing

			#pragma multi_compile _ LOD_FADE_CROSSFADE

			#pragma multi_compile_fog

			#define ASE_FOG 1

			#define _TRANSLUCENCY_ASE 1

			#define _EMISSION

			#define _ALPHATEST_ON 1

			#define _NORMALMAP 1

			#define ASE_SRP_VERSION 999999



			

			#pragma vertex vert

			#pragma fragment frag

#if ASE_SRP_VERSION >= 110000

			#pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW

#endif

			#define SHADERPASS_SHADOWCASTER



			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"



			#define ASE_NEEDS_VERT_NORMAL

			#define ASE_NEEDS_VERT_POSITION

			#pragma shader_feature_local _USEDFORTRUNK_ON

			#pragma shader_feature_local _USEGLOBALWINDSETTINGS_ON

			#pragma shader_feature_local _USESGLOBALWINDSETTINGS_ON
			#include "Assets/Visual Design Cafe/Nature Renderer/Shader Includes/Nature Renderer.templatex"
			#pragma instancing_options procedural:SetupNatureRenderer forwardadd renderinglayer





			struct VertexInput

			{

				float4 vertex : POSITION;

				float3 ase_normal : NORMAL;

				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID

			};



			struct VertexOutput

			{

				float4 clipPos : SV_POSITION;

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)

				float3 worldPos : TEXCOORD0;

				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)

				float4 shadowCoord : TEXCOORD1;

				#endif

				float4 ase_texcoord2 : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID

				UNITY_VERTEX_OUTPUT_STEREO

			};



			CBUFFER_START(UnityPerMaterial)

			float4 _EmissiveColor;

			float4 _FoliageEmissiveColor;

			float4 _FoliageColorBottom;

			float4 _FoliageColorTop;

			float4 _ColorTint;

			float2 _WindSwayDirection;

			int _Culling;

			float _FoliageRoughness;

			float _EmissiveIntensity;

			float _FoliageEmissiveIntensity;

			float _NormalIntensity;

			float _FoliageNormalIntensity;

			float _TextureSize;

			float _GradientFallout;

			float _RoughnessIntensity;

			float _GradientOffset;

			float _WIndSwayFrequency;

			float _WIndSwayIntensity;

			float _WindOffsetIntensity;

			float _WindJitterSpeed;

			float _WindRustleSize;

			float _WindScrollSpeed;

			float _FoliageSize;

			float _AlphaClip;

			#ifdef _TRANSMISSION_ASE

				float _TransmissionShadow;

			#endif

			#ifdef _TRANSLUCENCY_ASE

				float _TransStrength;

				float _TransNormal;

				float _TransScattering;

				float _TransDirect;

				float _TransAmbient;

				float _TransShadow;

			#endif

			#ifdef TESSELLATION_ON

				float _TessPhongStrength;

				float _TessValue;

				float _TessMin;

				float _TessMax;

				float _TessEdgeLength;

				float _TessMaxDisp;

			#endif

			CBUFFER_END

			sampler2D _NoiseTexture;

			float varWindRustleScrollSpeed;

			float Float0;

			float varWindSwayIntensity;

			float2 varWindDirection;

			float varWindSwayFrequency;

			sampler2D _FoliageColorMap;





			

			float3 _LightDirection;

#if ASE_SRP_VERSION >= 110000 

			float3 _LightPosition;

#endif

			VertexOutput VertexFunction( VertexInput v )

			{

				VertexOutput o;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );



				float temp_output_18_0_g87 = _WindScrollSpeed;

				#ifdef _USEGLOBALWINDSETTINGS_ON

				float staticSwitch25_g87 = ( temp_output_18_0_g87 * varWindRustleScrollSpeed );

				#else

				float staticSwitch25_g87 = temp_output_18_0_g87;

				#endif

				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;

				float2 appendResult4_g87 = (float2(ase_worldPos.x , ase_worldPos.z));

				float2 temp_output_7_0_g87 = ( appendResult4_g87 * _WindRustleSize );

				float2 panner9_g87 = ( ( staticSwitch25_g87 * _TimeParameters.x ) * float2( 1,1 ) + temp_output_7_0_g87);

				float temp_output_19_0_g87 = _WindJitterSpeed;

				#ifdef _USEGLOBALWINDSETTINGS_ON

				float staticSwitch26_g87 = ( temp_output_19_0_g87 * Float0 );

				#else

				float staticSwitch26_g87 = temp_output_19_0_g87;

				#endif

				float2 panner13_g87 = ( ( _TimeParameters.x * staticSwitch26_g87 ) * float2( 1,1 ) + ( temp_output_7_0_g87 * float2( 2,2 ) ));

				float4 lerpResult30_g87 = lerp( float4( float3(0,0,0) , 0.0 ) , ( pow( tex2Dlod( _NoiseTexture, float4( panner9_g87, 0, 0.0) ) , 1.0 ) * tex2Dlod( _NoiseTexture, float4( panner13_g87, 0, 0.0) ) ) , _WindOffsetIntensity);

				float temp_output_27_0_g83 = _WIndSwayIntensity;

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float staticSwitch33_g83 = ( temp_output_27_0_g83 * varWindSwayIntensity );

				#else

				float staticSwitch33_g83 = temp_output_27_0_g83;

				#endif

				float temp_output_14_0_g83 = ( ( v.vertex.xyz.y * ( staticSwitch33_g83 / 100.0 ) ) + 1.0 );

				float temp_output_16_0_g83 = ( temp_output_14_0_g83 * temp_output_14_0_g83 );

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float2 staticSwitch41_g83 = varWindDirection;

				#else

				float2 staticSwitch41_g83 = _WindSwayDirection;

				#endif

				float2 clampResult10_g83 = clamp( staticSwitch41_g83 , float2( -1,-1 ) , float2( 1,1 ) );

				float4 transform1_g83 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));

				float4 appendResult3_g83 = (float4(transform1_g83.x , 0.0 , transform1_g83.z , 0.0));

				float dotResult4_g84 = dot( appendResult3_g83.xy , float2( 12.9898,78.233 ) );

				float lerpResult10_g84 = lerp( 1.0 , 1.01 , frac( ( sin( dotResult4_g84 ) * 43758.55 ) ));

				float mulTime9_g83 = _TimeParameters.x * lerpResult10_g84;

				float temp_output_29_0_g83 = _WIndSwayFrequency;

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float staticSwitch34_g83 = ( temp_output_29_0_g83 * varWindSwayFrequency );

				#else

				float staticSwitch34_g83 = temp_output_29_0_g83;

				#endif

				float2 break26_g83 = ( ( ( temp_output_16_0_g83 * temp_output_16_0_g83 ) - temp_output_16_0_g83 ) * ( ( ( staticSwitch41_g83 * float2( 4,4 ) ) + sin( ( ( clampResult10_g83 * mulTime9_g83 ) * staticSwitch34_g83 ) ) ) / float2( 4,4 ) ) );

				float4 appendResult25_g83 = (float4(break26_g83.x , 0.0 , break26_g83.y , 0.0));

				float4 temp_output_246_0 = appendResult25_g83;

				#ifdef _USEDFORTRUNK_ON

				float4 staticSwitch100 = temp_output_246_0;

				#else

				float4 staticSwitch100 = ( ( float4( v.ase_normal , 0.0 ) * lerpResult30_g87 ) + temp_output_246_0 );

				#endif

				float4 vWind116 = staticSwitch100;

				

				o.ase_texcoord2.xy = v.ase_texcoord.xy;

				

				//setting value to unused interpolator channels and avoid initialization warnings

				o.ase_texcoord2.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					float3 defaultVertexValue = v.vertex.xyz;

				#else

					float3 defaultVertexValue = float3(0, 0, 0);

				#endif

				float3 vertexValue = vWind116.rgb;

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					v.vertex.xyz = vertexValue;

				#else

					v.vertex.xyz += vertexValue;

				#endif



				v.ase_normal = v.ase_normal;



				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)

				o.worldPos = positionWS;

				#endif

				float3 normalWS = TransformObjectToWorldDir(v.ase_normal);



		#if ASE_SRP_VERSION >= 110000 

			#if _CASTING_PUNCTUAL_LIGHT_SHADOW

				float3 lightDirectionWS = normalize(_LightPosition - positionWS);

			#else

				float3 lightDirectionWS = _LightDirection;

			#endif

				float4 clipPos = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

			#if UNITY_REVERSED_Z

				clipPos.z = min(clipPos.z, UNITY_NEAR_CLIP_VALUE);

			#else

				clipPos.z = max(clipPos.z, UNITY_NEAR_CLIP_VALUE);

			#endif

		#else

				float4 clipPos = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

			#if UNITY_REVERSED_Z

				clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);

			#else

				clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);

			#endif

		#endif



				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)

					VertexPositionInputs vertexInput = (VertexPositionInputs)0;

					vertexInput.positionWS = positionWS;

					vertexInput.positionCS = clipPos;

					o.shadowCoord = GetShadowCoord( vertexInput );

				#endif

				o.clipPos = clipPos;

				return o;

			}



			#if defined(TESSELLATION_ON)

			struct VertexControl

			{

				float4 vertex : INTERNALTESSPOS;

				float3 ase_normal : NORMAL;

				float4 ase_texcoord : TEXCOORD0;



				UNITY_VERTEX_INPUT_INSTANCE_ID

			};



			struct TessellationFactors

			{

				float edge[3] : SV_TessFactor;

				float inside : SV_InsideTessFactor;

			};



			VertexControl vert ( VertexInput v )

			{

				VertexControl o;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.vertex = v.vertex;

				o.ase_normal = v.ase_normal;

				o.ase_texcoord = v.ase_texcoord;

				return o;

			}



			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)

			{

				TessellationFactors o;

				float4 tf = 1;

				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;

				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;

				#if defined(ASE_FIXED_TESSELLATION)

				tf = FixedTess( tessValue );

				#elif defined(ASE_DISTANCE_TESSELLATION)

				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );

				#elif defined(ASE_LENGTH_TESSELLATION)

				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );

				#elif defined(ASE_LENGTH_CULL_TESSELLATION)

				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );

				#endif

				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;

				return o;

			}



			[domain("tri")]

			[partitioning("fractional_odd")]

			[outputtopology("triangle_cw")]

			[patchconstantfunc("TessellationFunction")]

			[outputcontrolpoints(3)]

			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)

			{

			   return patch[id];

			}



			[domain("tri")]

			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)

			{

				VertexInput o = (VertexInput) 0;

				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;

				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;

				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;

				#if defined(ASE_PHONG_TESSELLATION)

				float3 pp[3];

				for (int i = 0; i < 3; ++i)

					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));

				float phongStrength = _TessPhongStrength;

				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;

				#endif

				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);

				return VertexFunction(o);

			}

			#else

			VertexOutput vert ( VertexInput v )

			{

				return VertexFunction( v );

			}

			#endif



			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)

				#define ASE_SV_DEPTH SV_DepthLessEqual  

			#else

				#define ASE_SV_DEPTH SV_Depth

			#endif



			half4 frag(	VertexOutput IN 

						#ifdef ASE_DEPTH_WRITE_ON

						,out float outputDepth : ASE_SV_DEPTH

						#endif

						 ) : SV_TARGET

			{

				UNITY_SETUP_INSTANCE_ID( IN );

				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)

				float3 WorldPosition = IN.worldPos;

				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );



				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)

					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)

						ShadowCoords = IN.shadowCoord;

					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)

						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );

					#endif

				#endif



				float temp_output_235_0 = ( _FoliageSize / 100.0 );

				float2 appendResult234 = (float2(temp_output_235_0 , temp_output_235_0));

				float2 texCoord236 = IN.ase_texcoord2.xy * appendResult234 + float2( 0,0 );

				float4 tex2DNode124 = tex2D( _FoliageColorMap, texCoord236 );

				float vFoliageOpacity173 = tex2DNode124.a;

				#ifdef _USEDFORTRUNK_ON

				float staticSwitch182 = 1.0;

				#else

				float staticSwitch182 = vFoliageOpacity173;

				#endif

				

				float Alpha = staticSwitch182;

				float AlphaClipThreshold = _AlphaClip;

				float AlphaClipThresholdShadow = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON

				float DepthValue = 0;

				#endif



				#ifdef _ALPHATEST_ON

					#ifdef _ALPHATEST_SHADOW_ON

						clip(Alpha - AlphaClipThresholdShadow);

					#else

						clip(Alpha - AlphaClipThreshold);

					#endif

				#endif



				#ifdef LOD_FADE_CROSSFADE

					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );

				#endif

				#ifdef ASE_DEPTH_WRITE_ON

					outputDepth = DepthValue;

				#endif

				return 0;

			}



			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly"  "NatureRendererInstancing"="True" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

			

			#define _NORMAL_DROPOFF_TS 1

			#pragma multi_compile_instancing

			#pragma multi_compile _ LOD_FADE_CROSSFADE

			#pragma multi_compile_fog

			#define ASE_FOG 1

			#define _TRANSLUCENCY_ASE 1

			#define _EMISSION

			#define _ALPHATEST_ON 1

			#define _NORMALMAP 1

			#define ASE_SRP_VERSION 999999



			

			#pragma vertex vert

			#pragma fragment frag



			#define SHADERPASS_DEPTHONLY



			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"



			#define ASE_NEEDS_VERT_NORMAL

			#define ASE_NEEDS_VERT_POSITION

			#pragma shader_feature_local _USEDFORTRUNK_ON

			#pragma shader_feature_local _USEGLOBALWINDSETTINGS_ON

			#pragma shader_feature_local _USESGLOBALWINDSETTINGS_ON
			#include "Assets/Visual Design Cafe/Nature Renderer/Shader Includes/Nature Renderer.templatex"
			#pragma instancing_options procedural:SetupNatureRenderer forwardadd renderinglayer





			struct VertexInput

			{

				float4 vertex : POSITION;

				float3 ase_normal : NORMAL;

				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID

			};



			struct VertexOutput

			{

				float4 clipPos : SV_POSITION;

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)

				float3 worldPos : TEXCOORD0;

				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)

				float4 shadowCoord : TEXCOORD1;

				#endif

				float4 ase_texcoord2 : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID

				UNITY_VERTEX_OUTPUT_STEREO

			};



			CBUFFER_START(UnityPerMaterial)

			float4 _EmissiveColor;

			float4 _FoliageEmissiveColor;

			float4 _FoliageColorBottom;

			float4 _FoliageColorTop;

			float4 _ColorTint;

			float2 _WindSwayDirection;

			int _Culling;

			float _FoliageRoughness;

			float _EmissiveIntensity;

			float _FoliageEmissiveIntensity;

			float _NormalIntensity;

			float _FoliageNormalIntensity;

			float _TextureSize;

			float _GradientFallout;

			float _RoughnessIntensity;

			float _GradientOffset;

			float _WIndSwayFrequency;

			float _WIndSwayIntensity;

			float _WindOffsetIntensity;

			float _WindJitterSpeed;

			float _WindRustleSize;

			float _WindScrollSpeed;

			float _FoliageSize;

			float _AlphaClip;

			#ifdef _TRANSMISSION_ASE

				float _TransmissionShadow;

			#endif

			#ifdef _TRANSLUCENCY_ASE

				float _TransStrength;

				float _TransNormal;

				float _TransScattering;

				float _TransDirect;

				float _TransAmbient;

				float _TransShadow;

			#endif

			#ifdef TESSELLATION_ON

				float _TessPhongStrength;

				float _TessValue;

				float _TessMin;

				float _TessMax;

				float _TessEdgeLength;

				float _TessMaxDisp;

			#endif

			CBUFFER_END

			sampler2D _NoiseTexture;

			float varWindRustleScrollSpeed;

			float Float0;

			float varWindSwayIntensity;

			float2 varWindDirection;

			float varWindSwayFrequency;

			sampler2D _FoliageColorMap;





			

			VertexOutput VertexFunction( VertexInput v  )

			{

				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);



				float temp_output_18_0_g87 = _WindScrollSpeed;

				#ifdef _USEGLOBALWINDSETTINGS_ON

				float staticSwitch25_g87 = ( temp_output_18_0_g87 * varWindRustleScrollSpeed );

				#else

				float staticSwitch25_g87 = temp_output_18_0_g87;

				#endif

				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;

				float2 appendResult4_g87 = (float2(ase_worldPos.x , ase_worldPos.z));

				float2 temp_output_7_0_g87 = ( appendResult4_g87 * _WindRustleSize );

				float2 panner9_g87 = ( ( staticSwitch25_g87 * _TimeParameters.x ) * float2( 1,1 ) + temp_output_7_0_g87);

				float temp_output_19_0_g87 = _WindJitterSpeed;

				#ifdef _USEGLOBALWINDSETTINGS_ON

				float staticSwitch26_g87 = ( temp_output_19_0_g87 * Float0 );

				#else

				float staticSwitch26_g87 = temp_output_19_0_g87;

				#endif

				float2 panner13_g87 = ( ( _TimeParameters.x * staticSwitch26_g87 ) * float2( 1,1 ) + ( temp_output_7_0_g87 * float2( 2,2 ) ));

				float4 lerpResult30_g87 = lerp( float4( float3(0,0,0) , 0.0 ) , ( pow( tex2Dlod( _NoiseTexture, float4( panner9_g87, 0, 0.0) ) , 1.0 ) * tex2Dlod( _NoiseTexture, float4( panner13_g87, 0, 0.0) ) ) , _WindOffsetIntensity);

				float temp_output_27_0_g83 = _WIndSwayIntensity;

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float staticSwitch33_g83 = ( temp_output_27_0_g83 * varWindSwayIntensity );

				#else

				float staticSwitch33_g83 = temp_output_27_0_g83;

				#endif

				float temp_output_14_0_g83 = ( ( v.vertex.xyz.y * ( staticSwitch33_g83 / 100.0 ) ) + 1.0 );

				float temp_output_16_0_g83 = ( temp_output_14_0_g83 * temp_output_14_0_g83 );

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float2 staticSwitch41_g83 = varWindDirection;

				#else

				float2 staticSwitch41_g83 = _WindSwayDirection;

				#endif

				float2 clampResult10_g83 = clamp( staticSwitch41_g83 , float2( -1,-1 ) , float2( 1,1 ) );

				float4 transform1_g83 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));

				float4 appendResult3_g83 = (float4(transform1_g83.x , 0.0 , transform1_g83.z , 0.0));

				float dotResult4_g84 = dot( appendResult3_g83.xy , float2( 12.9898,78.233 ) );

				float lerpResult10_g84 = lerp( 1.0 , 1.01 , frac( ( sin( dotResult4_g84 ) * 43758.55 ) ));

				float mulTime9_g83 = _TimeParameters.x * lerpResult10_g84;

				float temp_output_29_0_g83 = _WIndSwayFrequency;

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float staticSwitch34_g83 = ( temp_output_29_0_g83 * varWindSwayFrequency );

				#else

				float staticSwitch34_g83 = temp_output_29_0_g83;

				#endif

				float2 break26_g83 = ( ( ( temp_output_16_0_g83 * temp_output_16_0_g83 ) - temp_output_16_0_g83 ) * ( ( ( staticSwitch41_g83 * float2( 4,4 ) ) + sin( ( ( clampResult10_g83 * mulTime9_g83 ) * staticSwitch34_g83 ) ) ) / float2( 4,4 ) ) );

				float4 appendResult25_g83 = (float4(break26_g83.x , 0.0 , break26_g83.y , 0.0));

				float4 temp_output_246_0 = appendResult25_g83;

				#ifdef _USEDFORTRUNK_ON

				float4 staticSwitch100 = temp_output_246_0;

				#else

				float4 staticSwitch100 = ( ( float4( v.ase_normal , 0.0 ) * lerpResult30_g87 ) + temp_output_246_0 );

				#endif

				float4 vWind116 = staticSwitch100;

				

				o.ase_texcoord2.xy = v.ase_texcoord.xy;

				

				//setting value to unused interpolator channels and avoid initialization warnings

				o.ase_texcoord2.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					float3 defaultVertexValue = v.vertex.xyz;

				#else

					float3 defaultVertexValue = float3(0, 0, 0);

				#endif

				float3 vertexValue = vWind116.rgb;

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					v.vertex.xyz = vertexValue;

				#else

					v.vertex.xyz += vertexValue;

				#endif



				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				float4 positionCS = TransformWorldToHClip( positionWS );



				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)

				o.worldPos = positionWS;

				#endif



				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)

					VertexPositionInputs vertexInput = (VertexPositionInputs)0;

					vertexInput.positionWS = positionWS;

					vertexInput.positionCS = positionCS;

					o.shadowCoord = GetShadowCoord( vertexInput );

				#endif

				o.clipPos = positionCS;

				return o;

			}



			#if defined(TESSELLATION_ON)

			struct VertexControl

			{

				float4 vertex : INTERNALTESSPOS;

				float3 ase_normal : NORMAL;

				float4 ase_texcoord : TEXCOORD0;



				UNITY_VERTEX_INPUT_INSTANCE_ID

			};



			struct TessellationFactors

			{

				float edge[3] : SV_TessFactor;

				float inside : SV_InsideTessFactor;

			};



			VertexControl vert ( VertexInput v )

			{

				VertexControl o;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.vertex = v.vertex;

				o.ase_normal = v.ase_normal;

				o.ase_texcoord = v.ase_texcoord;

				return o;

			}



			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)

			{

				TessellationFactors o;

				float4 tf = 1;

				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;

				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;

				#if defined(ASE_FIXED_TESSELLATION)

				tf = FixedTess( tessValue );

				#elif defined(ASE_DISTANCE_TESSELLATION)

				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );

				#elif defined(ASE_LENGTH_TESSELLATION)

				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );

				#elif defined(ASE_LENGTH_CULL_TESSELLATION)

				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );

				#endif

				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;

				return o;

			}



			[domain("tri")]

			[partitioning("fractional_odd")]

			[outputtopology("triangle_cw")]

			[patchconstantfunc("TessellationFunction")]

			[outputcontrolpoints(3)]

			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)

			{

			   return patch[id];

			}



			[domain("tri")]

			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)

			{

				VertexInput o = (VertexInput) 0;

				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;

				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;

				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;

				#if defined(ASE_PHONG_TESSELLATION)

				float3 pp[3];

				for (int i = 0; i < 3; ++i)

					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));

				float phongStrength = _TessPhongStrength;

				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;

				#endif

				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);

				return VertexFunction(o);

			}

			#else

			VertexOutput vert ( VertexInput v )

			{

				return VertexFunction( v );

			}

			#endif



			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)

				#define ASE_SV_DEPTH SV_DepthLessEqual  

			#else

				#define ASE_SV_DEPTH SV_Depth

			#endif

			half4 frag(	VertexOutput IN 

						#ifdef ASE_DEPTH_WRITE_ON

						,out float outputDepth : ASE_SV_DEPTH

						#endif

						 ) : SV_TARGET

			{

				UNITY_SETUP_INSTANCE_ID(IN);

				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );



				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)

				float3 WorldPosition = IN.worldPos;

				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );



				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)

					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)

						ShadowCoords = IN.shadowCoord;

					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)

						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );

					#endif

				#endif



				float temp_output_235_0 = ( _FoliageSize / 100.0 );

				float2 appendResult234 = (float2(temp_output_235_0 , temp_output_235_0));

				float2 texCoord236 = IN.ase_texcoord2.xy * appendResult234 + float2( 0,0 );

				float4 tex2DNode124 = tex2D( _FoliageColorMap, texCoord236 );

				float vFoliageOpacity173 = tex2DNode124.a;

				#ifdef _USEDFORTRUNK_ON

				float staticSwitch182 = 1.0;

				#else

				float staticSwitch182 = vFoliageOpacity173;

				#endif

				

				float Alpha = staticSwitch182;

				float AlphaClipThreshold = _AlphaClip;

				#ifdef ASE_DEPTH_WRITE_ON

				float DepthValue = 0;

				#endif



				#ifdef _ALPHATEST_ON

					clip(Alpha - AlphaClipThreshold);

				#endif



				#ifdef LOD_FADE_CROSSFADE

					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );

				#endif

				#ifdef ASE_DEPTH_WRITE_ON

				outputDepth = DepthValue;

				#endif



				return 0;

			}

			ENDHLSL
		}
		
		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta"  "NatureRendererInstancing"="True" }

			Cull Off

			HLSLPROGRAM

			

			#define _NORMAL_DROPOFF_TS 1

			#pragma multi_compile_instancing

			#pragma multi_compile _ LOD_FADE_CROSSFADE

			#pragma multi_compile_fog

			#define ASE_FOG 1

			#define _TRANSLUCENCY_ASE 1

			#define _EMISSION

			#define _ALPHATEST_ON 1

			#define _NORMALMAP 1

			#define ASE_SRP_VERSION 999999



			

			#pragma vertex vert

			#pragma fragment frag



			#define SHADERPASS_META



			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"



			#define ASE_NEEDS_VERT_NORMAL

			#define ASE_NEEDS_VERT_POSITION

			#pragma shader_feature_local _USEDFORTRUNK_ON

			#pragma shader_feature_local _USEGLOBALWINDSETTINGS_ON

			#pragma shader_feature_local _USESGLOBALWINDSETTINGS_ON





			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#include "Assets/Visual Design Cafe/Nature Renderer/Shader Includes/Nature Renderer.templatex"
			#pragma instancing_options procedural:SetupNatureRenderer forwardadd renderinglayer



			struct VertexInput

			{

				float4 vertex : POSITION;

				float3 ase_normal : NORMAL;

				float4 texcoord1 : TEXCOORD1;

				float4 texcoord2 : TEXCOORD2;

				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID

			};



			struct VertexOutput

			{

				float4 clipPos : SV_POSITION;

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)

				float3 worldPos : TEXCOORD0;

				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)

				float4 shadowCoord : TEXCOORD1;

				#endif

				float4 ase_texcoord2 : TEXCOORD2;

				float4 ase_texcoord3 : TEXCOORD3;

				UNITY_VERTEX_INPUT_INSTANCE_ID

				UNITY_VERTEX_OUTPUT_STEREO

			};



			CBUFFER_START(UnityPerMaterial)

			float4 _EmissiveColor;

			float4 _FoliageEmissiveColor;

			float4 _FoliageColorBottom;

			float4 _FoliageColorTop;

			float4 _ColorTint;

			float2 _WindSwayDirection;

			int _Culling;

			float _FoliageRoughness;

			float _EmissiveIntensity;

			float _FoliageEmissiveIntensity;

			float _NormalIntensity;

			float _FoliageNormalIntensity;

			float _TextureSize;

			float _GradientFallout;

			float _RoughnessIntensity;

			float _GradientOffset;

			float _WIndSwayFrequency;

			float _WIndSwayIntensity;

			float _WindOffsetIntensity;

			float _WindJitterSpeed;

			float _WindRustleSize;

			float _WindScrollSpeed;

			float _FoliageSize;

			float _AlphaClip;

			#ifdef _TRANSMISSION_ASE

				float _TransmissionShadow;

			#endif

			#ifdef _TRANSLUCENCY_ASE

				float _TransStrength;

				float _TransNormal;

				float _TransScattering;

				float _TransDirect;

				float _TransAmbient;

				float _TransShadow;

			#endif

			#ifdef TESSELLATION_ON

				float _TessPhongStrength;

				float _TessValue;

				float _TessMin;

				float _TessMax;

				float _TessEdgeLength;

				float _TessMaxDisp;

			#endif

			CBUFFER_END

			sampler2D _NoiseTexture;

			float varWindRustleScrollSpeed;

			float Float0;

			float varWindSwayIntensity;

			float2 varWindDirection;

			float varWindSwayFrequency;

			sampler2D _FoliageColorMap;

			sampler2D _ColorMap;

			sampler2D _EmissiveMap;





			

			VertexOutput VertexFunction( VertexInput v  )

			{

				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);



				float temp_output_18_0_g87 = _WindScrollSpeed;

				#ifdef _USEGLOBALWINDSETTINGS_ON

				float staticSwitch25_g87 = ( temp_output_18_0_g87 * varWindRustleScrollSpeed );

				#else

				float staticSwitch25_g87 = temp_output_18_0_g87;

				#endif

				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;

				float2 appendResult4_g87 = (float2(ase_worldPos.x , ase_worldPos.z));

				float2 temp_output_7_0_g87 = ( appendResult4_g87 * _WindRustleSize );

				float2 panner9_g87 = ( ( staticSwitch25_g87 * _TimeParameters.x ) * float2( 1,1 ) + temp_output_7_0_g87);

				float temp_output_19_0_g87 = _WindJitterSpeed;

				#ifdef _USEGLOBALWINDSETTINGS_ON

				float staticSwitch26_g87 = ( temp_output_19_0_g87 * Float0 );

				#else

				float staticSwitch26_g87 = temp_output_19_0_g87;

				#endif

				float2 panner13_g87 = ( ( _TimeParameters.x * staticSwitch26_g87 ) * float2( 1,1 ) + ( temp_output_7_0_g87 * float2( 2,2 ) ));

				float4 lerpResult30_g87 = lerp( float4( float3(0,0,0) , 0.0 ) , ( pow( tex2Dlod( _NoiseTexture, float4( panner9_g87, 0, 0.0) ) , 1.0 ) * tex2Dlod( _NoiseTexture, float4( panner13_g87, 0, 0.0) ) ) , _WindOffsetIntensity);

				float temp_output_27_0_g83 = _WIndSwayIntensity;

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float staticSwitch33_g83 = ( temp_output_27_0_g83 * varWindSwayIntensity );

				#else

				float staticSwitch33_g83 = temp_output_27_0_g83;

				#endif

				float temp_output_14_0_g83 = ( ( v.vertex.xyz.y * ( staticSwitch33_g83 / 100.0 ) ) + 1.0 );

				float temp_output_16_0_g83 = ( temp_output_14_0_g83 * temp_output_14_0_g83 );

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float2 staticSwitch41_g83 = varWindDirection;

				#else

				float2 staticSwitch41_g83 = _WindSwayDirection;

				#endif

				float2 clampResult10_g83 = clamp( staticSwitch41_g83 , float2( -1,-1 ) , float2( 1,1 ) );

				float4 transform1_g83 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));

				float4 appendResult3_g83 = (float4(transform1_g83.x , 0.0 , transform1_g83.z , 0.0));

				float dotResult4_g84 = dot( appendResult3_g83.xy , float2( 12.9898,78.233 ) );

				float lerpResult10_g84 = lerp( 1.0 , 1.01 , frac( ( sin( dotResult4_g84 ) * 43758.55 ) ));

				float mulTime9_g83 = _TimeParameters.x * lerpResult10_g84;

				float temp_output_29_0_g83 = _WIndSwayFrequency;

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float staticSwitch34_g83 = ( temp_output_29_0_g83 * varWindSwayFrequency );

				#else

				float staticSwitch34_g83 = temp_output_29_0_g83;

				#endif

				float2 break26_g83 = ( ( ( temp_output_16_0_g83 * temp_output_16_0_g83 ) - temp_output_16_0_g83 ) * ( ( ( staticSwitch41_g83 * float2( 4,4 ) ) + sin( ( ( clampResult10_g83 * mulTime9_g83 ) * staticSwitch34_g83 ) ) ) / float2( 4,4 ) ) );

				float4 appendResult25_g83 = (float4(break26_g83.x , 0.0 , break26_g83.y , 0.0));

				float4 temp_output_246_0 = appendResult25_g83;

				#ifdef _USEDFORTRUNK_ON

				float4 staticSwitch100 = temp_output_246_0;

				#else

				float4 staticSwitch100 = ( ( float4( v.ase_normal , 0.0 ) * lerpResult30_g87 ) + temp_output_246_0 );

				#endif

				float4 vWind116 = staticSwitch100;

				

				o.ase_texcoord2 = v.vertex;

				o.ase_texcoord3.xy = v.ase_texcoord.xy;

				

				//setting value to unused interpolator channels and avoid initialization warnings

				o.ase_texcoord3.zw = 0;

				

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					float3 defaultVertexValue = v.vertex.xyz;

				#else

					float3 defaultVertexValue = float3(0, 0, 0);

				#endif

				float3 vertexValue = vWind116.rgb;

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					v.vertex.xyz = vertexValue;

				#else

					v.vertex.xyz += vertexValue;

				#endif



				v.ase_normal = v.ase_normal;



				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)

				o.worldPos = positionWS;

				#endif



				o.clipPos = MetaVertexPosition( v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)

					VertexPositionInputs vertexInput = (VertexPositionInputs)0;

					vertexInput.positionWS = positionWS;

					vertexInput.positionCS = o.clipPos;

					o.shadowCoord = GetShadowCoord( vertexInput );

				#endif

				return o;

			}



			#if defined(TESSELLATION_ON)

			struct VertexControl

			{

				float4 vertex : INTERNALTESSPOS;

				float3 ase_normal : NORMAL;

				float4 texcoord1 : TEXCOORD1;

				float4 texcoord2 : TEXCOORD2;

				float4 ase_texcoord : TEXCOORD0;



				UNITY_VERTEX_INPUT_INSTANCE_ID

			};



			struct TessellationFactors

			{

				float edge[3] : SV_TessFactor;

				float inside : SV_InsideTessFactor;

			};



			VertexControl vert ( VertexInput v )

			{

				VertexControl o;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.vertex = v.vertex;

				o.ase_normal = v.ase_normal;

				o.texcoord1 = v.texcoord1;

				o.texcoord2 = v.texcoord2;

				o.ase_texcoord = v.ase_texcoord;

				return o;

			}



			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)

			{

				TessellationFactors o;

				float4 tf = 1;

				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;

				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;

				#if defined(ASE_FIXED_TESSELLATION)

				tf = FixedTess( tessValue );

				#elif defined(ASE_DISTANCE_TESSELLATION)

				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );

				#elif defined(ASE_LENGTH_TESSELLATION)

				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );

				#elif defined(ASE_LENGTH_CULL_TESSELLATION)

				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );

				#endif

				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;

				return o;

			}



			[domain("tri")]

			[partitioning("fractional_odd")]

			[outputtopology("triangle_cw")]

			[patchconstantfunc("TessellationFunction")]

			[outputcontrolpoints(3)]

			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)

			{

			   return patch[id];

			}



			[domain("tri")]

			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)

			{

				VertexInput o = (VertexInput) 0;

				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;

				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;

				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;

				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;

				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;

				#if defined(ASE_PHONG_TESSELLATION)

				float3 pp[3];

				for (int i = 0; i < 3; ++i)

					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));

				float phongStrength = _TessPhongStrength;

				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;

				#endif

				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);

				return VertexFunction(o);

			}

			#else

			VertexOutput vert ( VertexInput v )

			{

				return VertexFunction( v );

			}

			#endif



			half4 frag(VertexOutput IN  ) : SV_TARGET

			{

				UNITY_SETUP_INSTANCE_ID(IN);

				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );



				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)

				float3 WorldPosition = IN.worldPos;

				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );



				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)

					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)

						ShadowCoords = IN.shadowCoord;

					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)

						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );

					#endif

				#endif



				float4 lerpResult115 = lerp( _FoliageColorBottom , _FoliageColorTop , saturate( ( ( IN.ase_texcoord2.xyz.y + _GradientOffset ) * ( _GradientFallout * 2 ) ) ));

				float temp_output_235_0 = ( _FoliageSize / 100.0 );

				float2 appendResult234 = (float2(temp_output_235_0 , temp_output_235_0));

				float2 texCoord236 = IN.ase_texcoord3.xy * appendResult234 + float2( 0,0 );

				float4 tex2DNode124 = tex2D( _FoliageColorMap, texCoord236 );

				float4 vFoliageColor172 = ( lerpResult115 * tex2DNode124 );

				float2 texCoord18_g88 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );

				float2 temp_output_19_0_g88 = ( texCoord18_g88 / ( _TextureSize / 100.0 ) );

				#ifdef _USEDFORTRUNK_ON

				float4 staticSwitch82 = ( _ColorTint * tex2D( _ColorMap, temp_output_19_0_g88 ) );

				#else

				float4 staticSwitch82 = vFoliageColor172;

				#endif

				

				float4 vFoliageEmissive225 = ( _FoliageEmissiveColor * _FoliageEmissiveIntensity );

				#ifdef _USEDFORTRUNK_ON

				float4 staticSwitch228 = ( ( tex2D( _EmissiveMap, temp_output_19_0_g88 ) * _EmissiveColor ) * _EmissiveIntensity );

				#else

				float4 staticSwitch228 = vFoliageEmissive225;

				#endif

				

				float vFoliageOpacity173 = tex2DNode124.a;

				#ifdef _USEDFORTRUNK_ON

				float staticSwitch182 = 1.0;

				#else

				float staticSwitch182 = vFoliageOpacity173;

				#endif

				

				

				float3 Albedo = staticSwitch82.rgb;

				float3 Emission = staticSwitch228.rgb;

				float Alpha = staticSwitch182;

				float AlphaClipThreshold = _AlphaClip;



				#ifdef _ALPHATEST_ON

					clip(Alpha - AlphaClipThreshold);

				#endif



				MetaInput metaInput = (MetaInput)0;

				metaInput.Albedo = Albedo;

				metaInput.Emission = Emission;

				

				return MetaFragment(metaInput);

			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D"  "NatureRendererInstancing"="True" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM

			

			#define _NORMAL_DROPOFF_TS 1

			#pragma multi_compile_instancing

			#pragma multi_compile _ LOD_FADE_CROSSFADE

			#pragma multi_compile_fog

			#define ASE_FOG 1

			#define _TRANSLUCENCY_ASE 1

			#define _EMISSION

			#define _ALPHATEST_ON 1

			#define _NORMALMAP 1

			#define ASE_SRP_VERSION 999999



			

			#pragma vertex vert

			#pragma fragment frag



			#define SHADERPASS_2D



			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			

			#define ASE_NEEDS_VERT_NORMAL

			#define ASE_NEEDS_VERT_POSITION

			#pragma shader_feature_local _USEDFORTRUNK_ON

			#pragma shader_feature_local _USEGLOBALWINDSETTINGS_ON

			#pragma shader_feature_local _USESGLOBALWINDSETTINGS_ON





			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#include "Assets/Visual Design Cafe/Nature Renderer/Shader Includes/Nature Renderer.templatex"
			#pragma instancing_options procedural:SetupNatureRenderer forwardadd renderinglayer



			struct VertexInput

			{

				float4 vertex : POSITION;

				float3 ase_normal : NORMAL;

				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID

			};



			struct VertexOutput

			{

				float4 clipPos : SV_POSITION;

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)

				float3 worldPos : TEXCOORD0;

				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)

				float4 shadowCoord : TEXCOORD1;

				#endif

				float4 ase_texcoord2 : TEXCOORD2;

				float4 ase_texcoord3 : TEXCOORD3;

				UNITY_VERTEX_INPUT_INSTANCE_ID

				UNITY_VERTEX_OUTPUT_STEREO

			};



			CBUFFER_START(UnityPerMaterial)

			float4 _EmissiveColor;

			float4 _FoliageEmissiveColor;

			float4 _FoliageColorBottom;

			float4 _FoliageColorTop;

			float4 _ColorTint;

			float2 _WindSwayDirection;

			int _Culling;

			float _FoliageRoughness;

			float _EmissiveIntensity;

			float _FoliageEmissiveIntensity;

			float _NormalIntensity;

			float _FoliageNormalIntensity;

			float _TextureSize;

			float _GradientFallout;

			float _RoughnessIntensity;

			float _GradientOffset;

			float _WIndSwayFrequency;

			float _WIndSwayIntensity;

			float _WindOffsetIntensity;

			float _WindJitterSpeed;

			float _WindRustleSize;

			float _WindScrollSpeed;

			float _FoliageSize;

			float _AlphaClip;

			#ifdef _TRANSMISSION_ASE

				float _TransmissionShadow;

			#endif

			#ifdef _TRANSLUCENCY_ASE

				float _TransStrength;

				float _TransNormal;

				float _TransScattering;

				float _TransDirect;

				float _TransAmbient;

				float _TransShadow;

			#endif

			#ifdef TESSELLATION_ON

				float _TessPhongStrength;

				float _TessValue;

				float _TessMin;

				float _TessMax;

				float _TessEdgeLength;

				float _TessMaxDisp;

			#endif

			CBUFFER_END

			sampler2D _NoiseTexture;

			float varWindRustleScrollSpeed;

			float Float0;

			float varWindSwayIntensity;

			float2 varWindDirection;

			float varWindSwayFrequency;

			sampler2D _FoliageColorMap;

			sampler2D _ColorMap;





			

			VertexOutput VertexFunction( VertexInput v  )

			{

				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID( v );

				UNITY_TRANSFER_INSTANCE_ID( v, o );

				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );



				float temp_output_18_0_g87 = _WindScrollSpeed;

				#ifdef _USEGLOBALWINDSETTINGS_ON

				float staticSwitch25_g87 = ( temp_output_18_0_g87 * varWindRustleScrollSpeed );

				#else

				float staticSwitch25_g87 = temp_output_18_0_g87;

				#endif

				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;

				float2 appendResult4_g87 = (float2(ase_worldPos.x , ase_worldPos.z));

				float2 temp_output_7_0_g87 = ( appendResult4_g87 * _WindRustleSize );

				float2 panner9_g87 = ( ( staticSwitch25_g87 * _TimeParameters.x ) * float2( 1,1 ) + temp_output_7_0_g87);

				float temp_output_19_0_g87 = _WindJitterSpeed;

				#ifdef _USEGLOBALWINDSETTINGS_ON

				float staticSwitch26_g87 = ( temp_output_19_0_g87 * Float0 );

				#else

				float staticSwitch26_g87 = temp_output_19_0_g87;

				#endif

				float2 panner13_g87 = ( ( _TimeParameters.x * staticSwitch26_g87 ) * float2( 1,1 ) + ( temp_output_7_0_g87 * float2( 2,2 ) ));

				float4 lerpResult30_g87 = lerp( float4( float3(0,0,0) , 0.0 ) , ( pow( tex2Dlod( _NoiseTexture, float4( panner9_g87, 0, 0.0) ) , 1.0 ) * tex2Dlod( _NoiseTexture, float4( panner13_g87, 0, 0.0) ) ) , _WindOffsetIntensity);

				float temp_output_27_0_g83 = _WIndSwayIntensity;

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float staticSwitch33_g83 = ( temp_output_27_0_g83 * varWindSwayIntensity );

				#else

				float staticSwitch33_g83 = temp_output_27_0_g83;

				#endif

				float temp_output_14_0_g83 = ( ( v.vertex.xyz.y * ( staticSwitch33_g83 / 100.0 ) ) + 1.0 );

				float temp_output_16_0_g83 = ( temp_output_14_0_g83 * temp_output_14_0_g83 );

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float2 staticSwitch41_g83 = varWindDirection;

				#else

				float2 staticSwitch41_g83 = _WindSwayDirection;

				#endif

				float2 clampResult10_g83 = clamp( staticSwitch41_g83 , float2( -1,-1 ) , float2( 1,1 ) );

				float4 transform1_g83 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));

				float4 appendResult3_g83 = (float4(transform1_g83.x , 0.0 , transform1_g83.z , 0.0));

				float dotResult4_g84 = dot( appendResult3_g83.xy , float2( 12.9898,78.233 ) );

				float lerpResult10_g84 = lerp( 1.0 , 1.01 , frac( ( sin( dotResult4_g84 ) * 43758.55 ) ));

				float mulTime9_g83 = _TimeParameters.x * lerpResult10_g84;

				float temp_output_29_0_g83 = _WIndSwayFrequency;

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float staticSwitch34_g83 = ( temp_output_29_0_g83 * varWindSwayFrequency );

				#else

				float staticSwitch34_g83 = temp_output_29_0_g83;

				#endif

				float2 break26_g83 = ( ( ( temp_output_16_0_g83 * temp_output_16_0_g83 ) - temp_output_16_0_g83 ) * ( ( ( staticSwitch41_g83 * float2( 4,4 ) ) + sin( ( ( clampResult10_g83 * mulTime9_g83 ) * staticSwitch34_g83 ) ) ) / float2( 4,4 ) ) );

				float4 appendResult25_g83 = (float4(break26_g83.x , 0.0 , break26_g83.y , 0.0));

				float4 temp_output_246_0 = appendResult25_g83;

				#ifdef _USEDFORTRUNK_ON

				float4 staticSwitch100 = temp_output_246_0;

				#else

				float4 staticSwitch100 = ( ( float4( v.ase_normal , 0.0 ) * lerpResult30_g87 ) + temp_output_246_0 );

				#endif

				float4 vWind116 = staticSwitch100;

				

				o.ase_texcoord2 = v.vertex;

				o.ase_texcoord3.xy = v.ase_texcoord.xy;

				

				//setting value to unused interpolator channels and avoid initialization warnings

				o.ase_texcoord3.zw = 0;

				

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					float3 defaultVertexValue = v.vertex.xyz;

				#else

					float3 defaultVertexValue = float3(0, 0, 0);

				#endif

				float3 vertexValue = vWind116.rgb;

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					v.vertex.xyz = vertexValue;

				#else

					v.vertex.xyz += vertexValue;

				#endif



				v.ase_normal = v.ase_normal;



				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				float4 positionCS = TransformWorldToHClip( positionWS );



				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)

				o.worldPos = positionWS;

				#endif



				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)

					VertexPositionInputs vertexInput = (VertexPositionInputs)0;

					vertexInput.positionWS = positionWS;

					vertexInput.positionCS = positionCS;

					o.shadowCoord = GetShadowCoord( vertexInput );

				#endif



				o.clipPos = positionCS;

				return o;

			}



			#if defined(TESSELLATION_ON)

			struct VertexControl

			{

				float4 vertex : INTERNALTESSPOS;

				float3 ase_normal : NORMAL;

				float4 ase_texcoord : TEXCOORD0;



				UNITY_VERTEX_INPUT_INSTANCE_ID

			};



			struct TessellationFactors

			{

				float edge[3] : SV_TessFactor;

				float inside : SV_InsideTessFactor;

			};



			VertexControl vert ( VertexInput v )

			{

				VertexControl o;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.vertex = v.vertex;

				o.ase_normal = v.ase_normal;

				o.ase_texcoord = v.ase_texcoord;

				return o;

			}



			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)

			{

				TessellationFactors o;

				float4 tf = 1;

				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;

				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;

				#if defined(ASE_FIXED_TESSELLATION)

				tf = FixedTess( tessValue );

				#elif defined(ASE_DISTANCE_TESSELLATION)

				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );

				#elif defined(ASE_LENGTH_TESSELLATION)

				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );

				#elif defined(ASE_LENGTH_CULL_TESSELLATION)

				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );

				#endif

				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;

				return o;

			}



			[domain("tri")]

			[partitioning("fractional_odd")]

			[outputtopology("triangle_cw")]

			[patchconstantfunc("TessellationFunction")]

			[outputcontrolpoints(3)]

			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)

			{

			   return patch[id];

			}



			[domain("tri")]

			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)

			{

				VertexInput o = (VertexInput) 0;

				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;

				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;

				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;

				#if defined(ASE_PHONG_TESSELLATION)

				float3 pp[3];

				for (int i = 0; i < 3; ++i)

					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));

				float phongStrength = _TessPhongStrength;

				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;

				#endif

				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);

				return VertexFunction(o);

			}

			#else

			VertexOutput vert ( VertexInput v )

			{

				return VertexFunction( v );

			}

			#endif



			half4 frag(VertexOutput IN  ) : SV_TARGET

			{

				UNITY_SETUP_INSTANCE_ID( IN );

				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );



				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)

				float3 WorldPosition = IN.worldPos;

				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );



				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)

					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)

						ShadowCoords = IN.shadowCoord;

					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)

						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );

					#endif

				#endif



				float4 lerpResult115 = lerp( _FoliageColorBottom , _FoliageColorTop , saturate( ( ( IN.ase_texcoord2.xyz.y + _GradientOffset ) * ( _GradientFallout * 2 ) ) ));

				float temp_output_235_0 = ( _FoliageSize / 100.0 );

				float2 appendResult234 = (float2(temp_output_235_0 , temp_output_235_0));

				float2 texCoord236 = IN.ase_texcoord3.xy * appendResult234 + float2( 0,0 );

				float4 tex2DNode124 = tex2D( _FoliageColorMap, texCoord236 );

				float4 vFoliageColor172 = ( lerpResult115 * tex2DNode124 );

				float2 texCoord18_g88 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );

				float2 temp_output_19_0_g88 = ( texCoord18_g88 / ( _TextureSize / 100.0 ) );

				#ifdef _USEDFORTRUNK_ON

				float4 staticSwitch82 = ( _ColorTint * tex2D( _ColorMap, temp_output_19_0_g88 ) );

				#else

				float4 staticSwitch82 = vFoliageColor172;

				#endif

				

				float vFoliageOpacity173 = tex2DNode124.a;

				#ifdef _USEDFORTRUNK_ON

				float staticSwitch182 = 1.0;

				#else

				float staticSwitch182 = vFoliageOpacity173;

				#endif

				

				

				float3 Albedo = staticSwitch82.rgb;

				float Alpha = staticSwitch182;

				float AlphaClipThreshold = _AlphaClip;



				half4 color = half4( Albedo, Alpha );



				#ifdef _ALPHATEST_ON

					clip(Alpha - AlphaClipThreshold);

				#endif



				return color;

			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormals"  "NatureRendererInstancing"="True" }

			ZWrite On
			Blend One Zero
            ZTest LEqual
            ZWrite On

			HLSLPROGRAM

			

			#define _NORMAL_DROPOFF_TS 1

			#pragma multi_compile_instancing

			#pragma multi_compile _ LOD_FADE_CROSSFADE

			#pragma multi_compile_fog

			#define ASE_FOG 1

			#define _TRANSLUCENCY_ASE 1

			#define _EMISSION

			#define _ALPHATEST_ON 1

			#define _NORMALMAP 1

			#define ASE_SRP_VERSION 999999



			

			#pragma vertex vert

			#pragma fragment frag



			#define SHADERPASS_DEPTHNORMALSONLY



			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"



			#define ASE_NEEDS_VERT_NORMAL

			#define ASE_NEEDS_VERT_POSITION

			#pragma shader_feature_local _USEDFORTRUNK_ON

			#pragma shader_feature_local _USEGLOBALWINDSETTINGS_ON

			#pragma shader_feature_local _USESGLOBALWINDSETTINGS_ON
			#include "Assets/Visual Design Cafe/Nature Renderer/Shader Includes/Nature Renderer.templatex"
			#pragma instancing_options procedural:SetupNatureRenderer forwardadd renderinglayer





			struct VertexInput

			{

				float4 vertex : POSITION;

				float3 ase_normal : NORMAL;

				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID

			};



			struct VertexOutput

			{

				float4 clipPos : SV_POSITION;

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)

				float3 worldPos : TEXCOORD0;

				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)

				float4 shadowCoord : TEXCOORD1;

				#endif

				float3 worldNormal : TEXCOORD2;

				float4 ase_texcoord3 : TEXCOORD3;

				UNITY_VERTEX_INPUT_INSTANCE_ID

				UNITY_VERTEX_OUTPUT_STEREO

			};



			CBUFFER_START(UnityPerMaterial)

			float4 _EmissiveColor;

			float4 _FoliageEmissiveColor;

			float4 _FoliageColorBottom;

			float4 _FoliageColorTop;

			float4 _ColorTint;

			float2 _WindSwayDirection;

			int _Culling;

			float _FoliageRoughness;

			float _EmissiveIntensity;

			float _FoliageEmissiveIntensity;

			float _NormalIntensity;

			float _FoliageNormalIntensity;

			float _TextureSize;

			float _GradientFallout;

			float _RoughnessIntensity;

			float _GradientOffset;

			float _WIndSwayFrequency;

			float _WIndSwayIntensity;

			float _WindOffsetIntensity;

			float _WindJitterSpeed;

			float _WindRustleSize;

			float _WindScrollSpeed;

			float _FoliageSize;

			float _AlphaClip;

			#ifdef _TRANSMISSION_ASE

				float _TransmissionShadow;

			#endif

			#ifdef _TRANSLUCENCY_ASE

				float _TransStrength;

				float _TransNormal;

				float _TransScattering;

				float _TransDirect;

				float _TransAmbient;

				float _TransShadow;

			#endif

			#ifdef TESSELLATION_ON

				float _TessPhongStrength;

				float _TessValue;

				float _TessMin;

				float _TessMax;

				float _TessEdgeLength;

				float _TessMaxDisp;

			#endif

			CBUFFER_END

			sampler2D _NoiseTexture;

			float varWindRustleScrollSpeed;

			float Float0;

			float varWindSwayIntensity;

			float2 varWindDirection;

			float varWindSwayFrequency;

			sampler2D _FoliageColorMap;





			

			VertexOutput VertexFunction( VertexInput v  )

			{

				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);



				float temp_output_18_0_g87 = _WindScrollSpeed;

				#ifdef _USEGLOBALWINDSETTINGS_ON

				float staticSwitch25_g87 = ( temp_output_18_0_g87 * varWindRustleScrollSpeed );

				#else

				float staticSwitch25_g87 = temp_output_18_0_g87;

				#endif

				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;

				float2 appendResult4_g87 = (float2(ase_worldPos.x , ase_worldPos.z));

				float2 temp_output_7_0_g87 = ( appendResult4_g87 * _WindRustleSize );

				float2 panner9_g87 = ( ( staticSwitch25_g87 * _TimeParameters.x ) * float2( 1,1 ) + temp_output_7_0_g87);

				float temp_output_19_0_g87 = _WindJitterSpeed;

				#ifdef _USEGLOBALWINDSETTINGS_ON

				float staticSwitch26_g87 = ( temp_output_19_0_g87 * Float0 );

				#else

				float staticSwitch26_g87 = temp_output_19_0_g87;

				#endif

				float2 panner13_g87 = ( ( _TimeParameters.x * staticSwitch26_g87 ) * float2( 1,1 ) + ( temp_output_7_0_g87 * float2( 2,2 ) ));

				float4 lerpResult30_g87 = lerp( float4( float3(0,0,0) , 0.0 ) , ( pow( tex2Dlod( _NoiseTexture, float4( panner9_g87, 0, 0.0) ) , 1.0 ) * tex2Dlod( _NoiseTexture, float4( panner13_g87, 0, 0.0) ) ) , _WindOffsetIntensity);

				float temp_output_27_0_g83 = _WIndSwayIntensity;

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float staticSwitch33_g83 = ( temp_output_27_0_g83 * varWindSwayIntensity );

				#else

				float staticSwitch33_g83 = temp_output_27_0_g83;

				#endif

				float temp_output_14_0_g83 = ( ( v.vertex.xyz.y * ( staticSwitch33_g83 / 100.0 ) ) + 1.0 );

				float temp_output_16_0_g83 = ( temp_output_14_0_g83 * temp_output_14_0_g83 );

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float2 staticSwitch41_g83 = varWindDirection;

				#else

				float2 staticSwitch41_g83 = _WindSwayDirection;

				#endif

				float2 clampResult10_g83 = clamp( staticSwitch41_g83 , float2( -1,-1 ) , float2( 1,1 ) );

				float4 transform1_g83 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));

				float4 appendResult3_g83 = (float4(transform1_g83.x , 0.0 , transform1_g83.z , 0.0));

				float dotResult4_g84 = dot( appendResult3_g83.xy , float2( 12.9898,78.233 ) );

				float lerpResult10_g84 = lerp( 1.0 , 1.01 , frac( ( sin( dotResult4_g84 ) * 43758.55 ) ));

				float mulTime9_g83 = _TimeParameters.x * lerpResult10_g84;

				float temp_output_29_0_g83 = _WIndSwayFrequency;

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float staticSwitch34_g83 = ( temp_output_29_0_g83 * varWindSwayFrequency );

				#else

				float staticSwitch34_g83 = temp_output_29_0_g83;

				#endif

				float2 break26_g83 = ( ( ( temp_output_16_0_g83 * temp_output_16_0_g83 ) - temp_output_16_0_g83 ) * ( ( ( staticSwitch41_g83 * float2( 4,4 ) ) + sin( ( ( clampResult10_g83 * mulTime9_g83 ) * staticSwitch34_g83 ) ) ) / float2( 4,4 ) ) );

				float4 appendResult25_g83 = (float4(break26_g83.x , 0.0 , break26_g83.y , 0.0));

				float4 temp_output_246_0 = appendResult25_g83;

				#ifdef _USEDFORTRUNK_ON

				float4 staticSwitch100 = temp_output_246_0;

				#else

				float4 staticSwitch100 = ( ( float4( v.ase_normal , 0.0 ) * lerpResult30_g87 ) + temp_output_246_0 );

				#endif

				float4 vWind116 = staticSwitch100;

				

				o.ase_texcoord3.xy = v.ase_texcoord.xy;

				

				//setting value to unused interpolator channels and avoid initialization warnings

				o.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					float3 defaultVertexValue = v.vertex.xyz;

				#else

					float3 defaultVertexValue = float3(0, 0, 0);

				#endif

				float3 vertexValue = vWind116.rgb;

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					v.vertex.xyz = vertexValue;

				#else

					v.vertex.xyz += vertexValue;

				#endif



				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				float3 normalWS = TransformObjectToWorldNormal( v.ase_normal );

				float4 positionCS = TransformWorldToHClip( positionWS );



				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)

				o.worldPos = positionWS;

				#endif



				o.worldNormal = normalWS;



				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)

					VertexPositionInputs vertexInput = (VertexPositionInputs)0;

					vertexInput.positionWS = positionWS;

					vertexInput.positionCS = positionCS;

					o.shadowCoord = GetShadowCoord( vertexInput );

				#endif

				o.clipPos = positionCS;

				return o;

			}



			#if defined(TESSELLATION_ON)

			struct VertexControl

			{

				float4 vertex : INTERNALTESSPOS;

				float3 ase_normal : NORMAL;

				float4 ase_texcoord : TEXCOORD0;



				UNITY_VERTEX_INPUT_INSTANCE_ID

			};



			struct TessellationFactors

			{

				float edge[3] : SV_TessFactor;

				float inside : SV_InsideTessFactor;

			};



			VertexControl vert ( VertexInput v )

			{

				VertexControl o;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.vertex = v.vertex;

				o.ase_normal = v.ase_normal;

				o.ase_texcoord = v.ase_texcoord;

				return o;

			}



			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)

			{

				TessellationFactors o;

				float4 tf = 1;

				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;

				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;

				#if defined(ASE_FIXED_TESSELLATION)

				tf = FixedTess( tessValue );

				#elif defined(ASE_DISTANCE_TESSELLATION)

				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );

				#elif defined(ASE_LENGTH_TESSELLATION)

				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );

				#elif defined(ASE_LENGTH_CULL_TESSELLATION)

				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );

				#endif

				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;

				return o;

			}



			[domain("tri")]

			[partitioning("fractional_odd")]

			[outputtopology("triangle_cw")]

			[patchconstantfunc("TessellationFunction")]

			[outputcontrolpoints(3)]

			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)

			{

			   return patch[id];

			}



			[domain("tri")]

			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)

			{

				VertexInput o = (VertexInput) 0;

				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;

				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;

				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;

				#if defined(ASE_PHONG_TESSELLATION)

				float3 pp[3];

				for (int i = 0; i < 3; ++i)

					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));

				float phongStrength = _TessPhongStrength;

				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;

				#endif

				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);

				return VertexFunction(o);

			}

			#else

			VertexOutput vert ( VertexInput v )

			{

				return VertexFunction( v );

			}

			#endif



			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)

				#define ASE_SV_DEPTH SV_DepthLessEqual  

			#else

				#define ASE_SV_DEPTH SV_Depth

			#endif

			half4 frag(	VertexOutput IN 

						#ifdef ASE_DEPTH_WRITE_ON

						,out float outputDepth : ASE_SV_DEPTH

						#endif

						 ) : SV_TARGET

			{

				UNITY_SETUP_INSTANCE_ID(IN);

				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );



				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)

				float3 WorldPosition = IN.worldPos;

				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );



				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)

					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)

						ShadowCoords = IN.shadowCoord;

					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)

						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );

					#endif

				#endif



				float temp_output_235_0 = ( _FoliageSize / 100.0 );

				float2 appendResult234 = (float2(temp_output_235_0 , temp_output_235_0));

				float2 texCoord236 = IN.ase_texcoord3.xy * appendResult234 + float2( 0,0 );

				float4 tex2DNode124 = tex2D( _FoliageColorMap, texCoord236 );

				float vFoliageOpacity173 = tex2DNode124.a;

				#ifdef _USEDFORTRUNK_ON

				float staticSwitch182 = 1.0;

				#else

				float staticSwitch182 = vFoliageOpacity173;

				#endif

				

				float Alpha = staticSwitch182;

				float AlphaClipThreshold = _AlphaClip;

				#ifdef ASE_DEPTH_WRITE_ON

				float DepthValue = 0;

				#endif



				#ifdef _ALPHATEST_ON

					clip(Alpha - AlphaClipThreshold);

				#endif



				#ifdef LOD_FADE_CROSSFADE

					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );

				#endif

				

				#ifdef ASE_DEPTH_WRITE_ON

				outputDepth = DepthValue;

				#endif

				

				return float4(PackNormalOctRectEncode(TransformWorldToViewDir(IN.worldNormal, true)), 0.0, 0.0);

			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "GBuffer"
			Tags { "LightMode"="UniversalGBuffer"  "NatureRendererInstancing"="True" }
			
			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			

			#define _NORMAL_DROPOFF_TS 1

			#pragma multi_compile_instancing

			#pragma multi_compile _ LOD_FADE_CROSSFADE

			#pragma multi_compile_fog

			#define ASE_FOG 1

			#define _TRANSLUCENCY_ASE 1

			#define _EMISSION

			#define _ALPHATEST_ON 1

			#define _NORMALMAP 1

			#define ASE_SRP_VERSION 999999



			

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS

			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

			#pragma multi_compile _ _SHADOWS_SOFT

			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

			#pragma multi_compile _ _GBUFFER_NORMALS_OCT

			

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED

			#pragma multi_compile _ LIGHTMAP_ON



			#pragma vertex vert

			#pragma fragment frag



			#define SHADERPASS SHADERPASS_GBUFFER



			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"



			#if ASE_SRP_VERSION <= 70108

			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR

			#endif



			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)

			    #define ENABLE_TERRAIN_PERPIXEL_NORMAL

			#endif



			#define ASE_NEEDS_VERT_NORMAL

			#define ASE_NEEDS_VERT_POSITION

			#pragma shader_feature_local _USEDFORTRUNK_ON

			#pragma shader_feature_local _USEGLOBALWINDSETTINGS_ON

			#pragma shader_feature_local _USESGLOBALWINDSETTINGS_ON
			#include "Assets/Visual Design Cafe/Nature Renderer/Shader Includes/Nature Renderer.templatex"
			#pragma instancing_options procedural:SetupNatureRenderer forwardadd renderinglayer





			struct VertexInput

			{

				float4 vertex : POSITION;

				float3 ase_normal : NORMAL;

				float4 ase_tangent : TANGENT;

				float4 texcoord1 : TEXCOORD1;

				float4 texcoord : TEXCOORD0;

				

				UNITY_VERTEX_INPUT_INSTANCE_ID

			};



			struct VertexOutput

			{

				float4 clipPos : SV_POSITION;

				float4 lightmapUVOrVertexSH : TEXCOORD0;

				half4 fogFactorAndVertexLight : TEXCOORD1;

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)

				float4 shadowCoord : TEXCOORD2;

				#endif

				float4 tSpace0 : TEXCOORD3;

				float4 tSpace1 : TEXCOORD4;

				float4 tSpace2 : TEXCOORD5;

				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)

				float4 screenPos : TEXCOORD6;

				#endif

				float4 ase_texcoord7 : TEXCOORD7;

				float4 ase_texcoord8 : TEXCOORD8;

				UNITY_VERTEX_INPUT_INSTANCE_ID

				UNITY_VERTEX_OUTPUT_STEREO

			};



			CBUFFER_START(UnityPerMaterial)

			float4 _EmissiveColor;

			float4 _FoliageEmissiveColor;

			float4 _FoliageColorBottom;

			float4 _FoliageColorTop;

			float4 _ColorTint;

			float2 _WindSwayDirection;

			int _Culling;

			float _FoliageRoughness;

			float _EmissiveIntensity;

			float _FoliageEmissiveIntensity;

			float _NormalIntensity;

			float _FoliageNormalIntensity;

			float _TextureSize;

			float _GradientFallout;

			float _RoughnessIntensity;

			float _GradientOffset;

			float _WIndSwayFrequency;

			float _WIndSwayIntensity;

			float _WindOffsetIntensity;

			float _WindJitterSpeed;

			float _WindRustleSize;

			float _WindScrollSpeed;

			float _FoliageSize;

			float _AlphaClip;

			#ifdef _TRANSMISSION_ASE

				float _TransmissionShadow;

			#endif

			#ifdef _TRANSLUCENCY_ASE

				float _TransStrength;

				float _TransNormal;

				float _TransScattering;

				float _TransDirect;

				float _TransAmbient;

				float _TransShadow;

			#endif

			#ifdef TESSELLATION_ON

				float _TessPhongStrength;

				float _TessValue;

				float _TessMin;

				float _TessMax;

				float _TessEdgeLength;

				float _TessMaxDisp;

			#endif

			CBUFFER_END

			sampler2D _NoiseTexture;

			float varWindRustleScrollSpeed;

			float Float0;

			float varWindSwayIntensity;

			float2 varWindDirection;

			float varWindSwayFrequency;

			sampler2D _FoliageColorMap;

			sampler2D _ColorMap;

			sampler2D _FoliageNormalMap;

			sampler2D _NormalMap;

			sampler2D _EmissiveMap;

			sampler2D _ORMMap;





			

			VertexOutput VertexFunction( VertexInput v  )

			{

				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);



				float temp_output_18_0_g87 = _WindScrollSpeed;

				#ifdef _USEGLOBALWINDSETTINGS_ON

				float staticSwitch25_g87 = ( temp_output_18_0_g87 * varWindRustleScrollSpeed );

				#else

				float staticSwitch25_g87 = temp_output_18_0_g87;

				#endif

				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;

				float2 appendResult4_g87 = (float2(ase_worldPos.x , ase_worldPos.z));

				float2 temp_output_7_0_g87 = ( appendResult4_g87 * _WindRustleSize );

				float2 panner9_g87 = ( ( staticSwitch25_g87 * _TimeParameters.x ) * float2( 1,1 ) + temp_output_7_0_g87);

				float temp_output_19_0_g87 = _WindJitterSpeed;

				#ifdef _USEGLOBALWINDSETTINGS_ON

				float staticSwitch26_g87 = ( temp_output_19_0_g87 * Float0 );

				#else

				float staticSwitch26_g87 = temp_output_19_0_g87;

				#endif

				float2 panner13_g87 = ( ( _TimeParameters.x * staticSwitch26_g87 ) * float2( 1,1 ) + ( temp_output_7_0_g87 * float2( 2,2 ) ));

				float4 lerpResult30_g87 = lerp( float4( float3(0,0,0) , 0.0 ) , ( pow( tex2Dlod( _NoiseTexture, float4( panner9_g87, 0, 0.0) ) , 1.0 ) * tex2Dlod( _NoiseTexture, float4( panner13_g87, 0, 0.0) ) ) , _WindOffsetIntensity);

				float temp_output_27_0_g83 = _WIndSwayIntensity;

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float staticSwitch33_g83 = ( temp_output_27_0_g83 * varWindSwayIntensity );

				#else

				float staticSwitch33_g83 = temp_output_27_0_g83;

				#endif

				float temp_output_14_0_g83 = ( ( v.vertex.xyz.y * ( staticSwitch33_g83 / 100.0 ) ) + 1.0 );

				float temp_output_16_0_g83 = ( temp_output_14_0_g83 * temp_output_14_0_g83 );

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float2 staticSwitch41_g83 = varWindDirection;

				#else

				float2 staticSwitch41_g83 = _WindSwayDirection;

				#endif

				float2 clampResult10_g83 = clamp( staticSwitch41_g83 , float2( -1,-1 ) , float2( 1,1 ) );

				float4 transform1_g83 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));

				float4 appendResult3_g83 = (float4(transform1_g83.x , 0.0 , transform1_g83.z , 0.0));

				float dotResult4_g84 = dot( appendResult3_g83.xy , float2( 12.9898,78.233 ) );

				float lerpResult10_g84 = lerp( 1.0 , 1.01 , frac( ( sin( dotResult4_g84 ) * 43758.55 ) ));

				float mulTime9_g83 = _TimeParameters.x * lerpResult10_g84;

				float temp_output_29_0_g83 = _WIndSwayFrequency;

				#ifdef _USESGLOBALWINDSETTINGS_ON

				float staticSwitch34_g83 = ( temp_output_29_0_g83 * varWindSwayFrequency );

				#else

				float staticSwitch34_g83 = temp_output_29_0_g83;

				#endif

				float2 break26_g83 = ( ( ( temp_output_16_0_g83 * temp_output_16_0_g83 ) - temp_output_16_0_g83 ) * ( ( ( staticSwitch41_g83 * float2( 4,4 ) ) + sin( ( ( clampResult10_g83 * mulTime9_g83 ) * staticSwitch34_g83 ) ) ) / float2( 4,4 ) ) );

				float4 appendResult25_g83 = (float4(break26_g83.x , 0.0 , break26_g83.y , 0.0));

				float4 temp_output_246_0 = appendResult25_g83;

				#ifdef _USEDFORTRUNK_ON

				float4 staticSwitch100 = temp_output_246_0;

				#else

				float4 staticSwitch100 = ( ( float4( v.ase_normal , 0.0 ) * lerpResult30_g87 ) + temp_output_246_0 );

				#endif

				float4 vWind116 = staticSwitch100;

				

				o.ase_texcoord7 = v.vertex;

				o.ase_texcoord8.xy = v.texcoord.xy;

				

				//setting value to unused interpolator channels and avoid initialization warnings

				o.ase_texcoord8.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					float3 defaultVertexValue = v.vertex.xyz;

				#else

					float3 defaultVertexValue = float3(0, 0, 0);

				#endif

				float3 vertexValue = vWind116.rgb;

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					v.vertex.xyz = vertexValue;

				#else

					v.vertex.xyz += vertexValue;

				#endif

				v.ase_normal = v.ase_normal;



				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				float3 positionVS = TransformWorldToView( positionWS );

				float4 positionCS = TransformWorldToHClip( positionWS );



				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );



				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);

				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);

				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);



				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );

				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );



				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)

					o.lightmapUVOrVertexSH.zw = v.texcoord;

					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;

				#endif



				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );

				#ifdef ASE_FOG

					half fogFactor = ComputeFogFactor( positionCS.z );

				#else

					half fogFactor = 0;

				#endif

				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

				

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)

				VertexPositionInputs vertexInput = (VertexPositionInputs)0;

				vertexInput.positionWS = positionWS;

				vertexInput.positionCS = positionCS;

				o.shadowCoord = GetShadowCoord( vertexInput );

				#endif

				

				o.clipPos = positionCS;

				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)

				o.screenPos = ComputeScreenPos(positionCS);

				#endif

				return o;

			}

			

			#if defined(TESSELLATION_ON)

			struct VertexControl

			{

				float4 vertex : INTERNALTESSPOS;

				float3 ase_normal : NORMAL;

				float4 ase_tangent : TANGENT;

				float4 texcoord : TEXCOORD0;

				float4 texcoord1 : TEXCOORD1;

				

				UNITY_VERTEX_INPUT_INSTANCE_ID

			};



			struct TessellationFactors

			{

				float edge[3] : SV_TessFactor;

				float inside : SV_InsideTessFactor;

			};



			VertexControl vert ( VertexInput v )

			{

				VertexControl o;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.vertex = v.vertex;

				o.ase_normal = v.ase_normal;

				o.ase_tangent = v.ase_tangent;

				o.texcoord = v.texcoord;

				o.texcoord1 = v.texcoord1;

				

				return o;

			}



			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)

			{

				TessellationFactors o;

				float4 tf = 1;

				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;

				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;

				#if defined(ASE_FIXED_TESSELLATION)

				tf = FixedTess( tessValue );

				#elif defined(ASE_DISTANCE_TESSELLATION)

				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );

				#elif defined(ASE_LENGTH_TESSELLATION)

				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );

				#elif defined(ASE_LENGTH_CULL_TESSELLATION)

				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );

				#endif

				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;

				return o;

			}



			[domain("tri")]

			[partitioning("fractional_odd")]

			[outputtopology("triangle_cw")]

			[patchconstantfunc("TessellationFunction")]

			[outputcontrolpoints(3)]

			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)

			{

			   return patch[id];

			}



			[domain("tri")]

			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)

			{

				VertexInput o = (VertexInput) 0;

				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;

				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;

				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;

				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;

				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;

				

				#if defined(ASE_PHONG_TESSELLATION)

				float3 pp[3];

				for (int i = 0; i < 3; ++i)

					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));

				float phongStrength = _TessPhongStrength;

				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;

				#endif

				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);

				return VertexFunction(o);

			}

			#else

			VertexOutput vert ( VertexInput v )

			{

				return VertexFunction( v );

			}

			#endif



			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)

				#define ASE_SV_DEPTH SV_DepthLessEqual  

			#else

				#define ASE_SV_DEPTH SV_Depth

			#endif

			FragmentOutput frag ( VertexOutput IN 

								#ifdef ASE_DEPTH_WRITE_ON

								,out float outputDepth : ASE_SV_DEPTH

								#endif

								 )

			{

				UNITY_SETUP_INSTANCE_ID(IN);

				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);



				#ifdef LOD_FADE_CROSSFADE

					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );

				#endif



				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)

					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;

					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));

					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);

					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);

				#else

					float3 WorldNormal = normalize( IN.tSpace0.xyz );

					float3 WorldTangent = IN.tSpace1.xyz;

					float3 WorldBiTangent = IN.tSpace2.xyz;

				#endif

				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);

				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)

				float4 ScreenPos = IN.screenPos;

				#endif



				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)

					ShadowCoords = IN.shadowCoord;

				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)

					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );

				#endif

	

				WorldViewDirection = SafeNormalize( WorldViewDirection );



				float4 lerpResult115 = lerp( _FoliageColorBottom , _FoliageColorTop , saturate( ( ( IN.ase_texcoord7.xyz.y + _GradientOffset ) * ( _GradientFallout * 2 ) ) ));

				float temp_output_235_0 = ( _FoliageSize / 100.0 );

				float2 appendResult234 = (float2(temp_output_235_0 , temp_output_235_0));

				float2 texCoord236 = IN.ase_texcoord8.xy * appendResult234 + float2( 0,0 );

				float4 tex2DNode124 = tex2D( _FoliageColorMap, texCoord236 );

				float4 vFoliageColor172 = ( lerpResult115 * tex2DNode124 );

				float2 texCoord18_g88 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );

				float2 temp_output_19_0_g88 = ( texCoord18_g88 / ( _TextureSize / 100.0 ) );

				#ifdef _USEDFORTRUNK_ON

				float4 staticSwitch82 = ( _ColorTint * tex2D( _ColorMap, temp_output_19_0_g88 ) );

				#else

				float4 staticSwitch82 = vFoliageColor172;

				#endif

				

				float3 unpack176 = UnpackNormalScale( tex2D( _FoliageNormalMap, texCoord236 ), _FoliageNormalIntensity );

				unpack176.z = lerp( 1, unpack176.z, saturate(_FoliageNormalIntensity) );

				float3 vFoliageNormal226 = unpack176;

				float3 unpack26_g88 = UnpackNormalScale( tex2D( _NormalMap, temp_output_19_0_g88 ), _NormalIntensity );

				unpack26_g88.z = lerp( 1, unpack26_g88.z, saturate(_NormalIntensity) );

				#ifdef _USEDFORTRUNK_ON

				float3 staticSwitch97 = unpack26_g88;

				#else

				float3 staticSwitch97 = vFoliageNormal226;

				#endif

				

				float4 vFoliageEmissive225 = ( _FoliageEmissiveColor * _FoliageEmissiveIntensity );

				#ifdef _USEDFORTRUNK_ON

				float4 staticSwitch228 = ( ( tex2D( _EmissiveMap, temp_output_19_0_g88 ) * _EmissiveColor ) * _EmissiveIntensity );

				#else

				float4 staticSwitch228 = vFoliageEmissive225;

				#endif

				

				float4 tex2DNode9_g89 = tex2D( _ORMMap, temp_output_19_0_g88 );

				#ifdef _USEDFORTRUNK_ON

				float staticSwitch98 = ( 1.0 - ( tex2DNode9_g89.g * _RoughnessIntensity ) );

				#else

				float staticSwitch98 = ( 1.0 - _FoliageRoughness );

				#endif

				

				float vFoliageOpacity173 = tex2DNode124.a;

				#ifdef _USEDFORTRUNK_ON

				float staticSwitch182 = 1.0;

				#else

				float staticSwitch182 = vFoliageOpacity173;

				#endif

				

				float3 Albedo = staticSwitch82.rgb;

				float3 Normal = staticSwitch97;

				float3 Emission = staticSwitch228.rgb;

				float3 Specular = 0.5;

				float Metallic = 0;

				float Smoothness = staticSwitch98;

				float Occlusion = 1;

				float Alpha = staticSwitch182;

				float AlphaClipThreshold = _AlphaClip;

				float AlphaClipThresholdShadow = 0.5;

				float3 BakedGI = 0;

				float3 RefractionColor = 1;

				float RefractionIndex = 1;

				float3 Transmission = 1;

				float3 Translucency = 1;

				#ifdef ASE_DEPTH_WRITE_ON

				float DepthValue = 0;

				#endif



				#ifdef _ALPHATEST_ON

					clip(Alpha - AlphaClipThreshold);

				#endif



				InputData inputData;

				inputData.positionWS = WorldPosition;

				inputData.viewDirectionWS = WorldViewDirection;

				inputData.shadowCoord = ShadowCoords;



				#ifdef _NORMALMAP

					#if _NORMAL_DROPOFF_TS

					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));

					#elif _NORMAL_DROPOFF_OS

					inputData.normalWS = TransformObjectToWorldNormal(Normal);

					#elif _NORMAL_DROPOFF_WS

					inputData.normalWS = Normal;

					#endif

					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);

				#else

					inputData.normalWS = WorldNormal;

				#endif



				#ifdef ASE_FOG

					inputData.fogCoord = IN.fogFactorAndVertexLight.x;

				#endif



				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)

					float3 SH = SampleSH(inputData.normalWS.xyz);

				#else

					float3 SH = IN.lightmapUVOrVertexSH.xyz;

				#endif



				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );

				#ifdef _ASE_BAKEDGI

					inputData.bakedGI = BakedGI;

				#endif



				BRDFData brdfData;

				InitializeBRDFData( Albedo, Metallic, Specular, Smoothness, Alpha, brdfData);

				half4 color;

				color.rgb = GlobalIllumination( brdfData, inputData.bakedGI, Occlusion, inputData.normalWS, inputData.viewDirectionWS);

				color.a = Alpha;



				#ifdef _TRANSMISSION_ASE

				{

					float shadow = _TransmissionShadow;

				

					Light mainLight = GetMainLight( inputData.shadowCoord );

					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;

					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );

					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;

					color.rgb += Albedo * mainTransmission;

				

					#ifdef _ADDITIONAL_LIGHTS

						int transPixelLightCount = GetAdditionalLightsCount();

						for (int i = 0; i < transPixelLightCount; ++i)

						{

							Light light = GetAdditionalLight(i, inputData.positionWS);

							float3 atten = light.color * light.distanceAttenuation;

							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

				

							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;

							color.rgb += Albedo * transmission;

						}

					#endif

				}

				#endif

				

				#ifdef _TRANSLUCENCY_ASE

				{

					float shadow = _TransShadow;

					float normal = _TransNormal;

					float scattering = _TransScattering;

					float direct = _TransDirect;

					float ambient = _TransAmbient;

					float strength = _TransStrength;

				

					Light mainLight = GetMainLight( inputData.shadowCoord );

					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;

					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );

				

					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;

					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );

					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;

					color.rgb += Albedo * mainTranslucency * strength;

				

					#ifdef _ADDITIONAL_LIGHTS

						int transPixelLightCount = GetAdditionalLightsCount();

						for (int i = 0; i < transPixelLightCount; ++i)

						{

							Light light = GetAdditionalLight(i, inputData.positionWS);

							float3 atten = light.color * light.distanceAttenuation;

							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

				

							half3 lightDir = light.direction + inputData.normalWS * normal;

							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );

							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;

							color.rgb += Albedo * translucency * strength;

						}

					#endif

				}

				#endif

				

				#ifdef _REFRACTION_ASE

					float4 projScreenPos = ScreenPos / ScreenPos.w;

					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal, 0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );

					projScreenPos.xy += refractionOffset.xy;

					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;

					color.rgb = lerp( refraction, color.rgb, color.a );

					color.a = 1;

				#endif

				

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY

					color.rgb *= color.a;

				#endif

				

				#ifdef ASE_FOG

					#ifdef TERRAIN_SPLAT_ADDPASS

						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );

					#else

						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);

					#endif

				#endif

				

				#ifdef ASE_DEPTH_WRITE_ON

					outputDepth = DepthValue;

				#endif

				

				return BRDFDataToGbuffer(brdfData, inputData, Smoothness, Emission + color.rgb);

			}



			ENDHLSL
		}
		
	}
	
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18935
309;326;2221;1688;1378.814;717.4525;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;233;-3416.064,-1881.969;Inherit;False;Property;_FoliageSize;Foliage Size;4;0;Create;True;0;0;0;False;0;False;100;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;235;-3240.15,-1876.368;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;234;-3105.964,-1883.67;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;122;-3225.14,916.776;Inherit;False;1580.315;688.4131;Comment;11;194;212;116;100;246;77;101;104;102;252;256;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;110;-3364.521,-2127.726;Inherit;False;Property;_GradientOffset;Gradient Offset;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-3366.52,-2044.726;Inherit;False;Property;_GradientFallout;Gradient Fallout;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;123;-2704.77,-2184.466;Inherit;True;Property;_FoliageColorMap;Foliage Color Map;1;2;[Header];[SingleLineTexture];Create;True;2;Foliage;.;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ColorNode;114;-3026.939,-2483.921;Inherit;False;Property;_FoliageColorBottom;Foliage Color Bottom;6;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0,0.7909455,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;131;-3076.101,-2118.791;Inherit;False;PA_SF_ObjectGradient;-1;;52;f7566061dd2a41c4bbc5f0e0ea7b5f5b;0;2;8;FLOAT;0;False;9;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;174;-2688.444,-1929.97;Inherit;True;Property;_FoliageNormalMap;Foliage Normal Map;2;2;[Normal];[SingleLineTexture];Create;True;0;0;0;False;0;False;None;None;False;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;236;-2948.401,-1905.561;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;102;-2988.226,1355.111;Inherit;False;Property;_WIndSwayFrequency;WInd Sway Frequency;44;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;104;-2975.226,1433.112;Inherit;False;Property;_WindSwayDirection;Wind Sway Direction;42;0;Create;True;0;0;0;False;0;False;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;101;-2974.226,1274.111;Inherit;False;Property;_WIndSwayIntensity;WInd Sway Intensity;43;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;252;-3038.711,1039.359;Inherit;False;Property;_WindOffsetIntensity;Wind Offset Intensity;38;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;194;-3040.178,955.6157;Inherit;False;Property;_WindScrollSpeed;Wind Scroll Speed;36;0;Create;True;0;0;0;False;0;False;0.05;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;256;-3037.416,1113.652;Inherit;False;Property;_WindRustleSize;Wind Rustle Size;39;0;Create;True;0;0;0;False;0;False;0.035;0;0;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;112;-3015.271,-2301.595;Inherit;False;Property;_FoliageColorTop;Foliage Color Top;5;1;[Header];Create;True;0;0;0;False;0;False;1,1,1,0;1,0,0.7909455,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;212;-3035.043,1195.038;Inherit;False;Property;_WindJitterSpeed;Wind Jitter Speed;37;0;Create;True;0;0;0;False;0;False;0.05;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;115;-2669.975,-2319.941;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;222;-2686.453,-1661.345;Inherit;False;Property;_FoliageEmissiveColor;Foliage Emissive Color;9;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;224;-2686.941,-1481.076;Inherit;False;Property;_FoliageEmissiveIntensity;Foliage Emissive Intensity;12;0;Create;True;0;0;0;False;0;False;0;0;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;124;-2448.591,-2185.154;Inherit;True;Property;_TextureSample2;Texture Sample 2;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;177;-2412.641,-1743.529;Inherit;False;Property;_FoliageNormalIntensity;Foliage Normal Intensity;10;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;257;-2726.183,1062.027;Inherit;False;PA_SF_WindRustleNoise;33;;87;7733c52bc6ce2e94b9c81cb72dee5854;0;4;18;FLOAT;0;False;33;FLOAT;1;False;35;FLOAT;0.035;False;19;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;246;-2715.526,1201.211;Inherit;False;PA_SF_WindSway;40;;83;bc8ec8a781a3c384e9042e29b2eae6d5;0;3;27;FLOAT;0;False;29;FLOAT;1;False;30;FLOAT2;1,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;175;-2432.265,-1930.658;Inherit;True;Property;_TextureSample3;Texture Sample 3;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;83;-3049.222,-875.8011;Inherit;False;1072.199;1674.577;Comment;12;95;94;93;92;91;90;89;88;87;86;85;84;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;92;-2919.356,540.1759;Inherit;False;Property;_EmissiveColor;Emissive Color;31;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;88;-2984.222,-173.8012;Inherit;False;Property;_NormalIntensity;Normal Intensity;27;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-2911.356,713.1755;Inherit;False;Property;_EmissiveIntensity;Emissive Intensity;32;0;Create;True;0;0;0;False;0;False;0;0;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;77;-2377.781,1061.776;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;95;-2918.222,-546.8016;Inherit;False;Property;_ColorTint;Color Tint;26;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;85;-2937.222,-372.8016;Inherit;True;Property;_NormalMap;Normal Map;22;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;None;89eef02d5852fe94d8e7e39680da95a4;True;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;125;-2104.469,-2320.945;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.UnpackScaleNormalNode;176;-2091.523,-1925.919;Inherit;False;Tangent;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;93;-2985.696,191.0175;Inherit;False;Property;_AOIntensity;AO Intensity;30;0;Create;True;0;0;0;False;0;False;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;89;-2938.222,-98.80125;Inherit;True;Property;_ORMMap;ORM Map;23;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;None;05a24179c8ae4bc428099e07a6d83a7f;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;91;-2982.374,268.7941;Inherit;False;Property;_MetallicIntensity;Metallic Intensity;29;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;223;-2381.069,-1656.414;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;84;-2941.222,-738.8015;Inherit;True;Property;_ColorMap;Color Map;21;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;None;6fa9c1ab592c6af4f8cadc03ccd976d1;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;86;-2980.222,106.1985;Inherit;False;Property;_RoughnessIntensity;Roughness Intensity;28;0;Create;True;0;0;0;False;0;False;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;94;-2936.834,349.4665;Inherit;True;Property;_EmissiveMap;Emissive Map;24;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;87;-2881.222,-826.8011;Inherit;False;Property;_TextureSize;Texture Size;25;0;Create;True;0;0;0;False;0;False;100;100;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;226;-1863.922,-1931.901;Inherit;False;vFoliageNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;100;-2201.238,1170.427;Inherit;False;Property;_TrunkMaterial;Trunk Material?;14;0;Create;True;0;0;0;False;1;Header(TRUNK);False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;82;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;173;-2071.848,-2087.301;Inherit;False;vFoliageOpacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;172;-1890.849,-2326.3;Inherit;False;vFoliageColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;255;-2387.722,-260.9431;Inherit;False;PA_SF_BasePBR_OpaqueORM;15;;88;a21dcaebf1379e5439b421c5da1cd710;0;12;20;FLOAT;100;False;9;SAMPLER2D;0;False;15;COLOR;0,0,0,0;False;22;SAMPLER2D;;False;28;FLOAT;1;False;59;SAMPLER2D;;False;38;FLOAT;1;False;73;FLOAT;0;False;39;FLOAT;0;False;52;SAMPLER2D;;False;53;COLOR;0,0,0,0;False;54;FLOAT;0;False;6;COLOR;0;FLOAT3;7;COLOR;6;FLOAT;5;FLOAT;3;FLOAT;8
Node;AmplifyShaderEditor.RangedFloatNode;220;-1830.553,34.33481;Inherit;False;Property;_FoliageRoughness;Foliage Roughness;11;0;Create;True;0;0;0;False;0;False;0.85;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;225;-2175.064,-1660.412;Inherit;False;vFoliageEmissive;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;229;-1722.231,-128.673;Inherit;False;225;vFoliageEmissive;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;187;-554.4885,146.7166;Inherit;False;Constant;_Float1;Float 1;34;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;227;-1586.681,-288.1677;Inherit;False;226;vFoliageNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;116;-1951.639,1170.127;Inherit;False;vWind;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;232;-1865.879,90.96267;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;181;-1580.058,-441.5723;Inherit;False;172;vFoliageColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;230;-1559.062,40.15799;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;237;-1831.389,-49.0491;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;183;-619.5778,71.2254;Inherit;False;173;vFoliageOpacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-706.6788,-1057.925;Inherit;False;Property;_MaskClipValue;Mask Clip Value;3;0;Create;True;0;0;0;False;0;False;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;97;-1388.557,-270.4312;Inherit;False;Property;_TrunkMaterial;Trunk Material?;14;0;Create;True;0;0;0;False;1;Header(TRUNK);False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;82;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;182;-369.5775,72.2254;Inherit;False;Property;_TrunkMaterial;Trunk Material?;14;0;Create;True;0;0;0;False;1;Header(TRUNK);False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;82;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;82;-1388.657,-440.5339;Inherit;False;Property;_UsedforTrunk;Used for Trunk?;14;0;Create;True;0;0;0;False;1;Header (Trunk);False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;221;-1527.62,162.4118;Inherit;False;Constant;_AO;AO;34;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;109;-704.7553,-965.1958;Inherit;False;Property;_Culling;Culling;0;1;[Enum];Create;True;0;3;Back;2;Front;1;Off;0;0;True;0;False;2;0;True;0;1;INT;0
Node;AmplifyShaderEditor.StaticSwitch;228;-1386.606,-89.33887;Inherit;False;Property;_TrunkMaterial;Trunk Material?;14;0;Create;True;0;0;0;False;1;Header(TRUNK);False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;82;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;99;-1387.107,162.772;Inherit;False;Property;_TrunkMaterial;Trunk Material?;13;0;Create;True;0;0;0;False;1;Header(TRUNK);False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;82;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;117;-304.8967,175.2606;Inherit;False;116;vWind;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;266;-307.8145,-256.4525;Inherit;False;Property;_AlphaClip;Alpha Clip;13;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;231;-1890.55,167.2511;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;98;-1389.094,36.90163;Inherit;False;Property;_TrunkMaterial;Trunk Material?;14;0;Create;True;0;0;0;False;1;Header(TRUNK);False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;82;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;258;53.61451,-423.2648;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;260;53.61451,-423.2648;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;262;53.61451,-423.2648;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;261;53.61451,-423.2648;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;263;53.61451,-423.2648;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=Universal2D;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;264;53.61451,-423.2648;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthNormals;0;6;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=DepthNormals;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;265;53.61451,-423.2648;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;GBuffer;0;7;GBuffer;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalGBuffer;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;259;53.61451,-423.2648;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;Polyart/Dreamscape/URP/Tree Wind;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;18;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;False;0;Hidden/InternalErrorShader;0;0;Standard;38;Workflow;1;0;Surface;0;0;  Refraction Model;0;0;  Blend;0;0;Two Sided;0;637931379247729126;Fragment Normal Space,InvertActionOnDeselection;0;0;Transmission;0;0;  Transmission Shadow;0.5,False,-1;0;Translucency;1;637931378795099614;  Translucency Strength;1,False,-1;0;  Normal Distortion;0.5,False,-1;0;  Scattering;2,False,-1;0;  Direct;0.9,False,-1;0;  Ambient;0.1,False,-1;0;  Shadow;0.5,False,-1;0;Cast Shadows;1;0;  Use Shadow Threshold;0;0;Receive Shadows;1;0;GPU Instancing;1;0;LOD CrossFade;1;0;Built-in Fog;1;0;_FinalColorxAlpha;0;0;Meta Pass;1;0;Override Baked GI;0;0;Extra Pre Pass;0;0;DOTS Instancing;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,-1;0;  Type;0;0;  Tess;16,False,-1;0;  Min;10,False,-1;0;  Max;25,False,-1;0;  Edge Length;16,False,-1;0;  Max Displacement;25,False,-1;0;Write Depth;0;0;  Early Z;0;0;Vertex Position,InvertActionOnDeselection;1;0;0;8;False;True;True;True;True;True;True;True;False;;False;0
WireConnection;235;0;233;0
WireConnection;234;0;235;0
WireConnection;234;1;235;0
WireConnection;131;8;110;0
WireConnection;131;9;111;0
WireConnection;236;0;234;0
WireConnection;115;0;114;0
WireConnection;115;1;112;0
WireConnection;115;2;131;0
WireConnection;124;0;123;0
WireConnection;124;1;236;0
WireConnection;257;18;194;0
WireConnection;257;33;252;0
WireConnection;257;35;256;0
WireConnection;257;19;212;0
WireConnection;246;27;101;0
WireConnection;246;29;102;0
WireConnection;246;30;104;0
WireConnection;175;0;174;0
WireConnection;175;1;236;0
WireConnection;77;0;257;0
WireConnection;77;1;246;0
WireConnection;125;0;115;0
WireConnection;125;1;124;0
WireConnection;176;0;175;0
WireConnection;176;1;177;0
WireConnection;223;0;222;0
WireConnection;223;1;224;0
WireConnection;226;0;176;0
WireConnection;100;1;77;0
WireConnection;100;0;246;0
WireConnection;173;0;124;4
WireConnection;172;0;125;0
WireConnection;255;20;87;0
WireConnection;255;9;84;0
WireConnection;255;15;95;0
WireConnection;255;22;85;0
WireConnection;255;28;88;0
WireConnection;255;59;89;0
WireConnection;255;38;86;0
WireConnection;255;73;93;0
WireConnection;255;39;91;0
WireConnection;255;52;94;0
WireConnection;255;53;92;0
WireConnection;255;54;90;0
WireConnection;225;0;223;0
WireConnection;116;0;100;0
WireConnection;232;0;255;5
WireConnection;230;0;220;0
WireConnection;237;0;255;6
WireConnection;97;1;227;0
WireConnection;97;0;255;7
WireConnection;182;1;183;0
WireConnection;182;0;187;0
WireConnection;82;1;181;0
WireConnection;82;0;255;0
WireConnection;228;1;229;0
WireConnection;228;0;237;0
WireConnection;99;1;221;0
WireConnection;99;0;231;0
WireConnection;231;0;255;8
WireConnection;98;1;230;0
WireConnection;98;0;232;0
WireConnection;259;0;82;0
WireConnection;259;1;97;0
WireConnection;259;2;228;0
WireConnection;259;4;98;0
WireConnection;259;6;182;0
WireConnection;259;7;266;0
WireConnection;259;8;117;0
ASEEND*/
//CHKSM=7982208D6494DAABCAF332B078497A4A3E2CEB68