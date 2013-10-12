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
	
	[SerializeField]
	private Environment environment;
	public Environment Environment { get { return environment; } }
	
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
	
	/// <summary>
	/// Runs a one-step production of the underlying L-system.
	/// </summary>
	public void Produce() {
		// Go through atoms in the current generation and produce them all
		List<Atom> nextGeneration = new List<Atom>();
		foreach (Atom atom in currentGeneration) {
			// Check if the area is populated enough
			if (Environment.DensityAt(atom.Node.position) < Environment.populationDensityMinimum) {
				continue;
			}

			// Produce the atom and add the product to the next gen
			nextGeneration.AddRange(atom.Produce(Environment));
		}
		
		currentGeneration = nextGeneration;
		
		actualGenerations++;
		
#if UNITY_EDITOR
		RefillCachedData();
#endif
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

		if (environment.populationDensity != null) {
			// Draw population texture bounds
			const float height = 100f;
			Vector3 size = new Vector3(environment.populationDensity.width * environment.populationDensityScale, height,
			                           environment.populationDensity.height * environment.populationDensityScale);
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
		traversalQueue.Enqueue(environment.MapNodeTree);
		quadTrees.Add(environment.MapNodeTree);
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
		
		Environment.Clear();
	}
#endif
}
