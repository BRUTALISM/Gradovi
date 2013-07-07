using System;
using System.Collections.Generic;
using UnityEngine;

public class BranchAtom : Atom {
	/// <summary>
	/// The atom that created athis branch atom.
	/// </summary>
	// FIXME: Implement. Use this to find out the orientation of the road that created this instance.
	public Atom Creator { get; set; }
	
	public BranchAtom(Atom creator) {
		Creator = creator;
	}
	
	public override List<Atom> Produce(Environment environment) {
		List<Atom> production = new List<Atom>();
		
		List<RoadAtom> roads = environment.Rule.SpawnRoads(this, environment);
		
		// TODO: Check some additional local conditions before returning the result.
		
		foreach (RoadAtom road in roads) production.Add(road);
		return production;
	}
}