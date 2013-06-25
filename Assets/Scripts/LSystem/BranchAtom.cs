using System;
using System.Collections.Generic;
using UnityEngine;

public class BranchAtom : Atom {
	private const int MAX_ROADS = 4;
	
	public override List<Atom> Produce(Environment environment) {
		List<Atom> production = new List<Atom>();
		
		// TODO: Temporary
		// Spawn a number of road atoms
		int roadCount = (int) Mathf.Floor(UnityEngine.Random.value * MAX_ROADS) + 1;
		for (int i = 0; i < roadCount; i++) {
			Vector3 direction = Quaternion.Euler(0f, i * 360f / roadCount + UnityEngine.Random.value * 30f, 0f)
				* Vector3.forward;
			RoadAtom road = new RoadAtom(direction);
			road.Node = Node;
			
			production.Add(road);
		}
		
		return production;
	}
}