using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CityGenerator))]
public class CityGeneratorEditor : Editor {
	private CityGenerator generator;
	
	void Awake() {
		generator = target as CityGenerator;
	}
	
	public override void OnInspectorGUI() {
		generator.name = EditorGUILayout.TextField("Name:", generator.name);
		
		EditorGUILayout.LabelField("Number of generations: " + generator.targetGenerations);
		
		if (GUILayout.Button("Step")) {
			generator.targetGenerations++;
			generator.Produce();
		}
		if (GUILayout.Button("Reset")) {
			generator.Reset();
		}
		
		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
	}
}
