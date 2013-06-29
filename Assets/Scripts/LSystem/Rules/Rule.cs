using System;
using System.Collections.Generic;

public abstract class Rule {
	// TODO: Just an idea - a global uncertainty factor for all rules, can be set from the editor. Zero uncertainty
	//       means the L-system generates the same street graph every time it is run. Individual rules can (but don't
	//       have to) define what UncertaintyFactor > 0 means.
	public static float UncertaintyFactor { get; set; }
	
	public Rule() {}
	
	public abstract List<RoadAtom> SpawnRoads(BranchAtom currentAtom, Environment env);
	
	public abstract float CalculateRoadLength(RoadAtom currentAtom, Environment env);
}

// FIXME: Implement.
//public class RoadParams {
//	public RoadAtom Atom { get; set; }
//	public Environment Environment { get; set; }
//}
//
//public class RoadResult {}
//
//public class IntersectionParams {
//	public BranchAtom Atom { get; set; }
//	public Environment Environment { get; set; }
//}
//
//public class IntersectionResult {}