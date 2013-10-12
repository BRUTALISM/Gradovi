using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Encapsulates data about the environment the city generator will be performing in. Things like terrain data,
/// population density, etc.
/// </summary>
[Serializable]
public class Environment {
	public enum RuleType {
		Radial,
		Rectangular,
		Mixed
	}

	public Vector3 Origin {
		get {
			return generator != null ? generator.transform.position : Vector3.zero;
		}
	}

	[SerializeField]
	private Transform generator;
	
	private QuadTree<MapNode> mapNodeTree = new QuadTree<MapNode>(-500f, 500f, -500f, 500f);
	public QuadTree<MapNode> MapNodeTree { get { return mapNodeTree; } }

	/// <summary>
	/// The type of rule used to generate the map.
	/// </summary>
	public RuleType ruleType;

	/// <summary>
	/// The texture which represents the population density. White is densest, black is no population.
	/// </summary>
	public Texture2D populationDensity;

	/// <summary>
	/// How much is the population density texture stretched over the terrain. A value of 1 means that 1 pixel
	/// represents 1 meter of terrain.
	/// </summary>
	public float populationDensityScale = 1f;

	/// <summary>
	/// The population density minimum. If the population is below this value at a given coordinate, nothing is
	/// produced.
	/// </summary>
	public float populationDensityMinimum = 0.01f;
	
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
		switch (ruleType) {
		case RuleType.Radial:
			return RadialRule.Instance;
		case RuleType.Rectangular:
			return RectangularRule.Instance;
		case RuleType.Mixed:
			// FIXME: Implement. Read rule switching information from a bitmap.
		default:
			return RadialRule.Instance;
		}
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

	public float DensityAt(Vector3 position) {
		// If no population texture is set, just return max density
		if (populationDensity == null) return 1f;

		// Convert position to local coordinates
		Vector3 localPosition = position - Origin;

		// If the position exceeds the area of the population texture, assume no population
		if (Mathf.Abs(localPosition.x) > populationDensityScale * populationDensity.width / 2 ||
		    Mathf.Abs(localPosition.z) > populationDensityScale * populationDensity.height / 2) return 0f;

		// Fetch the value of the pixel at the given coordinates, taking into account populationDensityScale
		int x = Mathf.FloorToInt(populationDensity.width / 2 + (localPosition.x / populationDensityScale));
		int z = Mathf.FloorToInt(populationDensity.height / 2 + (localPosition.z / populationDensityScale));
		Color color = populationDensity.GetPixel(x, z);

		// Average the colors, this is our density
		return (color.r + color.g + color.b) / 3;
	}
}