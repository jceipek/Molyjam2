using UnityEngine;
using System.Collections;

public class CMJ2Manager : MonoBehaviour 
{
	
	public const int LAYER_HERO = 8;
	public const int LAYER_GROUND = 9;
	public const int LAYER_LADDER = 10;
	public const int LAYER_SPIKE = 11;
	public const int LAYER_GATE = 12;
	public const int LAYER_DIRECTIVE = 16;	
	public const int LAYER_GUI = 30;
			
	public const int MASK_ALL_GROUND = (1 << LAYER_GROUND) | (1 << LAYER_SPIKE);
	public const int MASK_ALL_TRIGGERS = (1 << LAYER_LADDER) | (1 << LAYER_GATE);
	public const int MASK_ALL_EXCEPT_HERO = ~(1 << LAYER_HERO);	

    public string m_user = "default";

	public GUIText m_scoreText;
	
    public static CMJ2Manager g;
    
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
		
	public void directives (int now, int total)
	{
		m_scoreText.text = now + " / " + total;
	}

}
