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
    public GameObject m_currObject;
    public int m_selectedIndex;
    public List<string> m_objectNames;

    protected Dictionary<string, CMJ2TileConfig> m_tileNameToConfigMap;

	void Awake ()
    {
    	// Set up singleton
        g = this;
    }

    void Start ()
    {
        m_objectNames = CMJ2LevelManager.g.EnumerateObjectNames();
        Debug.Log("Editor Instructions:");
        Debug.Log("Click and drag to add objects");
        Debug.Log("Hold alt while clicking to delete objects");
        Debug.Log("Cycle through objects by pressing the U/D arrows or using the scroll wheel");
    }

    private void CyclePrev ()
    {
        m_selectedIndex = ((m_selectedIndex + m_objectNames.Count) - 1) % m_objectNames.Count;
    }

    private void CycleNext ()
    {
        m_selectedIndex = ((m_selectedIndex + m_objectNames.Count) + 1) % m_objectNames.Count;
    }

    void Update ()
    {
        bool clicking = Input.GetButton("Click");
        bool deleteModifier = Input.GetButton("Delete Modifier");
        float scrollDir = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetButtonDown("Cycle Next")) scrollDir = 1;
        if (Input.GetButtonDown("Cycle Prev")) scrollDir = -1;
        if (scrollDir != 0)
        {
            if (scrollDir > 0) CycleNext();
            if (scrollDir < 0) CyclePrev();
            Debug.Log("Switched to " + m_objectNames[m_selectedIndex]);
        }

        if (clicking)
        {
            Cell cell = CMJ2EnvironmentManager.g.ScreenPosToCell(Input.mousePosition);
            GameObject origObjInCell = CMJ2EnvironmentManager.g.GetOriginalObjectInCell(cell);
            GameObject playerObjInCell = CMJ2EnvironmentManager.g.GetPlayerPlacedObjectInCell(cell);
            if (deleteModifier)
            {
                if (origObjInCell)
                {
                    CMJ2EnvironmentManager.g.RemoveOriginalObjectFromCell(origObjInCell, cell);
                    Destroy(origObjInCell);
                }
                else if (playerObjInCell)
                {
                    CMJ2EnvironmentManager.g.RemovePlayerPlacedObjectFromCell(playerObjInCell, cell);
                    Destroy(playerObjInCell);
                }
            }
            else if (CMJ2EnvironmentManager.g.IsCellPartOfInterface(cell))
            {
                if (CMJ2EnvironmentManager.g.CanPlaceTypeAt(CMJ2Manager.LAYER_GROUND, cell))
                {
                    CMJ2LevelManager.g.InstantiateObjectByNameInCell(m_objectNames[m_selectedIndex], cell);
                }
            }
        }
    }

}
