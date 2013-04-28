using UnityEngine;
using System;
using System.Collections;

public enum StreetType {
	HIGHWAY,
	URBAN
}

public class Street {
	public StreetType type;
	public Intersection intersection1;
	public Intersection intersection2;
}
