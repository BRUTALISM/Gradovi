using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GeneratorController))]
public class GeneratorControllerEditor : Editor {
	private CityGenerator cityGenerator;

	void Awake() {
		cityGenerator = (target as GeneratorController).gameObject.GetComponent<CityGenerator>();
	}

	public override void OnInspectorGUI() {
		if (GUILayout.Button("Step +")) {
			cityGenerator.targetGenerations += 2;
			cityGenerator.Produce();
		}
		if (GUILayout.Button("Step -")) {
			cityGenerator.targetGenerations -= 2;
			cityGenerator.Produce();
		}
		if (GUILayout.Button("Reset")) {
			cityGenerator.Reset();
		}

		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
	}
}
