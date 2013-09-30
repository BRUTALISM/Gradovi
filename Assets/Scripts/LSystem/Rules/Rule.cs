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
	/// <returns>The array of (Vector3, float) pairs representing the coordinate of each probe and the probed value,
	/// respectively.</returns>
	/// <param name="worldPosition">World position.</param>
	/// <param name="direction">Direction.</param>
	/// <param name="env">Env.</param>
	/// <param name="prober">Prober.</param>
	/// <param name="numberOfProbes">Number of probes.</param>
	protected KeyValuePair<Vector3, float>[] ProbeFloat(Vector3 worldPosition, Vector3 direction, Environment env,
	                                                    FloatProber prober, int numberOfProbes = NumberOfProbes) {
		KeyValuePair<Vector3, float>[] probedValues = new KeyValuePair<Vector3, float>[numberOfProbes];

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
			probedValues[i] = new KeyValuePair<Vector3, float>(probePosition, prober(env, probePosition));
		}

		return probedValues;
	}

	public static float ElevationProber(Environment env, Vector3 position) {
		return env.ElevationAt(position.x, position.z);
	}

	public static float DensityProber(Environment env, Vector3 position) {
		return env.populationDensity.DensityAt(position);
	}

	/// <summary>
	/// Returns the direction of the least steep road among all probed roads.
	/// </summary>
	/// <returns>The direction of the road.</returns>
	/// <param name="position">Position.</param>
	/// <param name="roadDirection">Road direction.</param>
	/// <param name="currentElevation">Current elevation.</param>
	/// <param name="env">Environment.</param>
	protected Vector3 LeastSteepDirection(Vector3 position, Vector3 roadDirection, float currentElevation,
	                                      Environment env) {
		// Probe elevations around the given direction
		KeyValuePair<Vector3, float>[] elevationPairs = ProbeFloat(position, roadDirection, env, ElevationProber);

		// Find the location which has a minimum elevation difference relative to the current node's position
		KeyValuePair<Vector3, float> minimumPair;
		float minimumElevationDelta = float.MaxValue;
		foreach (KeyValuePair<Vector3, float> elevationPair in elevationPairs) {
			if (Mathf.Abs(currentElevation - elevationPair.Value) < minimumElevationDelta) {
				minimumElevationDelta = Mathf.Abs(currentElevation - elevationPair.Value);
				minimumPair = elevationPair;
			}
		}

		// We have the end location of the least steep road, now make it into a direction
		return (minimumPair.Key - position).normalized;
	}
}