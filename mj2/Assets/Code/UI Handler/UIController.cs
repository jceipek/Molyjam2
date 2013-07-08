using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour {
	public static UIController g;

	public GameObject debugObject;
	public UIProperties m_UIProperties;
	public Dictionary<string, List<GameObject>> originalObjects;
	public Dictionary<string, List<GameObject>> playerPlacedObjects;
	
	public Camera m_theCamera;

	public string StringifyPosition (Vector2 pos) {
		return Mathf.Floor(pos.x) +
		       "," +
		       Mathf.Floor(pos.y);
	}

	public Vector2 CellFromPos (Vector3 pos) {
		Vector2 transformedPoint = new Vector2();
		transformedPoint.x = Mathf.Floor(pos.x - gameObject.transform.position.x + 0.5f);
		transformedPoint.y = Mathf.Floor(pos.y - gameObject.transform.position.y + 0.5f);
		return transformedPoint;
	}

	public Vector2 PosFromCell (Vector2 cell) {
		Vector2 transformedPoint = new Vector2();
		transformedPoint.x = cell.x + gameObject.transform.position.x;
		transformedPoint.y = cell.y + gameObject.transform.position.y;
		return transformedPoint;
	}

	public bool IsCellPartOfLevel (Vector2 pos) {
		return (pos.x >= 0 && pos.y >= 0 &&
			    pos.x < m_UIProperties.levelDims.x &&
			    pos.y < m_UIProperties.levelDims.y);
	}

	public bool IsCellPartOfInterfacePanel (Vector2 pos) {
		Vector2 distFromLevel = new Vector2();
		distFromLevel.x = m_UIProperties.interfacePanelOrigin.position.x;
		distFromLevel.x -= gameObject.transform.position.x;
		distFromLevel.y = m_UIProperties.interfacePanelOrigin.position.y;
		distFromLevel.y -= gameObject.transform.position.y;

		Vector2 posOffset = pos - distFromLevel;

		return (posOffset.x >= 0 && posOffset.y >= 0 &&
			    posOffset.x < m_UIProperties.interfacePanelDims.x &&
			    posOffset.y < m_UIProperties.interfacePanelDims.y);
	}

	public bool IsCellPartOfInterface (Vector2 pos) {
		return IsCellPartOfLevel(pos) || IsCellPartOfInterfacePanel(pos);
	}

	public bool DoesCellContainObject (Vector2 pos) {
		return (DoesCellContainPlayerPlacedObject(pos) || DoesCellContainOriginalObject(pos));
	}

	public bool DoesCellContainPlayerPlacedObject (Vector2 pos) {
		string posStr = this.StringifyPosition(pos);
		if (playerPlacedObjects.ContainsKey(posStr)) {
			return (playerPlacedObjects[posStr].Count > 0);
		}
		return false;
	}
	
	public GameObject GetObjectInPos (Vector2 pos) {
		string posStr = this.StringifyPosition(pos);
		if (playerPlacedObjects.ContainsKey(posStr))
			return (playerPlacedObjects[posStr].Count > 0 ? playerPlacedObjects[posStr][0] : null);
		return null;
	}
	public void RemoveObjectInPos (Vector2 pos) {
		string posStr = this.StringifyPosition(pos);
		if (playerPlacedObjects.ContainsKey(posStr) &&
			playerPlacedObjects[posStr].Count > 0)
			playerPlacedObjects[posStr].RemoveAt(0);
	}
	
	public void AddObjectToPos (GameObject obj, Vector2 pos) {
		
		string posStr = this.StringifyPosition(pos);
		
		if (!playerPlacedObjects.ContainsKey(posStr)) {
			playerPlacedObjects.Add(posStr, new List<GameObject>());
		}
		playerPlacedObjects[posStr].Add(obj);
	}

	public bool DoesCellContainOriginalObject (Vector2 pos) {
		string posStr = this.StringifyPosition(pos);
		if (originalObjects.ContainsKey(posStr)) {
			return (originalObjects[posStr].Count > 0);
		}
		return false;
	}

	public bool DoesCellContainObjectType (Vector2 pos, int tpe) {
		string posStr = this.StringifyPosition(pos);
		if (playerPlacedObjects.ContainsKey(posStr)) {
			foreach (GameObject obj in playerPlacedObjects[posStr]) {
				if (obj.layer == tpe) {
					return true;
				}	
			}
		}
		if (originalObjects.ContainsKey(posStr)) {
			foreach (GameObject obj in originalObjects[posStr]) {
				if (obj.layer == tpe) {
					return true;
				}	
			}
		}
		return false;
	}

	public bool CanPlaceAt (GameObject obj, Vector2 pos) {
		// TODO: Add special cases for any objects that must be placed on the ground

		if (!IsCellPartOfInterface(pos)) {
			return false;
		}

		// No object can be placed on the gate
    	if (DoesCellContainObjectType(pos, CMJ2Manager.LAYER_GATE)) {
			return false;
		}

		switch (obj.layer)
		{
		    case CMJ2Manager.LAYER_GROUND: 
    			if (DoesCellContainObjectType(pos, CMJ2Manager.LAYER_GROUND) ||
    				DoesCellContainObjectType(pos, CMJ2Manager.LAYER_SPIKE)) {
    				return false;
    			}
		        break;

		    case CMJ2Manager.LAYER_LADDER:
		        if (DoesCellContainObjectType(pos, CMJ2Manager.LAYER_LADDER)) {
    				return false;
    			}
		        break;

		    case CMJ2Manager.LAYER_SPIKE:
    			if (DoesCellContainObjectType(pos, CMJ2Manager.LAYER_GROUND) ||
    				DoesCellContainObjectType(pos, CMJ2Manager.LAYER_SPIKE)) {
    				return false;
    			}
		    	break;

		    default:
		        break;
		}

		return true;
	}

	Vector2 MapPointToSquare (Vector3 point) {
		Vector2 transformedPoint = m_theCamera.ScreenToWorldPoint(point);
		return CellFromPos(transformedPoint);
	}

	Vector3 MapSquareToPoint (Vector2 square) {
		Vector2 pos = PosFromCell(square);
		return new Vector3(pos.x,pos.y,0);
	}

	void Awake () {
		g = this;
		originalObjects = new Dictionary<string, List<GameObject>>();
		playerPlacedObjects = new Dictionary<string, List<GameObject>>();
		m_UIProperties = gameObject.GetComponent<UIProperties>();
	}

	public void CreateDebugObjectAtCell (Vector2 cell) {
		Instantiate(debugObject, MapSquareToPoint(cell), Quaternion.identity);
	}
	
	public GameObject m_selected;
	Vector2 m_selectedPos;

	void Update () {
		
		bool press = Input.GetButtonDown("Click");
		bool release = Input.GetButtonUp("Click");
 		if ((m_selected == null && press) ||
 			(m_selected && release)) {

			Vector2 pos = MapPointToSquare (Input.mousePosition);
			
			if (m_selected)
			{
				if (CanPlaceAt(m_selected, pos))
				{
					print("can place");
				}
				else
				{
					pos = m_selectedPos;
					print("blocked");
				}
				
				AddObjectToPos(m_selected, pos);				
	        	m_selected.transform.position = MapSquareToPoint(pos);
				m_selected = null;
				
			}
			else
			{
	 			if (DoesCellContainPlayerPlacedObject(pos)) {
	 				print ("Filled");
	 				
	 				m_selectedPos = pos;
	 				m_selected = GetObjectInPos(pos);
	 				if (m_selected)
	 					RemoveObjectInPos(pos);
	 			} else {
	 				if (DoesCellContainObject(pos)) {
	 					print ("Contains Object");
	 				} else {
	 					print ("Empty");
	 				}
	 				
	 				//if (IsCellPartOfInterface(MapPointToSquare (Input.mousePosition))) {
	 				//	UIController.g.CreateDebugObjectAtCell(MapPointToSquare (Input.mousePosition));	
	 				//}
	 				
	 			}
			}
            /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray))
                Instantiate(particle, transform.position, transform.rotation) as GameObject;
            */
        }
        
        if (m_selected != null)
        {
        	//print(MapPointToSquare (Input.mousePosition));
        	m_selected.transform.position = MapSquareToPoint(MapPointToSquare (Input.mousePosition)) + new Vector3 (0f, 0f, -9f);
        }
	}
}
