using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CMJ2Tile : MonoBehaviour 
{
	public bool m_moveable = false;
	
	public Vector2 dims;
	//public Vector2 cellAnchor;

	public List<Cell> EnumerateCells () {
		List<Cell> cells = new List<Cell>();

		for (int x = 0; x < dims.x; x++) {
			for (int y = 0; y < dims.y; y++) {
				Vector3 deltaDim = new Vector3(dims.x/2 - x, dims.y/2 - y, 0);
				Cell cell = CMJ2EnvironmentManager.g.WorldPosToCell(gameObject.transform.position - deltaDim);
				cells.Add(cell);
			}
		}

		return cells;
	}

	void Start () {
		if (m_moveable)
		{
			List<Cell> cells = EnumerateCells();
			foreach (Cell cell in cells) {
				CMJ2EnvironmentManager.g.AddPlayerPlacedObjectToCell(gameObject, cell);
			}
		}	
		else
		{
			List<Cell> cells = EnumerateCells();
			foreach (Cell cell in cells) {
				CMJ2EnvironmentManager.g.AddOriginalObjectToCell(gameObject, cell);
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
