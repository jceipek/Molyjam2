using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using System.Text;
using System.IO;

public class CMJ2Editor : MonoBehaviour
{
    public static CMJ2Editor g;

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
        Debug.Log("Save JSON to console by pressing 's'");

        FloatingObjectInCell(m_objectNames[m_selectedIndex], new Cell(0,0));
    }

    private void FloatingObjectInCell (string name, Cell cell)
    {
        Vector3 pos = CMJ2EnvironmentManager.g.CellToWorldPos(cell, -9f);
        m_currObject = CMJ2LevelManager.g.InstantiateObjectByNameInCell(name, cell);
        m_currObject.transform.position = pos;
        CMJ2Tile tile = m_currObject.GetComponent<CMJ2Tile>();
        if (tile)
        {
            tile.m_mapOnStart = false; // TODO (Julian): Remove when all objects are loaded from a file rather than a scene
        }

    }

    private void DeleteFloatingObject ()
    {
        if (m_currObject) Destroy(m_currObject);
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
        Cell cell = CMJ2EnvironmentManager.g.ScreenPosToCell(Input.mousePosition);

        bool save = Input.GetButtonDown("Save");
        bool clicking = Input.GetButton("Click");
        bool deleteModifier = Input.GetButton("Delete Modifier");
        float scrollDir = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetButtonDown("Cycle Next")) scrollDir = 1;
        if (Input.GetButtonDown("Cycle Prev")) scrollDir = -1;
        if (scrollDir != 0)
        {
            if (scrollDir > 0) CycleNext();
            if (scrollDir < 0) CyclePrev();
            DeleteFloatingObject();
            FloatingObjectInCell(m_objectNames[m_selectedIndex], cell);
        }

        if (save)
        {
            CMJ2EnvironmentManager.g.SerializeEnvironment();
        }

        if (m_currObject)
        {
            m_currObject.transform.position = CMJ2EnvironmentManager.g.CellToWorldPos(cell, -9f);
        }

        if (clicking)
        {
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
                CMJ2LevelManager.g.TryInstantiateObjectByNameInCell(m_objectNames[m_selectedIndex], cell);
            }
        }
    }

}
