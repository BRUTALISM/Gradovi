using UnityEngine;
using System;
using System.Collections;

public enum EdgeType {
	HIGHWAY,
	URBAN
}

public class MapEdge {
	public EdgeType Type { get; set; }
	public MapNode FromNode { get; set; }
	public MapNode ToNode { get; set; }
}
