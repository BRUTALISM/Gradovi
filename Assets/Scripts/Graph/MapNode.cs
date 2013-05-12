using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapNode {
	public List<MapEdge> Edges { get; set; }
	public Vector3 Position { get; set; }
	public List<Atom> Atoms { get; set; }
}
