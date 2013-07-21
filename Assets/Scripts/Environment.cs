using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Encapsulates data about the environment the city generator will be performing in. Things like terrain data,
/// population density, etc.
/// </summary>
public class Environment {
	// TODO: Make this class serializable and customizable from the editor.
	
	public Vector3 origin;
	
	private QuadTree<MapNode> mapNodeTree = new QuadTree<MapNode>(-500f, 500f, -500f, 500f);
	public QuadTree<MapNode> MapNodeTree { get { return mapNodeTree; } }
	
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
	
	/// <summary>
	/// The radius within which a node has to be to be considered another node's neighbour.
	/// </summary>
	public float neighboursSearchRadius = 50f;
	
	/// <summary>
	/// The distance between two nodes below which the nodes will be merged into one (while preserving edges).
	/// </summary>
	public float nodeMergingMaximumDistance = 30f;
	
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
}