using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CityGenerator : MonoBehaviour {
	public string axiom;
	
	public Intersection cityCenter;
	
	void Start() {
		
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
		
	}
}
