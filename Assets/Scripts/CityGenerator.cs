using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CityGenerator : MonoBehaviour {
	public int NumberOfGenerations { get; private set; }
	
	private List<Atom> currentGeneration = new List<Atom>();
	private MapNode streetGraph;
	
	void Start() {
		Reset();
	}
	
	void Update() {
		if (Application.isPlaying) {
			// Play mode
		} else {
			// Edit mode
		}
	}
	
	/// <summary>
	/// Runs a one-step production of the underlying L-system.
	/// </summary>
	public void Step() {
		// Go through atoms in the current generation and produce them all
		List<Atom> nextGeneration = new List<Atom>();
		foreach (Atom atom in currentGeneration) {
			nextGeneration.AddRange(atom.Produce());
		}
		
		currentGeneration = nextGeneration;
		
		NumberOfGenerations++;
	}
	
	public void Reset() {
		if (currentGeneration == null || currentGeneration.Count > 0) currentGeneration = new List<Atom>();
		streetGraph = new MapNode();
		NumberOfGenerations = 0;
	}
}
