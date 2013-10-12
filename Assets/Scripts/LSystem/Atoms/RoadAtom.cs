using System;
using System.Collections.Generic;
using UnityEngine;

public class RoadAtom : Atom {
	private Vector3 forward;
	public Vector3 Forward { get { return forward; } }
	
	public RoadAtom(Vector3 forward) : this(forward, null) {}
	
	public RoadAtom(Vector3 forward, MapNode node) {
		this.forward = forward.normalized;
		this.Node = node;
	}

	public override List<Atom> Produce(CityGenerator generator) {
		List<Atom> production = new List<Atom>();
		
		// Create a new map node
		Rule rule = generator.RuleAtCoordinates(Node.position);
		MapNode spawn = new MapNode(Node.position + forward * rule.CalculateRoadLength(this, generator));
		
		// Fetch the spawned node's neighbours
		List<MapNode> neighbours = generator.GetNeighbours(spawn, generator.neighboursSearchRadius);
		
		// Check if the node is close to one of its neighbours so that they can be merged into one node
		bool merged = false;
		Vector2 spawnPosition = spawn.PositionAsVector2;
		foreach (MapNode neighbour in neighbours) {
			// Convert coords to 2D, we don't want elevation messing around with our merging algorithm
			Vector2 neighbourPosition = neighbour.PositionAsVector2;

			// Check for proximity
			if (Vector2.Distance(spawnPosition, neighbourPosition) <= generator.nodeMergingMaximumDistance) {
				// The neighbour merges
				spawn = neighbour;

				// Stop iterating
				merged = true;
				break;
			}
		}

		// Create a map edge between the current map node and the spawn
		MapEdge spawnedEdge = new MapEdge(Node, spawn);

		if (!merged) {
			// Perform the intersection of the spawned node's edge against the neighbours' edges
			bool intersected = Intersect(spawn, spawnedEdge, neighbours);
			
			// Raycast to check for water
			RaycastHit hit;
			if (Physics.Raycast(spawn.position + Vector3.up * 1000f, Vector3.down, out hit)) {
				if (hit.collider.gameObject.tag == "Water") {
					// Spawned over water, remove the edge from the starting node
					Node.edges.Remove(spawnedEdge);
					
					// Skip this node
					return production;
				}

				// Set the Y coordinate of the spawned node to match the height where the raycast hit
				spawn.position.y = hit.point.y + 0.1f;
			}

			// Add the newly created map node to the environment
			generator.AddMapNode(spawn);
			
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
		// We'll need a set of all intersections we found for the given edge - in the general case there might be
		// several intersections so we must find the one closest to the starting node (this.Node)
		HashSet<MapNode> intersectionNodes = null;
		
		// Iterate through all neighbours
		foreach (MapNode neighbour in neighbours) {
			foreach (MapEdge neighboursEdge in neighbour.edges) {
				if (spawnedEdge.FromNode != neighboursEdge.FromNode &&
					spawnedEdge.FromNode != neighboursEdge.ToNode &&
					spawnedEdge.ToNode != neighboursEdge.FromNode &&
					spawnedEdge.ToNode != neighboursEdge.ToNode) {
					
					// Try to find an intersection
					MapNode intersection = spawnedEdge.Intersection(neighboursEdge);
					if (intersection != null) {
						// Found - add the edge which generated the intersection to its edge list. Technically this is
						// invalid behavior (since the intersection lies somewhere *on* that edge), but it's only
						// temporarily there until we reconnect the nodes and edges properly in the next part of the
						// algorithm.
						intersection.edges.Add(neighboursEdge);

						// Add it to the set
						if (intersectionNodes == null) intersectionNodes = new HashSet<MapNode>();
						intersectionNodes.Add(intersection);
					}
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