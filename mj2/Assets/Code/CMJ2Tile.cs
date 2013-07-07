using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CMJ2Tile : MonoBehaviour 
{
	public bool m_moveable = false;
	
	public Vector2 dims;
	//public Vector2 cellAnchor;

	public List<string> EnumeratePositions () {
		List<string> positions = new List<string>();

		for (int x = 0; x < dims.x; x++) {
			for (int y = 0; y < dims.y; y++) {
				Vector3 deltaDim = new Vector3(dims.x/2 - x, dims.y/2 - y, 0);
				Vector2 pos = UIController.g.CellFromPos(gameObject.transform.position - deltaDim);
				positions.Add(UIController.g.StringifyPosition(pos));
			}
		}

		return positions;
	}

	void Start () {
		if (m_moveable)
		{
			List<string> positions = this.EnumeratePositions();
			foreach (string pos in positions) {
				if (!UIController.g.playerPlacedObjects.ContainsKey(pos)) {
					UIController.g.playerPlacedObjects.Add(pos, new List<GameObject>());
					//print ("Added list");
				}
				UIController.g.playerPlacedObjects[pos].Add(gameObject);
			}
		}	
		else
		{
			List<string> positions = this.EnumeratePositions();
			foreach (string pos in positions) {
				if (!UIController.g.originalObjects.ContainsKey(pos)) {
					UIController.g.originalObjects.Add(pos, new List<GameObject>());
					//print ("Added list");
				}
				UIController.g.originalObjects[pos].Add(gameObject);
			}
		}
	}

	void OnDrawGizmosSelected () {
		Gizmos.color = Color.red;

 		Vector3 bounds = new Vector3 (dims.x, dims.y, 1);
 		//Vector3 anch = new Vector3 (cellAnchor.x, cellAnchor.y);
        //Gizmos.DrawWireCube (transform.position + anch, bounds);
        Gizmos.DrawWireCube (transform.position, bounds);

        Gizmos.color = Color.green;
		Gizmos.DrawWireCube (transform.position, bounds);        
	}
}
