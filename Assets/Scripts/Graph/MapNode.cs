using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapNode {
	public List<MapEdge> Edges;
	public Vector3 Position;
	
	public MapNode(Vector3 position) {
		Edges = new List<MapEdge>();
		Position = position;
	}
}
