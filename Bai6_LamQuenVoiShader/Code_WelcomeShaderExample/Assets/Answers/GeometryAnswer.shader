Shader "Unlit/GeometryAnswer"
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
                if( v.vertex.y < 0)
                {
                    float2 xz = v.vertex.xz * _BottomScale;
                    o.pos = float4(xz.x, v.vertex.y, xz.y, 1.0);
                } else {
                    o.pos = v.vertex;
                }
                
                o.uv = v.uv;
                o.normal = v.normal;
                //
                float3 worldViewDir = UnityWorldSpaceViewDir(v.vertex.xyz);
                o.viewDir = normalize(worldViewDir);
                return o;
            }

            centerQuad centerOfQuad(v2g input[3])
            {
                float4 ab = input[1].pos - input[0].pos;
                float4 ac = input[2].pos - input[0].pos;
                float4 bc = input[2].pos - input[1].pos;

                float abLength = length(ab);
                float acLength = length(ac);
                float bcLength = length(bc);

                centerQuad res;

                if(abLength > acLength && abLength > bcLength)
                {
                    //ab
                    res.center.pos = (input[0].pos + input[1].pos) / 2;
                    res.center.uv = (input[0].uv + input[1].uv) / 2;
                    res.center.normal = (input[0].normal + input[1].normal) / 2;
                    //
                    res.index = 2;

                } else if(acLength > abLength && acLength > bcLength)
                {
                    //ac
                    res.center.pos = (input[0].pos + input[2].pos) / 2;
                    res.center.uv = (input[0].uv + input[2].uv) / 2;
                    res.center.normal = (input[0].normal + input[2].normal) / 2;
                    //
                    res.index = 1;
                } else 
                {
                    //bc
                    res.center.pos = (input[1].pos + input[2].pos) / 2;
                    res.center.uv = (input[1].uv + input[2].uv) / 2;
                    res.center.normal = (input[1].normal + input[2].normal) / 2;
                    //
                    res.index = 0;
                }

                return res;
            };

            g2f G2fFromV2G(v2g input)
            {
                 g2f o;
                 o.pos = UnityObjectToClipPos(input.pos); // Transform to clip space
                 o.uv = input.uv;
                 o.normal = UnityObjectToWorldNormal(input.normal);

                 return o;
            };

            bool IsClockWise(float3 a, float3 b, float3 c, float3 view)
            {
                float3 ab = normalize(b - a);
                float3 ac = normalize(c - a);
                //
                float3 n = cross(ab, ac);

                float test = dot(n, view);
                return test > 0;
            };

            // Geometry Shader
            [maxvertexcount(6)] // Specifies max number of vertices the GS can output
            void geom(triangle v2g input[3], inout TriangleStream<g2f> triStream)
            {
                // This is where you manipulate or generate new geometry.
                // The 'input' is an array of 3 vertices (for a triangle input).
                if(input[0].pos.y > 0 && input[1].pos.y > 0 && input[2].pos.y > 0)
                {
                    centerQuad center = centerOfQuad(input);
                    center.center.pos = center.center.pos + float4(center.center.normal * _Height, 1.0);
                    g2f c = G2fFromV2G(center.center);
                    g2f a = G2fFromV2G(input[center.index]);
                    //

                    for (float i = 0; i < 3; i++)
                    {
                        if(i != center.index)
                        {
                            g2f p = G2fFromV2G(input[i]);
                            if(IsClockWise(a.pos.xyz, c.pos.xyz, p.pos.xyz, input[i].viewDir))
                            {
                                
                                triStream.Append(a);
                                triStream.Append(c);
                                triStream.Append(p);
                            } else {
                    
                                triStream.Append(a);
                                triStream.Append(p);
                                triStream.Append(c);
                            }
                            
                        }
                    }

                } else 
                {
                    for (float i = 0; i < 3; i++)
                    {
                        triStream.Append(G2fFromV2G(input[i]));
                    } 
                }

                triStream.RestartStrip(); // Marks the end of the current primitive
            }

            fixed4 frag (g2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = float4(abs(i.normal), 1);
                if(i.uv.x < _BorderWidth) col = float4(0,0,0,0);
                if(i.uv.x > 1 - _BorderWidth) col = float4(0,0,0,0);
                if(i.uv.y < _BorderWidth) col = float4(0,0,0,0);
                if(i.uv.y > 1 - _BorderWidth) col = float4(0,0,0,0);
                return col;
            }
            ENDCG
        }
    }
}
