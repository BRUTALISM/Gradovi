using System;
using System.Collections.Generic;
using UnityEngine;

public class RadialRule : Rule {
	private static RadialRule instance;
	public static RadialRule Instance {
		get {
			if (instance == null) instance = new RadialRule();
			return instance;
		}
	}
	
	private RadialRule() {}
	
	private const int MAX_ROADS = 3;
	private const float MAX_ANGLE_FROM_CENTER = 30f;
	
	public override List<RoadAtom> SpawnRoads(BranchAtom currentAtom, Environment env) {
		// TODO: Temporary
		int roadCount = (int) Mathf.Floor(UnityEngine.Random.value * MAX_ROADS) + 1;
		
		List<RoadAtom> production = new List<RoadAtom>();
		float angleIncrement = 2 * MAX_ANGLE_FROM_CENTER / roadCount;
		float angle = -MAX_ANGLE_FROM_CENTER;
		
		Vector3 fromCenter = currentAtom.Node.Position - env.origin;
		if (fromCenter == Vector3.zero) fromCenter = Vector3.forward;
		
		for (int i = 0; i < roadCount; i++) {
			Vector3 direction = Quaternion.Euler(0f, angle, 0f) * fromCenter;
			
			RoadAtom road = new RoadAtom(direction);
			road.Node = currentAtom.Node;
			production.Add(road);
			
			angle += angleIncrement;
		}
		
		return production;
	}

	public override float CalculateRoadLength(RoadAtom currentAtom, Environment env) {
		// FIXME: Implement.
		return 50f + UnityEngine.Random.value * 100f;
	}
}