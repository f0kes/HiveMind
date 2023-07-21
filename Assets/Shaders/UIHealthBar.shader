﻿Shader "UI/UIHealthBar"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [HDR] _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _CurrentMaxHealth ("Current Max Health", Float) = 2000

        _SmallDivisorHealth ("Small Divisor Health", Float) = 200
        _BigDivisorHealth ("Big Divisor Health", Float) = 1000

        _SmallDivisorPixelWidth ("Small Divisor Pixel Width", Int) = 1
        _BigDivisorPixelWidth ("Big Divisor Pixel Width", Int) = 2

        _SmallDivisorColor ("Small Divisor Color", Color) = (0,0,0,1)
        _BigDivisorColor ("Big Divisor Color", Color) = (0,0,0,1)

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
            half4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            float _CurrentMaxHealth;

            float _SmallDivisorHealth;
            float _BigDivisorHealth;

            int _SmallDivisorPixelWidth;
            int _BigDivisorPixelWidth;

            fixed4 _SmallDivisorColor;
            fixed4 _BigDivisorColor;

            float modulo(float a, float b) {
                return a - b * floor(a / b);
            }

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

            fixed4 frag(v2f IN) : SV_Target {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                float smallDivisionPercent = _SmallDivisorHealth / _CurrentMaxHealth;
                float distToSmall = abs(modulo(IN.texcoord.x, smallDivisionPercent) - smallDivisionPercent);
                float alpha = 1-clamp(_CurrentMaxHealth / (_SmallDivisorHealth * 50), 0, 1);
                if (IN.texcoord.y > 0.4f && distToSmall < 0.01f * _SmallDivisorPixelWidth)
                {
                    color = (_SmallDivisorColor- color )  * alpha + color;
                }
                float bigDivisionPercent = _BigDivisorHealth / _CurrentMaxHealth;
                float distToBig = abs(modulo(IN.texcoord.x, bigDivisionPercent) - bigDivisionPercent);
                if (distToBig < 0.01f * _BigDivisorPixelWidth)
                {
                    color = _BigDivisorColor;
                }
                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
            ENDCG
        }
    }
}