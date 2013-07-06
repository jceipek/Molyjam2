using UnityEngine;
using System.Collections;

public class CMJ2Manager : MonoBehaviour 
{
	
	public const int LAYER_HERO = 8;
	public const int LAYER_GROUND = 9;
	public const int LAYER_SPIKE = 10;

    public string m_user = "default";

    public static CMJ2Manager g;

	void Awake () 
    {
        g = this;
	}

}
