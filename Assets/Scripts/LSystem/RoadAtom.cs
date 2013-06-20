using System;
using System.Collections.Generic;

public class RoadAtom : Atom {
	/// <summary>
	/// This property is used as a break flag. As soon as it goes below zero, the road atom stops producing roads.
	/// </summary>
	public float DelayCount { get; set; }
	
	public override List<Atom> Produce() {
		throw new NotImplementedException();
	}
}