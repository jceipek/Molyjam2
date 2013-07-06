using UnityEngine;
using System.Collections;

public class CMJ2Manager : MonoBehaviour 
{

    public string m_user = "default";

    public static CMJ2Manager g;

	void Awake () 
    {
        g = this;
	}

}
