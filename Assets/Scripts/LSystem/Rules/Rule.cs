using System;
using System.Collections.Generic;

public abstract class Rule {
	public Rule() {}
	
	public abstract List<RoadAtom> SpawnRoads(BranchAtom currentAtom, Environment env);
	
	public abstract float CalculateRoadLength(RoadAtom currentAtom, Environment env);
}