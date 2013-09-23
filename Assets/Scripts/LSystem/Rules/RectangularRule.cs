using System;
using System.Collections.Generic;
using UnityEngine;

public class RectangularRule : Rule {
	private static RectangularRule instance;
	public static RectangularRule Instance {
		get {
			if (instance == null) instance = new RectangularRule();
			return instance;
		}
	}
	
	private RectangularRule() {}
	
	public override List<RoadAtom> SpawnRoads(BranchAtom currentAtom, Environment env) {
		List<RoadAtom> production = new List<RoadAtom>();

		// TODO: Temporary!
		// Create roads shooting in all directions.
		production.Add(new RoadAtom(Vector3.forward, currentAtom.Node));
		production.Add(new RoadAtom(Vector3.back, currentAtom.Node));
		production.Add(new RoadAtom(Vector3.left, currentAtom.Node));
		production.Add(new RoadAtom(Vector3.right, currentAtom.Node));

		return production;
	}
}