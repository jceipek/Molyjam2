using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CMJ2Tile : MonoBehaviour
{
	public bool m_moveable = false;
	public bool m_mapOnStart = true; // TODO (Julian): Remove when all objects are loaded from a file rather than a scene

	public string m_objectIdentifier;
	public Vector2 m_dims;
	public Vector2 m_cellAnchor;

	public List<Cell> EnumerateCellsAroundCell (Cell originCell) {
		List<Cell> cells = new List<Cell>();

		Vector3 worldPos = CMJ2EnvironmentManager.g.CellToWorldPos(originCell);

		for (int x = 0; x < m_dims.x; x++) {
			for (int y = 0; y < m_dims.y; y++) {
				Vector3 deltaDim = new Vector3(x + m_cellAnchor.x, y + m_cellAnchor.y);
				Cell cell = CMJ2EnvironmentManager.g.WorldPosToCell(worldPos + deltaDim);
				cells.Add(cell);
			}
		}

		return cells;
	}

	public List<Cell> EnumerateCellsFromBase () {
		List<Cell> cells = new List<Cell>();

		Vector3 worldPos = CMJ2EnvironmentManager.g.CellToWorldPos(GetBaseCell());

		for (int x = 0; x < m_dims.x; x++) {
			for (int y = 0; y < m_dims.y; y++) {
				Vector3 deltaDim = new Vector3(x, y);
				Cell cell = CMJ2EnvironmentManager.g.WorldPosToCell(worldPos + deltaDim);
				cells.Add(cell);
			}
		}

		return cells;
	}

	public Cell GetBaseCell ()
	{
		Vector3 deltaDim = new Vector3(m_cellAnchor.x + m_dims.x/2f - 1f, m_cellAnchor.y + m_dims.x/2f - 1f);
		Cell cell = CMJ2EnvironmentManager.g.WorldPosToCell(gameObject.transform.position + deltaDim);
		return cell;
	}

	void Start () {
		Debug.Log("HERO!!!!!!!!!!!!!!!!");
		// TODO (Julian): Remove when all objects are loaded from a file rather than a scene
		if (m_mapOnStart)
		{
			if (m_moveable)
			{
				List<Cell> cells = EnumerateCellsFromBase();
				foreach (Cell cell in cells) {
					CMJ2EnvironmentManager.g.AddPlayerPlacedObjectToCell(gameObject, cell);
				}
			}
			else
			{
				List<Cell> cells = EnumerateCellsFromBase();
				foreach (Cell cell in cells) {
					CMJ2EnvironmentManager.g.AddOriginalObjectToCell(gameObject, cell);
				}
			}
		}
	}

	void OnDrawGizmosSelected () {
		Gizmos.color = Color.blue;

		Vector3 bounds = new Vector3 (m_dims.x, m_dims.y, 1);
		Vector3 offset = new Vector3(m_dims.x/2f - 0.5f, m_dims.y/2f - 0.5f, 0f);
 		Vector3 anch = new Vector3 (m_cellAnchor.x, m_cellAnchor.y);
        Gizmos.DrawWireCube (transform.position + anch + offset, bounds);

        Gizmos.color = Color.red;
		Gizmos.DrawWireCube (transform.position + anch, Vector3.one*0.99f);
	}
}
