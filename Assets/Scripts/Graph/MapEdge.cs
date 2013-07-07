using UnityEngine;
using System;
using System.Collections;

public enum EdgeType {
	URBAN,
	HIGHWAY
}

public class MapEdge {
	public EdgeType Type { get; set; }
	public MapNode FromNode { get; set; }
	public MapNode ToNode { get; set; }
	
	public MapEdge(MapNode a, MapNode b) {
		FromNode = a;
		ToNode = b;
		
		a.edges.Add(this);
		b.edges.Add(this);
	}
}
