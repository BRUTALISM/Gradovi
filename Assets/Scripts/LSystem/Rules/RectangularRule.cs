using System;
using System.Collections.Generic;

public class RectangularRule : Rule {
	private static RectangularRule instance;
	public static RectangularRule Instance {
		get {
			if (instance == null) instance = new RectangularRule();
			return instance;
		}
	}
	
	private RectangularRule() {}
	
	public override List<RoadAtom> SpawnRoads(BranchAtom currentAtom, Environment env) {
		throw new NotImplementedException();
	}

	public override float CalculateRoadLength(RoadAtom currentAtom, Environment env) {
		throw new NotImplementedException();
	}
}