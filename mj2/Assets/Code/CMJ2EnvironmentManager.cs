using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (UIProperties))]
public class CMJ2EnvironmentManager : MonoBehaviour {
	public static CMJ2EnvironmentManager g;

	public GameObject m_debugObject;
	public Camera m_mainCamera;

	private Dictionary<Cell, List<GameObject>> m_originalObjects;
	private Dictionary<Cell, List<GameObject>> m_playerPlacedObjects;
	private Dictionary<int, List<int>> m_placementRules;

	private UIProperties m_UIProperties;

	void Awake ()
	{
		g = this;
		m_originalObjects = new Dictionary<Cell, List<GameObject>>();
		m_playerPlacedObjects = new Dictionary<Cell, List<GameObject>>();
		if (!m_UIProperties) m_UIProperties = gameObject.GetComponent<UIProperties>();
	}

	void Start ()
	{
		// Here because it depends on CMJ2Manager.g
		LoadPlacementRules();
	}

	protected void LoadPlacementRules ()
	{
		Dictionary<int, List<int>> map = new Dictionary<int, List<int>>();
        string txt = System.IO.File.ReadAllText(Application.dataPath + "/Levels/config_tiles.json");
        Hashtable configData = MiniJSON.jsonDecode(txt) as Hashtable;
        foreach (Hashtable ruleData in (configData["placement_rules"] as ArrayList))
        {
        	int l = CMJ2Manager.g.GetTypeFromString(ruleData["name"] as string);
        	map[l] = new List<int>();
			foreach (string ruleDestData in (ruleData["cannot_place_on"] as ArrayList))
			{
				map[l].Add(CMJ2Manager.g.GetTypeFromString(ruleDestData));
			}
        }
		m_placementRules = map;
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

	public GameObject GetOriginalObjectInCell (Cell cell)
	{
		if (m_originalObjects.ContainsKey(cell))
		{
			int len = m_originalObjects[cell].Count;
			return (len > 0 ? m_originalObjects[cell][len - 1] : null);
		}
		return null;
	}

	public void RemoveOriginalObjectFromCell (GameObject obj, Cell cell)
	{
		if (m_originalObjects.ContainsKey(cell) &&
			m_originalObjects[cell].Count > 0)
		{
			m_originalObjects[cell].Remove(obj);
		}
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

	public bool CanPlaceTileAt (CMJ2Tile tile, Cell destCell)
	{
		// TODO: Add special cases for any objects that must be placed on the ground
		List<Cell> cells = tile.EnumerateCellsFromBase(destCell);
		int type = tile.gameObject.layer;
		foreach (Cell cell in cells)
		{
			if (!CanPlaceTypeAt(type, cell))
			{
				return false;
			}
		}
		return true;
	}

	private bool CanPlaceTypeAt (int type, Cell cell)
	{
		if (!IsCellPartOfInterface(cell))
		{
			return false;
		}

		foreach (int objType in m_placementRules[type])
		{
			if (DoesCellContainObjectType(cell, objType)) return false;
		}

		return true;
	}

	public void CreateDebugObjectAtCell (Cell cell)
	{
		Instantiate(m_debugObject, CellToWorldPos(cell), Quaternion.identity);
	}

	public void SerializeEnvironment ()
	{
		Hashtable hash = new Hashtable();
		ArrayList objectList = new ArrayList();
		ArrayList placeableObjectList = new ArrayList();
		hash["objects"] = objectList;
		hash["placeable_objects"] = placeableObjectList;
		foreach (KeyValuePair<Cell, List<GameObject>> pair in m_originalObjects)
		{
			Cell cell = pair.Key;
			Hashtable pos = new Hashtable();
			pos["x"] = cell.X;
			pos["y"] = cell.Y;
			foreach (GameObject go in pair.Value) {
				CMJ2Tile tile = go.GetComponent<CMJ2Tile>();
				Hashtable obj = new Hashtable();
				obj["name"] = tile.objectIdentifier;
				obj["pos"] = pos;
				objectList.Add(obj);
			}
		}
		foreach (KeyValuePair<Cell, List<GameObject>> pair in m_playerPlacedObjects)
		{
			Cell cell = pair.Key;
			Hashtable pos = new Hashtable();
			pos["x"] = cell.X;
			pos["y"] = cell.Y;
			foreach (GameObject go in pair.Value) {
				CMJ2Tile tile = go.GetComponent<CMJ2Tile>();
				Hashtable obj = new Hashtable();
				obj["name"] = tile.objectIdentifier;
				obj["pos"] = pos;
				placeableObjectList.Add(obj);
			}
		}
		Debug.Log(MiniJSON.jsonEncode(hash));
	}
}
