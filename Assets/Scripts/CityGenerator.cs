using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// City generator.
/// </summary>
[ExecuteInEditMode]
public class CityGenerator : MonoBehaviour {
	/// <summary>
	/// How many generations this generator was set up to produce.
	/// </summary>
	public int targetGenerations;

	public enum RuleType {
		Radial,
		Rectangular,
		Mixed
	}

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
	
	/// <summary>
	/// How many generations this generator currently has produced. If this number is smaller than
	/// <c>targetGenerations</c>, the <c>currentGeneration</c> is produced a number of times, until the target number
	/// of generations is reached.
	/// </summary>
	private int actualGenerations;
	
	/// <summary>
	/// The current generation of atoms, produced <c>actualGenerations</c> number of times.
	/// </summary>
	private List<Atom> currentGeneration;
	
	/// <summary>
	/// The root node of the street graph. This is the axiom's <c>Node</c> property.
	/// </summary>
	public MapNode RootNode { get; private set; }

	void Update() {
		if (Application.isPlaying) {
			// Play mode
		} else {
			// Edit mode
			UpdateInEditMode();
		}
	}
	
	private void UpdateInEditMode() {
		if (actualGenerations > targetGenerations) {
			int rememberTargetGenerations = targetGenerations;
			Reset();
			targetGenerations = rememberTargetGenerations;
		}

		ProduceIfNecessary();
		
		// TODO: Check if the Y coordinate of the generator is 0, this has to be true at all times!
	}

	public void AddMapNode(MapNode node) {
		mapNodeTree = mapNodeTree.Add(node);
	}

	public List<MapNode> GetNeighbours(MapNode node, float radius) {
		return mapNodeTree.GetNeighbours(node, radius);
	}
	
	/// <summary>
	/// Runs a one-step production of the underlying L-system.
	/// </summary>
	public void Produce() {
		// Go through atoms in the current generation and produce them all
		List<Atom> nextGeneration = new List<Atom>();
		foreach (Atom atom in currentGeneration) {
			// Check if the area is populated enough
			if (DensityAt(atom.Node.position) < populationDensityMinimum) {
				continue;
			}

			// Produce the atom and add the product to the next gen
			nextGeneration.AddRange(atom.Produce(this));
		}
		
		currentGeneration = nextGeneration;
		
		actualGenerations++;
		
#if UNITY_EDITOR
		RefillCachedData();
#endif
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
		Vector3 localPosition = position - transform.position;

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
	
	public void Reset() {
		currentGeneration = null;
		RootNode = null;
		
		targetGenerations = actualGenerations = 0;
		
#if UNITY_EDITOR
		ClearCachedData();
#endif
	}
	
	private void ProduceIfNecessary() {
		// Check if we're here for the first time
		if (currentGeneration == null) {
			// Initialize the list
			currentGeneration = new List<Atom>();
			
			// Create the axiom
			// TODO: Add a configurable axiom
			Atom axiom = new BranchAtom(null);
			axiom.Node = new MapNode(transform.position);
			RootNode = axiom.Node;

			// Find out correct height for it
			RaycastHit hit;
			if (Physics.Raycast(transform.position + Vector3.up * 1000f, Vector3.down, out hit)) {
				axiom.Node.position.y = hit.point.y + 0.1f;
			}

			currentGeneration.Add(axiom);

			actualGenerations = 0;
		}
		
		// Check if we need to produce the L-system
		while (actualGenerations < targetGenerations) {
			Produce();
		}
	}
	
#if UNITY_EDITOR
	// Cached data for drawing in the scene view
	private HashSet<MapNode> intersections = new HashSet<MapNode>();
	private HashSet<MapEdge> roads = new HashSet<MapEdge>();
	private HashSet<QuadTree<MapNode>> quadTrees = new HashSet<QuadTree<MapNode>>();
	
	public void OnDrawGizmos() {
		// Draw all roads
		Gizmos.color = Color.red;
		foreach (MapEdge road in roads) {
			Gizmos.DrawLine(road.FromNode.position, road.ToNode.position);
		}
		
		// Draw all intersections
		foreach (MapNode intersection in intersections) {
//			Gizmos.color = Color.red;
			Gizmos.DrawSphere(intersection.position, 1f);
//			Gizmos.color = Color.green;
//			Gizmos.DrawLine(intersection.position, intersection.position +
//			                Vector3.up * Environment.populationDensity.DensityAt(intersection.position));
		}

		if (populationDensity != null) {
			// Draw population texture bounds
			const float height = 100f;
			Vector3 size = new Vector3(populationDensity.width * populationDensityScale, height,
			                           populationDensity.height * populationDensityScale);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(transform.position - Vector3.down * height / 2, size);
		}
		
		// Draw quadtrees from the environment
//		Gizmos.color = Color.cyan;
//		foreach (QuadTree<MapNode> quadTree in quadTrees) {
//			Vector3 center = new Vector3(quadTree.CenterX, 0f, quadTree.CenterY);
//			Vector3 size = new Vector3(quadTree.MaximumX - quadTree.MinimumX, 1f,
//				quadTree.MaximumY - quadTree.MinimumY);
//			Gizmos.DrawWireCube(center, size);
//		}
	}
	
	private void RefillCachedData() {
		// TODO: Very inefficient!
		intersections.Clear();
		roads.Clear();
		
		// Do a flood fill of the city graph and cache all nodes and edges into local hashsets
		Queue<MapNode> waitingNodes = new Queue<MapNode>();
		waitingNodes.Enqueue(RootNode);
		
		while (waitingNodes.Count > 0) {
			MapNode currentNode = waitingNodes.Dequeue();
			foreach (MapEdge edge in currentNode.edges) {
				roads.Add(edge);
				
				MapNode oppositeNode = (currentNode == edge.FromNode ? edge.ToNode : edge.FromNode);
				if (!intersections.Contains(oppositeNode)) {
					waitingNodes.Enqueue(oppositeNode);
					intersections.Add(oppositeNode);
				}
			}
		}
		
		// Traverse the quad tree hierarchy and fill the hash set
		Queue<QuadTree<MapNode>> traversalQueue = new Queue<QuadTree<MapNode>>();
		traversalQueue.Enqueue(mapNodeTree);
		quadTrees.Add(mapNodeTree);
		while (traversalQueue.Count > 0) {
			QuadTree<MapNode> tree = traversalQueue.Dequeue();
			if (tree.SubtreesReadOnly != null) {
				foreach (QuadTree<MapNode> subtree in tree.SubtreesReadOnly) {
					traversalQueue.Enqueue(subtree);
					quadTrees.Add(subtree);
				}
			}
		}
	}
	
	private void ClearCachedData() {
		intersections.Clear();
		roads.Clear();
		quadTrees.Clear();
		
		mapNodeTree.Clear();
		mapNodeTree = new QuadTree<MapNode>(-500f, 500f, -500f, 500f);
	}
#endif
}
