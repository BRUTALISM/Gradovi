using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapNode {
	public List<MapEdge> edges;
	public Vector3 position;
	
	public MapNode(Vector3 position) {
		edges = new List<MapEdge>();
		this.position = position;
	}
}
