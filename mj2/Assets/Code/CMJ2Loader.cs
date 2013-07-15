using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using System.Text;
using System.IO;

public class CMJ2Loader : MonoBehaviour
{
    public static CMJ2Loader g;

    public List<string> m_levelList;
    public int m_currentLevel;

    private CMJ2LevelData m_dataForNextLevel;

    protected Dictionary<string, CMJ2TileConfig> m_tileNameToConfigMap;

	void Awake ()
    {
    	// Set up singleton
        g = this;
    }

    protected Dictionary<string, CMJ2TileConfig> tileConfig ()
    {
        Dictionary<string, CMJ2TileConfig> map = new Dictionary<string, CMJ2TileConfig>();
        string txt = System.IO.File.ReadAllText(Application.dataPath + "/Levels/config_tiles.json");
        Hashtable configData = MiniJSON.jsonDecode(txt) as Hashtable;

        foreach (Hashtable objData in (configData["objects"] as ArrayList))
        {
            CMJ2TileConfig config = new CMJ2TileConfig(objData);
            map.Add(config.m_name, config);
        }
        return map;
    }

    protected void createLevelFromData (CMJ2LevelData lvl)
    {
        foreach (CMJ2Object obj in lvl.m_originalObjects)
        {
            GameObject.Instantiate(obj.m_prefab, obj.m_pos, Quaternion.identity);
        }

        foreach (CMJ2Object obj in lvl.m_placeableObjects)
        {
            GameObject placeable = GameObject.Instantiate(obj.m_prefab, obj.m_pos, Quaternion.identity) as GameObject;
            placeable.GetComponent<CMJ2Tile>().m_moveable = true;
        }
    }

    public CMJ2LevelData loadLevel (int levelIndex)
    {
        string txt = System.IO.File.ReadAllText(Application.dataPath + m_levelList[levelIndex]);
        CMJ2LevelData data = CMJ2LevelManager.g.CreateLevelDataFromJSONString(txt);
        return data;
    }

    void Start ()
    {
        m_dataForNextLevel = loadLevel(m_currentLevel);
        createLevelFromData(m_dataForNextLevel);
        //m_tileNameToConfigMap = tileConfig();
        //loadLevel(0);
        //string txt = System.IO.File.ReadAllText(Application.dataPath + "/Levels/level1.json");
        //CMJ2LevelData test = CMJ2LevelManager.g.CreateLevelDataFromJSONString(txt);
        //createLevelFromData(test);
    }

}
