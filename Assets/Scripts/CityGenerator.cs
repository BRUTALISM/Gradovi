using UnityEngine;
using System.Text;
using System.Collections;

[ExecuteInEditMode]
public class CityGenerator : MonoBehaviour {
	public string axiom;
	private string currentProduction;
	public int NumberOfProductions { get; private set; }
	
	public MapNode streetGraph;
	
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
		StringBuilder stringBuilder = new StringBuilder(currentProduction);
		
		for (int i = 0; i < currentProduction.Length; i++) {
			char atom = currentProduction[i];
//			stringBuilder.Append(Produce(atom, i));
		}
		
		currentProduction = stringBuilder.ToString();
		NumberOfProductions++;
	}
	
	public void Reset() {
		currentProduction = null;
		streetGraph = new MapNode();
		NumberOfProductions = 0;
	}
}
