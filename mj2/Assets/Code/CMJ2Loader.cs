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

    protected Dictionary<string, CMJ2TileConfig> m_tileNameToConfigMap;

	void Awake () 
    {
    	// Set up singleton
        g = this;
    }

    protected class CMJ2TileConfig
    {
        public string m_name;
        public GameObject m_prefab;
        public int m_type;
        public float m_zDepth;

        public CMJ2TileConfig (Hashtable data)
        {
            m_name = data["name"] as string;
            m_prefab = Resources.Load(data["resource_path"] as string) as GameObject;
            FieldInfo info = CMJ2Manager.g.GetType().GetField(data["type"] as string);
            m_type = (int)info.GetValue(CMJ2Manager.g);
            m_zDepth = (float)((double)data["z_depth"]);
        }
    }

    protected class CMJ2Object
    {
    	public GameObject m_prefab;
    	public Vector3 m_pos;

        public CMJ2Object (Hashtable data, Dictionary<string, CMJ2TileConfig> configInfo)
        {
            foreach (object key in (data)) {
                print (key as string);
            }
            string name = data["name"] as string;
            print ("NAME: "+name);
            if (configInfo.ContainsKey(name))
            {
                CMJ2TileConfig config = configInfo[name];
                m_prefab = config.m_prefab;
                m_pos = new Vector3((float)((double)(data["pos"] as Hashtable)["x"]),
                                  (float)((double)(data["pos"] as Hashtable)["y"]),
                                  config.m_zDepth);
            }
            else
            {
                print("No config for object with name: "+name);
            }
        }
    }

    protected class CMJ2LevelData
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
            foreach (Hashtable obj in (lvlData["objects"] as ArrayList))
            {
                m_originalObjects.Add(new CMJ2Object(obj, configInfo));
            }
            foreach (Hashtable obj in (lvlData["objects"] as ArrayList))
            {
                m_placeableObjects.Add(new CMJ2Object(obj, configInfo));
            }
    	}
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
    }

    public void loadLevel (int levelIndex)
    {
        float start = Time.realtimeSinceStartup;
        string txt = System.IO.File.ReadAllText(Application.dataPath + m_levelList[levelIndex]);
        CMJ2LevelData test = new CMJ2LevelData(txt, m_tileNameToConfigMap);
        print(Time.realtimeSinceStartup - start);
    }

    void Start ()
    { 
        m_tileNameToConfigMap = tileConfig();
        //loadLevel(0);
        string txt = System.IO.File.ReadAllText(Application.dataPath + "/Levels/level1.json");
        CMJ2LevelData test = new CMJ2LevelData(txt, m_tileNameToConfigMap);
        createLevelFromData(test);
    }

}
