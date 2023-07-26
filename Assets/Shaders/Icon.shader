Shader "UI/Icon"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [HDR] _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 0)

        _LookupMainColor ("Lookup Main Color", Color) = (0,0,0,1)
        _LookupAlphaColor ("Lookup Alpha Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Size", float) = 5

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _MainTex_ST;

            float4 _Color;
            fixed4 _OutlineColor;
            float _OutlineWidth;

            float4 _LookupMainColor;
            float4 _LookupAlphaColor;

            fixed4 _TextureSampleAdd;
            float4 _ClipRect;


            v2f vert(appdata_t v) {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

            float applyScale(float base, float scale) {

            }

            float4 convertColor(float4 IN) {
                float mainDot = dot(IN, _LookupMainColor);
                float alphaDot = dot(IN, _LookupAlphaColor);
                if (mainDot >= alphaDot)
                {
                    return _Color;
                }
                return float4(0, 0, 0, 0);
            }


            fixed4 frag(v2f IN) : SV_Target {
                float4 color = (tex2D(_MainTex, IN.texcoord));
                color = convertColor(color);

                #define DIV_SQRT_2 0.70710678118
                float2 directions[8] = {
                    float2(1, 0), float2(0, 1), float2(-1, 0), float2(0, -1),
                    float2(DIV_SQRT_2, DIV_SQRT_2), float2(-DIV_SQRT_2, DIV_SQRT_2),
                    float2(-DIV_SQRT_2, -DIV_SQRT_2), float2(DIV_SQRT_2, -DIV_SQRT_2)
                };
                float maxAlpha = 0;
                for (uint index = 0; index < 8; index++)
                {
                    float2 sampleUV = IN.texcoord + directions[index] * _MainTex_TexelSize.x *_OutlineWidth/*magic number*/;
                    maxAlpha = max(maxAlpha, convertColor(tex2D(_MainTex, sampleUV)).a);
                }
                color.rgb = lerp(_OutlineColor.rgb/*magic color*/, color.rgb, color.a);
                color.a = max(color.a, maxAlpha);
                

                return color;
            }
            ENDCG
        }
    }
}