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
	
	public override List<RoadAtom> SpawnRoads(BranchAtom currentAtom, CityGenerator gen) {
		List<RoadAtom> production = new List<RoadAtom>();

		if (currentAtom.Creator != null) {
			// Get the orientation of the "parent" road (the one we're continuing in this production)
			MapNode toNode = currentAtom.Node;
			MapNode fromNode = currentAtom.Node.edges[0].FromNode;
			Vector3 parentDirection = (toNode.position - fromNode.position).normalized;

			// Get the elevation at the current point
			float currentElevation = gen.ElevationAt(currentAtom.Node.position);
			
			// Create three roads: left, right and straight with regard to the "parent" road's direction
			float[] angles = new float[] { -90f, 0f, 90f };
			foreach (float angle in angles) {
				// Rotate the direction vector to get the direction we'll probe in
				Vector3 roadDirection = Quaternion.Euler(0f, angle, 0f) * parentDirection;

				// Probe elevations around the given direction and get the direction of the road which is least steep
				roadDirection = LeastSteepDirection(currentAtom.Node.position, roadDirection, currentElevation, gen);
				
				// Create a new RoadAtom with the given road direction
				RoadAtom roadAtom = new RoadAtom(roadDirection, currentAtom.Node, Rule.Type.Rectangular);
				
				// Add it to the production
				production.Add(roadAtom);
			}
		} else {
			// This is the axiom, just spawn roads in all directions
			production.Add(new RoadAtom(Vector3.forward, currentAtom.Node, Rule.Type.Rectangular));
			production.Add(new RoadAtom(Vector3.back, currentAtom.Node, Rule.Type.Rectangular));
			production.Add(new RoadAtom(Vector3.left, currentAtom.Node, Rule.Type.Rectangular));
			production.Add(new RoadAtom(Vector3.right, currentAtom.Node, Rule.Type.Rectangular));
		}

		return production;
	}
}