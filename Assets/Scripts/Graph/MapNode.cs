using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapNode : ICoordinate2D {
	public List<MapEdge> edges;
	public Vector3 position;
	
	// ICoordinate2D implementation
	public float X { get { return position.x; } }
	public float Y { get { return position.z; } }
	
	public MapNode(Vector3 position) {
		edges = new List<MapEdge>();
		this.position = position;
	}
}
