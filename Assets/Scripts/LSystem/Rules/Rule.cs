using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Rule {
	public delegate float FloatProber(Environment env, Vector3 position);

	protected const int NumberOfProbes = 5;

	public Rule() {}
	
	public abstract List<RoadAtom> SpawnRoads(BranchAtom currentAtom, Environment env);
	
	public virtual float CalculateRoadLength(RoadAtom currentAtom, Environment env) {
		float lengthFactor = 1f - env.populationDensity.NormalizedDensityAt(currentAtom.Node.position);
		return env.minimumRoadLength + lengthFactor * (env.maximumRoadLength - env.minimumRoadLength);
	}

	/// <summary>
	/// Probes the environment by spawning a <paramref name="numberOfProbes"/> probes at the given
	/// <paramref name="worldPosition"/> around the given <paramref name="direction"/>, using the given
	/// <paramref name="prober"/> delegate to query the wanted result from the environment at the given coordinates. The
	/// probing vectors are spawned around the given direction vector, such that
	/// floor(<paramref name="numberOfProbes"/>/2) vectors are spawned on the left and right sides of the direction
	/// vector, and one probe which is aligned with the direction vector. The angle between probing vectors is such that
	/// the angle between the leftmost (and rightmost) vector and the direction vector is exactly <paramref name="env"/>
	/// .maximumRoadDeviationDegrees. The length of each probe is <paramref name="env"/>.minimumRoadLength.
	/// </summary>
	/// <returns>The float.</returns>
	/// <param name="worldPosition">World position.</param>
	/// <param name="direction">Direction.</param>
	/// <param name="env">Env.</param>
	/// <param name="prober">Prober.</param>
	/// <param name="numberOfProbes">Number of probes.</param>
	protected float[] ProbeFloat(Vector3 worldPosition, Vector3 direction, Environment env, FloatProber prober,
	                             int numberOfProbes = NumberOfProbes) {
		float[] probedValues = new float[numberOfProbes];

		float angleIncrement = env.maximumRoadDeviationDegrees / (numberOfProbes / 2);
		for (int i = 0, angleIndex = - numberOfProbes / 2; i < numberOfProbes; i++, angleIndex++) {
			// Rotate the current probe relative to the given direction based on i and angleIncrement
			Vector3 probeDirection = Quaternion.AngleAxis(angleIndex * angleIncrement, Vector3.up) *
				direction.normalized;

			// Calculate the exact position of the probe
			// (We use env.minimumRoadLength as a good candidate for probe length, it might be made into a configurable
			// editor option at some point.)
			Vector3 probePosition = worldPosition + probeDirection * env.minimumRoadLength;

			// Now probe
			probedValues[i] = prober(env, probePosition);
		}

		return probedValues;
	}

	public static float ElevationProber(Environment env, Vector3 position) {
		return env.ElevationAt(position.x, position.z);
	}

	public static float DensityProber(Environment env, Vector3 position) {
		return env.populationDensity.DensityAt(position);
	}
}