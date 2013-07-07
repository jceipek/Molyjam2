using UnityEngine;
using System.Collections;

public class UIProperties : MonoBehaviour {

	public Vector2 levelDims;
	public Vector2 interfacePanelDims;
	public Transform interfacePanelOrigin;

	void OnDrawGizmos () {
 		Gizmos.color = Color.black;

 		Vector3 bounds = new Vector3 (levelDims.x, levelDims.y, 1);
 		Vector3 anchor = new Vector3 (levelDims.x/2f - 0.5f, levelDims.y/2f - 0.5f, 1f);
        Gizmos.DrawWireCube (transform.position + anchor, bounds);

		bounds = new Vector3 (interfacePanelDims.x, interfacePanelDims.y, 1);
		anchor = new Vector3 (interfacePanelDims.x/2f - 0.5f, interfacePanelDims.y/2f - 0.5f, 1f);
        Gizmos.DrawWireCube (interfacePanelOrigin.position + anchor, bounds);
	}
}
