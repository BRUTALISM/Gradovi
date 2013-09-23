using System;
using System.Collections.Generic;

public abstract class Rule {
	public Rule() {}
	
	public abstract List<RoadAtom> SpawnRoads(BranchAtom currentAtom, Environment env);
	
	public virtual float CalculateRoadLength(RoadAtom currentAtom, Environment env) {
		float lengthFactor = 1f - env.populationDensity.NormalizedDensityAt(currentAtom.Node.position);
		return env.minimumRoadLength + lengthFactor * (env.maximumRoadLength - env.minimumRoadLength);
	}
}