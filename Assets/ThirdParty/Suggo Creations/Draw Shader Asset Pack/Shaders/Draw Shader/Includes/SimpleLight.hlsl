#ifndef SIMPLE_LIGHTING_INCLUDED
#define SIMPLE_LIGHTING_INCLUDED

struct SimpleLightingData {
	float3 albedo;
	float3 light;
};

float3 SimpleLightHandling(SimpleLightingData d){
		float3 color = d.albedo * d.light;
		return color;
}

float3 CalculateSimpleLighting(SimpleLightingData d) {
		return SimpleLightHandling(d);
}

void SimpleLight_float(float3 Albedo, float3 light, out float3 Color){
		SimpleLightingData d;
		d.albedo = Albedo;
		d.light = light;
		Color = CalculateSimpleLighting(d);
	}

#endif