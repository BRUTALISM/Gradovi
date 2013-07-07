using System;
using System.Collections.Generic;
using UnityEngine;

public class RoadAtom : Atom {
	/// <summary>
	/// This property is used as a break flag. As soon as it goes below zero, the road atom stops producing.
	/// </summary>
	// FIXME: Implement.
	public float DelayCount { get; set; }
	
	private Vector3 forward;
	
	public RoadAtom(Vector3 forward) : this(forward, null) {}
	
	public RoadAtom(Vector3 forward, MapNode node) {
		this.forward = forward.normalized;
		this.Node = node;
	}
	
	public override List<Atom> Produce(Environment environment) {
		// Create a new map node
		MapNode spawn = new MapNode(Node.position + forward * environment.Rule.CalculateRoadLength(this, environment));
		
		// Create a map edge between the current map node and the newly spawned node
		new MapEdge(Node, spawn);
		
		// TODO: Check if the newly generated edge intersects another edge, or is close to some existing branch.
		
		// For now, the road atom only summons a branch atom
		Atom branch = new BranchAtom(this);
		branch.Node = spawn;
		
		// Create the list of results and just add the branch atom to it
		List<Atom> production = new List<Atom>();
		production.Add(branch);
		return production;
	}
}