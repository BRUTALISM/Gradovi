using System;
using System.Collections.Generic;
using UnityEngine;

public class BranchAtom : Atom {
	/// <summary>
	/// The atom that created this branch atom.
	/// </summary>
	public Atom Creator { get; set; }
	
	public BranchAtom(Atom creator) {
		Creator = creator;
	}
	
	public override List<Atom> Produce(Environment environment) {
		List<Atom> production = new List<Atom>();

		float x = Creator != null ? Creator.Node.X : 0f;
		float z = Creator != null ? Creator.Node.Y : 0f;
		Rule rule = environment.RuleAtCoordinates(new Vector3(x, 0f, z));
		List<RoadAtom> roads = rule.SpawnRoads(this, environment);

		foreach (RoadAtom road in roads) production.Add(road);
		return production;
	}
}