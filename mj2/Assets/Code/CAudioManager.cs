using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CAudioManager : MonoBehaviour 
{
	
	public static CAudioManager g;

	Dictionary<string, CAudioEffectSource> m_effects;
	Dictionary<string, CAudioMusicSource> m_musics;

	public bool m_loadAudioPrefs = true;
	public float m_musicVolume = 1f;
	public float m_effectsVolume = 1f;

	bool m_muted = false;
	
	void Awake ()
	{
		g = this;
	}

	void Start ()
	{
		m_effects = new Dictionary<string, CAudioEffectSource> ();
		CAudioEffectSource[] fxs = GetComponentsInChildren<CAudioEffectSource>();
		foreach (CAudioEffectSource fx in fxs)
			m_effects.Add(fx.name, fx);

		m_musics = new Dictionary<string, CAudioMusicSource> ();
		/*CAudioMusicSource[] muss = GetComponentsInChildren<CAudioMusicSource>();
		foreach (CAudioMusicSource mus in muss)
			m_musics.Add(mus.name, mus);*/

		// Find also inactive children
		foreach (Transform musx in transform)
		{
			CAudioMusicSource mus = musx.GetComponent<CAudioMusicSource>();
			if (mus != null)
				m_musics.Add(mus.name, mus);
		}

		if (m_loadAudioPrefs)
			loadAudioPrefs();
	}

	public void loadAudioPrefs ()
	{
		if (!PlayerPrefs.HasKey(CMJ2Manager.g.m_user + ".audio_fx_volume"))
		{
			saveAudioPrefs();
			return;
		}
		m_effectsVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(CMJ2Manager.g.m_user + ".audio_fx_volume", m_effectsVolume));
		m_musicVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(CMJ2Manager.g.m_user + ".audio_music_volume", m_musicVolume));
	}

	public void saveAudioPrefs ()
	{
		PlayerPrefs.SetFloat(CMJ2Manager.g.m_user + ".audio_fx_volume", m_effectsVolume);
		PlayerPrefs.SetFloat(CMJ2Manager.g.m_user + ".audio_music_volume", m_musicVolume);
	}
	
	public void playEffect (string key, float vol, Vector3 pos)
	{
		CAudioEffectSource fx;
		if (m_effects.TryGetValue(key, out fx))
			fx.play(vol, pos);
	}

	public void playEffect (string key, float vol, Vector3 pos, float pitch)
	{
		CAudioEffectSource fx;
		if (m_effects.TryGetValue(key, out fx))
			fx.play(vol, pos, pitch);
	}

	public void fadeOutAllMusicSources (float fadeout_len = -1f, CAudioMusicSource exceptsrc = null)
	{
		print("fadeout all " + exceptsrc);
		CAudioMusicSource[] srcs = transform.GetComponentsInChildren<CAudioMusicSource>();
		foreach (CAudioMusicSource src in srcs)
		{
			if (src != exceptsrc)
			{
				src.m_playImmediately = false; // Cancel play at Start or duplicate
				if (src.isPlaying)
					src.fadeOut(fadeout_len);
			}
		}
	}

	void activateMusicSource (CAudioMusicSource newsrc)
	{
		// Fadeout everything but this, according to their predef fadeout time
		fadeOutAllMusicSources(-1, newsrc);
		newsrc.gameObject.active = true;
	}

	public void playMusic (CAudioMusicSource byval)
	{
		if (!byval.isPlaying)
			activateMusicSource(byval);
	}

	public void playMusic (string byname)
	{
		CAudioMusicSource mus;
		if (m_musics.TryGetValue(byname, out mus) &&
		    !mus.isPlaying)
			activateMusicSource(mus);
	}

	public void setMusicVolume (float vol)
	{
		if (vol == m_musicVolume)
			return;
		
		m_musicVolume = vol;
		foreach (CAudioMusicSource aum in m_musics.Values)
			aum.applyVolume();

		saveAudioPrefs();
	}

	public void setEffectsVolume (float vol)
	{
		if (vol == m_effectsVolume)
			return;
		
		m_effectsVolume = vol;
		foreach (CAudioEffectSource aufx in m_effects.Values)
			aufx.applyVolume();

		saveAudioPrefs();
	}

}
