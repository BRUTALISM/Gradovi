using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// The generic quad tree class. Its elements need to have a 2D coordinate representation. If the given maximum number
/// of elements per tree is exceeded, the tree automatically splits itself into four subtrees, and transfers all of its
/// elements to them. Similarly, if an element is added that sits outside the quadtree's boundaries, a parent subtree
/// is created and the given subtree is set as one of its four subtrees.
/// Check out http://en.wikipedia.org/wiki/Quadtree for more info.
/// </summary>
public class QuadTree<T> where T : ICoordinate2D {
	#region Fields and properties
	
	private float minimumX;
	public float MinimumX { get { return minimumX; } }
	private float maximumX;
	public float MaximumX { get { return maximumX; } }
	
	private float minimumY;
	public float MinimumY { get { return minimumY; } }
	private float maximumY;
	public float MaximumY { get { return maximumY; } }
	
	public float CenterX { get { return (minimumX + maximumX) / 2; } }
	public float CenterY { get { return (minimumY + maximumY) / 2; } }
	
	private int maximumElements;
	public int MaximumElements { get { return maximumElements; } }
	
	/// <summary>
	/// The subtrees array. If the current instance has subtrees, it has exactly four of them. Each subtree has half
	/// the width of its parent tree.
	/// Subtrees are numbered in the following way:
	///  y^
	///   |2 3
	///   |0 1
	///   +--->x
	/// </summary>
	private QuadTree<T>[] subtrees;
	
	/// <summary>
	/// The subtrees as a read-only collection. Needed for inspection, such as for drawing gizmos in the Unity editor.
	/// </summary>
	private ReadOnlyCollection<QuadTree<T>> subtreesReadOnly;
	public ReadOnlyCollection<QuadTree<T>> SubtreesReadOnly {
		get { return subtreesReadOnly; }
	}
	
	/// <summary>
	/// The elements contained in this quadtree. If the current instance has subtrees, this field is null.
	/// </summary>
	private HashSet<T> elements;
	
	#endregion
	
	#region Public methods
	
	/// <summary>
	/// Initializes a new instance of the <see cref="QuadTree`1"/> class.
	/// </summary>
	/// <param name='minimumX'>Minimum x.</param>
	/// <param name='maximumX'>Maximum x.</param>
	/// <param name='minimumY'>Minimum y.</param>
	/// <param name='maximumY'>Maximum y.</param>
	/// <param name='maximumElements'>
	/// The maximum number of elements this tree will hold before the next addition splits it into four subtrees.
	/// </param>
	public QuadTree(float minimumX, float maximumX, float minimumY, float maximumY, int maximumElements = 10) {
		this.minimumX = minimumX;
		this.maximumX = maximumX;
		
		this.minimumY = minimumY;
		this.maximumY = maximumY;
		
		this.maximumElements = maximumElements;
	}
	
	/// <summary>
	/// Adds the specified element to the quad tree. If the element is outside of the quad tree's bounds, this method
	/// creates a new parent quad tree, sets the current tree as one of its subtrees, and returns the parent.
	/// If the element was inside the quad tree's bounds, the tree itself is returned, to make client code easier. A
	/// typical invocation should look like:
	/// <code>myQuadTree = myQuadTree.Add(someElement);</code>
	/// </summary>
	/// <param name='element'>
	/// Element.
	/// </param>
	public QuadTree<T> Add(T element) {
		if (BoundsCheck(element)) {
			return CreateParent(element);
		} else if (subtrees != null) {
			SubtreeAdd(element);
		} else if (elements == null) {
			elements = new HashSet<T>();
			elements.Add(element);
		} else if (elements.Count == maximumElements) {
			Split();
			Add(element);
		} else {
			elements.Add(element);
		}
		
		return this;
	}
	
	/// <summary>
	/// Clear this instance and all its subtrees.
	/// </summary>
	public void Clear() {
		if (subtrees != null) foreach (QuadTree<T> subtree in subtrees) subtree.Clear();
		if (elements != null) elements.Clear();
		
		subtrees = null;
		elements = null;
	}
	
	/// <summary>
	/// Gets the neighbours of the given element.
	/// </summary>
	/// <returns>
	/// The neighbours.
	/// </returns>
	/// <param name='element'>
	/// Element.
	/// </param>
	/// <param name='blockyRadius'>
	/// The "radius" of the search around the given element. It's called blocky because the search is not performed
	/// within a circle defined by the element's center and the given radius, but within the minimal square drawn around
	/// that circle.
	/// </param>
	public List<T> GetNeighbours(T element, float blockyRadius) {
		return GetNeighboursRecursive(element, blockyRadius, new List<T>());
	}
	
	/// <summary>
	/// Gets all elements, recurses into subtrees.
	/// </summary>
	/// <returns>
	/// All elements contained within this tree, all its subtrees, all of its subtrees' subtrees, and so on.
	/// </returns>
	public List<T> GetAllElements() {
		return GetAllElementsRecursive(new List<T>());
	}
	
	public override string ToString() {
		return String.Format("QuadTree: X[{0}:{1}] Y[{2}:{3}] maxElements = {4}",
			minimumX, maximumX, minimumY, maximumY, maximumElements);
	}
	
	#endregion
	
	#region Private methods
	
	private bool BoundsCheck(T element) {
		float x = element.X;
		float y = element.Y;
		return x < minimumX || x > maximumX || y < minimumY || y > maximumY;
	}
	
	private void Split() {
		CreateSubtrees();
		
		foreach (T element in elements) {
			SubtreeAdd(element);
		}
		
		elements = null;
	}
	
	private void CreateSubtrees() {
		subtrees = new QuadTree<T>[4];
		subtrees[0] = new QuadTree<T>(minimumX, CenterX, minimumY, CenterY, maximumElements);
		subtrees[1] = new QuadTree<T>(CenterX, maximumX, minimumY, CenterY, maximumElements);
		subtrees[2] = new QuadTree<T>(minimumX, CenterX, CenterY, maximumY, maximumElements);
		subtrees[3] = new QuadTree<T>(CenterX, maximumX, CenterY, maximumY, maximumElements);
		
		subtreesReadOnly = new ReadOnlyCollection<QuadTree<T>>(new List<QuadTree<T>>(subtrees));
	}
	
	private void SubtreeAdd(T element) {
		if (element.X <= CenterX) {
			if (element.Y <= CenterY) subtrees[0].Add(element);
			else subtrees[2].Add(element);
		} else {
			if (element.Y <= CenterY) subtrees[1].Add(element);
			else subtrees[3].Add(element);
		}
	}
	
	private QuadTree<T> CreateParent(T element) {
		float minX, maxX, minY, maxY;
		float currentWidthX = maximumX - minimumX;
		float currentWidthY = maximumY - minimumY;
		int subtreeIndex = 0;
		
		//  y^
		//   |2 3
		//   |0 1
		//   +--->x
		if (element.X < minimumX) {
			minX = minimumX - currentWidthX;
			maxX = maximumX;
			subtreeIndex = 1;
		} else {
			minX = minimumX;
			maxX = maximumX + currentWidthX;
		}
		
		if (element.Y < minimumY) {
			minY = minimumY - currentWidthY;
			maxY = maximumY;
			subtreeIndex += 2;
		} else {
			minY = minimumY;
			maxY = maximumY + currentWidthY;
		}
		
		QuadTree<T> parent = new QuadTree<T>(minX, maxX, minY, maxY, maximumElements);
		parent.CreateSubtrees();
		parent.subtrees[subtreeIndex] = this;
		return parent;
	}
	
	private List<T> GetAllElementsRecursive(List<T> elementsSoFar) {
		if (elements != null) {
			elementsSoFar.AddRange(elements);
		} else foreach (QuadTree<T> subtree in subtrees) {
			subtree.GetAllElementsRecursive(elementsSoFar);
		}
		
		return elementsSoFar;
	}
	
	private List<T> GetNeighboursRecursive(T element, float blockyRadius, List<T> neighboursSoFar) {
		float leftBound = element.X - blockyRadius;
		float rightBound = element.X + blockyRadius;
		float bottomBound = element.Y - blockyRadius;
		float topBound = element.Y + blockyRadius;
		
		// Check if there's any part of the search square that intersects this instance's region
		// Note: It's easier to reason about the inverse condition - if the two regions are not overlapping.
		bool notOverlapping = (leftBound < minimumX && rightBound < minimumX) ||
			(leftBound > maximumX && rightBound > maximumX) ||
			(bottomBound < minimumY && topBound < minimumY) ||
			(bottomBound > maximumY && topBound > maximumY);
		if (!notOverlapping) {
			// They overlap, add all elements or recurse into subtrees
			if (elements != null) {
				// Check if each node is within the radius and return only the ones that are
				foreach (T node in elements) {
					if (NodeWithinBounds(node, leftBound, rightBound, topBound, bottomBound)) {
						neighboursSoFar.Add(node);
					}
				}
			} else if (subtrees != null) foreach (QuadTree<T> subtree in subtrees) {
				subtree.GetNeighboursRecursive(element, blockyRadius, neighboursSoFar);
			}
		}
		
		return neighboursSoFar;
	}
	
	private bool NodeWithinBounds(T node, float left, float right, float top, float bottom) {
		return node.X >= left && node.X <= right && node.Y >= bottom && node.Y <= top;
	}
	
	#endregion
}
