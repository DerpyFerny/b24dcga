Shader "Unlit/GeometryPractice"
{
    Properties
    {
        _BottomScale("Bottom Scale", Range (1, 3)) = 1
        _BorderWidth("Border Width", Range (0, 1)) = 1
        _Height("Pyramid Height", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal: NORMAL;
            };

            // Struct to pass data from vertex shader to geometry shader (v2g)
            struct v2g
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float3 viewDir: TEXCOORD1;
            };

            // Struct to pass data from geometry shader to fragment shader (g2f)
            struct g2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct centerQuad
            {
                float index;
                v2g center;
            };

            float _BottomScale;
            float _BorderWidth;
            float _Height;



            v2g vert (appdata v)
            {
                v2g o;
                //------Start Question 1--------
                o.pos = v.vertex;
                o.uv = v.uv;
                o.normal = v.normal;
                //------End Question 1--------
                return o;
            }

            //----------Start Question 3------------
            centerQuad centerOfQuad(v2g input[3])
            {
                centerQuad res;
                return res;
            };

            bool IsClockWise(float3 a, float3 b, float3 c, float3 view)
            {

                return false;
            };
            //----------End Question 3------------

            // Geometry Shader
            [maxvertexcount(3)] // Specifies max number of vertices the GS can output
            void geom(triangle v2g input[3], inout TriangleStream<g2f> triStream)
            {
                // This is where you manipulate or generate new geometry.
                // The 'input' is an array of 3 vertices (for a triangle input).

                //------------Start Question 4-------------
                for (float i = 0; i < 3; i++)
                {
                    g2f o;
                    o.pos = UnityObjectToClipPos(input[i].pos); // Transform to clip space
                    o.uv = input[i].uv;
                    o.normal = UnityObjectToWorldNormal(input[i].normal);
                    triStream.Append(o);
                } 
                //------------End Question 4-------------

                triStream.RestartStrip(); // Marks the end of the current primitive
            }

            fixed4 frag (g2f i) : SV_Target
            {
                //------------Start Question 2-------------
                // sample the texture
                fixed4 col = float4(1, 1, 1, 1);
                return col;
                //------------End Question 2-------------
            }
            ENDCG
        }
    }
}
