using System;

/// <summary>
/// An atom is the smallest building unit of an L-System. When a production is performed on an L-system,
/// each atom is produced into n atoms (n >= 0) according to production rules.
/// </summary>
public class Atom {
	public Atom Parent { get; set; }
	public List<Atom> Children { get; set; }
}
