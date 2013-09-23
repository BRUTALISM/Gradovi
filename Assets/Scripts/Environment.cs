using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Encapsulates data about the environment the city generator will be performing in. Things like terrain data,
/// population density, etc.
/// </summary>
[Serializable]
public class Environment {
	public Vector3 Origin {
		get {
			return populationDensity != null ? populationDensity.transform.position : Vector3.zero;
		}
	}
	
	private QuadTree<MapNode> mapNodeTree = new QuadTree<MapNode>(-500f, 500f, -500f, 500f);
	public QuadTree<MapNode> MapNodeTree { get { return mapNodeTree; } }
	
	public PopulationDensity populationDensity;

	/// <summary>
	/// The population density minimum. If the population is below this value at a given coordinate, nothing is
	/// produced.
	/// </summary>
	public float populationDensityMinimum = 1f;
	
	/// <summary>
	/// The radius within which a node has to be considered another node's neighbour.
	/// </summary>
	public float neighboursSearchRadius = 50f;
	
	/// <summary>
	/// The distance between two nodes below which the nodes will be merged into one (while preserving edges).
	/// </summary>
	public float nodeMergingMaximumDistance = 30f;

	/// <summary>
	/// The minimum length of the generated roads. Note that roads might be shorter than this value if an intersection
	/// occurs. This variable only controls the minimum length of the roads as they exit the production, before the
	/// intersection algorithm runs.
	/// </summary>
	public float minimumRoadLength = 5f;
	
	/// <summary>
	/// The maximum length of generated roads. Keep this value under <c>neighboursSearchRadius</c> if you want proper
	/// road intersection detection.
	/// </summary>
	public float maximumRoadLength = 50f;
	
	public void AddMapNode(MapNode node) {
		mapNodeTree = mapNodeTree.Add(node);
	}
	
	public List<MapNode> GetNeighbours(MapNode node, float radius) {
		return mapNodeTree.GetNeighbours(node, radius);
	}
	
	public void Clear() {
		mapNodeTree.Clear();
		mapNodeTree = new QuadTree<MapNode>(-500f, 500f, -500f, 500f);
	}

	/// <summary>
	/// Gets the rule which should be used to continue producing the L-system at the given coordinates.
	/// </summary>
	/// <returns>The rule at given coordinates.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	public Rule RuleAtCoordinates(float x, float z) {
//		if (Mathf.Sqrt(x * x + z * z) < 200f) return RectangularRule.Instance;
//		else return RadialRule.Instance;
		return RadialRule.Instance;
	}

	public float ElevationAt(float x, float z) {
		RaycastHit hit;
		if (Physics.Raycast(new Vector3(x, 1000f, z), Vector3.down, out hit)) {
			return hit.point.y;
		}

		return 0f;
	}

	public float SlopeAt(float x, float z) {
		// FIXME: Implement. Should return the slope coefficient of the terrain at the given point. (Raycast, normal, ...)
		return 0f;
	}
}