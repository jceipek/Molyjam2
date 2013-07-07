using UnityEngine;
using System.Collections;

public class DebugObjectController : MonoBehaviour {

	void OnDrawGizmos () {
 		Gizmos.color = Color.black;
        Gizmos.DrawWireCube (transform.position, Vector3.one);
	}
}
