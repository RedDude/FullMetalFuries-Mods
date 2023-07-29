sampler2D input : register(s0);

float4 main(float2 texCoord : TEXCOORD) : COLOR
{
    float4 color = tex2D(input, texCoord);
    return color;
}
