using System;
using System.Collections.Generic;

/// <summary>
/// The generic quad tree class. Its elements need to have a 2D coordinate representation. If the given maximum number
/// of elements per tree is exceeded, the tree automatically splits itself into four subtrees, and transfers all of its
/// elements to them. Similarly, if an element is added that sits outside the quadtree's boundaries, a parent subtree
/// is created and the given subtree is set as one of its four subtrees.
/// Check out http://en.wikipedia.org/wiki/Quadtree for more info.
/// </summary>
public class QuadTree<T> where T : ICoordinate2D {
	private float minimumX;
	private float maximumX;
	
	private float minimumY;
	private float maximumY;
	
	private float boundaryX;
	private float boundaryY;
	
	private int maximumElements;
	
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
	/// The elements contained in this quadtree. If the current instance has subtrees, this field is null.
	/// </summary>
	private HashSet<T> elements;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="QuadTree`1"/> class.
	/// </summary>
	/// <param name='minimumX'>Minimum x.</param>
	/// <param name='minimumY'>Minimum y.</param>
	/// <param name='maximumX'>Maximum x.</param>
	/// <param name='maximumY'>Maximum y.</param>
	/// <param name='maximumElements'>
	/// The maximum number of elements this tree will hold before the next addition splits it into four subtrees.
	/// </param>
	public QuadTree(float minimumX, float minimumY, float maximumX, float maximumY, int maximumElements = 10) {
		this.minimumX = minimumX;
		this.maximumX = maximumX;
		
		this.minimumY = minimumY;
		this.maximumY = maximumY;
		
		this.boundaryX = (maximumX - minimumX) / 2;
		this.boundaryY = (maximumY - minimumY) / 2;
		
		this.maximumElements = maximumElements;
	}
	
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
		
		return null;
	}
	
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
		float halfwayX = (maximumX - minimumX) / 2;
		float halfwayY = (maximumY - minimumY) / 2;
		
		subtrees = new QuadTree<T>[4];
		subtrees[0] = new QuadTree<T>(minimumX, minimumY, halfwayX, halfwayY, maximumElements);
		subtrees[1] = new QuadTree<T>(halfwayX, minimumY, maximumX, halfwayY, maximumElements);
		subtrees[2] = new QuadTree<T>(minimumX, halfwayY, halfwayX, maximumY, maximumElements);
		subtrees[3] = new QuadTree<T>(halfwayX, halfwayY, maximumX, maximumY, maximumElements);
	}
	
	private void SubtreeAdd(T element) {
		float x = element.X;
		float y = element.Y;
		
		if (x <= boundaryX) {
			if (y <= boundaryY) subtrees[0].Add(element);
			else subtrees[2].Add(element);
		} else {
			if (y <= boundaryY) subtrees[1].Add(element);
			else subtrees[3].Add(element);
		}
	}
	
	private QuadTree<T> CreateParent(T element) {
		float minX, maxX, minY, maxY;
		float currentWidthX = maximumX - minimumX;
		float currentWidthY = maximumY - minimumY;
		int subtreeIndex = 0;
		
		if (element.X < minimumX) {
			minX = minimumX - currentWidthX;
			maxX = maximumX;
			subtreeIndex |= 1;
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
		
		QuadTree<T> parent = new QuadTree<T>(minX, minY, maxX, maxY, maximumElements);
		parent.CreateSubtrees();
		parent.subtrees[subtreeIndex] = this;
		return parent;
	}
}
