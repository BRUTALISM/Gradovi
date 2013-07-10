using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Encapsulates data about the environment the city generator will be performing in. Things like terrain data,
/// population density, etc.
/// </summary>
public class Environment {
	public Vector3 origin;
	
	private QuadTree<MapNode> mapNodeQuadTree = new QuadTree<MapNode>(-500f, 500f, -500f, 500f);
	public QuadTree<MapNode> MapNodeQuadTree { get { return mapNodeQuadTree; } }
	
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
	
	public void AddMapNode(MapNode node) {
		mapNodeQuadTree = mapNodeQuadTree.Add(node);
	}
	
	public List<MapNode> GetNeighbours(MapNode node, float radius) {
		// FIXME: Implement.
		return new List<MapNode>();
	}
	
	public void Clear() {
		mapNodeQuadTree.Clear();
		mapNodeQuadTree = new QuadTree<MapNode>(-500f, 500f, -500f, 500f);
	}
}