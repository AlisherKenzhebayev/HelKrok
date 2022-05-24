Shader "Slider Shader"
{
    Properties
    {
        Value_("Value", Range(0, 5)) = 0.5
        Color_("Color", Color) = (1, 0.1300032, 0, 0)
        [HDR]EdgeColor_("Edge Color", Color) = (1, 0.1830188, 0.1830188, 0)
        [NoScaleOffset]SlidingTexture_("SlidingTexture", 2D) = "white" {}
        ColorSlidingTexture_("ColorSlidingTexture", Color) = (0.3333178, 0, 1, 0)
        ValueSliding_("ValueSliding", Range(0, 5)) = 1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

        _Stencil("Stencil ID", Float) = 0
        _StencilComp("StencilComp", Float) = 8
        _StencilOp("StencilOp", Float) = 0
        _StencilReadMask("StencilReadMask", Float) = 255
        _StencilWriteMask("StencilWriteMask", Float) = 255
        _ColorMask("ColorMask", Float) = 15
    }
        SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue" = "Transparent"
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
    ZTest[unity_GUIZTestMode]
    ZWrite Off


    Stencil{
        Ref[_Stencil]
        Comp[_StencilComp]
        Pass[_StencilOp]
        ReadMask[_StencilReadMask]
        WriteMask[_StencilWriteMask]
    }
    ColorMask[_ColorMask]

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

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEUNLIT
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

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
        float3 TimeParameters;
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
        float4 interp0 : TEXCOORD0;
        float4 interp1 : TEXCOORD1;
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
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
        output.interp1.xyzw = input.color;
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
        output.texCoord0 = input.interp0.xyzw;
        output.color = input.interp1.xyzw;
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
float Value_;
float4 Color_;
float4 EdgeColor_;
float4 SlidingTexture__TexelSize;
float4 ColorSlidingTexture_;
float ValueSliding_;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Point_Repeat);
TEXTURE2D(SlidingTexture_);
SAMPLER(samplerSlidingTexture_);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

void Unity_Blend_Multiply_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    Out = Base * Blend;
    Out = lerp(Base, Out, Opacity);
}


inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
}

inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
{
    return (1.0 - t) * a + (t * b);
}


inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    f = f * f * (3.0 - 2.0 * f);

    uv = abs(frac(uv) - 0.5);
    float2 c0 = i + float2(0.0, 0.0);
    float2 c1 = i + float2(1.0, 0.0);
    float2 c2 = i + float2(0.0, 1.0);
    float2 c3 = i + float2(1.0, 1.0);
    float r0 = Unity_SimpleNoise_RandomValue_float(c0);
    float r1 = Unity_SimpleNoise_RandomValue_float(c1);
    float r2 = Unity_SimpleNoise_RandomValue_float(c2);
    float r3 = Unity_SimpleNoise_RandomValue_float(c3);

    float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
    float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
    float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
    return t;
}
void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
{
    float t = 0.0;

    float freq = pow(2.0, float(0));
    float amp = pow(0.5, float(3 - 0));
    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

    freq = pow(2.0, float(1));
    amp = pow(0.5, float(3 - 1));
    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

    freq = pow(2.0, float(2));
    amp = pow(0.5, float(3 - 2));
    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

    Out = t;
}

void Unity_RadialShear_float(float2 UV, float2 Center, float2 Strength, float2 Offset, out float2 Out)
{
    float2 delta = UV - Center;
    float delta2 = dot(delta.xy, delta.xy);
    float2 delta_offset = delta2 * Strength;
    Out = UV + float2(delta.y, -delta.x) * delta_offset + Offset;
}

void Unity_Lerp_float2(float2 A, float2 B, float2 T, out float2 Out)
{
    Out = lerp(A, B, T);
}

void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Blend_Overwrite_float2(float2 Base, float2 Blend, out float2 Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

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

// Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    UnityTexture2D _Property_a78dacf2efb9493dbe0e79ee5fc215e6_Out_0 = UnityBuildTexture2DStructNoScale(SlidingTexture_);
    float _Property_bc24b82264d34d0992fd37bdb30f405f_Out_0 = ValueSliding_;
    float _Multiply_6786c543d4a14b62896a84d0d525626e_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_bc24b82264d34d0992fd37bdb30f405f_Out_0, _Multiply_6786c543d4a14b62896a84d0d525626e_Out_2);
    float2 _Vector2_9db0fae4ee994846af0c753943001006_Out_0 = float2(_Multiply_6786c543d4a14b62896a84d0d525626e_Out_2, 0);
    float2 _TilingAndOffset_a4183de8ceed47c9a52e1fe1a375bdb5_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_9db0fae4ee994846af0c753943001006_Out_0, _TilingAndOffset_a4183de8ceed47c9a52e1fe1a375bdb5_Out_3);
    float4 _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a78dacf2efb9493dbe0e79ee5fc215e6_Out_0.tex, UnityBuildSamplerStateStruct(SamplerState_Point_Repeat).samplerstate, _TilingAndOffset_a4183de8ceed47c9a52e1fe1a375bdb5_Out_3);
    float _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_R_4 = _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_RGBA_0.r;
    float _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_G_5 = _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_RGBA_0.g;
    float _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_B_6 = _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_RGBA_0.b;
    float _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_A_7 = _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_RGBA_0.a;
    float4 _Property_08aec882b86d48248dcaaea118601b6a_Out_0 = ColorSlidingTexture_;
    float4 _Blend_b787bf0a29034d33aa7abe39340479c2_Out_2;
    Unity_Blend_Multiply_float4(_SampleTexture2D_f013a1c68e324b459591029c3d8610e6_RGBA_0, _Property_08aec882b86d48248dcaaea118601b6a_Out_0, _Blend_b787bf0a29034d33aa7abe39340479c2_Out_2, _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_A_7);
    float4 _Property_d4e864f3413d47bf97aa076edae750a9_Out_0 = Color_;
    float4 _Property_c6936505159249b082742007d429efa1_Out_0 = IsGammaSpace() ? LinearToSRGB(EdgeColor_) : EdgeColor_;
    float _Property_cea953a373cc4a57a75f747d5f6497f4_Out_0 = Value_;
    float _Multiply_7be8170b40b3403ea5c8629a0a7c3c37_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_cea953a373cc4a57a75f747d5f6497f4_Out_0, _Multiply_7be8170b40b3403ea5c8629a0a7c3c37_Out_2);
    float2 _Vector2_dd750c4420754239ac6bf2ae6c925c4c_Out_0 = float2(_Multiply_7be8170b40b3403ea5c8629a0a7c3c37_Out_2, 0);
    float2 _TilingAndOffset_1baf1f60700949c88260f1e33c5717c0_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_dd750c4420754239ac6bf2ae6c925c4c_Out_0, _TilingAndOffset_1baf1f60700949c88260f1e33c5717c0_Out_3);
    float _SimpleNoise_2feeee5dca554330b10dff8aeaf6b789_Out_2;
    Unity_SimpleNoise_float(_TilingAndOffset_1baf1f60700949c88260f1e33c5717c0_Out_3, 50, _SimpleNoise_2feeee5dca554330b10dff8aeaf6b789_Out_2);
    float2 _RadialShear_4b2d71e88d264f74a740b42e8edec708_Out_4;
    Unity_RadialShear_float((_SimpleNoise_2feeee5dca554330b10dff8aeaf6b789_Out_2.xx), float2 (0.5, 0.5), float2 (10, 10), float2 (0, 0), _RadialShear_4b2d71e88d264f74a740b42e8edec708_Out_4);
    float2 _Lerp_310c9cf06e434deba5c56b0b9f055678_Out_3;
    Unity_Lerp_float2((_Property_d4e864f3413d47bf97aa076edae750a9_Out_0.xy), (_Property_c6936505159249b082742007d429efa1_Out_0.xy), _RadialShear_4b2d71e88d264f74a740b42e8edec708_Out_4, _Lerp_310c9cf06e434deba5c56b0b9f055678_Out_3);
    float _Remap_b43f839de88647edb5ccb0a6a1daa0c3_Out_3;
    Unity_Remap_float(_SampleTexture2D_f013a1c68e324b459591029c3d8610e6_A_7, float2 (0, 1), float2 (1, 0), _Remap_b43f839de88647edb5ccb0a6a1daa0c3_Out_3);
    float2 _Blend_c42bdf2fa78041a6880c6a797d035dd7_Out_2;
    Unity_Blend_Overwrite_float2((_Blend_b787bf0a29034d33aa7abe39340479c2_Out_2.xy), _Lerp_310c9cf06e434deba5c56b0b9f055678_Out_3, _Blend_c42bdf2fa78041a6880c6a797d035dd7_Out_2, _Remap_b43f839de88647edb5ccb0a6a1daa0c3_Out_3);
    surface.BaseColor = (float3(_Blend_c42bdf2fa78041a6880c6a797d035dd7_Out_2, 0.0));
    surface.Alpha = 1;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

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





    output.uv0 = input.texCoord0;
    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

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
    ZTest[unity_GUIZTestMode]
    ZWrite Off


    Stencil{
        Ref[_Stencil]
        Comp[_StencilComp]
        Pass[_StencilOp]
        ReadMask[_StencilReadMask]
        WriteMask[_StencilWriteMask]
    }
    ColorMask[_ColorMask]
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

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEFORWARD
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

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
        float3 TimeParameters;
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
        float4 interp0 : TEXCOORD0;
        float4 interp1 : TEXCOORD1;
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
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
        output.interp1.xyzw = input.color;
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
        output.texCoord0 = input.interp0.xyzw;
        output.color = input.interp1.xyzw;
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
float Value_;
float4 Color_;
float4 EdgeColor_;
float4 SlidingTexture__TexelSize;
float4 ColorSlidingTexture_;
float ValueSliding_;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Point_Repeat);
TEXTURE2D(SlidingTexture_);
SAMPLER(samplerSlidingTexture_);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

void Unity_Blend_Multiply_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    Out = Base * Blend;
    Out = lerp(Base, Out, Opacity);
}


inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
}

inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
{
    return (1.0 - t) * a + (t * b);
}


inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    f = f * f * (3.0 - 2.0 * f);

    uv = abs(frac(uv) - 0.5);
    float2 c0 = i + float2(0.0, 0.0);
    float2 c1 = i + float2(1.0, 0.0);
    float2 c2 = i + float2(0.0, 1.0);
    float2 c3 = i + float2(1.0, 1.0);
    float r0 = Unity_SimpleNoise_RandomValue_float(c0);
    float r1 = Unity_SimpleNoise_RandomValue_float(c1);
    float r2 = Unity_SimpleNoise_RandomValue_float(c2);
    float r3 = Unity_SimpleNoise_RandomValue_float(c3);

    float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
    float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
    float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
    return t;
}
void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
{
    float t = 0.0;

    float freq = pow(2.0, float(0));
    float amp = pow(0.5, float(3 - 0));
    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

    freq = pow(2.0, float(1));
    amp = pow(0.5, float(3 - 1));
    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

    freq = pow(2.0, float(2));
    amp = pow(0.5, float(3 - 2));
    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

    Out = t;
}

void Unity_RadialShear_float(float2 UV, float2 Center, float2 Strength, float2 Offset, out float2 Out)
{
    float2 delta = UV - Center;
    float delta2 = dot(delta.xy, delta.xy);
    float2 delta_offset = delta2 * Strength;
    Out = UV + float2(delta.y, -delta.x) * delta_offset + Offset;
}

void Unity_Lerp_float2(float2 A, float2 B, float2 T, out float2 Out)
{
    Out = lerp(A, B, T);
}

void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Blend_Overwrite_float2(float2 Base, float2 Blend, out float2 Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

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

// Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    UnityTexture2D _Property_a78dacf2efb9493dbe0e79ee5fc215e6_Out_0 = UnityBuildTexture2DStructNoScale(SlidingTexture_);
    float _Property_bc24b82264d34d0992fd37bdb30f405f_Out_0 = ValueSliding_;
    float _Multiply_6786c543d4a14b62896a84d0d525626e_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_bc24b82264d34d0992fd37bdb30f405f_Out_0, _Multiply_6786c543d4a14b62896a84d0d525626e_Out_2);
    float2 _Vector2_9db0fae4ee994846af0c753943001006_Out_0 = float2(_Multiply_6786c543d4a14b62896a84d0d525626e_Out_2, 0);
    float2 _TilingAndOffset_a4183de8ceed47c9a52e1fe1a375bdb5_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_9db0fae4ee994846af0c753943001006_Out_0, _TilingAndOffset_a4183de8ceed47c9a52e1fe1a375bdb5_Out_3);
    float4 _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a78dacf2efb9493dbe0e79ee5fc215e6_Out_0.tex, UnityBuildSamplerStateStruct(SamplerState_Point_Repeat).samplerstate, _TilingAndOffset_a4183de8ceed47c9a52e1fe1a375bdb5_Out_3);
    float _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_R_4 = _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_RGBA_0.r;
    float _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_G_5 = _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_RGBA_0.g;
    float _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_B_6 = _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_RGBA_0.b;
    float _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_A_7 = _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_RGBA_0.a;
    float4 _Property_08aec882b86d48248dcaaea118601b6a_Out_0 = ColorSlidingTexture_;
    float4 _Blend_b787bf0a29034d33aa7abe39340479c2_Out_2;
    Unity_Blend_Multiply_float4(_SampleTexture2D_f013a1c68e324b459591029c3d8610e6_RGBA_0, _Property_08aec882b86d48248dcaaea118601b6a_Out_0, _Blend_b787bf0a29034d33aa7abe39340479c2_Out_2, _SampleTexture2D_f013a1c68e324b459591029c3d8610e6_A_7);
    float4 _Property_d4e864f3413d47bf97aa076edae750a9_Out_0 = Color_;
    float4 _Property_c6936505159249b082742007d429efa1_Out_0 = IsGammaSpace() ? LinearToSRGB(EdgeColor_) : EdgeColor_;
    float _Property_cea953a373cc4a57a75f747d5f6497f4_Out_0 = Value_;
    float _Multiply_7be8170b40b3403ea5c8629a0a7c3c37_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_cea953a373cc4a57a75f747d5f6497f4_Out_0, _Multiply_7be8170b40b3403ea5c8629a0a7c3c37_Out_2);
    float2 _Vector2_dd750c4420754239ac6bf2ae6c925c4c_Out_0 = float2(_Multiply_7be8170b40b3403ea5c8629a0a7c3c37_Out_2, 0);
    float2 _TilingAndOffset_1baf1f60700949c88260f1e33c5717c0_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_dd750c4420754239ac6bf2ae6c925c4c_Out_0, _TilingAndOffset_1baf1f60700949c88260f1e33c5717c0_Out_3);
    float _SimpleNoise_2feeee5dca554330b10dff8aeaf6b789_Out_2;
    Unity_SimpleNoise_float(_TilingAndOffset_1baf1f60700949c88260f1e33c5717c0_Out_3, 50, _SimpleNoise_2feeee5dca554330b10dff8aeaf6b789_Out_2);
    float2 _RadialShear_4b2d71e88d264f74a740b42e8edec708_Out_4;
    Unity_RadialShear_float((_SimpleNoise_2feeee5dca554330b10dff8aeaf6b789_Out_2.xx), float2 (0.5, 0.5), float2 (10, 10), float2 (0, 0), _RadialShear_4b2d71e88d264f74a740b42e8edec708_Out_4);
    float2 _Lerp_310c9cf06e434deba5c56b0b9f055678_Out_3;
    Unity_Lerp_float2((_Property_d4e864f3413d47bf97aa076edae750a9_Out_0.xy), (_Property_c6936505159249b082742007d429efa1_Out_0.xy), _RadialShear_4b2d71e88d264f74a740b42e8edec708_Out_4, _Lerp_310c9cf06e434deba5c56b0b9f055678_Out_3);
    float _Remap_b43f839de88647edb5ccb0a6a1daa0c3_Out_3;
    Unity_Remap_float(_SampleTexture2D_f013a1c68e324b459591029c3d8610e6_A_7, float2 (0, 1), float2 (1, 0), _Remap_b43f839de88647edb5ccb0a6a1daa0c3_Out_3);
    float2 _Blend_c42bdf2fa78041a6880c6a797d035dd7_Out_2;
    Unity_Blend_Overwrite_float2((_Blend_b787bf0a29034d33aa7abe39340479c2_Out_2.xy), _Lerp_310c9cf06e434deba5c56b0b9f055678_Out_3, _Blend_c42bdf2fa78041a6880c6a797d035dd7_Out_2, _Remap_b43f839de88647edb5ccb0a6a1daa0c3_Out_3);
    surface.BaseColor = (float3(_Blend_c42bdf2fa78041a6880c6a797d035dd7_Out_2, 0.0));
    surface.Alpha = 1;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

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





    output.uv0 = input.texCoord0;
    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

    ENDHLSL
}
    }
        FallBack "Hidden/Shader Graph/FallbackError"
}