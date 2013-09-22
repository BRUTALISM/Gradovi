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
		// Implementation of the intersection algorithm found at:
		// http://britneyspeerpink.wordpress.com/2011/08/06/line-intersection/
		
		Vector2 p = this.FromNode.PositionAsVector2;
		Vector2 r = this.ToNode.PositionAsVector2 - p;
		Vector2 q = other.FromNode.PositionAsVector2;
		Vector2 s = other.ToNode.PositionAsVector2 - q;
		
		Func<Vector2, Vector2, float> CrossMag = (v1, v2) => {
			return v1.x * v2.y - v1.y * v2.x;
		};
		
		float w = CrossMag(r, s);
		
		if (Mathf.Approximately(w, 0f)) return null;
		else if (Mathf.Approximately(CrossMag(q - p, r), 0f)) return null;
		else {
			float t = CrossMag(q - p, s) / w;
			Vector2 intersectionP = p + (t * r);
			
			// Check if the intersection lies within both line segments' bounds
			Func<Vector2, Vector2, Vector2, bool> BoundsCheck = (intersection, p1, p2) => {
				return Mathf.Min(p1.x, p2.x) <= intersection.x &&
					Mathf.Max(p1.x, p2.x) >=  intersection.x &&
					Mathf.Min(p1.y, p2.y) <= intersection.y &&
					Mathf.Max(p1.y, p2.y) >= intersection.y;
			};
			
			if (BoundsCheck(intersectionP, this.FromNode.PositionAsVector2, this.ToNode.PositionAsVector2) &&
				BoundsCheck(intersectionP, other.FromNode.PositionAsVector2, other.ToNode.PositionAsVector2)) {
				// Intersection found
				float y = (FromNode.position + t * (ToNode.position - FromNode.position)).y;
				return new MapNode(new Vector3(intersectionP.x, y, intersectionP.y));
			}
		}
		
		return null;
	}
}
