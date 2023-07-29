// XNA 4.0 Shader Programming #1 - Ambient light

// Matrix
float4x4 World;
float4x4 View;
float4x4 Projection;

// Light related
float4 AmbientColor;
float AmbientIntensity;

// The input for the VertexShader
struct VertexShaderInput
{
    float4 Position : POSITION0;
};

// The output from the vertex shader, used for later processing
struct VertexShaderOutput
{
    float4 Position : POSITION0;
};

// The VertexShader.
//VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
//{
//    VertexShaderOutput output;
//
//    float4 worldPosition = mul(input.Position, World);
//    float4 viewPosition = mul(worldPosition, View);
//    output.Position = mul(viewPosition, Projection);
//
//    return output;
//}

float3 CouleurRVB(in int Hex)
{
    // 0xABCDEF
    int AB = (Hex & 0x00FF0000) >> 16;
    int CD = (Hex & 0x0000FF00) >> 8;
    int EF = Hex & 0x000000FF;
    return pow(float3(AB, CD, EF)/255.0, float3(2.2));
}
float3 ColorRamp_Constant(in float4 T, float4 A, in float4 B, in float4 C, in float4 D)
{
    if(T.r < B.w) return A.xyz;
    if(T.g < C.w) return B.xyz;
    if(T.b < D.w) return C.xyz;
    return D.xyz;
}


// The Pixel Shader
float4 PixelShaderFunction(VertexShaderOutput fragCoord) : COLOR0
{
    float4 color = tex2D(inputTexture, uv);
    // Normalized pixel coordinates (from 0 to 1)
    float2 uv = fragCoord.xy / iResolution.xy;
//    float2 mousePos = iMouse.xy / iResolution.xy;
    float4 tex = texture(iChannel0, uv);
    float3 greyScale = float3(.5, .5, .5);
//    float2 centerPoint = float2(mousePos.x, mousePos.y);
    float2 centerPoint = float3(.5, .5)

//    float speed = 1.0;
//    float linewidth = 0.58;
//    float grad = 3.0;
//    float4 col1 = float4(0.3,0.0,0.0,1.0);
//    float4 col2 = float4(0.85,0.85,0.85,1.0);
//
//    float2 light = float2(cos(iTime*.5), sin(iTime*.5));
//
//    float2 direction = normalize(texture(iChannel1,uv).xy - .5);
//    float sparkle = dot(direction, light);
//
//    sparkle = step(.999,sparkle);

    // Sample texture...
    float4 t1 = texture(iChannel0, uv);

    // Plot line...
    float2 linepos = uv;
    linepos.x = linepos.x - mod(iTime*speed,2.0)+0.5;

    float y = linepos.x*grad;
	float s = smoothstep( y-linewidth, y, linepos.y) - smoothstep( y, y+linewidth, linepos.y);

    float4 A = float4(CouleurRVB(0x8B0504), 0.5);
    float4 B = float4(CouleurRVB(0xD35405), 0.5);
    float4 C = float4(CouleurRVB(0xFADE05), 0.75);
    float4 D = float4(CouleurRVB(0xF8EF74), 0.9);
    float T = uv.x * 1.1 - 0.05;


    float2 distanceVector = uv - centerPoint;
    float dist = sqrt(dot(distanceVector, distanceVector));
    float3 col = 0.5 + 0.5*cos(iTime+uv.xyx);
    //col = sin(iTime+uv.xyx);
    float4 color = float4(mousePos.x, mousePos.y, dist, dist);
//    if(dist < .15) {
     fragColor = float4( float3(dot( tex.rgb, greyScale)), tex.a);
     float3 Couleur = ColorRamp_Constant(fragColor, A, B, C, D);
     //fragColor = float4(0, tex.g, tex.b, tex.a);
     fragColor = float4(Couleur, tex.a);
     //fragColor += sparkle;
     //fragColor += ((s*col1)+(s*col2));
//    }
     // fragColor = float4( float3(dot( tex.rgb, greyScale)), tex.a);
//   else
//      fragColor = tex;
     //fragColor = float4(mousePos.x, mousePos.y, iMouse.z, iMouse.w);

    return fragColor;//AmbientColor*AmbientIntensity;
}

// Our Techinique
technique Technique1
{
    pass Pass1
    {
//        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
