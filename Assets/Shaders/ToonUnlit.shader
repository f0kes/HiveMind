Shader "Unlit/ToonUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        [HDR] _AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)
        [HDR] _SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
        _Glossiness("Glossiness", Float) = 32
        [HDR]_RimColor("Rim Color", Color) = (0.9,0.9,0.9,1)
        _RimIntensity("Rim Intensity", Float) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "LightMode" = "UniversalForward"
            "PassFlags" = "OnlyDirectional"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "UnityCG.cginc"
            #include "Lighting.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldNormal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float4 _AmbientColor;
            float _Glossiness;
            float4 _SpecularColor;
            float4 _RimColor;
            float _RimIntensity;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float3 viewDir = normalize(i.viewDir);
                float3 normal = normalize(i.worldNormal);
                float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);

                float NdotL = dot(_WorldSpaceLightPos0, normal);
                float NdotH = dot(halfVector, normal);
                float4 rimDot = (1 - dot(viewDir, normal));


                float lightIntensity = smoothstep(0, 0.02, NdotL);
                float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
                float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
                float rimIntensity = rimDot > (1 - _RimIntensity) ? 1 : 0;

                float4 specular = _SpecularColor * specularIntensitySmooth;
                float4 light = _LightColor0 * lightIntensity;
                float4 rim = _RimColor * rimIntensity;


                UNITY_APPLY_FOG(i.fogCoord, col);
                float4 ambient = _AmbientColor + light + specular + rim;
                col = col * ambient * _Color;
                if (rimIntensity > 0)
                {
                    col = _RimColor;
                }
                return col;
            }
            ENDCG
        }
    }
}