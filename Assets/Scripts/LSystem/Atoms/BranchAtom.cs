using System;
using System.Collections.Generic;
using UnityEngine;

public class BranchAtom : Atom {
	public override List<Atom> Produce(Environment environment) {
		List<Atom> production = new List<Atom>();
		
		List<RoadAtom> roads = environment.Rule.SpawnRoads(this, environment);
		
		// TODO: Check some additional local conditions before returning the result.
		
		foreach (RoadAtom road in roads) production.Add(road);
		return production;
	}
}