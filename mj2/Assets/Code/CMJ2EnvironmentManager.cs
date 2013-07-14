using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (UIProperties))]
public class CMJ2EnvironmentManager : MonoBehaviour {
	public static CMJ2EnvironmentManager g;

	public GameObject m_debugObject;
	public Dictionary<Cell, List<GameObject>> m_originalObjects;
	public Dictionary<Cell, List<GameObject>> m_playerPlacedObjects;

	public UIProperties m_UIProperties;

	public Camera m_mainCamera;

	public GameObject m_selected;
	Cell m_selectedPos;

	void Awake ()
	{
		g = this;
		m_originalObjects = new Dictionary<Cell, List<GameObject>>();
		m_playerPlacedObjects = new Dictionary<Cell, List<GameObject>>();
		if (!m_UIProperties) m_UIProperties = gameObject.GetComponent<UIProperties>();
	}

	public Cell WorldPosToCell (Vector3 pos)
	{
		Cell cell = new Cell((int)(pos.x - gameObject.transform.position.x + 0.5f),
							 (int)(pos.y - gameObject.transform.position.y + 0.5f));
		return cell;
	}

	public Vector3 CellToWorldPos (Cell cell, float zDepth = 0.0f)
	{
		Vector3 worldPos =  new Vector3(worldPos.x = cell.X + gameObject.transform.position.x,
										worldPos.y = cell.Y + gameObject.transform.position.y,
										zDepth);
		return worldPos;
	}

	public Cell ScreenPosToCell (Vector3 pos)
	{
		Vector3 transformedPoint = m_mainCamera.ScreenToWorldPoint(pos);
		return WorldPosToCell(transformedPoint);
	}

	public bool IsCellPartOfLevel (Cell cell)
	{
		return (cell.X >= 0 && cell.Y >= 0 &&
			    cell.X < m_UIProperties.levelDims.x &&
			    cell.Y < m_UIProperties.levelDims.y);
	}

	public bool IsCellPartOfInterfacePanel (Cell cell)
	{
		Vector2 distFromLevel = new Vector2();
		distFromLevel.x = m_UIProperties.interfacePanelOrigin.position.x;
		distFromLevel.x -= gameObject.transform.position.x;
		distFromLevel.y = m_UIProperties.interfacePanelOrigin.position.y;
		distFromLevel.y -= gameObject.transform.position.y;

		Vector2 posOffset = new Vector2(cell.X - distFromLevel.x,
										cell.Y - distFromLevel.y);

		return (posOffset.x >= 0 && posOffset.y >= 0 &&
			    posOffset.x < m_UIProperties.interfacePanelDims.x &&
			    posOffset.y < m_UIProperties.interfacePanelDims.y);
	}

	public bool IsCellPartOfInterface (Cell cell)
	{
		return (IsCellPartOfLevel(cell) || IsCellPartOfInterfacePanel(cell));
	}

	public bool DoesCellContainObject (Cell cell)
	{
		return (DoesCellContainPlayerPlacedObject(cell) || DoesCellContainOriginalObject(cell));
	}

	public bool DoesCellContainPlayerPlacedObject (Cell cell)
	{
		if (m_playerPlacedObjects.ContainsKey(cell))
		{
			return (m_playerPlacedObjects[cell].Count > 0);
		}
		return false;
	}

	public GameObject GetPlayerPlacedObjectInCell (Cell cell)
	{
		if (m_playerPlacedObjects.ContainsKey(cell))
		{
			int len = m_playerPlacedObjects[cell].Count;
			return (len > 0 ? m_playerPlacedObjects[cell][len - 1] : null);
		}
		return null;
	}

	public void RemovePlayerPlacedObjectFromCell (GameObject obj, Cell cell)
	{
		if (m_playerPlacedObjects.ContainsKey(cell) &&
			m_playerPlacedObjects[cell].Count > 0)
		{
			m_playerPlacedObjects[cell].Remove(obj);
		}

	}

	public void AddPlayerPlacedObjectToCell (GameObject obj, Cell cell)
	{
		if (!m_playerPlacedObjects.ContainsKey(cell))
		{
			m_playerPlacedObjects.Add(cell, new List<GameObject>());
		}
		m_playerPlacedObjects[cell].Add(obj);
	}

	public void AddOriginalObjectToCell (GameObject obj, Cell cell)
	{
		if (!m_originalObjects.ContainsKey(cell))
		{
			m_originalObjects.Add(cell, new List<GameObject>());
		}
		m_originalObjects[cell].Add(obj);
	}

	public bool DoesCellContainOriginalObject (Cell cell)
	{
		if (m_originalObjects.ContainsKey(cell))
		{
			return (m_originalObjects[cell].Count > 0);
		}
		return false;
	}

	public bool DoesCellContainObjectType (Cell cell, int objType) {
		if (m_playerPlacedObjects.ContainsKey(cell))
		{
			foreach (GameObject obj in m_playerPlacedObjects[cell])
			{
				if (obj.layer == objType)
				{
					return true;
				}
			}
		}
		if (m_originalObjects.ContainsKey(cell))
		{
			foreach (GameObject obj in m_originalObjects[cell])
			{
				if (obj.layer == objType)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CanPlaceAt (GameObject obj, Cell cell)
	{
		// TODO: Add special cases for any objects that must be placed on the ground

		if (!IsCellPartOfInterface(cell))
		{
			return false;
		}

		// No object can be placed on the gate
    	if (DoesCellContainObjectType(cell, CMJ2Manager.LAYER_GATE))
    	{
			return false;
		}

		switch (obj.layer)
		{
		    case CMJ2Manager.LAYER_GROUND:
    			if (DoesCellContainObjectType(cell, CMJ2Manager.LAYER_GROUND) ||
    				DoesCellContainObjectType(cell, CMJ2Manager.LAYER_SPIKE))
    			{
    				return false;
    			}
		        break;

		    case CMJ2Manager.LAYER_LADDER:
		        if (DoesCellContainObjectType(cell, CMJ2Manager.LAYER_LADDER))
		        {
    				return false;
    			}
		        break;

		    case CMJ2Manager.LAYER_SPIKE:
    			if (DoesCellContainObjectType(cell, CMJ2Manager.LAYER_GROUND) ||
    				DoesCellContainObjectType(cell, CMJ2Manager.LAYER_SPIKE))
    			{
    				return false;
    			}
		    	break;

		    default:
		        break;
		}

		return true;
	}

	public void CreateDebugObjectAtCell (Cell cell)
	{
		Instantiate(m_debugObject, CellToWorldPos(cell), Quaternion.identity);
	}
}
