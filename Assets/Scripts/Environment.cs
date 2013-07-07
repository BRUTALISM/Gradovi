using System;
using UnityEngine;

/// <summary>
/// Encapsulates data about the environment the city generator will be performing in. Things like terrain data,
/// population density, etc.
/// </summary>
public class Environment {
	public Vector3 origin;
	
	/// <summary>
	/// This property acts as a factory method for returning the correct <c>Rule</c> instance based on the current
	/// environmental conditions.
	/// </summary>
	public Rule Rule {
		get {
			// FIXME: Implement properly.
//			if (/* nesto nesto == */ true) {
				return RadialRule.Instance;
//			} else {
//				return null;
//			}
		}
	}
	
	public PopulationDensity populationDensity;
}