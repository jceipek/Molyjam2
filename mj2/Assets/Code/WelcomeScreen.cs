using UnityEngine;
using System.Collections;

public class WelcomeScreen : MonoBehaviour {

	public string m_levelToLoad = "Level 1";
	
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(audio);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			Destroy(this);
			Application.LoadLevel(m_levelToLoad);
		}
	}
}
