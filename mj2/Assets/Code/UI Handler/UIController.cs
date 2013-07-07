using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour {
	public static UIController g;

	public UIProperties m_UIProperties;
	public Dictionary<string, List<GameObject>> originalObjects;
	public Dictionary<string, List<GameObject>> playerPlacedObjects;

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
		return false;
	}

	public bool CanPlaceAt (GameObject obj, Vector2 pos) {
		// TODO: Add special cases for objects that must be placed on the ground
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
		Vector2 transformedPoint = Camera.main.ScreenToWorldPoint(point);
		return CellFromPos(transformedPoint);
	}

	void Awake () {
		g = this;
		originalObjects = new Dictionary<string, List<GameObject>>();
		playerPlacedObjects = new Dictionary<string, List<GameObject>>();
		m_UIProperties = gameObject.GetComponent<UIProperties>();
	}

	void Update () {
 		if (Input.GetButtonDown("Click")) {
 			print (MapPointToSquare (Input.mousePosition));
 			
 			if (DoesCellContainObject(MapPointToSquare (Input.mousePosition))) {
 				print ("Filled");
 			} else {
 				print ("Empty");
 			}
 			
            /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray))
                Instantiate(particle, transform.position, transform.rotation) as GameObject;
            */
        }
	}
}
