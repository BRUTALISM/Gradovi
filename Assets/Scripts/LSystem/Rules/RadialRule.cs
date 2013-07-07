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
		List<RoadAtom> production = new List<RoadAtom>();
		
//		// TODO: Temporary
//		int roadCount = (int) Mathf.Floor(UnityEngine.Random.value * MAX_ROADS) + 1;
//		
//		float angleIncrement = 2 * MAX_ANGLE_FROM_CENTER / roadCount;
//		float angle = -MAX_ANGLE_FROM_CENTER;
//		
//		Vector3 fromCenter = currentAtom.Node.Position - env.origin;
//		if (fromCenter == Vector3.zero) fromCenter = Vector3.forward;
//		
//		for (int i = 0; i < roadCount; i++) {
//			Vector3 direction = Quaternion.Euler(0f, angle, 0f) * fromCenter;
//			
//			RoadAtom road = new RoadAtom(direction);
//			road.Node = currentAtom.Node;
//			production.Add(road);
//			
//			angle += angleIncrement;
//		}
		
		if (currentAtom.Creator != null) {
			// Determine the vector from the creator atom's position and the current node's position, and split the
			// result into two groups:
			//   - the vector is more aligned with the radius vector from the origin to the current node
			//   - the vector is more aligned with the tangent of the circle which the radius vector defines
			Vector3 radiusVector = currentAtom.Node.position - env.origin;
			Vector3 fromCreator = currentAtom.Node.position - currentAtom.Creator.Node.position;
			
			Vector3 tangent = Quaternion.Euler(0f, -90f, 0f) * radiusVector;
			
			Vector3 left = (Quaternion.Euler(0f, -90f, 0f) * fromCreator).normalized;
			Vector3 straight = fromCreator.normalized;
			Vector3 right = (Quaternion.Euler(0f, 90f, 0f) * fromCreator).normalized;
			
			if (Mathf.Abs(Vector3.Angle(radiusVector, fromCreator)) < 45f) {
				// The incoming orientation is more aligned with the radius vector
				left = AlignVector(left, tangent);
				straight = AlignVector(straight, radiusVector);
				right = AlignVector(right, tangent);
			} else {
				// The incoming orientation is more aligned with the tangent
				left = AlignVector(left, radiusVector);
				straight = AlignVector(straight, tangent);
				right = AlignVector(right, radiusVector);
			}
			
			// For now, spawn roads in all three directions
			// TODO: Temporary
			production.Add(new RoadAtom(left, currentAtom.Node));
			production.Add(new RoadAtom(straight, currentAtom.Node));
			production.Add(new RoadAtom(right, currentAtom.Node));
		} else {
			// There's no creator (which means this is the axiom node), so we'll create roads shooting in all directions
			// TODO: Temporary
			production.Add(new RoadAtom(Vector3.forward, currentAtom.Node));
			production.Add(new RoadAtom(Vector3.back, currentAtom.Node));
			production.Add(new RoadAtom(Vector3.left, currentAtom.Node));
			production.Add(new RoadAtom(Vector3.right, currentAtom.Node));
		}
		
		return production;
	}

	public override float CalculateRoadLength(RoadAtom currentAtom, Environment env) {
		// FIXME: Implement.
		return 10f + UnityEngine.Random.value * 10f + Vector3.Distance(env.origin, currentAtom.Node.position);
	}
	
	private Vector3 AlignVector(Vector3 vec, Vector3 target) {
		// Get the angle between the two
		float angle = Vector3.Angle(vec, target);
		
		// Since rotation angles are always positive, we need to try to rotate the vector in two different directions,
		// and then see which one is more correct
		Vector3 result1 = Quaternion.Euler(0f, angle, 0f) * vec;
		Vector3 result2 = Quaternion.Euler(0f, -angle, 0f) * vec;
		
		return Mathf.Approximately(Vector3.Angle(result1, target), 0f) ? result1 : result2;
	}
}