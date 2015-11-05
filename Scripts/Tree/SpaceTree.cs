using UnityEngine;
using System.Collections;

public class SpaceTree {

	private static Node rootNode = null;

	public static Tile getTile (CanAddr cAddr) {
		if (rootNode == null) { rootNode = new Node(); }
		return rootNode.getTile(cAddr);
	}

	public static void updateState () {
		if (rootNode == null) { rootNode = new Node(); }
		rootNode.updateState();
	}

	public static void updateProperties () {
		if (rootNode == null) { rootNode = new Node(); }
		rootNode.updateProperties();
	}

	public static void setState (CanAddr cAddr, byte state) {
		if (rootNode == null) { rootNode = new Node(); }
		rootNode.setState(cAddr, state);
	}
}
