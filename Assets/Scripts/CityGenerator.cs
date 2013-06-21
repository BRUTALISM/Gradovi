using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CityGenerator : MonoBehaviour {
	/// <summary>
	/// How many generations this generator was set up to produce.
	/// </summary>
	public int targetGenerations;
	
	/// <summary>
	/// How many generations this generator currently has produced. If this number is smaller than
	/// <c>targetGenerations</c>, the <c>currentGeneration</c> is produced a number of times, until the target number
	/// of generations is reached.
	/// </summary>
	private int actualGenerations;
	
	/// <summary>
	/// The current generation of atoms, produced <c>actualGenerations</c> number of times.
	/// </summary>
	private List<Atom> currentGeneration;
	
	/// <summary>
	/// The root node of the street graph. This is the axiom's <c>Node</c> property.
	/// </summary>
	private MapNode rootNode;
	
	// FIXME: Implement.
	private Environment environment;
	
	void Start() {
		
	}
	
	void Update() {
		if (Application.isPlaying) {
			// Play mode
		} else {
			// Edit mode
			ProduceIfNecessary();
		}
	}
	
	/// <summary>
	/// Runs a one-step production of the underlying L-system.
	/// </summary>
	public void Produce() {
		// Go through atoms in the current generation and produce them all
		List<Atom> nextGeneration = new List<Atom>();
		foreach (Atom atom in currentGeneration) {
			nextGeneration.AddRange(atom.Produce(environment));
		}
		
		currentGeneration = nextGeneration;
		
		actualGenerations++;
	}
	
	public void Reset() {
		currentGeneration = null;
		rootNode = null;
		
		targetGenerations = actualGenerations = 0;
	}
	
	private void ProduceIfNecessary() {
		// Check if we're here for the first time
		if (currentGeneration == null) {
			// Initialize the list
			currentGeneration = new List<Atom>();
			
			// Add the axiom to the generation
			// TODO: Add a configurable axiom
			Atom axiom = new BranchAtom();
			axiom.Node = new MapNode();
			rootNode = axiom.Node;
			currentGeneration.Add(axiom);
		}
		
		// Check if we need to produce the L-system
		while (actualGenerations < targetGenerations) {
			Produce();
		}
	}
}
