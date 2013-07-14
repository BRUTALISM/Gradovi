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
		List<Atom> production = new List<Atom>();
		
		// Create a new map node
		MapNode spawn = new MapNode(Node.position + forward * environment.Rule.CalculateRoadLength(this, environment));
		
		// Fetch the spawned node's neighbours
		List<MapNode> neighbours = environment.GetNeighbours(spawn, environment.neighboursSearchRadius);
		bool mergedWithNeighbour = false;
		foreach (MapNode neighbour in neighbours) {
			// Check if the node is close to the neighbour so that they can be merged into one node
			if (Vector3.Distance(spawn.position, neighbour.position) <= environment.nodeMergingMaximumDistance) {
				// The spawned node will be merged with the neighbour, just create a new edge from the starting node
				// to the neighbour and stop producing
				new MapEdge(Node, neighbour);
				mergedWithNeighbour = true;
				break;
			}
		}
		
		if (!mergedWithNeighbour) {
			// Create a map edge between the current map node and the newly spawned node
			new MapEdge(Node, spawn);
			
			// Add the newly created map node to the environment
			environment.AddMapNode(spawn);
			
			// TODO: Check if the newly generated edge intersects another edge
			
			// For now, the road atom only summons a branch atom
			Atom branch = new BranchAtom(this);
			branch.Node = spawn;
			
			// Add the branch atom to the list of results and that's it
			production.Add(branch);
		}
		
		return production;
	}
}