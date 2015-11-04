using UnityEngine;
using System.Collections;

public class SpaceTree {

	private static Node rootNode = null;

	public static Tile getTile (CanAddr cAddr) {
		if (rootNode == null) { rootNode = new Node(); }
		return rootNode.getTile(cAddr, Config.TREE_DEPTH - 1);
	}

	public static void updateState () {
		if (rootNode == null) { rootNode = new Node(); }
		rootNode.updateState();
	}

	public static void updateProperties () {
		if (rootNode == null) { rootNode = new Node(); }
		rootNode.updateProperties();
	}
}
