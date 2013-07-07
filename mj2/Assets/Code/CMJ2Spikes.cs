using UnityEngine;
using System.Collections;

public class CMJ2Spikes : CMJ2Tile 
{
	void OnTriggerEnter (Collider col)
	{
		if (!col.isTrigger)
			print("Spikes collided with " + col.name);
			
		if (col.gameObject.layer == CMJ2Manager.LAYER_HERO)
		{
			CMJ2Hero hero = col.GetComponent<CMJ2Hero>();
			if (hero == null)
				hero = col.transform.parent.GetComponent<CMJ2Hero>();
				
			if (hero != null)
				hero.changeState(CMJ2Hero.CMJ2HeroState.SPIKED);
		}
	}
}
