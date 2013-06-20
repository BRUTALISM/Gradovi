using UnityEngine;
using System.Collections;

public enum DensityFalloff {
	Linear,
	Quadratic,
	Exponential
}

public class PopulationDensity : MonoBehaviour {
	public float radius = 1000f;
	public float densityAtCenter = 100;
	public DensityFalloff densityFalloff = DensityFalloff.Linear;
	
	void OnDrawGizmos() {
		Gizmos.DrawWireSphere(transform.position, radius);
	}
	
	public float DensityAt(Vector3 worldPosition) {
		switch (densityFalloff) {
		case DensityFalloff.Linear:
			return Mathf.Clamp((transform.InverseTransformPoint(worldPosition) - transform.position).magnitude / radius, 0f, 1f) *
				densityAtCenter;
		case DensityFalloff.Quadratic:
			// FIXME: Implement.
			return 1f;
		case DensityFalloff.Exponential:
			// FIXME: Implement.
			return 1f;
		default:
			return 0f;
		}
	}
}
