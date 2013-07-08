using UnityEngine;
using System.Collections;

public class WelcomeScreen : MonoBehaviour {

	public string m_levelToLoad = "Level 1";
		
    public static WelcomeScreen g;
    
	void Awake () 
    {
    	if (g != null)
    	{
    		DestroyImmediate(gameObject);
    		return;
    	}
    	
        g = this;
        
       DontDestroyOnLoad(gameObject);
       DontDestroyOnLoad(this);
	}

	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			Destroy(this);
			Application.LoadLevel(m_levelToLoad);
		}
	}
}
