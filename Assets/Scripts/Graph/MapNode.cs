using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents one 2D node on the map. It has a list of edges, connecting the given instance to other nodes.
/// </summary>
public class MapNode : ICoordinate2D {
	public List<MapEdge> edges;
	public Vector3 position;
	
	public Vector2 PositionAsVector2 {
		get { return new Vector2(position.x, position.z); }
	}
	
	// ICoordinate2D implementation
	public float X { get { return position.x; } }
	public float Y { get { return position.z; } }
	
	public MapNode(Vector3 position) {
		edges = new List<MapEdge>();
		this.position = position;
	}
	
	public float Distance(MapNode other) {
		return Vector3.Distance(this.position, other.position);
	}

	public override string ToString() {
		return string.Format("[MapNode: PositionAsVector2={0}, X={1}, Y={2}]", PositionAsVector2, X, Y);
	}
}
