Shader "Shader/Stretch2D"
{
    Properties
    {
        [NoScaleOffset] _MainTex("MainTex", 2D) = "white" {}
        _stretch("stretch", Range(0, 1)) = 0
        _direction("direction", Vector) = (1, 1, 0, 0)
        _strength("strength", Float) = 0.1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
        SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue" = "Transparent"
        // DisableBatching: <None>
        "ShaderGraphShader" = "true"
        "ShaderGraphTargetId" = "UniversalSpriteUnlitSubTarget"
    }
    Pass
    {
        Name "Sprite Unlit"
        Tags
        {
            "LightMode" = "Universal2D"
        }

        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag

        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>

        // Defines

        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEUNLIT


        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

        PackedVaryings PackVaryings(Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

        Varyings UnpackVaryings(PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }


        // --------------------------------------------------
        // Graph

        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float2 _direction;
        float _strength;
        float _stretch;
        CBUFFER_END


            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions

            void Unity_Normalize_float2(float2 In, out float2 Out)
            {
                Out = normalize(In);
            }

            void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
            {
                Out = A * B;
            }

            void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
            {
                Out = A - B;
            }

            void Unity_Multiply_float_float(float A, float B, out float Out)
            {
                Out = A * B;
            }

            void Unity_Absolute_float2(float2 In, out float2 Out)
            {
                Out = abs(In);
            }

            void Unity_Spherize_float(float2 UV, float2 Center, float2 Strength, float2 Offset, out float2 Out)
            {
                float2 delta = UV - Center;
                float delta2 = dot(delta.xy, delta.xy);
                float delta4 = delta2 * delta2;
                float2 delta_offset = delta4 * Strength;
                Out = UV + delta * delta_offset + Offset;
            }

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
                float Alpha;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                UnityTexture2D _Property_2d018405d2884d8abc562191f622e5aa_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                float2 _Vector2_8b876309288c4bdd96a028baf16acebf_Out_0_Vector2 = float2(float(0.5), float(0.5));
                float2 _Property_9f63573bac044566b3ea3efd296e1303_Out_0_Vector2 = _direction;
                float2 _Normalize_9798ef6063dc41bd92e7b1cb61a16656_Out_1_Vector2;
                Unity_Normalize_float2(_Property_9f63573bac044566b3ea3efd296e1303_Out_0_Vector2, _Normalize_9798ef6063dc41bd92e7b1cb61a16656_Out_1_Vector2);
                float _Float_06ee17a37e7244a6a02edda75666fdea_Out_0_Float = float(0.5);
                float2 _Multiply_24d60340aad14aa7990eb6cd5223db94_Out_2_Vector2;
                Unity_Multiply_float2_float2(_Normalize_9798ef6063dc41bd92e7b1cb61a16656_Out_1_Vector2, (_Float_06ee17a37e7244a6a02edda75666fdea_Out_0_Float.xx), _Multiply_24d60340aad14aa7990eb6cd5223db94_Out_2_Vector2);
                float2 _Subtract_bcf1405c908f423ebdd724cb48390a99_Out_2_Vector2;
                Unity_Subtract_float2(_Vector2_8b876309288c4bdd96a028baf16acebf_Out_0_Vector2, _Multiply_24d60340aad14aa7990eb6cd5223db94_Out_2_Vector2, _Subtract_bcf1405c908f423ebdd724cb48390a99_Out_2_Vector2);
                float _Property_5a6a84e00f704496a16784a7fc8d17aa_Out_0_Float = _strength;
                float _Property_a57b44eb19d24763ab115e0dbedaf86a_Out_0_Float = _stretch;
                float _Multiply_647d5c342eff4042b83ef39a8d2129a2_Out_2_Float;
                Unity_Multiply_float_float(_Property_5a6a84e00f704496a16784a7fc8d17aa_Out_0_Float, _Property_a57b44eb19d24763ab115e0dbedaf86a_Out_0_Float, _Multiply_647d5c342eff4042b83ef39a8d2129a2_Out_2_Float);
                float2 _Multiply_14706037ceb045c8a4fed57601575028_Out_2_Vector2;
                Unity_Multiply_float2_float2(_Normalize_9798ef6063dc41bd92e7b1cb61a16656_Out_1_Vector2, (_Multiply_647d5c342eff4042b83ef39a8d2129a2_Out_2_Float.xx), _Multiply_14706037ceb045c8a4fed57601575028_Out_2_Vector2);
                float2 _Absolute_1eeae6262cbd480e9ca0c38032c6b72c_Out_1_Vector2;
                Unity_Absolute_float2(_Multiply_14706037ceb045c8a4fed57601575028_Out_2_Vector2, _Absolute_1eeae6262cbd480e9ca0c38032c6b72c_Out_1_Vector2);
                float2 _Spherize_2149b1ef94ad4b27ab820bef14f09436_Out_4_Vector2;
                Unity_Spherize_float(IN.uv0.xy, _Subtract_bcf1405c908f423ebdd724cb48390a99_Out_2_Vector2, _Absolute_1eeae6262cbd480e9ca0c38032c6b72c_Out_1_Vector2, float2 (0, 0), _Spherize_2149b1ef94ad4b27ab820bef14f09436_Out_4_Vector2);
                float4 _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_2d018405d2884d8abc562191f622e5aa_Out_0_Texture2D.tex, _Property_2d018405d2884d8abc562191f622e5aa_Out_0_Texture2D.samplerstate, _Property_2d018405d2884d8abc562191f622e5aa_Out_0_Texture2D.GetTransformedUV(_Spherize_2149b1ef94ad4b27ab820bef14f09436_Out_4_Vector2));
                float _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_R_4_Float = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.r;
                float _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_G_5_Float = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.g;
                float _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_B_6_Float = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.b;
                float _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_A_7_Float = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.a;
                surface.BaseColor = (_SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.xyz);
                surface.Alpha = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_A_7_Float;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

            #ifdef HAVE_VFX_MODIFICATION
            #if VFX_USE_GRAPH_VALUES
                uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
            #endif
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

            #endif








                #if UNITY_UV_STARTS_AT_TOP
                #else
                #endif


                output.uv0 = input.texCoord0;
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                    return output;
            }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif

            ENDHLSL
            }
            Pass
            {
                Name "SceneSelectionPass"
                Tags
                {
                    "LightMode" = "SceneSelectionPass"
                }

                // Render State
                Cull Off

                // Debug
                // <None>

                // --------------------------------------------------
                // Pass

                HLSLPROGRAM

                // Pragmas
                #pragma target 2.0
                #pragma exclude_renderers d3d11_9x
                #pragma vertex vert
                #pragma fragment frag

                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>

                // Defines

                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD0
                #define VARYINGS_NEED_TEXCOORD0
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_DEPTHONLY
                #define SCENESELECTIONPASS 1



                // custom interpolator pre-include
                /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                // --------------------------------------------------
                // Structs and Packing

                // custom interpolators pre packing
                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                     float4 uv0 : TEXCOORD0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                     float4 texCoord0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                     float4 uv0;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float4 texCoord0 : INTERP0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };

                PackedVaryings PackVaryings(Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.texCoord0.xyzw = input.texCoord0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }

                Varyings UnpackVaryings(PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.texCoord0 = input.texCoord0.xyzw;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }


                // --------------------------------------------------
                // Graph

                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_TexelSize;
                float2 _direction;
                float _strength;
                float _stretch;
                CBUFFER_END


                    // Object and Global properties
                    SAMPLER(SamplerState_Linear_Repeat);
                    TEXTURE2D(_MainTex);
                    SAMPLER(sampler_MainTex);

                    // Graph Includes
                    // GraphIncludes: <None>

                    // -- Property used by ScenePickingPass
                    #ifdef SCENEPICKINGPASS
                    float4 _SelectionID;
                    #endif

                    // -- Properties used by SceneSelectionPass
                    #ifdef SCENESELECTIONPASS
                    int _ObjectId;
                    int _PassValue;
                    #endif

                    // Graph Functions

                    void Unity_Normalize_float2(float2 In, out float2 Out)
                    {
                        Out = normalize(In);
                    }

                    void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                    {
                        Out = A * B;
                    }

                    void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
                    {
                        Out = A - B;
                    }

                    void Unity_Multiply_float_float(float A, float B, out float Out)
                    {
                        Out = A * B;
                    }

                    void Unity_Absolute_float2(float2 In, out float2 Out)
                    {
                        Out = abs(In);
                    }

                    void Unity_Spherize_float(float2 UV, float2 Center, float2 Strength, float2 Offset, out float2 Out)
                    {
                        float2 delta = UV - Center;
                        float delta2 = dot(delta.xy, delta.xy);
                        float delta4 = delta2 * delta2;
                        float2 delta_offset = delta4 * Strength;
                        Out = UV + delta * delta_offset + Offset;
                    }

                    // Custom interpolators pre vertex
                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                    // Graph Vertex
                    struct VertexDescription
                    {
                        float3 Position;
                        float3 Normal;
                        float3 Tangent;
                    };

                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                    {
                        VertexDescription description = (VertexDescription)0;
                        description.Position = IN.ObjectSpacePosition;
                        description.Normal = IN.ObjectSpaceNormal;
                        description.Tangent = IN.ObjectSpaceTangent;
                        return description;
                    }

                    // Custom interpolators, pre surface
                    #ifdef FEATURES_GRAPH_VERTEX
                    Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                    {
                    return output;
                    }
                    #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                    #endif

                    // Graph Pixel
                    struct SurfaceDescription
                    {
                        float Alpha;
                    };

                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                    {
                        SurfaceDescription surface = (SurfaceDescription)0;
                        UnityTexture2D _Property_2d018405d2884d8abc562191f622e5aa_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                        float2 _Vector2_8b876309288c4bdd96a028baf16acebf_Out_0_Vector2 = float2(float(0.5), float(0.5));
                        float2 _Property_9f63573bac044566b3ea3efd296e1303_Out_0_Vector2 = _direction;
                        float2 _Normalize_9798ef6063dc41bd92e7b1cb61a16656_Out_1_Vector2;
                        Unity_Normalize_float2(_Property_9f63573bac044566b3ea3efd296e1303_Out_0_Vector2, _Normalize_9798ef6063dc41bd92e7b1cb61a16656_Out_1_Vector2);
                        float _Float_06ee17a37e7244a6a02edda75666fdea_Out_0_Float = float(0.5);
                        float2 _Multiply_24d60340aad14aa7990eb6cd5223db94_Out_2_Vector2;
                        Unity_Multiply_float2_float2(_Normalize_9798ef6063dc41bd92e7b1cb61a16656_Out_1_Vector2, (_Float_06ee17a37e7244a6a02edda75666fdea_Out_0_Float.xx), _Multiply_24d60340aad14aa7990eb6cd5223db94_Out_2_Vector2);
                        float2 _Subtract_bcf1405c908f423ebdd724cb48390a99_Out_2_Vector2;
                        Unity_Subtract_float2(_Vector2_8b876309288c4bdd96a028baf16acebf_Out_0_Vector2, _Multiply_24d60340aad14aa7990eb6cd5223db94_Out_2_Vector2, _Subtract_bcf1405c908f423ebdd724cb48390a99_Out_2_Vector2);
                        float _Property_5a6a84e00f704496a16784a7fc8d17aa_Out_0_Float = _strength;
                        float _Property_a57b44eb19d24763ab115e0dbedaf86a_Out_0_Float = _stretch;
                        float _Multiply_647d5c342eff4042b83ef39a8d2129a2_Out_2_Float;
                        Unity_Multiply_float_float(_Property_5a6a84e00f704496a16784a7fc8d17aa_Out_0_Float, _Property_a57b44eb19d24763ab115e0dbedaf86a_Out_0_Float, _Multiply_647d5c342eff4042b83ef39a8d2129a2_Out_2_Float);
                        float2 _Multiply_14706037ceb045c8a4fed57601575028_Out_2_Vector2;
                        Unity_Multiply_float2_float2(_Normalize_9798ef6063dc41bd92e7b1cb61a16656_Out_1_Vector2, (_Multiply_647d5c342eff4042b83ef39a8d2129a2_Out_2_Float.xx), _Multiply_14706037ceb045c8a4fed57601575028_Out_2_Vector2);
                        float2 _Absolute_1eeae6262cbd480e9ca0c38032c6b72c_Out_1_Vector2;
                        Unity_Absolute_float2(_Multiply_14706037ceb045c8a4fed57601575028_Out_2_Vector2, _Absolute_1eeae6262cbd480e9ca0c38032c6b72c_Out_1_Vector2);
                        float2 _Spherize_2149b1ef94ad4b27ab820bef14f09436_Out_4_Vector2;
                        Unity_Spherize_float(IN.uv0.xy, _Subtract_bcf1405c908f423ebdd724cb48390a99_Out_2_Vector2, _Absolute_1eeae6262cbd480e9ca0c38032c6b72c_Out_1_Vector2, float2 (0, 0), _Spherize_2149b1ef94ad4b27ab820bef14f09436_Out_4_Vector2);
                        float4 _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_2d018405d2884d8abc562191f622e5aa_Out_0_Texture2D.tex, _Property_2d018405d2884d8abc562191f622e5aa_Out_0_Texture2D.samplerstate, _Property_2d018405d2884d8abc562191f622e5aa_Out_0_Texture2D.GetTransformedUV(_Spherize_2149b1ef94ad4b27ab820bef14f09436_Out_4_Vector2));
                        float _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_R_4_Float = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.r;
                        float _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_G_5_Float = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.g;
                        float _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_B_6_Float = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.b;
                        float _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_A_7_Float = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.a;
                        surface.Alpha = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_A_7_Float;
                        return surface;
                    }

                    // --------------------------------------------------
                    // Build Graph Inputs
                    #ifdef HAVE_VFX_MODIFICATION
                    #define VFX_SRP_ATTRIBUTES Attributes
                    #define VFX_SRP_VARYINGS Varyings
                    #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                    #endif
                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                    {
                        VertexDescriptionInputs output;
                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                        output.ObjectSpaceNormal = input.normalOS;
                        output.ObjectSpaceTangent = input.tangentOS.xyz;
                        output.ObjectSpacePosition = input.positionOS;

                        return output;
                    }
                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                    {
                        SurfaceDescriptionInputs output;
                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                    #ifdef HAVE_VFX_MODIFICATION
                    #if VFX_USE_GRAPH_VALUES
                        uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                        /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                    #endif
                        /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                    #endif








                        #if UNITY_UV_STARTS_AT_TOP
                        #else
                        #endif


                        output.uv0 = input.texCoord0;
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                    #else
                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                    #endif
                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                            return output;
                    }

                    // --------------------------------------------------
                    // Main

                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

                    // --------------------------------------------------
                    // Visual Effect Vertex Invocations
                    #ifdef HAVE_VFX_MODIFICATION
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                    #endif

                    ENDHLSL
                    }
                    Pass
                    {
                        Name "ScenePickingPass"
                        Tags
                        {
                            "LightMode" = "Picking"
                        }

                        // Render State
                        Cull Back

                        // Debug
                        // <None>

                        // --------------------------------------------------
                        // Pass

                        HLSLPROGRAM

                        // Pragmas
                        #pragma target 2.0
                        #pragma exclude_renderers d3d11_9x
                        #pragma vertex vert
                        #pragma fragment frag

                        // Keywords
                        // PassKeywords: <None>
                        // GraphKeywords: <None>

                        // Defines

                        #define ATTRIBUTES_NEED_NORMAL
                        #define ATTRIBUTES_NEED_TANGENT
                        #define ATTRIBUTES_NEED_TEXCOORD0
                        #define VARYINGS_NEED_TEXCOORD0
                        #define FEATURES_GRAPH_VERTEX
                        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                        #define SHADERPASS SHADERPASS_DEPTHONLY
                        #define SCENEPICKINGPASS 1



                        // custom interpolator pre-include
                        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                        // Includes
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                        // --------------------------------------------------
                        // Structs and Packing

                        // custom interpolators pre packing
                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                        struct Attributes
                        {
                             float3 positionOS : POSITION;
                             float3 normalOS : NORMAL;
                             float4 tangentOS : TANGENT;
                             float4 uv0 : TEXCOORD0;
                            #if UNITY_ANY_INSTANCING_ENABLED
                             uint instanceID : INSTANCEID_SEMANTIC;
                            #endif
                        };
                        struct Varyings
                        {
                             float4 positionCS : SV_POSITION;
                             float4 texCoord0;
                            #if UNITY_ANY_INSTANCING_ENABLED
                             uint instanceID : CUSTOM_INSTANCE_ID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                            #endif
                        };
                        struct SurfaceDescriptionInputs
                        {
                             float4 uv0;
                        };
                        struct VertexDescriptionInputs
                        {
                             float3 ObjectSpaceNormal;
                             float3 ObjectSpaceTangent;
                             float3 ObjectSpacePosition;
                        };
                        struct PackedVaryings
                        {
                             float4 positionCS : SV_POSITION;
                             float4 texCoord0 : INTERP0;
                            #if UNITY_ANY_INSTANCING_ENABLED
                             uint instanceID : CUSTOM_INSTANCE_ID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                            #endif
                        };

                        PackedVaryings PackVaryings(Varyings input)
                        {
                            PackedVaryings output;
                            ZERO_INITIALIZE(PackedVaryings, output);
                            output.positionCS = input.positionCS;
                            output.texCoord0.xyzw = input.texCoord0;
                            #if UNITY_ANY_INSTANCING_ENABLED
                            output.instanceID = input.instanceID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                            output.cullFace = input.cullFace;
                            #endif
                            return output;
                        }

                        Varyings UnpackVaryings(PackedVaryings input)
                        {
                            Varyings output;
                            output.positionCS = input.positionCS;
                            output.texCoord0 = input.texCoord0.xyzw;
                            #if UNITY_ANY_INSTANCING_ENABLED
                            output.instanceID = input.instanceID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                            output.cullFace = input.cullFace;
                            #endif
                            return output;
                        }


                        // --------------------------------------------------
                        // Graph

                        // Graph Properties
                        CBUFFER_START(UnityPerMaterial)
                        float4 _MainTex_TexelSize;
                        float2 _direction;
                        float _strength;
                        float _stretch;
                        CBUFFER_END


                            // Object and Global properties
                            SAMPLER(SamplerState_Linear_Repeat);
                            TEXTURE2D(_MainTex);
                            SAMPLER(sampler_MainTex);

                            // Graph Includes
                            // GraphIncludes: <None>

                            // -- Property used by ScenePickingPass
                            #ifdef SCENEPICKINGPASS
                            float4 _SelectionID;
                            #endif

                            // -- Properties used by SceneSelectionPass
                            #ifdef SCENESELECTIONPASS
                            int _ObjectId;
                            int _PassValue;
                            #endif

                            // Graph Functions

                            void Unity_Normalize_float2(float2 In, out float2 Out)
                            {
                                Out = normalize(In);
                            }

                            void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                            {
                                Out = A * B;
                            }

                            void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
                            {
                                Out = A - B;
                            }

                            void Unity_Multiply_float_float(float A, float B, out float Out)
                            {
                                Out = A * B;
                            }

                            void Unity_Absolute_float2(float2 In, out float2 Out)
                            {
                                Out = abs(In);
                            }

                            void Unity_Spherize_float(float2 UV, float2 Center, float2 Strength, float2 Offset, out float2 Out)
                            {
                                float2 delta = UV - Center;
                                float delta2 = dot(delta.xy, delta.xy);
                                float delta4 = delta2 * delta2;
                                float2 delta_offset = delta4 * Strength;
                                Out = UV + delta * delta_offset + Offset;
                            }

                            // Custom interpolators pre vertex
                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                            // Graph Vertex
                            struct VertexDescription
                            {
                                float3 Position;
                                float3 Normal;
                                float3 Tangent;
                            };

                            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                            {
                                VertexDescription description = (VertexDescription)0;
                                description.Position = IN.ObjectSpacePosition;
                                description.Normal = IN.ObjectSpaceNormal;
                                description.Tangent = IN.ObjectSpaceTangent;
                                return description;
                            }

                            // Custom interpolators, pre surface
                            #ifdef FEATURES_GRAPH_VERTEX
                            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                            {
                            return output;
                            }
                            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                            #endif

                            // Graph Pixel
                            struct SurfaceDescription
                            {
                                float Alpha;
                            };

                            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                            {
                                SurfaceDescription surface = (SurfaceDescription)0;
                                UnityTexture2D _Property_2d018405d2884d8abc562191f622e5aa_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                                float2 _Vector2_8b876309288c4bdd96a028baf16acebf_Out_0_Vector2 = float2(float(0.5), float(0.5));
                                float2 _Property_9f63573bac044566b3ea3efd296e1303_Out_0_Vector2 = _direction;
                                float2 _Normalize_9798ef6063dc41bd92e7b1cb61a16656_Out_1_Vector2;
                                Unity_Normalize_float2(_Property_9f63573bac044566b3ea3efd296e1303_Out_0_Vector2, _Normalize_9798ef6063dc41bd92e7b1cb61a16656_Out_1_Vector2);
                                float _Float_06ee17a37e7244a6a02edda75666fdea_Out_0_Float = float(0.5);
                                float2 _Multiply_24d60340aad14aa7990eb6cd5223db94_Out_2_Vector2;
                                Unity_Multiply_float2_float2(_Normalize_9798ef6063dc41bd92e7b1cb61a16656_Out_1_Vector2, (_Float_06ee17a37e7244a6a02edda75666fdea_Out_0_Float.xx), _Multiply_24d60340aad14aa7990eb6cd5223db94_Out_2_Vector2);
                                float2 _Subtract_bcf1405c908f423ebdd724cb48390a99_Out_2_Vector2;
                                Unity_Subtract_float2(_Vector2_8b876309288c4bdd96a028baf16acebf_Out_0_Vector2, _Multiply_24d60340aad14aa7990eb6cd5223db94_Out_2_Vector2, _Subtract_bcf1405c908f423ebdd724cb48390a99_Out_2_Vector2);
                                float _Property_5a6a84e00f704496a16784a7fc8d17aa_Out_0_Float = _strength;
                                float _Property_a57b44eb19d24763ab115e0dbedaf86a_Out_0_Float = _stretch;
                                float _Multiply_647d5c342eff4042b83ef39a8d2129a2_Out_2_Float;
                                Unity_Multiply_float_float(_Property_5a6a84e00f704496a16784a7fc8d17aa_Out_0_Float, _Property_a57b44eb19d24763ab115e0dbedaf86a_Out_0_Float, _Multiply_647d5c342eff4042b83ef39a8d2129a2_Out_2_Float);
                                float2 _Multiply_14706037ceb045c8a4fed57601575028_Out_2_Vector2;
                                Unity_Multiply_float2_float2(_Normalize_9798ef6063dc41bd92e7b1cb61a16656_Out_1_Vector2, (_Multiply_647d5c342eff4042b83ef39a8d2129a2_Out_2_Float.xx), _Multiply_14706037ceb045c8a4fed57601575028_Out_2_Vector2);
                                float2 _Absolute_1eeae6262cbd480e9ca0c38032c6b72c_Out_1_Vector2;
                                Unity_Absolute_float2(_Multiply_14706037ceb045c8a4fed57601575028_Out_2_Vector2, _Absolute_1eeae6262cbd480e9ca0c38032c6b72c_Out_1_Vector2);
                                float2 _Spherize_2149b1ef94ad4b27ab820bef14f09436_Out_4_Vector2;
                                Unity_Spherize_float(IN.uv0.xy, _Subtract_bcf1405c908f423ebdd724cb48390a99_Out_2_Vector2, _Absolute_1eeae6262cbd480e9ca0c38032c6b72c_Out_1_Vector2, float2 (0, 0), _Spherize_2149b1ef94ad4b27ab820bef14f09436_Out_4_Vector2);
                                float4 _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_2d018405d2884d8abc562191f622e5aa_Out_0_Texture2D.tex, _Property_2d018405d2884d8abc562191f622e5aa_Out_0_Texture2D.samplerstate, _Property_2d018405d2884d8abc562191f622e5aa_Out_0_Texture2D.GetTransformedUV(_Spherize_2149b1ef94ad4b27ab820bef14f09436_Out_4_Vector2));
                                float _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_R_4_Float = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.r;
                                float _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_G_5_Float = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.g;
                                float _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_B_6_Float = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.b;
                                float _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_A_7_Float = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.a;
                                surface.Alpha = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_A_7_Float;
                                return surface;
                            }

                            // --------------------------------------------------
                            // Build Graph Inputs
                            #ifdef HAVE_VFX_MODIFICATION
                            #define VFX_SRP_ATTRIBUTES Attributes
                            #define VFX_SRP_VARYINGS Varyings
                            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                            #endif
                            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                            {
                                VertexDescriptionInputs output;
                                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                output.ObjectSpaceNormal = input.normalOS;
                                output.ObjectSpaceTangent = input.tangentOS.xyz;
                                output.ObjectSpacePosition = input.positionOS;

                                return output;
                            }
                            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                            {
                                SurfaceDescriptionInputs output;
                                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                            #ifdef HAVE_VFX_MODIFICATION
                            #if VFX_USE_GRAPH_VALUES
                                uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                                /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                            #endif
                                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                            #endif








                                #if UNITY_UV_STARTS_AT_TOP
                                #else
                                #endif


                                output.uv0 = input.texCoord0;
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                            #else
                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                            #endif
                            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                    return output;
                            }

                            // --------------------------------------------------
                            // Main

                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

                            // --------------------------------------------------
                            // Visual Effect Vertex Invocations
                            #ifdef HAVE_VFX_MODIFICATION
                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                            #endif

                            ENDHLSL
                            }
                            Pass
                            {
                                Name "Sprite Unlit"
                                Tags
                                {
                                    "LightMode" = "UniversalForward"
                                }

                                // Render State
                                Cull Off
                                Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                                ZTest LEqual
                                ZWrite Off

                                // Debug
                                // <None>

                                // --------------------------------------------------
                                // Pass

                                HLSLPROGRAM

                                // Pragmas
                                #pragma target 2.0
                                #pragma exclude_renderers d3d11_9x
                                #pragma vertex vert
                                #pragma fragment frag

                                // Keywords
                                #pragma multi_compile_fragment _ DEBUG_DISPLAY
                                // GraphKeywords: <None>

                                // Defines

                                #define ATTRIBUTES_NEED_NORMAL
                                #define ATTRIBUTES_NEED_TANGENT
                                #define ATTRIBUTES_NEED_TEXCOORD0
                                #define ATTRIBUTES_NEED_COLOR
                                #define VARYINGS_NEED_POSITION_WS
                                #define VARYINGS_NEED_TEXCOORD0
                                #define VARYINGS_NEED_COLOR
                                #define FEATURES_GRAPH_VERTEX
                                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                #define SHADERPASS SHADERPASS_SPRITEFORWARD


                                // custom interpolator pre-include
                                /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                // Includes
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                // --------------------------------------------------
                                // Structs and Packing

                                // custom interpolators pre packing
                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                struct Attributes
                                {
                                     float3 positionOS : POSITION;
                                     float3 normalOS : NORMAL;
                                     float4 tangentOS : TANGENT;
                                     float4 uv0 : TEXCOORD0;
                                     float4 color : COLOR;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                     uint instanceID : INSTANCEID_SEMANTIC;
                                    #endif
                                };
                                struct Varyings
                                {
                                     float4 positionCS : SV_POSITION;
                                     float3 positionWS;
                                     float4 texCoord0;
                                     float4 color;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                     uint instanceID : CUSTOM_INSTANCE_ID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                    #endif
                                };
                                struct SurfaceDescriptionInputs
                                {
                                     float4 uv0;
                                };
                                struct VertexDescriptionInputs
                                {
                                     float3 ObjectSpaceNormal;
                                     float3 ObjectSpaceTangent;
                                     float3 ObjectSpacePosition;
                                };
                                struct PackedVaryings
                                {
                                     float4 positionCS : SV_POSITION;
                                     float4 texCoord0 : INTERP0;
                                     float4 color : INTERP1;
                                     float3 positionWS : INTERP2;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                     uint instanceID : CUSTOM_INSTANCE_ID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                    #endif
                                };

                                PackedVaryings PackVaryings(Varyings input)
                                {
                                    PackedVaryings output;
                                    ZERO_INITIALIZE(PackedVaryings, output);
                                    output.positionCS = input.positionCS;
                                    output.texCoord0.xyzw = input.texCoord0;
                                    output.color.xyzw = input.color;
                                    output.positionWS.xyz = input.positionWS;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                    output.instanceID = input.instanceID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                    output.cullFace = input.cullFace;
                                    #endif
                                    return output;
                                }

                                Varyings UnpackVaryings(PackedVaryings input)
                                {
                                    Varyings output;
                                    output.positionCS = input.positionCS;
                                    output.texCoord0 = input.texCoord0.xyzw;
                                    output.color = input.color.xyzw;
                                    output.positionWS = input.positionWS.xyz;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                    output.instanceID = input.instanceID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                    output.cullFace = input.cullFace;
                                    #endif
                                    return output;
                                }


                                // --------------------------------------------------
                                // Graph

                                // Graph Properties
                                CBUFFER_START(UnityPerMaterial)
                                float4 _MainTex_TexelSize;
                                float2 _direction;
                                float _strength;
                                float _stretch;
                                CBUFFER_END


                                    // Object and Global properties
                                    SAMPLER(SamplerState_Linear_Repeat);
                                    TEXTURE2D(_MainTex);
                                    SAMPLER(sampler_MainTex);

                                    // Graph Includes
                                    // GraphIncludes: <None>

                                    // -- Property used by ScenePickingPass
                                    #ifdef SCENEPICKINGPASS
                                    float4 _SelectionID;
                                    #endif

                                    // -- Properties used by SceneSelectionPass
                                    #ifdef SCENESELECTIONPASS
                                    int _ObjectId;
                                    int _PassValue;
                                    #endif

                                    // Graph Functions

                                    void Unity_Normalize_float2(float2 In, out float2 Out)
                                    {
                                        Out = normalize(In);
                                    }

                                    void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                                    {
                                        Out = A * B;
                                    }

                                    void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
                                    {
                                        Out = A - B;
                                    }

                                    void Unity_Multiply_float_float(float A, float B, out float Out)
                                    {
                                        Out = A * B;
                                    }

                                    void Unity_Absolute_float2(float2 In, out float2 Out)
                                    {
                                        Out = abs(In);
                                    }

                                    void Unity_Spherize_float(float2 UV, float2 Center, float2 Strength, float2 Offset, out float2 Out)
                                    {
                                        float2 delta = UV - Center;
                                        float delta2 = dot(delta.xy, delta.xy);
                                        float delta4 = delta2 * delta2;
                                        float2 delta_offset = delta4 * Strength;
                                        Out = UV + delta * delta_offset + Offset;
                                    }

                                    // Custom interpolators pre vertex
                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                    // Graph Vertex
                                    struct VertexDescription
                                    {
                                        float3 Position;
                                        float3 Normal;
                                        float3 Tangent;
                                    };

                                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                    {
                                        VertexDescription description = (VertexDescription)0;
                                        description.Position = IN.ObjectSpacePosition;
                                        description.Normal = IN.ObjectSpaceNormal;
                                        description.Tangent = IN.ObjectSpaceTangent;
                                        return description;
                                    }

                                    // Custom interpolators, pre surface
                                    #ifdef FEATURES_GRAPH_VERTEX
                                    Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                    {
                                    return output;
                                    }
                                    #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                    #endif

                                    // Graph Pixel
                                    struct SurfaceDescription
                                    {
                                        float3 BaseColor;
                                        float Alpha;
                                    };

                                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                    {
                                        SurfaceDescription surface = (SurfaceDescription)0;
                                        UnityTexture2D _Property_2d018405d2884d8abc562191f622e5aa_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                                        float2 _Vector2_8b876309288c4bdd96a028baf16acebf_Out_0_Vector2 = float2(float(0.5), float(0.5));
                                        float2 _Property_9f63573bac044566b3ea3efd296e1303_Out_0_Vector2 = _direction;
                                        float2 _Normalize_9798ef6063dc41bd92e7b1cb61a16656_Out_1_Vector2;
                                        Unity_Normalize_float2(_Property_9f63573bac044566b3ea3efd296e1303_Out_0_Vector2, _Normalize_9798ef6063dc41bd92e7b1cb61a16656_Out_1_Vector2);
                                        float _Float_06ee17a37e7244a6a02edda75666fdea_Out_0_Float = float(0.5);
                                        float2 _Multiply_24d60340aad14aa7990eb6cd5223db94_Out_2_Vector2;
                                        Unity_Multiply_float2_float2(_Normalize_9798ef6063dc41bd92e7b1cb61a16656_Out_1_Vector2, (_Float_06ee17a37e7244a6a02edda75666fdea_Out_0_Float.xx), _Multiply_24d60340aad14aa7990eb6cd5223db94_Out_2_Vector2);
                                        float2 _Subtract_bcf1405c908f423ebdd724cb48390a99_Out_2_Vector2;
                                        Unity_Subtract_float2(_Vector2_8b876309288c4bdd96a028baf16acebf_Out_0_Vector2, _Multiply_24d60340aad14aa7990eb6cd5223db94_Out_2_Vector2, _Subtract_bcf1405c908f423ebdd724cb48390a99_Out_2_Vector2);
                                        float _Property_5a6a84e00f704496a16784a7fc8d17aa_Out_0_Float = _strength;
                                        float _Property_a57b44eb19d24763ab115e0dbedaf86a_Out_0_Float = _stretch;
                                        float _Multiply_647d5c342eff4042b83ef39a8d2129a2_Out_2_Float;
                                        Unity_Multiply_float_float(_Property_5a6a84e00f704496a16784a7fc8d17aa_Out_0_Float, _Property_a57b44eb19d24763ab115e0dbedaf86a_Out_0_Float, _Multiply_647d5c342eff4042b83ef39a8d2129a2_Out_2_Float);
                                        float2 _Multiply_14706037ceb045c8a4fed57601575028_Out_2_Vector2;
                                        Unity_Multiply_float2_float2(_Normalize_9798ef6063dc41bd92e7b1cb61a16656_Out_1_Vector2, (_Multiply_647d5c342eff4042b83ef39a8d2129a2_Out_2_Float.xx), _Multiply_14706037ceb045c8a4fed57601575028_Out_2_Vector2);
                                        float2 _Absolute_1eeae6262cbd480e9ca0c38032c6b72c_Out_1_Vector2;
                                        Unity_Absolute_float2(_Multiply_14706037ceb045c8a4fed57601575028_Out_2_Vector2, _Absolute_1eeae6262cbd480e9ca0c38032c6b72c_Out_1_Vector2);
                                        float2 _Spherize_2149b1ef94ad4b27ab820bef14f09436_Out_4_Vector2;
                                        Unity_Spherize_float(IN.uv0.xy, _Subtract_bcf1405c908f423ebdd724cb48390a99_Out_2_Vector2, _Absolute_1eeae6262cbd480e9ca0c38032c6b72c_Out_1_Vector2, float2 (0, 0), _Spherize_2149b1ef94ad4b27ab820bef14f09436_Out_4_Vector2);
                                        float4 _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_2d018405d2884d8abc562191f622e5aa_Out_0_Texture2D.tex, _Property_2d018405d2884d8abc562191f622e5aa_Out_0_Texture2D.samplerstate, _Property_2d018405d2884d8abc562191f622e5aa_Out_0_Texture2D.GetTransformedUV(_Spherize_2149b1ef94ad4b27ab820bef14f09436_Out_4_Vector2));
                                        float _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_R_4_Float = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.r;
                                        float _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_G_5_Float = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.g;
                                        float _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_B_6_Float = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.b;
                                        float _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_A_7_Float = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.a;
                                        surface.BaseColor = (_SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_RGBA_0_Vector4.xyz);
                                        surface.Alpha = _SampleTexture2D_ab3dcf7f5d2f4faaafbbc4c4befc3008_A_7_Float;
                                        return surface;
                                    }

                                    // --------------------------------------------------
                                    // Build Graph Inputs
                                    #ifdef HAVE_VFX_MODIFICATION
                                    #define VFX_SRP_ATTRIBUTES Attributes
                                    #define VFX_SRP_VARYINGS Varyings
                                    #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                    #endif
                                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                    {
                                        VertexDescriptionInputs output;
                                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                        output.ObjectSpaceNormal = input.normalOS;
                                        output.ObjectSpaceTangent = input.tangentOS.xyz;
                                        output.ObjectSpacePosition = input.positionOS;

                                        return output;
                                    }
                                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                    {
                                        SurfaceDescriptionInputs output;
                                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                    #ifdef HAVE_VFX_MODIFICATION
                                    #if VFX_USE_GRAPH_VALUES
                                        uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                                        /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                                    #endif
                                        /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                    #endif








                                        #if UNITY_UV_STARTS_AT_TOP
                                        #else
                                        #endif


                                        output.uv0 = input.texCoord0;
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                    #else
                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                    #endif
                                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                            return output;
                                    }

                                    // --------------------------------------------------
                                    // Main

                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                    #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

                                    // --------------------------------------------------
                                    // Visual Effect Vertex Invocations
                                    #ifdef HAVE_VFX_MODIFICATION
                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                    #endif

                                    ENDHLSL
                                    }
    }
        CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
                                        FallBack "Hidden/Shader Graph/FallbackError"
}