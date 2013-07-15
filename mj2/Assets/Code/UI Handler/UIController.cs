using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour {

	public GameObject m_selected;
	Cell m_selectedPos;

	void Update ()
	{

		bool press = Input.GetButtonDown("Click");
		bool release = Input.GetButtonUp("Click");
 		if ((m_selected == null && press) ||
 			(m_selected && release))
 		{
			Cell cell = CMJ2EnvironmentManager.g.ScreenPosToCell(Input.mousePosition);

			if (m_selected)
			{
				CMJ2Tile tile = m_selected.GetComponent<CMJ2Tile>();
				if (CMJ2EnvironmentManager.g.CanPlaceTileAt(tile, cell))
				{
					print("can place");
				}
				else
				{
					cell = m_selectedPos;
					print("blocked");
				}

				CMJ2EnvironmentManager.g.AddPlayerPlacedObjectToCell(m_selected, cell);
	        	m_selected.transform.position = CMJ2EnvironmentManager.g.CellToWorldPos(cell);
				m_selected = null;

			}
			else
			{
	 			if (CMJ2EnvironmentManager.g.DoesCellContainPlayerPlacedObject(cell))
	 			{
	 				print ("Filled");

	 				m_selectedPos = cell;
	 				m_selected = CMJ2EnvironmentManager.g.GetPlayerPlacedObjectInCell(cell);
	 				if (m_selected)
	 				{
	 					CMJ2EnvironmentManager.g.RemovePlayerPlacedObjectFromCell(m_selected, cell);
	 					// TODO: remove large objects from multiple cells
	 				}
	 			}
	 			else
	 			{
	 				if (CMJ2EnvironmentManager.g.DoesCellContainObject(cell))
	 				{
	 					print ("Contains Object");
	 				}
	 				else
	 				{
	 					print ("Empty");
	 				}
	 			}
			}
            /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray))
                Instantiate(particle, transform.position, transform.rotation) as GameObject;
            */
        }

        if (m_selected != null)
        {
        	m_selected.transform.position = CMJ2EnvironmentManager.g.CellToWorldPos(CMJ2EnvironmentManager.g.ScreenPosToCell(Input.mousePosition)) + new Vector3 (0f, 0f, -9f);
        }
	}
}
