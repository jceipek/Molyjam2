using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CMJ2Tile : MonoBehaviour 
{

	public List<string> EnumeratePositions () {
		// Override for objects taking up more than one square
		List<string> positions = new List<string>();
		Vector2 pos = UIController.g.CellFromPos(gameObject.transform.position);
		positions.Add(UIController.g.StringifyPosition(pos));
		return positions;
	}

	void Start () {
		List<string> positions = this.EnumeratePositions();
		foreach (string pos in positions) {
			if (!UIController.g.originalObjects.ContainsKey(pos)) {
				UIController.g.originalObjects.Add(pos, new List<GameObject>());
				print ("Added list");
			}
			UIController.g.originalObjects[pos].Add(gameObject);
		}
	}
}
