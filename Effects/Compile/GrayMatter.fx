﻿texture background;
sampler2D InputLayer0 = sampler_state
{
    texture = <background>;
};

texture map;
sampler2D InputLayer1 = sampler_state
{
    texture = <map>;
};

texture noise;
sampler2D InputLayer2 = sampler_state
{
    texture = <noise>;
    AddressU = wrap; 
    AddressV = wrap;
};

texture over;
sampler2D InputLayer3 = sampler_state
{
    texture = <over>;
};

float time;
float2 screensize;

float4 main(float2 uv : TEXCOORD0) : COLOR0
{
    float2 st = uv;
    float2 ns = st/screensize;
 
    float3 color = tex2D(InputLayer0,st).xyz;
    float distort = sin( min(1.0, tex2D(InputLayer1,st).x) * 3.14 ) * 0.1;
    float noise = tex2D(InputLayer2, (ns +  time * 0.1)).x;
    
    float push = noise * distort;
    st += push * 0.15;
    float3 final = tex2D(InputLayer0,st).xyz;
    
    float gray = (final.r + final.g + final.b) / 3.0;
    float power = min(1.0, tex2D(InputLayer1,st).x * 1.5 + push * 1.5);
    float3 grayscale = float3(gray, gray, gray) * power + final * (1.0 - power);
    final = grayscale;
    
    float3 chrome = float3(tex2D(InputLayer2, (ns +  time * 0.1)).x, tex2D(InputLayer2, (ns +  time * 0.2)).x, tex2D(InputLayer2, (ns +  time * 0.3)).x);
    final += push * chrome * 2.0;    
    final += push;   
    
    float4 overlay = tex2D(InputLayer3,st);
    overlay.a *= power;
    final.rgb = overlay.rgb * overlay.a + final.rgb * (1.0 - overlay.a);
    
    return float4(final,1.0);
}

technique Technique1
{
    pass GradientDistortionPass
    {
        PixelShader = compile ps_3_0 main();
    }
}