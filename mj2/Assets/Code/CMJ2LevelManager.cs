using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.IO;

public class CMJ2TileConfig
{
    public string m_name;
    public GameObject m_prefab;
    public int m_type;
    public float m_zDepth;

    public CMJ2TileConfig (Hashtable data)
    {
        m_name = data["name"] as string;
        m_prefab = Resources.Load(data["resource_path"] as string) as GameObject;
        m_type = CMJ2Manager.g.GetTypeFromString(data["type"] as string);
        m_zDepth = (float)((double)data["z_depth"]);
    }
}

public class CMJ2Object
{

    public GameObject m_prefab;
    public Cell m_cell;
    public Vector3 m_pos;

    class ObjectHasNoConfiguration : Exception
    {
        public ObjectHasNoConfiguration(string message) : base(message)
        {
        }

    }

    // Create from JSON Hash
    public CMJ2Object (Hashtable data, Dictionary<string, CMJ2TileConfig> configInfo)
    {
        string name = data["name"] as string;
        if (configInfo.ContainsKey(name))
        {
            CMJ2TileConfig config = configInfo[name];
            m_prefab = config.m_prefab;
            m_cell = new Cell((int)((double)(data["pos"] as Hashtable)["x"]),
                              (int)((double)(data["pos"] as Hashtable)["y"]));
            m_pos = CMJ2EnvironmentManager.g.CellToWorldPos(m_cell, config.m_zDepth);
        }
        else
        {
            throw new ObjectHasNoConfiguration(name);
        }
    }

    // Create from config object at location
    public CMJ2Object (CMJ2TileConfig config, Cell cell)
    {
        m_prefab = config.m_prefab;
        m_cell = cell;
        m_pos = CMJ2EnvironmentManager.g.CellToWorldPos(m_cell, config.m_zDepth);
    }
}

public class CMJ2LevelData
{
    public string m_levelName;
    public int m_directive_count;
    public List<CMJ2Object> m_originalObjects;
    public List<CMJ2Object> m_placeableObjects;

    public CMJ2LevelData (string data, Dictionary<string, CMJ2TileConfig> configInfo)
    {
        Hashtable lvlData = MiniJSON.jsonDecode(data) as Hashtable;
        m_levelName = lvlData["name"] as string;
        m_directive_count = (int)((double)lvlData["directive_count"]);
        m_originalObjects = new List<CMJ2Object>();
        m_placeableObjects = new List<CMJ2Object>();
        foreach (Hashtable obj in (lvlData["objects"] as ArrayList))
        {
            m_originalObjects.Add(new CMJ2Object(obj, configInfo));
        }
        foreach (Hashtable obj in (lvlData["placeable_objects"] as ArrayList))
        {
            m_placeableObjects.Add(new CMJ2Object(obj, configInfo));
        }
    }
}

public class CMJ2LevelManager : MonoBehaviour
{
    public static CMJ2LevelManager g;

    public List<string> m_levelList;
    public int m_currentLevel;

    protected Dictionary<string, CMJ2TileConfig> m_tileNameToConfigMap;

	void Awake ()
    {
    	// Set up singleton
        g = this;
    }

    void Start ()
    {
        // Here because manager must have loaded
        LoadTileConfig();
    }

    protected void LoadTileConfig ()
    {
        Dictionary<string, CMJ2TileConfig> map = new Dictionary<string, CMJ2TileConfig>();
        string txt = System.IO.File.ReadAllText(Application.dataPath + "/Levels/config_tiles.json");
        Hashtable configData = MiniJSON.jsonDecode(txt) as Hashtable;

        foreach (Hashtable objData in (configData["objects"] as ArrayList))
        {
            CMJ2TileConfig config = new CMJ2TileConfig(objData);
            map.Add(config.m_name, config);
        }
        m_tileNameToConfigMap = map;
    }

    public GameObject TryInstantiateObjectByNameInCell (string name, Cell cell)
    {
        CMJ2Object obj = new CMJ2Object (m_tileNameToConfigMap[name], cell);
        if (CMJ2EnvironmentManager.g.CanPlaceTypeAt(obj.m_prefab.layer, cell))
        {
            return GameObject.Instantiate(obj.m_prefab, obj.m_pos, Quaternion.identity) as GameObject;
        }
        return null;
    }

    public GameObject InstantiateObjectByNameInCell (string name, Cell cell)
    {
        CMJ2Object obj = new CMJ2Object (m_tileNameToConfigMap[name], cell);
        return GameObject.Instantiate(obj.m_prefab, obj.m_pos, Quaternion.identity) as GameObject;
    }

    public CMJ2LevelData CreateLevelDataFromJSONString(string json) {
        return new CMJ2LevelData(json, m_tileNameToConfigMap);
    }

    public List<string> EnumerateObjectNames ()
    {
        List<string> names = new List<string>();
        foreach (string name in m_tileNameToConfigMap.Keys)
        {
            names.Add(name);
        }
        return names;
    }

}
