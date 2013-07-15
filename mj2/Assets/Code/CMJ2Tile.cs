using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CMJ2Tile : MonoBehaviour
{
	public bool m_moveable = false;
	public bool m_mapOnStart = true; // TODO (Julian): Remove when all objects are loaded from a file rather than a scene

	public string objectIdentifier;
	public Vector2 dims;
	public Vector2 cellAnchor;

	public List<Cell> EnumerateCells () {
		List<Cell> cells = new List<Cell>();

		for (int x = 0; x < dims.x; x++) {
			for (int y = 0; y < dims.y; y++) {
				Vector3 deltaDim = new Vector3(x + cellAnchor.x + dims.x/2f - 1f, y + cellAnchor.y + dims.x/2f - 1f);
				Cell cell = CMJ2EnvironmentManager.g.WorldPosToCell(gameObject.transform.position + deltaDim);
				cells.Add(cell);
			}
		}

		return cells;
	}

	void Start () {
		// TODO (Julian): Remove when all objects are loaded from a file rather than a scene
		if (m_mapOnStart)
		{
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
	}

	void OnDrawGizmosSelected () {
		Gizmos.color = Color.blue;

		Vector3 bounds = new Vector3 (dims.x, dims.y, 1);
		Vector3 offset = new Vector3(dims.x/2f - 0.5f, dims.y/2f - 0.5f, 0f);
 		Vector3 anch = new Vector3 (cellAnchor.x, cellAnchor.y);
        Gizmos.DrawWireCube (transform.position + anch + offset, bounds);

        Gizmos.color = Color.red;
		Gizmos.DrawWireCube (transform.position + anch, Vector3.one*0.99f);
	}
}
