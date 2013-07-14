using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using System.Text;
using System.IO;

public class CMJ2Editor : MonoBehaviour
{
    public static CMJ2Editor g;

    public enum CMJ2EditorState {
        NONE = 0,
        CREATE,
        DELETE
    };

    public CMJ2EditorState m_editorState;
    public GameObject m_curr_object;

    protected Dictionary<string, CMJ2TileConfig> m_tileNameToConfigMap;

	void Awake ()
    {
    	// Set up singleton
        g = this;
    }

    void Update ()
    {
        bool press = Input.GetButton("Click");
        //bool release = Input.GetButtonUp("Click");
        if (press)
        {
            Cell cell = CMJ2EnvironmentManager.g.ScreenPosToCell(Input.mousePosition);
            if (CMJ2EnvironmentManager.g.IsCellPartOfInterface(cell))
            {
                // XXX: Careful! Right now, objects can be placed on top of existing objects
                if (CMJ2EnvironmentManager.g.CanPlaceTypeAt(CMJ2Manager.LAYER_GROUND, cell))
                {
                    CMJ2LevelManager.g.InstantiateObjectByNameInCell("Dirt Ground", cell);
                }
            }
        }
    }

}
