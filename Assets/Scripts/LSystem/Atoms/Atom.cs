using System;
using System.Collections.Generic;

/// <summary>
/// An atom is the smallest building unit of an L-System. When a production is performed on an L-system,
/// each atom is produced into n atoms (n >= 0) according to its production rules.
/// </summary>
public abstract class Atom {
	public Atom Left { get; set; }
	public Atom Right { get; set; }
	
	public MapNode Node { get; set; }
	
	/// <summary>
	/// Runs a production on this instance.
	/// </summary>
	/// <returns>
	/// A list of atoms for the next L-system generation.
	/// </returns>
	public abstract List<Atom> Produce(CityGenerator generator);
}
