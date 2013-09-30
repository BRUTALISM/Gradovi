using UnityEngine;
using System.Collections;

public enum DensityFalloff {
	Linear,
	Quadratic,
}

public class PopulationDensity : MonoBehaviour {
	public float radius = 1000f;
	public float densityAtCenter = 100;
	public DensityFalloff densityFalloff = DensityFalloff.Linear;
	
	public float DensityAt(Vector3 worldPosition) {
		// 1f at center of population, 0f at the border, decreasing linearly
		float distanceFactor = Mathf.Clamp(1f - (transform.position - worldPosition).magnitude / radius, 0f, 1f);

		switch (densityFalloff) {
		case DensityFalloff.Linear:
			return distanceFactor * densityAtCenter;
		case DensityFalloff.Quadratic:
			return (distanceFactor * distanceFactor) * densityAtCenter;
		default:
			return 0f;
		}
	}

	public float NormalizedDensityAt(Vector3 worldPosition) {
		return Mathf.Clamp01(DensityAt(worldPosition) / densityAtCenter);
	}
}
