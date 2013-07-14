using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour {
	public static UIController g;

	public GameObject m_debugObject;
	public UIProperties m_UIProperties;
	public Dictionary<Cell, List<GameObject>> m_originalObjects;
	public Dictionary<Cell, List<GameObject>> m_playerPlacedObjects;
	
	public Camera m_theCamera;

	public GameObject m_selected;
	Cell m_selectedPos;

	public Cell WorldPosToCell (Vector3 pos)
	{
		Cell cell = new Cell((int)(pos.x - gameObject.transform.position.x + 0.5f),
							 (int)(pos.y - gameObject.transform.position.y + 0.5f));
		return cell;
	}

	public Vector3 CellToWorldPos (Cell cell)
	{
		Vector2 pos = CellToScreenPos(cell);
		return new Vector3(pos.x, pos.y, 0);
	}

	public Cell ScreenPosToCell (Vector3 pos)
	{
		Vector3 transformedPoint = m_theCamera.ScreenToWorldPoint(pos);
		return WorldPosToCell(transformedPoint);
	}

	public Vector2 CellToScreenPos (Cell cell)
	{
		Vector2 screenPos = new Vector2();
		screenPos.x = cell.X + gameObject.transform.position.x;
		screenPos.y = cell.Y + gameObject.transform.position.y;
		return screenPos;
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
	
	public GameObject GetObjectInCell (Cell cell)
	{
		if (m_playerPlacedObjects.ContainsKey(cell))
		{
			return (m_playerPlacedObjects[cell].Count > 0 ? m_playerPlacedObjects[cell][0] : null);
		}
		return null;
	}
	public void RemoveObjectInCell (Cell cell)
	{
		if (m_playerPlacedObjects.ContainsKey(cell) &&
			m_playerPlacedObjects[cell].Count > 0)
		{
			m_playerPlacedObjects[cell].RemoveAt(0);
		}
			
	}
	
	public void AddObjectToCell (GameObject obj, Cell cell)
	{		
		if (!m_playerPlacedObjects.ContainsKey(cell))
		{
			m_playerPlacedObjects.Add(cell, new List<GameObject>());
		}
		m_playerPlacedObjects[cell].Add(obj);
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

	void Awake ()
	{
		g = this;
		m_originalObjects = new Dictionary<Cell, List<GameObject>>();
		m_playerPlacedObjects = new Dictionary<Cell, List<GameObject>>();
		m_UIProperties = gameObject.GetComponent<UIProperties>();
	}

	public void CreateDebugObjectAtCell (Cell cell)
	{
		Instantiate(m_debugObject, CellToWorldPos(cell), Quaternion.identity);
	}
	
	void Update ()
	{
		
		bool press = Input.GetButtonDown("Click");
		bool release = Input.GetButtonUp("Click");
 		if ((m_selected == null && press) ||
 			(m_selected && release))
 		{
			Cell cell = ScreenPosToCell(Input.mousePosition);
			
			if (m_selected)
			{
				if (CanPlaceAt(m_selected, cell))
				{
					print("can place");
				}
				else
				{
					cell = m_selectedPos;
					print("blocked");
				}
				
				AddObjectToCell(m_selected, cell);				
	        	m_selected.transform.position = CellToWorldPos(cell);
				m_selected = null;
				
			}
			else
			{
	 			if (DoesCellContainPlayerPlacedObject(cell))
	 			{
	 				print ("Filled");
	 				
	 				m_selectedPos = cell;
	 				m_selected = GetObjectInCell(cell);
	 				if (m_selected)
	 				{
	 					RemoveObjectInCell(cell);
	 				}
	 			}
	 			else
	 			{
	 				if (DoesCellContainObject(cell))
	 				{
	 					print ("Contains Object");
	 				} 
	 				else
	 				{
	 					print ("Empty");
	 				}
	 			}
			}
            /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray))
                Instantiate(particle, transform.position, transform.rotation) as GameObject;
            */
        }
        
        if (m_selected != null)
        {
        	m_selected.transform.position = CellToWorldPos(ScreenPosToCell (Input.mousePosition)) + new Vector3 (0f, 0f, -9f);
        }
	}
}
