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
		
		// Create a map edge between the current map node and the newly spawned node
		MapEdge spawnedEdge = new MapEdge(Node, spawn);
		
		// Perform the intersection of the spawned node's edge against the neighbours' edges
		bool intersected = Intersect(spawn, spawnedEdge, neighbours);
		
		bool merged = false;
		if (!intersected) {
			// Check if the node is close to one of its neighbours so that they can be merged into one node
			foreach (MapNode neighbour in neighbours) {
				if (Vector3.Distance(spawn.position, neighbour.position) <= environment.nodeMergingMaximumDistance) {
					// The spawned node will be merged with the neighbour, modify spawnedEdge's ToNode to point to it
					spawnedEdge.ToNode = neighbour;
					
					// Let's not forget to update the neighbour's edge list
					neighbour.edges.Add(spawnedEdge);
					
					// Stop iterating
					merged = true;
					break;
				}
			}
		}
		
		if (!merged) {
			// Add the newly created map node to the environment
			environment.AddMapNode(spawn);
			
			// Continue producing only if there were no intersections
			if (!intersected) {
				// Summon a branch atom
				Atom branch = new BranchAtom(this);
				branch.Node = spawn;
				
				// Add the branch atom to the list of results and we're done
				production.Add(branch);
			}
		}
		
		return production;
	}
	
	private bool Intersect(MapNode spawn, MapEdge spawnedEdge, List<MapNode> neighbours) {
		// We'll need a list of all intersections we found for the given edge - in the general case there might be
		// several intersections so we must find the one closest to the starting node (this.Node)
		List<MapNode> intersectionNodes = null;
		
		// Iterate through all neighbours
		foreach (MapNode neighbour in neighbours) {
			foreach (MapEdge neighboursEdge in neighbour.edges) {
				// Try to find an intersection
				MapNode intersection = spawnedEdge.Intersection(neighboursEdge);
				if (intersection != null) {
					// We found an intersection, add it to the list
					if (intersectionNodes == null) intersectionNodes = new List<MapNode>();
					intersectionNodes.Add(intersection);
					
					// Also add the edge which generated the intersection to its edge list. Technically this is invalid
					// behavior (since the intersection lies somewhere *on* that edge), but it's only temporarily there
					// until we reconnect the nodes and edges properly in the next part of the algorithm.
					intersection.edges.Add(neighboursEdge);
				}
			}
		}
		
		if (intersectionNodes != null) {
			// Go through all intersections and find the one closest to this.Node
			MapNode chosenIntersection = null;
			float minimumDistance = float.MaxValue;
			foreach (MapNode intersection in intersectionNodes) {
				float distance = Node.Distance(intersection);
				if (distance < minimumDistance) {
					chosenIntersection = intersection;
					minimumDistance = distance;
				}
			}
			
			// Move the spawn's position to be the same as chosen intersection's
			spawn.position = chosenIntersection.position;
			
			// Get the edge that generated this intersection
			MapEdge neighboursEdge = chosenIntersection.edges[0];
			
			// Fix the "invalid behavior" mentioned above
			chosenIntersection.edges.Clear();
			
			// Split the edge in two at the intersection
			MapNode neighboursFromNode = neighboursEdge.FromNode;
			MapNode neighboursToNode = neighboursEdge.ToNode;
			
			neighboursFromNode.edges.Remove(neighboursEdge);
			new MapEdge(neighboursFromNode, spawn);
			
			neighboursToNode.edges.Remove(neighboursEdge);
			new MapEdge(spawn, neighboursToNode);
			
			// Done
			intersectionNodes.Clear();
			return true;
		}
		
		return false;
	}
}