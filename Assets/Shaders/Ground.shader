// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Ground"
{
    Properties
    {
        _Grass ("Texture", 2D) = "white" {}
        _Mud ("Texture", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 pos : POSITION;
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed _TileType[900];

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 pos : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _Grass;
            float4 _Grass_ST;
            sampler2D _Mud;
            float4 _Mud_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Grass);
                o.pos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * 0.96666666666;

                int x = uv.x * 30;
                int y = uv.y * 30;

                float dx = (uv.x * 30.0) % 1;
                float dy = (uv.y * 30.0) % 1;

                float a = _TileType[(x * 30) + y];
                float b = _TileType[((x + 1) * 30) + y];
                float c = _TileType[(x * 30) + (y + 1)];
                float d = _TileType[((x + 1) * 30) + (y + 1)];

                float type = (1 - dx) * (
                    (1 - dy) * a + dy * c
                ) + dx * (
                    (1 - dy) * b + dy * d
                );

                fixed4 grass = tex2D(_Grass, i.pos * 3);
                fixed4 mud = tex2D(_Mud, i.pos * 3);

                return type < 0.5 ? grass : mud;
            }
            ENDCG
        }
    }
}
