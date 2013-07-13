using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using System.Text;
using System.IO;

public class CMJ2Loader : MonoBehaviour
{
    public static CMJ2Loader g;

    protected Dictionary<string, CMJ2TileConfig> tileNameToConfigMap;

	void Awake () 
    {
    	// Set up singleton
        g = this;
    }

    protected class CMJ2TileConfig
    {
        public string name;
        public GameObject prefab;
        public int type;
        public float zDepth;

        public CMJ2TileConfig (Hashtable data)
        {
            name = data["name"] as string;
            prefab = Resources.Load(data["resource_path"] as string) as GameObject;
            FieldInfo info = CMJ2Manager.g.GetType().GetField(data["type"] as string);
            type = (int)info.GetValue(CMJ2Manager.g);
            zDepth = (float)((double)data["z_depth"]);
        }
    }

    protected class CMJ2Object
    {
    	public GameObject prefab;
    	public Vector3 pos;

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
                prefab = config.prefab;
                pos = new Vector3((float)((double)(data["pos"] as Hashtable)["x"]),
                                  (float)((double)(data["pos"] as Hashtable)["y"]),
                                  config.zDepth);
            }
            else
            {
                print("No config for object with name: "+name);
            }
        }
    }

    protected class CMJ2LevelData
    {
    	public string levelName;
    	public int directive_count;
    	public List<CMJ2Object> objects;

    	public CMJ2LevelData (string data, Dictionary<string, CMJ2TileConfig> configInfo)
    	{
            Hashtable lvlData = MiniJSON.jsonDecode(data) as Hashtable;
            levelName = lvlData["name"] as string;
            directive_count = (int)((double)lvlData["directive_count"]);
            objects = new List<CMJ2Object>();
            foreach (Hashtable obj in (lvlData["objects"] as ArrayList))
            {
                print ("HI");
                objects.Add(new CMJ2Object(obj, configInfo));
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
            map.Add(config.name, config);
        }
        return map;
    }

    protected void createLevelFromData (CMJ2LevelData lvl)
    {
        foreach (CMJ2Object obj in lvl.objects)
        {
            GameObject.Instantiate(obj.prefab, obj.pos, Quaternion.identity);
        }
    }

    void Start ()
    { 
        tileNameToConfigMap = tileConfig();
        string txt = System.IO.File.ReadAllText(Application.dataPath + "/Levels/level1.json");
        print(txt);
        CMJ2LevelData test = new CMJ2LevelData(txt, tileNameToConfigMap);
        createLevelFromData(test);
    }

}
