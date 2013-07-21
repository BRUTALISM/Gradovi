using UnityEngine;
using System;
using System.Collections;

public enum EdgeType {
	URBAN,
	HIGHWAY
}

/// <summary>
/// A map edge connects two nodes, and it is of a certain type (see <see cref="EdgeType"/>).
/// </summary>
public class MapEdge {
	public EdgeType Type { get; set; }
	
	public MapNode FromNode { get; set; }

	public MapNode ToNode { get; set; }
	
	public float Length {
		get { return Vector3.Distance (FromNode.position, ToNode.position); }
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="MapEdge"/> class between the two given map nodes. The nodes'
	/// internal edge lists are updated to contain this instance.
	/// </summary>
	/// <param name='fromNode'>
	/// The "from" node.
	/// </param>
	/// <param name='toNode'>
	/// The "to" node.
	/// </param>
	public MapEdge(MapNode fromNode, MapNode toNode) {
		FromNode = fromNode;
		ToNode = toNode;
		
		fromNode.edges.Add (this);
		toNode.edges.Add (this);
	}
	
	/// <summary>
	/// Finds the intersection between two map edges. If there is an intersection, a new MapNode instance is created and
	/// returned at the intersection's coordinates. The original edges and their nodes are left unchanged. If there's no
	/// intersection, <c>null</c> is returned.
	/// </summary>
	/// <param name='other'>
	/// The edge to intersect with.
	/// </param>
	/// <returns>
	/// A newly spawned MapNode if there was an intersection, or <c>null</c> otherwise.
	/// </returns>
	public MapNode Intersection(MapEdge other) {
#warning Implement MapEdge.Intersection
		return null;
		
		// Implementation of the intersection algorithm found at:
		// http://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
		// (The C# solution is adapted from http://ideone.com/PnPJgb given in that answer.)
		
		Vector2 p = this.FromNode.PositionAsVector2;
		Vector2 r = this.ToNode.PositionAsVector2 - p;
		Vector2 q = other.FromNode.PositionAsVector2;
		Vector2 s = other.ToNode.PositionAsVector2 - q;
		
		Vector2 cmp = q - p;
		
		float cmpxr = cmp.x * r.x - cmp.x * r.x;
        float cmpxs = cmp.x * s.y - cmp.y * s.x;
        float rxs = r.x * s.y - r.y * s.x;
		
		if (cmpxr == 0f) {
			// Lines are collinear, and so intersect if they have any overlap
			if (((other.FromNode.X - this.FromNode.X < 0f) != (other.FromNode.X - this.ToNode.X < 0f)) ||
				((other.FromNode.Y - this.FromNode.Y < 0f) != (other.FromNode.Y - this.ToNode.Y < 0f))) {
				Debug.LogWarning("Lines are collinear, what do I do? :/");
				return null;
			}
		}
		
		if (rxs == 0f) {
			// Lines are parallel
			return null;
		}
		
		float rxsr = 1f / rxs;
		float t = cmpxs * rxsr;
		float u = cmpxr * rxsr;
		
		if ((t >= 0f) && (t <= 1f) && (u >= 0f) && (u <= 1f)) {
			// Found it! Now calculate the actual position of the intersection
			Vector2 intersection2D = p + r * t;
			return new MapNode(new Vector3(intersection2D.x, 0f, intersection2D.y));
		}
		
		return null;
	}
}
