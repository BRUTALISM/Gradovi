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
		generator.axiom = EditorGUILayout.TextField("Axiom:", generator.axiom);
		
		EditorGUILayout.LabelField("Extend me please!");
		
		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
	}
}
