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
	/// The radius within which a node has to be considered another node's neighbour. Keep this value greater than
	/// <see cref="maximumRoadLength"/> to insure production algorithm doesn't miss road intersections (roads cross but
	/// there's no node on that position).
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

	/// <summary>
	/// The maximum angle roads can deviate from the "perfect" route (the one which the production system would produce
	/// if there weren't any limiting factors such as water, steep hills, etc).
	/// </summary>
	public float maximumRoadDeviationDegrees = 30f;

	/// <summary>
	/// How much the slope of the road influences its length. See the <see cref="Rule"/> class'
	/// <see cref="CalculateRoadLength"/> method for details. Tweak until satisfied.
	/// </summary>
	public float slopeExaggeration = 5f;
	
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
	/// <param name="position">The position.</param>
	public Rule RuleAtCoordinates(Vector3 position) {
		// FIXME: Implement. Read rule switching information from a bitmap.
		return RectangularRule.Instance;
	}

	/// <summary>
	/// Returns terrain elevation at the given x and z coordinates.
	/// </summary>
	/// <returns>The <see cref="System.Single"/>The elevation.</returns>
	/// <param name="position">The position.</param>
	public float ElevationAt(Vector3 position) {
		RaycastHit hit;
		if (Physics.Raycast(new Vector3(position.x, 1000f, position.z), Vector3.down, out hit)) {
			return hit.point.y;
		}

		return 0f;
	}

	/// <summary>
	/// Returns the percentage (in the [0f, 1f] range) of the terrain slope at the given x and z coordinates, with 0f
	/// being completely flat and 1f being completely vertical.
	/// </summary>
	/// <returns>The <see cref="System.Single"/>The slope percentage.</returns>
	/// <param name="position">The position.</param>
	public float SlopePercentageAt(Vector3 position) {
		RaycastHit hit;
		if (Physics.Raycast(new Vector3(position.x, 1000f, position.z), Vector3.down, out hit)) {
			return Mathf.Abs(Vector3.Dot(hit.normal.normalized, Vector3.up));
		}

		return 0f;
	}
}