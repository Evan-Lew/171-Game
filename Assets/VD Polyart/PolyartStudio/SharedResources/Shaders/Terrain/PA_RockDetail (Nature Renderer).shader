// Automatically patched for procedural instancing with Nature Renderer: https://visualdesigncafe.com/nature-renderer
// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Polyart/Dreamscape/URP/Rock Detail (Nature Renderer)"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin][Header(BASE COLOR)]_TextureSize("Texture Size", Float) = 100
		_ColorMap("Color Map", 2D) = "white" {}
		_ColorTint("Color Tint", Color) = (1,1,1,0)
		[Header(BASE PBR)]_NormalMap("Normal Map", 2D) = "white" {}
		_NormalIntensity("Normal Intensity", Range( 0 , 1)) = 1
		_ORMMap("ORM Map", 2D) = "white" {}
		_RoughnessIntensity("Roughness Intensity", Range( 0 , 2)) = 1
		_AOIntensity("AO Intensity", Range( 0 , 2)) = 1
		_MetallicIntensity("Metallic Intensity", Range( 0 , 1)) = 0
		[Header(DETAIL MAPPING)][Toggle(_USEDETAILMAPS_ON)] _UseDetailMaps("Use Detail Maps?", Float) = 0
		_DetailSize("Detail Size", Float) = 100
		_DetailColorMap("Detail Color Map", 2D) = "white" {}
		_DetailColorIntensity("Detail Color Intensity", Range( 0 , 1)) = 1
		_DetailNormalMap("Detail Normal Map", 2D) = "bump" {}
		_DetailNormalIntensity("Detail Normal Intensity", Range( 0 , 1)) = 1
		[Toggle(_USETRIPLANAR_ON)] _UseTriplanar("Use Triplanar?", Float) = 1
		[Header(COVERAGE)][Toggle(_ENABLECOVERAGE_ON)] _EnableCoverage("Enable Coverage?", Float) = 0
		_CoverageSize("Coverage Size", Float) = 100
		_CoverageColorMap("Coverage Color Map", 2D) = "white" {}
		_CoverageNormalMap("Coverage Normal Map", 2D) = "white" {}
		_CoverageNormalIntensity("Coverage Normal Intensity", Range( 0 , 1)) = 1
		_CoverageORMMap(" Coverage ORM Map", 2D) = "white" {}
		_CoverageRoughnessIntensity("Coverage Roughness Intensity", Range( 0 , 2)) = 1
		_CoverageMetallicIntensity("Coverage Metallic Intensity", Range( 0 , 1)) = 0
		_SlopeOffset("Slope Offset", Float) = 0
		_SlopeContrast("Slope Contrast", Float) = 1
		[KeywordEnum(VertexBlending,NormalBlending,NoiseBlending)] _EdgeBlending("Edge Blending", Float) = 0
		_SlopeNoiseTexture("Slope Noise Texture", 2D) = "white" {}
		[ASEEnd]_NoiseTextureSize("Noise Texture Size", Float) = 100

		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5
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
		Cull Back
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



			#define ASE_NEEDS_FRAG_WORLD_POSITION

			#define ASE_NEEDS_FRAG_WORLD_NORMAL

			#define ASE_NEEDS_FRAG_WORLD_TANGENT

			#define ASE_NEEDS_FRAG_WORLD_BITANGENT

			#pragma shader_feature_local _ENABLECOVERAGE_ON

			#pragma shader_feature_local _USEDETAILMAPS_ON

			#pragma shader_feature_local _USETRIPLANAR_ON

			#pragma shader_feature_local _EDGEBLENDING_VERTEXBLENDING _EDGEBLENDING_NORMALBLENDING _EDGEBLENDING_NOISEBLENDING
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

				float3 ase_normal : NORMAL;

				UNITY_VERTEX_INPUT_INSTANCE_ID

				UNITY_VERTEX_OUTPUT_STEREO

			};



			CBUFFER_START(UnityPerMaterial)

			float4 _ColorTint;

			float _TextureSize;

			float _DetailSize;

			float _DetailColorIntensity;

			float _CoverageSize;

			float _SlopeContrast;

			float _SlopeOffset;

			float _NormalIntensity;

			float _NoiseTextureSize;

			float _DetailNormalIntensity;

			float _CoverageNormalIntensity;

			float _MetallicIntensity;

			float _CoverageMetallicIntensity;

			float _RoughnessIntensity;

			float _CoverageRoughnessIntensity;

			float _AOIntensity;

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

			sampler2D _ColorMap;

			sampler2D _DetailColorMap;

			sampler2D _CoverageColorMap;

			sampler2D _NormalMap;

			sampler2D _SlopeNoiseTexture;

			sampler2D _DetailNormalMap;

			sampler2D _CoverageNormalMap;

			sampler2D _ORMMap;

			sampler2D _CoverageORMMap;





			inline float4 TriplanarSampling12_g285( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )

			{

				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );

				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;

				float3 nsign = sign( worldNormal );

				half4 xNorm; half4 yNorm; half4 zNorm;

				xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );

				yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );

				zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );

				return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;

			}

			

			inline float4 TriplanarSampling35_g289( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )

			{

				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );

				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;

				float3 nsign = sign( worldNormal );

				half4 xNorm; half4 yNorm; half4 zNorm;

				xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );

				yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );

				zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );

				return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;

			}

			

			inline float3 TriplanarSampling19_g285( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )

			{

				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );

				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;

				float3 nsign = sign( worldNormal );

				half4 xNorm; half4 yNorm; half4 zNorm;

				xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );

				yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );

				zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );

				xNorm.xyz  = half3( UnpackNormalScale( xNorm, normalScale.y ).xy * float2(  nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;

				yNorm.xyz  = half3( UnpackNormalScale( yNorm, normalScale.x ).xy * float2(  nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;

				zNorm.xyz  = half3( UnpackNormalScale( zNorm, normalScale.y ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;

				return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + zNorm.xyz * projNormal.z );

			}

			



			VertexOutput VertexFunction( VertexInput v  )

			{

				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);



				o.ase_texcoord7.xy = v.texcoord.xy;

				o.ase_normal = v.ase_normal;

				

				//setting value to unused interpolator channels and avoid initialization warnings

				o.ase_texcoord7.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					float3 defaultVertexValue = v.vertex.xyz;

				#else

					float3 defaultVertexValue = float3(0, 0, 0);

				#endif

				float3 vertexValue = defaultVertexValue;

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



				float2 texCoord18_g283 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );

				float2 temp_output_19_0_g283 = ( texCoord18_g283 / ( _TextureSize / 100.0 ) );

				float4 temp_output_32_0 = ( _ColorTint * tex2D( _ColorMap, temp_output_19_0_g283 ) );

				float temp_output_8_0_g285 = ( 100.0 / _DetailSize );

				float2 temp_cast_1 = (temp_output_8_0_g285).xx;

				float2 temp_cast_2 = (temp_output_8_0_g285).xx;

				float4 triplanar12_g285 = TriplanarSampling12_g285( _DetailColorMap, WorldPosition, WorldNormal, 1.0, temp_cast_2, 1.0, 0 );

				#ifdef _USETRIPLANAR_ON

				float4 staticSwitch11_g285 = triplanar12_g285;

				#else

				float4 staticSwitch11_g285 = tex2D( _DetailColorMap, temp_cast_1 );

				#endif

				float4 lerpResult16_g286 = lerp( temp_output_32_0 , staticSwitch11_g285 , _DetailColorIntensity);

				#ifdef _USEDETAILMAPS_ON

				float4 staticSwitch77 = lerpResult16_g286;

				#else

				float4 staticSwitch77 = temp_output_32_0;

				#endif

				float4 color13_g287 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);

				float2 texCoord18_g287 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );

				float2 temp_output_19_0_g287 = ( texCoord18_g287 / ( _CoverageSize / 100.0 ) );

				float temp_output_1_0_g294 = _SlopeContrast;

				float3 objToWorldDir51_g289 = mul( GetObjectToWorldMatrix(), float4( IN.ase_normal, 0 ) ).xyz;

				float dotResult18_g289 = dot( objToWorldDir51_g289 , float3(0,2,0) );

				float temp_output_21_0_g289 = ( ( _SlopeOffset + dotResult18_g289 ) * 2.0 );

				float clampResult22_g289 = clamp( temp_output_21_0_g289 , 0.0 , 1.0 );

				float lerpResult7_g294 = lerp( ( 0.0 - temp_output_1_0_g294 ) , ( temp_output_1_0_g294 + 0.0 ) , clampResult22_g289);

				float clampResult8_g294 = clamp( lerpResult7_g294 , 0.0 , 1.0 );

				float temp_output_1_0_g290 = _SlopeContrast;

				float3 unpack26_g283 = UnpackNormalScale( tex2D( _NormalMap, temp_output_19_0_g283 ), _NormalIntensity );

				unpack26_g283.z = lerp( 1, unpack26_g283.z, saturate(_NormalIntensity) );

				float3 temp_output_32_7 = unpack26_g283;

				float3x3 ase_tangentToWorldFast = float3x3(WorldTangent.x,WorldBiTangent.x,WorldNormal.x,WorldTangent.y,WorldBiTangent.y,WorldNormal.y,WorldTangent.z,WorldBiTangent.z,WorldNormal.z);

				float3 tangentToWorldDir6_g289 = mul( ase_tangentToWorldFast, temp_output_32_7 );

				float dotResult7_g289 = dot( tangentToWorldDir6_g289 , float3(0,1,0) );

				float lerpResult11_g289 = lerp( 0.0 , 3.0 , ( dotResult7_g289 + _SlopeOffset ));

				float clampResult12_g289 = clamp( lerpResult11_g289 , 0.0 , 1.0 );

				float lerpResult7_g290 = lerp( ( 0.0 - temp_output_1_0_g290 ) , ( temp_output_1_0_g290 + 0.0 ) , clampResult12_g289);

				float clampResult8_g290 = clamp( lerpResult7_g290 , 0.0 , 1.0 );

				float temp_output_1_0_g292 = _SlopeContrast;

				float2 temp_cast_7 = (_NoiseTextureSize).xx;

				float4 triplanar35_g289 = TriplanarSampling35_g289( _SlopeNoiseTexture, WorldPosition, WorldNormal, 1.0, temp_cast_7, 1.0, 0 );

				float clampResult42_g289 = clamp( (triplanar35_g289).y , 0.0 , 1.0 );

				float lerpResult7_g292 = lerp( ( 0.0 - temp_output_1_0_g292 ) , ( temp_output_1_0_g292 + 0.0 ) , ( temp_output_21_0_g289 + clampResult42_g289 ));

				float clampResult8_g292 = clamp( lerpResult7_g292 , 0.0 , 1.0 );

				float clampResult26_g289 = clamp( clampResult8_g292 , 0.0 , 1.0 );

				#if defined(_EDGEBLENDING_VERTEXBLENDING)

				float staticSwitch43_g289 = clampResult8_g294;

				#elif defined(_EDGEBLENDING_NORMALBLENDING)

				float staticSwitch43_g289 = clampResult8_g290;

				#elif defined(_EDGEBLENDING_NOISEBLENDING)

				float staticSwitch43_g289 = clampResult26_g289;

				#else

				float staticSwitch43_g289 = clampResult8_g294;

				#endif

				float temp_output_76_0 = staticSwitch43_g289;

				float4 lerpResult55 = lerp( staticSwitch77 , ( color13_g287 * tex2D( _CoverageColorMap, temp_output_19_0_g287 ) ) , temp_output_76_0);

				#ifdef _ENABLECOVERAGE_ON

				float4 staticSwitch57 = lerpResult55;

				#else

				float4 staticSwitch57 = staticSwitch77;

				#endif

				

				float2 temp_cast_9 = (temp_output_8_0_g285).xx;

				float temp_output_18_0_g285 = _DetailNormalIntensity;

				float3 unpack17_g285 = UnpackNormalScale( tex2D( _DetailNormalMap, temp_cast_9 ), temp_output_18_0_g285 );

				unpack17_g285.z = lerp( 1, unpack17_g285.z, saturate(temp_output_18_0_g285) );

				float2 temp_cast_11 = (temp_output_8_0_g285).xx;

				float3x3 ase_worldToTangent = float3x3(WorldTangent,WorldBiTangent,WorldNormal);

				float3 triplanar19_g285 = TriplanarSampling19_g285( _DetailNormalMap, WorldPosition, WorldNormal, 1.0, temp_cast_11, temp_output_18_0_g285, 0 );

				float3 tanTriplanarNormal19_g285 = mul( ase_worldToTangent, triplanar19_g285 );

				#ifdef _USETRIPLANAR_ON

				float3 staticSwitch16_g285 = tanTriplanarNormal19_g285;

				#else

				float3 staticSwitch16_g285 = unpack17_g285;

				#endif

				#ifdef _USEDETAILMAPS_ON

				float3 staticSwitch81 = BlendNormal( temp_output_32_7 , staticSwitch16_g285 );

				#else

				float3 staticSwitch81 = temp_output_32_7;

				#endif

				float3 unpack26_g287 = UnpackNormalScale( tex2D( _CoverageNormalMap, temp_output_19_0_g287 ), _CoverageNormalIntensity );

				unpack26_g287.z = lerp( 1, unpack26_g287.z, saturate(_CoverageNormalIntensity) );

				float3 lerpResult56 = lerp( staticSwitch81 , unpack26_g287 , temp_output_76_0);

				#ifdef _ENABLECOVERAGE_ON

				float3 staticSwitch58 = lerpResult56;

				#else

				float3 staticSwitch58 = staticSwitch81;

				#endif

				

				float4 tex2DNode9_g284 = tex2D( _ORMMap, temp_output_19_0_g283 );

				float temp_output_32_3 = ( tex2DNode9_g284.b * _MetallicIntensity );

				float4 tex2DNode9_g288 = tex2D( _CoverageORMMap, temp_output_19_0_g287 );

				float lerpResult63 = lerp( ( tex2DNode9_g288.b * _CoverageMetallicIntensity ) , temp_output_32_3 , temp_output_76_0);

				#ifdef _ENABLECOVERAGE_ON

				float staticSwitch60 = lerpResult63;

				#else

				float staticSwitch60 = temp_output_32_3;

				#endif

				

				float temp_output_32_5 = ( 1.0 - ( tex2DNode9_g284.g * _RoughnessIntensity ) );

				float lerpResult62 = lerp( temp_output_32_5 , ( 1.0 - ( tex2DNode9_g288.g * _CoverageRoughnessIntensity ) ) , temp_output_76_0);

				#ifdef _ENABLECOVERAGE_ON

				float staticSwitch59 = lerpResult62;

				#else

				float staticSwitch59 = temp_output_32_5;

				#endif

				

				float lerpResult10_g284 = lerp( 1.0 , tex2DNode9_g284.r , _AOIntensity);

				float temp_output_32_8 = lerpResult10_g284;

				float lerpResult10_g288 = lerp( 1.0 , tex2DNode9_g288.r , 0.0);

				float lerpResult64 = lerp( lerpResult10_g288 , temp_output_32_8 , temp_output_76_0);

				#ifdef _ENABLECOVERAGE_ON

				float staticSwitch61 = lerpResult64;

				#else

				float staticSwitch61 = temp_output_32_8;

				#endif

				

				float3 Albedo = staticSwitch57.rgb;

				float3 Normal = staticSwitch58;

				float3 Emission = 0;

				float3 Specular = 0.5;

				float Metallic = staticSwitch60;

				float Smoothness = staticSwitch59;

				float Occlusion = staticSwitch61;

				float Alpha = 1;

				float AlphaClipThreshold = 0.5;

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

			#define _NORMALMAP 1

			#define ASE_SRP_VERSION 999999



			

			#pragma vertex vert

			#pragma fragment frag
			#pragma instancing_options procedural:SetupNatureRenderer forwardadd renderinglayer

#if ASE_SRP_VERSION >= 110000

			#pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW

#endif

			#define SHADERPASS_SHADOWCASTER



			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Assets/Visual Design Cafe/Nature Renderer/Shader Includes/Nature Renderer.templatex"



			



			struct VertexInput

			{

				float4 vertex : POSITION;

				float3 ase_normal : NORMAL;

				

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

				

				UNITY_VERTEX_INPUT_INSTANCE_ID

				UNITY_VERTEX_OUTPUT_STEREO

			};



			CBUFFER_START(UnityPerMaterial)

			float4 _ColorTint;

			float _TextureSize;

			float _DetailSize;

			float _DetailColorIntensity;

			float _CoverageSize;

			float _SlopeContrast;

			float _SlopeOffset;

			float _NormalIntensity;

			float _NoiseTextureSize;

			float _DetailNormalIntensity;

			float _CoverageNormalIntensity;

			float _MetallicIntensity;

			float _CoverageMetallicIntensity;

			float _RoughnessIntensity;

			float _CoverageRoughnessIntensity;

			float _AOIntensity;

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



				

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					float3 defaultVertexValue = v.vertex.xyz;

				#else

					float3 defaultVertexValue = float3(0, 0, 0);

				#endif

				float3 vertexValue = defaultVertexValue;

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



				

				float Alpha = 1;

				float AlphaClipThreshold = 0.5;

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

			#define _NORMALMAP 1

			#define ASE_SRP_VERSION 999999



			

			#pragma vertex vert

			#pragma fragment frag
			#pragma instancing_options procedural:SetupNatureRenderer forwardadd renderinglayer



			#define SHADERPASS_DEPTHONLY



			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Assets/Visual Design Cafe/Nature Renderer/Shader Includes/Nature Renderer.templatex"



			



			struct VertexInput

			{

				float4 vertex : POSITION;

				float3 ase_normal : NORMAL;

				

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

				

				UNITY_VERTEX_INPUT_INSTANCE_ID

				UNITY_VERTEX_OUTPUT_STEREO

			};



			CBUFFER_START(UnityPerMaterial)

			float4 _ColorTint;

			float _TextureSize;

			float _DetailSize;

			float _DetailColorIntensity;

			float _CoverageSize;

			float _SlopeContrast;

			float _SlopeOffset;

			float _NormalIntensity;

			float _NoiseTextureSize;

			float _DetailNormalIntensity;

			float _CoverageNormalIntensity;

			float _MetallicIntensity;

			float _CoverageMetallicIntensity;

			float _RoughnessIntensity;

			float _CoverageRoughnessIntensity;

			float _AOIntensity;

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

			



			

			VertexOutput VertexFunction( VertexInput v  )

			{

				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);



				

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					float3 defaultVertexValue = v.vertex.xyz;

				#else

					float3 defaultVertexValue = float3(0, 0, 0);

				#endif

				float3 vertexValue = defaultVertexValue;

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



				

				float Alpha = 1;

				float AlphaClipThreshold = 0.5;

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

			#define _NORMALMAP 1

			#define ASE_SRP_VERSION 999999



			

			#pragma vertex vert

			#pragma fragment frag



			#define SHADERPASS_META



			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"



			#define ASE_NEEDS_FRAG_WORLD_POSITION

			#define ASE_NEEDS_VERT_NORMAL

			#pragma shader_feature_local _ENABLECOVERAGE_ON

			#pragma shader_feature_local _USEDETAILMAPS_ON

			#pragma shader_feature_local _USETRIPLANAR_ON

			#pragma shader_feature_local _EDGEBLENDING_VERTEXBLENDING _EDGEBLENDING_NORMALBLENDING _EDGEBLENDING_NOISEBLENDING





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

				float4 ase_tangent : TANGENT;

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

				float3 ase_normal : NORMAL;

				float4 ase_texcoord4 : TEXCOORD4;

				float4 ase_texcoord5 : TEXCOORD5;

				UNITY_VERTEX_INPUT_INSTANCE_ID

				UNITY_VERTEX_OUTPUT_STEREO

			};



			CBUFFER_START(UnityPerMaterial)

			float4 _ColorTint;

			float _TextureSize;

			float _DetailSize;

			float _DetailColorIntensity;

			float _CoverageSize;

			float _SlopeContrast;

			float _SlopeOffset;

			float _NormalIntensity;

			float _NoiseTextureSize;

			float _DetailNormalIntensity;

			float _CoverageNormalIntensity;

			float _MetallicIntensity;

			float _CoverageMetallicIntensity;

			float _RoughnessIntensity;

			float _CoverageRoughnessIntensity;

			float _AOIntensity;

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

			sampler2D _ColorMap;

			sampler2D _DetailColorMap;

			sampler2D _CoverageColorMap;

			sampler2D _NormalMap;

			sampler2D _SlopeNoiseTexture;





			inline float4 TriplanarSampling12_g285( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )

			{

				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );

				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;

				float3 nsign = sign( worldNormal );

				half4 xNorm; half4 yNorm; half4 zNorm;

				xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );

				yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );

				zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );

				return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;

			}

			

			inline float4 TriplanarSampling35_g289( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )

			{

				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );

				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;

				float3 nsign = sign( worldNormal );

				half4 xNorm; half4 yNorm; half4 zNorm;

				xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );

				yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );

				zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );

				return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;

			}

			



			VertexOutput VertexFunction( VertexInput v  )

			{

				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);



				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);

				o.ase_texcoord3.xyz = ase_worldNormal;

				float3 ase_worldTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);

				o.ase_texcoord4.xyz = ase_worldTangent;

				float ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;

				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;

				o.ase_texcoord5.xyz = ase_worldBitangent;

				

				o.ase_texcoord2.xy = v.ase_texcoord.xy;

				o.ase_normal = v.ase_normal;

				

				//setting value to unused interpolator channels and avoid initialization warnings

				o.ase_texcoord2.zw = 0;

				o.ase_texcoord3.w = 0;

				o.ase_texcoord4.w = 0;

				o.ase_texcoord5.w = 0;

				

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					float3 defaultVertexValue = v.vertex.xyz;

				#else

					float3 defaultVertexValue = float3(0, 0, 0);

				#endif

				float3 vertexValue = defaultVertexValue;

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

				float4 ase_tangent : TANGENT;



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

				o.ase_tangent = v.ase_tangent;

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

				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;

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



				float2 texCoord18_g283 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );

				float2 temp_output_19_0_g283 = ( texCoord18_g283 / ( _TextureSize / 100.0 ) );

				float4 temp_output_32_0 = ( _ColorTint * tex2D( _ColorMap, temp_output_19_0_g283 ) );

				float temp_output_8_0_g285 = ( 100.0 / _DetailSize );

				float2 temp_cast_1 = (temp_output_8_0_g285).xx;

				float2 temp_cast_2 = (temp_output_8_0_g285).xx;

				float3 ase_worldNormal = IN.ase_texcoord3.xyz;

				float4 triplanar12_g285 = TriplanarSampling12_g285( _DetailColorMap, WorldPosition, ase_worldNormal, 1.0, temp_cast_2, 1.0, 0 );

				#ifdef _USETRIPLANAR_ON

				float4 staticSwitch11_g285 = triplanar12_g285;

				#else

				float4 staticSwitch11_g285 = tex2D( _DetailColorMap, temp_cast_1 );

				#endif

				float4 lerpResult16_g286 = lerp( temp_output_32_0 , staticSwitch11_g285 , _DetailColorIntensity);

				#ifdef _USEDETAILMAPS_ON

				float4 staticSwitch77 = lerpResult16_g286;

				#else

				float4 staticSwitch77 = temp_output_32_0;

				#endif

				float4 color13_g287 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);

				float2 texCoord18_g287 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );

				float2 temp_output_19_0_g287 = ( texCoord18_g287 / ( _CoverageSize / 100.0 ) );

				float temp_output_1_0_g294 = _SlopeContrast;

				float3 objToWorldDir51_g289 = mul( GetObjectToWorldMatrix(), float4( IN.ase_normal, 0 ) ).xyz;

				float dotResult18_g289 = dot( objToWorldDir51_g289 , float3(0,2,0) );

				float temp_output_21_0_g289 = ( ( _SlopeOffset + dotResult18_g289 ) * 2.0 );

				float clampResult22_g289 = clamp( temp_output_21_0_g289 , 0.0 , 1.0 );

				float lerpResult7_g294 = lerp( ( 0.0 - temp_output_1_0_g294 ) , ( temp_output_1_0_g294 + 0.0 ) , clampResult22_g289);

				float clampResult8_g294 = clamp( lerpResult7_g294 , 0.0 , 1.0 );

				float temp_output_1_0_g290 = _SlopeContrast;

				float3 unpack26_g283 = UnpackNormalScale( tex2D( _NormalMap, temp_output_19_0_g283 ), _NormalIntensity );

				unpack26_g283.z = lerp( 1, unpack26_g283.z, saturate(_NormalIntensity) );

				float3 temp_output_32_7 = unpack26_g283;

				float3 ase_worldTangent = IN.ase_texcoord4.xyz;

				float3 ase_worldBitangent = IN.ase_texcoord5.xyz;

				float3x3 ase_tangentToWorldFast = float3x3(ase_worldTangent.x,ase_worldBitangent.x,ase_worldNormal.x,ase_worldTangent.y,ase_worldBitangent.y,ase_worldNormal.y,ase_worldTangent.z,ase_worldBitangent.z,ase_worldNormal.z);

				float3 tangentToWorldDir6_g289 = mul( ase_tangentToWorldFast, temp_output_32_7 );

				float dotResult7_g289 = dot( tangentToWorldDir6_g289 , float3(0,1,0) );

				float lerpResult11_g289 = lerp( 0.0 , 3.0 , ( dotResult7_g289 + _SlopeOffset ));

				float clampResult12_g289 = clamp( lerpResult11_g289 , 0.0 , 1.0 );

				float lerpResult7_g290 = lerp( ( 0.0 - temp_output_1_0_g290 ) , ( temp_output_1_0_g290 + 0.0 ) , clampResult12_g289);

				float clampResult8_g290 = clamp( lerpResult7_g290 , 0.0 , 1.0 );

				float temp_output_1_0_g292 = _SlopeContrast;

				float2 temp_cast_7 = (_NoiseTextureSize).xx;

				float4 triplanar35_g289 = TriplanarSampling35_g289( _SlopeNoiseTexture, WorldPosition, ase_worldNormal, 1.0, temp_cast_7, 1.0, 0 );

				float clampResult42_g289 = clamp( (triplanar35_g289).y , 0.0 , 1.0 );

				float lerpResult7_g292 = lerp( ( 0.0 - temp_output_1_0_g292 ) , ( temp_output_1_0_g292 + 0.0 ) , ( temp_output_21_0_g289 + clampResult42_g289 ));

				float clampResult8_g292 = clamp( lerpResult7_g292 , 0.0 , 1.0 );

				float clampResult26_g289 = clamp( clampResult8_g292 , 0.0 , 1.0 );

				#if defined(_EDGEBLENDING_VERTEXBLENDING)

				float staticSwitch43_g289 = clampResult8_g294;

				#elif defined(_EDGEBLENDING_NORMALBLENDING)

				float staticSwitch43_g289 = clampResult8_g290;

				#elif defined(_EDGEBLENDING_NOISEBLENDING)

				float staticSwitch43_g289 = clampResult26_g289;

				#else

				float staticSwitch43_g289 = clampResult8_g294;

				#endif

				float temp_output_76_0 = staticSwitch43_g289;

				float4 lerpResult55 = lerp( staticSwitch77 , ( color13_g287 * tex2D( _CoverageColorMap, temp_output_19_0_g287 ) ) , temp_output_76_0);

				#ifdef _ENABLECOVERAGE_ON

				float4 staticSwitch57 = lerpResult55;

				#else

				float4 staticSwitch57 = staticSwitch77;

				#endif

				

				

				float3 Albedo = staticSwitch57.rgb;

				float3 Emission = 0;

				float Alpha = 1;

				float AlphaClipThreshold = 0.5;



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

			

			#define ASE_NEEDS_FRAG_WORLD_POSITION

			#define ASE_NEEDS_VERT_NORMAL

			#pragma shader_feature_local _ENABLECOVERAGE_ON

			#pragma shader_feature_local _USEDETAILMAPS_ON

			#pragma shader_feature_local _USETRIPLANAR_ON

			#pragma shader_feature_local _EDGEBLENDING_VERTEXBLENDING _EDGEBLENDING_NORMALBLENDING _EDGEBLENDING_NOISEBLENDING





			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#include "Assets/Visual Design Cafe/Nature Renderer/Shader Includes/Nature Renderer.templatex"
			#pragma instancing_options procedural:SetupNatureRenderer forwardadd renderinglayer



			struct VertexInput

			{

				float4 vertex : POSITION;

				float3 ase_normal : NORMAL;

				float4 ase_texcoord : TEXCOORD0;

				float4 ase_tangent : TANGENT;

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

				float3 ase_normal : NORMAL;

				float4 ase_texcoord4 : TEXCOORD4;

				float4 ase_texcoord5 : TEXCOORD5;

				UNITY_VERTEX_INPUT_INSTANCE_ID

				UNITY_VERTEX_OUTPUT_STEREO

			};



			CBUFFER_START(UnityPerMaterial)

			float4 _ColorTint;

			float _TextureSize;

			float _DetailSize;

			float _DetailColorIntensity;

			float _CoverageSize;

			float _SlopeContrast;

			float _SlopeOffset;

			float _NormalIntensity;

			float _NoiseTextureSize;

			float _DetailNormalIntensity;

			float _CoverageNormalIntensity;

			float _MetallicIntensity;

			float _CoverageMetallicIntensity;

			float _RoughnessIntensity;

			float _CoverageRoughnessIntensity;

			float _AOIntensity;

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

			sampler2D _ColorMap;

			sampler2D _DetailColorMap;

			sampler2D _CoverageColorMap;

			sampler2D _NormalMap;

			sampler2D _SlopeNoiseTexture;





			inline float4 TriplanarSampling12_g285( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )

			{

				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );

				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;

				float3 nsign = sign( worldNormal );

				half4 xNorm; half4 yNorm; half4 zNorm;

				xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );

				yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );

				zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );

				return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;

			}

			

			inline float4 TriplanarSampling35_g289( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )

			{

				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );

				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;

				float3 nsign = sign( worldNormal );

				half4 xNorm; half4 yNorm; half4 zNorm;

				xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );

				yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );

				zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );

				return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;

			}

			



			VertexOutput VertexFunction( VertexInput v  )

			{

				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID( v );

				UNITY_TRANSFER_INSTANCE_ID( v, o );

				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );



				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);

				o.ase_texcoord3.xyz = ase_worldNormal;

				float3 ase_worldTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);

				o.ase_texcoord4.xyz = ase_worldTangent;

				float ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;

				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;

				o.ase_texcoord5.xyz = ase_worldBitangent;

				

				o.ase_texcoord2.xy = v.ase_texcoord.xy;

				o.ase_normal = v.ase_normal;

				

				//setting value to unused interpolator channels and avoid initialization warnings

				o.ase_texcoord2.zw = 0;

				o.ase_texcoord3.w = 0;

				o.ase_texcoord4.w = 0;

				o.ase_texcoord5.w = 0;

				

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					float3 defaultVertexValue = v.vertex.xyz;

				#else

					float3 defaultVertexValue = float3(0, 0, 0);

				#endif

				float3 vertexValue = defaultVertexValue;

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

				float4 ase_tangent : TANGENT;



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

				o.ase_tangent = v.ase_tangent;

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

				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;

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



				float2 texCoord18_g283 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );

				float2 temp_output_19_0_g283 = ( texCoord18_g283 / ( _TextureSize / 100.0 ) );

				float4 temp_output_32_0 = ( _ColorTint * tex2D( _ColorMap, temp_output_19_0_g283 ) );

				float temp_output_8_0_g285 = ( 100.0 / _DetailSize );

				float2 temp_cast_1 = (temp_output_8_0_g285).xx;

				float2 temp_cast_2 = (temp_output_8_0_g285).xx;

				float3 ase_worldNormal = IN.ase_texcoord3.xyz;

				float4 triplanar12_g285 = TriplanarSampling12_g285( _DetailColorMap, WorldPosition, ase_worldNormal, 1.0, temp_cast_2, 1.0, 0 );

				#ifdef _USETRIPLANAR_ON

				float4 staticSwitch11_g285 = triplanar12_g285;

				#else

				float4 staticSwitch11_g285 = tex2D( _DetailColorMap, temp_cast_1 );

				#endif

				float4 lerpResult16_g286 = lerp( temp_output_32_0 , staticSwitch11_g285 , _DetailColorIntensity);

				#ifdef _USEDETAILMAPS_ON

				float4 staticSwitch77 = lerpResult16_g286;

				#else

				float4 staticSwitch77 = temp_output_32_0;

				#endif

				float4 color13_g287 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);

				float2 texCoord18_g287 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );

				float2 temp_output_19_0_g287 = ( texCoord18_g287 / ( _CoverageSize / 100.0 ) );

				float temp_output_1_0_g294 = _SlopeContrast;

				float3 objToWorldDir51_g289 = mul( GetObjectToWorldMatrix(), float4( IN.ase_normal, 0 ) ).xyz;

				float dotResult18_g289 = dot( objToWorldDir51_g289 , float3(0,2,0) );

				float temp_output_21_0_g289 = ( ( _SlopeOffset + dotResult18_g289 ) * 2.0 );

				float clampResult22_g289 = clamp( temp_output_21_0_g289 , 0.0 , 1.0 );

				float lerpResult7_g294 = lerp( ( 0.0 - temp_output_1_0_g294 ) , ( temp_output_1_0_g294 + 0.0 ) , clampResult22_g289);

				float clampResult8_g294 = clamp( lerpResult7_g294 , 0.0 , 1.0 );

				float temp_output_1_0_g290 = _SlopeContrast;

				float3 unpack26_g283 = UnpackNormalScale( tex2D( _NormalMap, temp_output_19_0_g283 ), _NormalIntensity );

				unpack26_g283.z = lerp( 1, unpack26_g283.z, saturate(_NormalIntensity) );

				float3 temp_output_32_7 = unpack26_g283;

				float3 ase_worldTangent = IN.ase_texcoord4.xyz;

				float3 ase_worldBitangent = IN.ase_texcoord5.xyz;

				float3x3 ase_tangentToWorldFast = float3x3(ase_worldTangent.x,ase_worldBitangent.x,ase_worldNormal.x,ase_worldTangent.y,ase_worldBitangent.y,ase_worldNormal.y,ase_worldTangent.z,ase_worldBitangent.z,ase_worldNormal.z);

				float3 tangentToWorldDir6_g289 = mul( ase_tangentToWorldFast, temp_output_32_7 );

				float dotResult7_g289 = dot( tangentToWorldDir6_g289 , float3(0,1,0) );

				float lerpResult11_g289 = lerp( 0.0 , 3.0 , ( dotResult7_g289 + _SlopeOffset ));

				float clampResult12_g289 = clamp( lerpResult11_g289 , 0.0 , 1.0 );

				float lerpResult7_g290 = lerp( ( 0.0 - temp_output_1_0_g290 ) , ( temp_output_1_0_g290 + 0.0 ) , clampResult12_g289);

				float clampResult8_g290 = clamp( lerpResult7_g290 , 0.0 , 1.0 );

				float temp_output_1_0_g292 = _SlopeContrast;

				float2 temp_cast_7 = (_NoiseTextureSize).xx;

				float4 triplanar35_g289 = TriplanarSampling35_g289( _SlopeNoiseTexture, WorldPosition, ase_worldNormal, 1.0, temp_cast_7, 1.0, 0 );

				float clampResult42_g289 = clamp( (triplanar35_g289).y , 0.0 , 1.0 );

				float lerpResult7_g292 = lerp( ( 0.0 - temp_output_1_0_g292 ) , ( temp_output_1_0_g292 + 0.0 ) , ( temp_output_21_0_g289 + clampResult42_g289 ));

				float clampResult8_g292 = clamp( lerpResult7_g292 , 0.0 , 1.0 );

				float clampResult26_g289 = clamp( clampResult8_g292 , 0.0 , 1.0 );

				#if defined(_EDGEBLENDING_VERTEXBLENDING)

				float staticSwitch43_g289 = clampResult8_g294;

				#elif defined(_EDGEBLENDING_NORMALBLENDING)

				float staticSwitch43_g289 = clampResult8_g290;

				#elif defined(_EDGEBLENDING_NOISEBLENDING)

				float staticSwitch43_g289 = clampResult26_g289;

				#else

				float staticSwitch43_g289 = clampResult8_g294;

				#endif

				float temp_output_76_0 = staticSwitch43_g289;

				float4 lerpResult55 = lerp( staticSwitch77 , ( color13_g287 * tex2D( _CoverageColorMap, temp_output_19_0_g287 ) ) , temp_output_76_0);

				#ifdef _ENABLECOVERAGE_ON

				float4 staticSwitch57 = lerpResult55;

				#else

				float4 staticSwitch57 = staticSwitch77;

				#endif

				

				

				float3 Albedo = staticSwitch57.rgb;

				float Alpha = 1;

				float AlphaClipThreshold = 0.5;



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

			#define _NORMALMAP 1

			#define ASE_SRP_VERSION 999999



			

			#pragma vertex vert

			#pragma fragment frag
			#pragma instancing_options procedural:SetupNatureRenderer forwardadd renderinglayer



			#define SHADERPASS_DEPTHNORMALSONLY



			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Assets/Visual Design Cafe/Nature Renderer/Shader Includes/Nature Renderer.templatex"



			



			struct VertexInput

			{

				float4 vertex : POSITION;

				float3 ase_normal : NORMAL;

				

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

				

				UNITY_VERTEX_INPUT_INSTANCE_ID

				UNITY_VERTEX_OUTPUT_STEREO

			};



			CBUFFER_START(UnityPerMaterial)

			float4 _ColorTint;

			float _TextureSize;

			float _DetailSize;

			float _DetailColorIntensity;

			float _CoverageSize;

			float _SlopeContrast;

			float _SlopeOffset;

			float _NormalIntensity;

			float _NoiseTextureSize;

			float _DetailNormalIntensity;

			float _CoverageNormalIntensity;

			float _MetallicIntensity;

			float _CoverageMetallicIntensity;

			float _RoughnessIntensity;

			float _CoverageRoughnessIntensity;

			float _AOIntensity;

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

			



			

			VertexOutput VertexFunction( VertexInput v  )

			{

				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);



				

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					float3 defaultVertexValue = v.vertex.xyz;

				#else

					float3 defaultVertexValue = float3(0, 0, 0);

				#endif

				float3 vertexValue = defaultVertexValue;

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



				

				float Alpha = 1;

				float AlphaClipThreshold = 0.5;

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



			#define ASE_NEEDS_FRAG_WORLD_POSITION

			#define ASE_NEEDS_FRAG_WORLD_NORMAL

			#define ASE_NEEDS_FRAG_WORLD_TANGENT

			#define ASE_NEEDS_FRAG_WORLD_BITANGENT

			#pragma shader_feature_local _ENABLECOVERAGE_ON

			#pragma shader_feature_local _USEDETAILMAPS_ON

			#pragma shader_feature_local _USETRIPLANAR_ON

			#pragma shader_feature_local _EDGEBLENDING_VERTEXBLENDING _EDGEBLENDING_NORMALBLENDING _EDGEBLENDING_NOISEBLENDING
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

				float3 ase_normal : NORMAL;

				UNITY_VERTEX_INPUT_INSTANCE_ID

				UNITY_VERTEX_OUTPUT_STEREO

			};



			CBUFFER_START(UnityPerMaterial)

			float4 _ColorTint;

			float _TextureSize;

			float _DetailSize;

			float _DetailColorIntensity;

			float _CoverageSize;

			float _SlopeContrast;

			float _SlopeOffset;

			float _NormalIntensity;

			float _NoiseTextureSize;

			float _DetailNormalIntensity;

			float _CoverageNormalIntensity;

			float _MetallicIntensity;

			float _CoverageMetallicIntensity;

			float _RoughnessIntensity;

			float _CoverageRoughnessIntensity;

			float _AOIntensity;

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

			sampler2D _ColorMap;

			sampler2D _DetailColorMap;

			sampler2D _CoverageColorMap;

			sampler2D _NormalMap;

			sampler2D _SlopeNoiseTexture;

			sampler2D _DetailNormalMap;

			sampler2D _CoverageNormalMap;

			sampler2D _ORMMap;

			sampler2D _CoverageORMMap;





			inline float4 TriplanarSampling12_g285( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )

			{

				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );

				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;

				float3 nsign = sign( worldNormal );

				half4 xNorm; half4 yNorm; half4 zNorm;

				xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );

				yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );

				zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );

				return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;

			}

			

			inline float4 TriplanarSampling35_g289( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )

			{

				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );

				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;

				float3 nsign = sign( worldNormal );

				half4 xNorm; half4 yNorm; half4 zNorm;

				xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );

				yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );

				zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );

				return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;

			}

			

			inline float3 TriplanarSampling19_g285( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )

			{

				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );

				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;

				float3 nsign = sign( worldNormal );

				half4 xNorm; half4 yNorm; half4 zNorm;

				xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );

				yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );

				zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );

				xNorm.xyz  = half3( UnpackNormalScale( xNorm, normalScale.y ).xy * float2(  nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;

				yNorm.xyz  = half3( UnpackNormalScale( yNorm, normalScale.x ).xy * float2(  nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;

				zNorm.xyz  = half3( UnpackNormalScale( zNorm, normalScale.y ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;

				return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + zNorm.xyz * projNormal.z );

			}

			



			VertexOutput VertexFunction( VertexInput v  )

			{

				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID(v);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);



				o.ase_texcoord7.xy = v.texcoord.xy;

				o.ase_normal = v.ase_normal;

				

				//setting value to unused interpolator channels and avoid initialization warnings

				o.ase_texcoord7.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS

					float3 defaultVertexValue = v.vertex.xyz;

				#else

					float3 defaultVertexValue = float3(0, 0, 0);

				#endif

				float3 vertexValue = defaultVertexValue;

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



				float2 texCoord18_g283 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );

				float2 temp_output_19_0_g283 = ( texCoord18_g283 / ( _TextureSize / 100.0 ) );

				float4 temp_output_32_0 = ( _ColorTint * tex2D( _ColorMap, temp_output_19_0_g283 ) );

				float temp_output_8_0_g285 = ( 100.0 / _DetailSize );

				float2 temp_cast_1 = (temp_output_8_0_g285).xx;

				float2 temp_cast_2 = (temp_output_8_0_g285).xx;

				float4 triplanar12_g285 = TriplanarSampling12_g285( _DetailColorMap, WorldPosition, WorldNormal, 1.0, temp_cast_2, 1.0, 0 );

				#ifdef _USETRIPLANAR_ON

				float4 staticSwitch11_g285 = triplanar12_g285;

				#else

				float4 staticSwitch11_g285 = tex2D( _DetailColorMap, temp_cast_1 );

				#endif

				float4 lerpResult16_g286 = lerp( temp_output_32_0 , staticSwitch11_g285 , _DetailColorIntensity);

				#ifdef _USEDETAILMAPS_ON

				float4 staticSwitch77 = lerpResult16_g286;

				#else

				float4 staticSwitch77 = temp_output_32_0;

				#endif

				float4 color13_g287 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);

				float2 texCoord18_g287 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );

				float2 temp_output_19_0_g287 = ( texCoord18_g287 / ( _CoverageSize / 100.0 ) );

				float temp_output_1_0_g294 = _SlopeContrast;

				float3 objToWorldDir51_g289 = mul( GetObjectToWorldMatrix(), float4( IN.ase_normal, 0 ) ).xyz;

				float dotResult18_g289 = dot( objToWorldDir51_g289 , float3(0,2,0) );

				float temp_output_21_0_g289 = ( ( _SlopeOffset + dotResult18_g289 ) * 2.0 );

				float clampResult22_g289 = clamp( temp_output_21_0_g289 , 0.0 , 1.0 );

				float lerpResult7_g294 = lerp( ( 0.0 - temp_output_1_0_g294 ) , ( temp_output_1_0_g294 + 0.0 ) , clampResult22_g289);

				float clampResult8_g294 = clamp( lerpResult7_g294 , 0.0 , 1.0 );

				float temp_output_1_0_g290 = _SlopeContrast;

				float3 unpack26_g283 = UnpackNormalScale( tex2D( _NormalMap, temp_output_19_0_g283 ), _NormalIntensity );

				unpack26_g283.z = lerp( 1, unpack26_g283.z, saturate(_NormalIntensity) );

				float3 temp_output_32_7 = unpack26_g283;

				float3x3 ase_tangentToWorldFast = float3x3(WorldTangent.x,WorldBiTangent.x,WorldNormal.x,WorldTangent.y,WorldBiTangent.y,WorldNormal.y,WorldTangent.z,WorldBiTangent.z,WorldNormal.z);

				float3 tangentToWorldDir6_g289 = mul( ase_tangentToWorldFast, temp_output_32_7 );

				float dotResult7_g289 = dot( tangentToWorldDir6_g289 , float3(0,1,0) );

				float lerpResult11_g289 = lerp( 0.0 , 3.0 , ( dotResult7_g289 + _SlopeOffset ));

				float clampResult12_g289 = clamp( lerpResult11_g289 , 0.0 , 1.0 );

				float lerpResult7_g290 = lerp( ( 0.0 - temp_output_1_0_g290 ) , ( temp_output_1_0_g290 + 0.0 ) , clampResult12_g289);

				float clampResult8_g290 = clamp( lerpResult7_g290 , 0.0 , 1.0 );

				float temp_output_1_0_g292 = _SlopeContrast;

				float2 temp_cast_7 = (_NoiseTextureSize).xx;

				float4 triplanar35_g289 = TriplanarSampling35_g289( _SlopeNoiseTexture, WorldPosition, WorldNormal, 1.0, temp_cast_7, 1.0, 0 );

				float clampResult42_g289 = clamp( (triplanar35_g289).y , 0.0 , 1.0 );

				float lerpResult7_g292 = lerp( ( 0.0 - temp_output_1_0_g292 ) , ( temp_output_1_0_g292 + 0.0 ) , ( temp_output_21_0_g289 + clampResult42_g289 ));

				float clampResult8_g292 = clamp( lerpResult7_g292 , 0.0 , 1.0 );

				float clampResult26_g289 = clamp( clampResult8_g292 , 0.0 , 1.0 );

				#if defined(_EDGEBLENDING_VERTEXBLENDING)

				float staticSwitch43_g289 = clampResult8_g294;

				#elif defined(_EDGEBLENDING_NORMALBLENDING)

				float staticSwitch43_g289 = clampResult8_g290;

				#elif defined(_EDGEBLENDING_NOISEBLENDING)

				float staticSwitch43_g289 = clampResult26_g289;

				#else

				float staticSwitch43_g289 = clampResult8_g294;

				#endif

				float temp_output_76_0 = staticSwitch43_g289;

				float4 lerpResult55 = lerp( staticSwitch77 , ( color13_g287 * tex2D( _CoverageColorMap, temp_output_19_0_g287 ) ) , temp_output_76_0);

				#ifdef _ENABLECOVERAGE_ON

				float4 staticSwitch57 = lerpResult55;

				#else

				float4 staticSwitch57 = staticSwitch77;

				#endif

				

				float2 temp_cast_9 = (temp_output_8_0_g285).xx;

				float temp_output_18_0_g285 = _DetailNormalIntensity;

				float3 unpack17_g285 = UnpackNormalScale( tex2D( _DetailNormalMap, temp_cast_9 ), temp_output_18_0_g285 );

				unpack17_g285.z = lerp( 1, unpack17_g285.z, saturate(temp_output_18_0_g285) );

				float2 temp_cast_11 = (temp_output_8_0_g285).xx;

				float3x3 ase_worldToTangent = float3x3(WorldTangent,WorldBiTangent,WorldNormal);

				float3 triplanar19_g285 = TriplanarSampling19_g285( _DetailNormalMap, WorldPosition, WorldNormal, 1.0, temp_cast_11, temp_output_18_0_g285, 0 );

				float3 tanTriplanarNormal19_g285 = mul( ase_worldToTangent, triplanar19_g285 );

				#ifdef _USETRIPLANAR_ON

				float3 staticSwitch16_g285 = tanTriplanarNormal19_g285;

				#else

				float3 staticSwitch16_g285 = unpack17_g285;

				#endif

				#ifdef _USEDETAILMAPS_ON

				float3 staticSwitch81 = BlendNormal( temp_output_32_7 , staticSwitch16_g285 );

				#else

				float3 staticSwitch81 = temp_output_32_7;

				#endif

				float3 unpack26_g287 = UnpackNormalScale( tex2D( _CoverageNormalMap, temp_output_19_0_g287 ), _CoverageNormalIntensity );

				unpack26_g287.z = lerp( 1, unpack26_g287.z, saturate(_CoverageNormalIntensity) );

				float3 lerpResult56 = lerp( staticSwitch81 , unpack26_g287 , temp_output_76_0);

				#ifdef _ENABLECOVERAGE_ON

				float3 staticSwitch58 = lerpResult56;

				#else

				float3 staticSwitch58 = staticSwitch81;

				#endif

				

				float4 tex2DNode9_g284 = tex2D( _ORMMap, temp_output_19_0_g283 );

				float temp_output_32_3 = ( tex2DNode9_g284.b * _MetallicIntensity );

				float4 tex2DNode9_g288 = tex2D( _CoverageORMMap, temp_output_19_0_g287 );

				float lerpResult63 = lerp( ( tex2DNode9_g288.b * _CoverageMetallicIntensity ) , temp_output_32_3 , temp_output_76_0);

				#ifdef _ENABLECOVERAGE_ON

				float staticSwitch60 = lerpResult63;

				#else

				float staticSwitch60 = temp_output_32_3;

				#endif

				

				float temp_output_32_5 = ( 1.0 - ( tex2DNode9_g284.g * _RoughnessIntensity ) );

				float lerpResult62 = lerp( temp_output_32_5 , ( 1.0 - ( tex2DNode9_g288.g * _CoverageRoughnessIntensity ) ) , temp_output_76_0);

				#ifdef _ENABLECOVERAGE_ON

				float staticSwitch59 = lerpResult62;

				#else

				float staticSwitch59 = temp_output_32_5;

				#endif

				

				float lerpResult10_g284 = lerp( 1.0 , tex2DNode9_g284.r , _AOIntensity);

				float temp_output_32_8 = lerpResult10_g284;

				float lerpResult10_g288 = lerp( 1.0 , tex2DNode9_g288.r , 0.0);

				float lerpResult64 = lerp( lerpResult10_g288 , temp_output_32_8 , temp_output_76_0);

				#ifdef _ENABLECOVERAGE_ON

				float staticSwitch61 = lerpResult64;

				#else

				float staticSwitch61 = temp_output_32_8;

				#endif

				

				float3 Albedo = staticSwitch57.rgb;

				float3 Normal = staticSwitch58;

				float3 Emission = 0;

				float3 Specular = 0.5;

				float Metallic = staticSwitch60;

				float Smoothness = staticSwitch59;

				float Occlusion = staticSwitch61;

				float Alpha = 1;

				float AlphaClipThreshold = 0.5;

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
309;326;2221;1688;-515.0134;534.6016;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;31;-1929.161,-329.2871;Inherit;False;1072.199;1674.577;Comment;14;33;18;30;21;29;19;16;32;10;15;11;14;13;82;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-1761.161,-280.2871;Inherit;False;Property;_TextureSize;Texture Size;6;0;Create;True;0;0;0;False;1;Header(BASE COLOR);False;100;100;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;11;-1821.161,-191.2872;Inherit;True;Property;_ColorMap;Color Map;7;0;Create;True;0;0;0;False;0;False;None;6fa9c1ab592c6af4f8cadc03ccd976d1;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;18;-1860.161,652.7126;Inherit;False;Property;_RoughnessIntensity;Roughness Intensity;12;0;Create;True;0;0;0;False;0;False;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-1862.313,815.3082;Inherit;False;Property;_MetallicIntensity;Metallic Intensity;14;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-1864.161,372.7129;Inherit;False;Property;_NormalIntensity;Normal Intensity;10;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-1865.635,737.5316;Inherit;False;Property;_AOIntensity;AO Intensity;13;0;Create;True;0;0;0;False;0;False;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;19;-1816.773,895.9807;Inherit;True;Property;_EmissiveMap;Emissive Map;15;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;30;-1792.295,1259.69;Inherit;False;Property;_EmissiveIntensity;Emissive Intensity;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;-1798.161,-0.2872119;Inherit;False;Property;_ColorTint;Color Tint;8;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;16;-1818.161,447.7129;Inherit;True;Property;_ORMMap;ORM Map;11;0;Create;True;0;0;0;False;0;False;None;05a24179c8ae4bc428099e07a6d83a7f;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;14;-1817.161,173.7128;Inherit;True;Property;_NormalMap;Normal Map;9;0;Create;True;0;0;0;False;1;Header(BASE PBR);False;None;89eef02d5852fe94d8e7e39680da95a4;True;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;85;-1665.047,-797.7583;Inherit;True;Property;_DetailNormalMap;Detail Normal Map;22;0;Create;True;0;0;0;False;0;False;None;be64dfac24183654ab4db09aef4ceec1;False;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;84;-1665.047,-1013.758;Inherit;True;Property;_DetailColorMap;Detail Color Map;20;0;Create;True;0;0;0;False;0;False;None;ec8ee5f91cfd4d94badc416a88494b8a;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ColorNode;29;-1799.295,1086.69;Inherit;False;Property;_EmissiveColor;Emissive Color;16;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;146;-1555.887,-480.2272;Inherit;False;Property;_DetailSize;Detail Size;19;0;Create;True;0;0;0;False;0;False;100;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-1714.344,-579.924;Inherit;False;Property;_DetailNormalIntensity;Detail Normal Intensity;23;0;Create;True;0;0;0;False;0;False;1;0.674;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;147;-1140.479,-554.282;Inherit;False;PA_SF_DetailTexture;24;;285;3e3ce62e92fb3dc48a5a47a0f7420594;0;5;6;SAMPLER2D;0;False;14;SAMPLER2D;0;False;18;FLOAT;1;False;9;FLOAT;100;False;13;FLOAT;1;False;2;COLOR;0;FLOAT3;1
Node;AmplifyShaderEditor.CommentaryNode;39;-1884.634,1396.396;Inherit;False;1051.886;1058.01;Comment;8;40;47;50;52;51;48;46;41;;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;32;-1267.661,283.571;Inherit;False;PA_SF_BasePBR_OpaqueORM;0;;283;a21dcaebf1379e5439b421c5da1cd710;0;12;20;FLOAT;100;False;9;SAMPLER2D;0;False;15;COLOR;0,0,0,0;False;22;SAMPLER2D;;False;28;FLOAT;1;False;59;SAMPLER2D;;False;38;FLOAT;1;False;73;FLOAT;0;False;39;FLOAT;0;False;52;SAMPLER2D;;False;53;COLOR;0,0,0,0;False;54;FLOAT;0;False;6;COLOR;0;FLOAT3;7;COLOR;6;FLOAT;5;FLOAT;3;FLOAT;8
Node;AmplifyShaderEditor.RangedFloatNode;82;-1139.487,-228.8505;Inherit;False;Property;_DetailColorIntensity;Detail Color Intensity;21;0;Create;True;0;0;0;False;0;False;1;0.445;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;117;-470.4984,-270.0388;Inherit;False;PA_SF_DetailMapping;-1;;286;926b4928e95edad4f8ece8c367ad2fa5;0;5;3;FLOAT4;0,0,0,0;False;4;FLOAT3;0,0,0;False;17;FLOAT;0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;2;FLOAT4;0;FLOAT3;18
Node;AmplifyShaderEditor.RangedFloatNode;40;-1839.295,2322.321;Inherit;False;Property;_CoverageMetallicIntensity;Coverage Metallic Intensity;33;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;38;-674.0967,782.8811;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;35;-854.2053,895.7217;Inherit;True;Property;_SlopeNoiseTexture;Slope Noise Texture;40;0;Create;True;0;0;0;False;0;False;None;8965de8a437b2304f8e4aa6a137ab1ed;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;48;-1794.143,1744.06;Inherit;True;Property;_CoverageNormalMap;Coverage Normal Map;29;0;Create;True;0;0;0;False;0;False;None;0dff9cf3c870a7246bb082226a59938d;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;50;-1797.532,2027.616;Inherit;True;Property;_CoverageORMMap; Coverage ORM Map;31;0;Create;True;0;0;0;False;0;False;None;0f9d7517a3ecfb24289ab0ea84df8b39;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;37;-829.2182,1088.488;Inherit;False;Property;_NoiseTextureSize;Noise Texture Size;41;0;Create;True;0;0;0;False;0;False;100;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-1736.948,1446.396;Inherit;False;Property;_CoverageSize;Coverage Size;27;0;Create;True;0;0;0;False;0;False;100;31.78;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-1844.312,2226.64;Inherit;False;Property;_CoverageRoughnessIntensity;Coverage Roughness Intensity;32;0;Create;True;0;0;0;False;0;False;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;51;-1796.948,1534.396;Inherit;True;Property;_CoverageColorMap;Coverage Color Map;28;0;Create;True;0;0;0;False;0;False;None;892bf7a6f07887d47930a8ca4dec5b1a;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;47;-1841.143,1943.06;Inherit;False;Property;_CoverageNormalIntensity;Coverage Normal Intensity;30;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;52;-1325.896,1787.005;Inherit;False;PA_SF_BasePBR_OpaqueORM;0;;287;a21dcaebf1379e5439b421c5da1cd710;0;12;20;FLOAT;100;False;9;SAMPLER2D;0;False;15;COLOR;0,0,0,0;False;22;SAMPLER2D;;False;28;FLOAT;1;False;59;SAMPLER2D;;False;38;FLOAT;1;False;73;FLOAT;0;False;39;FLOAT;0;False;52;SAMPLER2D;;False;53;COLOR;0,0,0,0;False;54;FLOAT;0;False;6;COLOR;0;FLOAT3;7;COLOR;6;FLOAT;5;FLOAT;3;FLOAT;8
Node;AmplifyShaderEditor.StaticSwitch;81;-6.565722,-166.6609;Inherit;False;Property;_UseDetailMaps;Use Detail Maps?;18;0;Create;True;0;0;0;False;1;Header(COVERAGE);False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;77;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;76;-358.0807,1073.428;Inherit;False;PA_SF_VerticalBlend;34;;289;b9497802ac41dee4999b8364feedf4c9;0;3;36;SAMPLER2D;;False;38;FLOAT;1;False;2;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;77;-7.613848,-306.7618;Inherit;False;Property;_UseDetailMaps;Use Detail Maps?;18;0;Create;True;0;0;0;False;1;Header(DETAIL MAPPING);False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;55;499.448,742.1913;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;63;494.9045,1018.544;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;64;496.4579,1276.388;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;62;497.8784,1144.956;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;56;503.5745,890.6774;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;60;1210.189,316.053;Inherit;False;Property;_EnableCoverage;Enable Coverage?;26;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;59;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;57;1209.443,64.64606;Inherit;False;Property;_EnableCoverage;Enable Coverage?;26;0;Create;True;0;0;0;False;1;Header(COVERAGE);False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;59;1210.327,428.8028;Inherit;False;Property;_EnableCoverage;Enable Coverage?;26;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;61;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;61;1212.44,548.1245;Inherit;False;Property;_EnableCoverage;Enable Coverage?;26;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;57;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;58;1206.277,166.14;Inherit;False;Property;_EnableCoverage;Enable Coverage?;26;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;59;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;154;2077.191,68.90057;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=Universal2D;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;155;2077.191,68.90057;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthNormals;0;6;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=DepthNormals;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;149;2077.191,68.90057;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;150;2077.191,68.90057;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;Polyart/Dreamscape/URP/Rock Detail;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;18;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;False;0;Hidden/InternalErrorShader;0;0;Standard;38;Workflow;1;0;Surface;0;0;  Refraction Model;0;0;  Blend;0;0;Two Sided;1;0;Fragment Normal Space,InvertActionOnDeselection;0;0;Transmission;0;0;  Transmission Shadow;0.5,False,-1;0;Translucency;0;0;  Translucency Strength;1,False,-1;0;  Normal Distortion;0.5,False,-1;0;  Scattering;2,False,-1;0;  Direct;0.9,False,-1;0;  Ambient;0.1,False,-1;0;  Shadow;0.5,False,-1;0;Cast Shadows;1;0;  Use Shadow Threshold;0;0;Receive Shadows;1;0;GPU Instancing;1;0;LOD CrossFade;1;0;Built-in Fog;1;0;_FinalColorxAlpha;0;0;Meta Pass;1;0;Override Baked GI;0;0;Extra Pre Pass;0;0;DOTS Instancing;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,-1;0;  Type;0;0;  Tess;16,False,-1;0;  Min;10,False,-1;0;  Max;25,False,-1;0;  Edge Length;16,False,-1;0;  Max Displacement;25,False,-1;0;Write Depth;0;0;  Early Z;0;0;Vertex Position,InvertActionOnDeselection;1;0;0;8;False;True;True;True;True;True;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;151;2077.191,68.90057;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;152;2077.191,68.90057;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;153;2077.191,68.90057;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;156;2077.191,68.90057;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;GBuffer;0;7;GBuffer;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalGBuffer;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;147;6;84;0
WireConnection;147;14;85;0
WireConnection;147;18;86;0
WireConnection;147;9;146;0
WireConnection;32;20;10;0
WireConnection;32;9;11;0
WireConnection;32;15;13;0
WireConnection;32;22;14;0
WireConnection;32;28;15;0
WireConnection;32;59;16;0
WireConnection;32;38;18;0
WireConnection;32;73;33;0
WireConnection;32;39;21;0
WireConnection;32;52;19;0
WireConnection;32;53;29;0
WireConnection;32;54;30;0
WireConnection;117;3;147;0
WireConnection;117;4;147;1
WireConnection;117;17;82;0
WireConnection;117;14;32;0
WireConnection;117;15;32;7
WireConnection;38;0;32;7
WireConnection;52;20;46;0
WireConnection;52;9;51;0
WireConnection;52;22;48;0
WireConnection;52;28;47;0
WireConnection;52;59;50;0
WireConnection;52;38;41;0
WireConnection;52;39;40;0
WireConnection;81;1;32;7
WireConnection;81;0;117;18
WireConnection;76;36;35;0
WireConnection;76;38;37;0
WireConnection;76;2;38;0
WireConnection;77;1;32;0
WireConnection;77;0;117;0
WireConnection;55;0;77;0
WireConnection;55;1;52;0
WireConnection;55;2;76;0
WireConnection;63;0;52;3
WireConnection;63;1;32;3
WireConnection;63;2;76;0
WireConnection;64;0;52;8
WireConnection;64;1;32;8
WireConnection;64;2;76;0
WireConnection;62;0;32;5
WireConnection;62;1;52;5
WireConnection;62;2;76;0
WireConnection;56;0;81;0
WireConnection;56;1;52;7
WireConnection;56;2;76;0
WireConnection;60;1;32;3
WireConnection;60;0;63;0
WireConnection;57;1;77;0
WireConnection;57;0;55;0
WireConnection;59;1;32;5
WireConnection;59;0;62;0
WireConnection;61;1;32;8
WireConnection;61;0;64;0
WireConnection;58;1;81;0
WireConnection;58;0;56;0
WireConnection;150;0;57;0
WireConnection;150;1;58;0
WireConnection;150;3;60;0
WireConnection;150;4;59;0
WireConnection;150;5;61;0
ASEEND*/
//CHKSM=3985082079327F82A6FE327B85D70E624A7E14DD