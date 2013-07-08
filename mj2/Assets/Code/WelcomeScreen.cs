using UnityEngine;
using System.Collections;

public class WelcomeScreen : MonoBehaviour {

	public string m_levelToLoad = "Level 1";
		
    public static WelcomeScreen g;
    
	void Awake () 
    {
        g = this;
	}

	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			Application.LoadLevel(m_levelToLoad);
		}
	}
}
