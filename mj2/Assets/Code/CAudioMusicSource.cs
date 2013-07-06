using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class CAudioMusicSource : MonoBehaviour 
{
	const float CHECK_FREQ = 0.25f;
	float m_nextCheck = 0;

	public bool m_playImmediately = true;
	public float m_fadeInAt = 0f;
	public float m_fadeOutAt = 2400f;

	public bool m_destroyWhenComplete = true;

	public float m_fadeInLength = 1f;
	public float m_fadeOutLength = 1f;

	public float m_activateSourceAt = 2400f;
	public CAudioMusicSource m_activateSource;

	bool m_playing;
	public bool isPlaying {
		get { return m_playing; }
	}

	float m_origVolume = 1f;
	float m_baseVolume = 0f;

	bool m_fadingIn;
	bool m_fadingOut;

#if UNITY_EDITOR
	public float m_currentTime = 0;
#endif
	
	// Use this for initialization
	void Awake () 
	{
		m_fadingIn = false;
		m_fadingOut = false;
		m_playing = false;
	}

	void Start ()
	{
		print("Music " + name + " started at " + m_fadeInAt + "/" + audio.clip.length);

        m_origVolume = audio.volume;
        m_baseVolume = audio.volume = 0f;

        if (m_playImmediately)
			play(true);
	}

	public void fadeIn (float tm = -1f)
	{
		if (m_fadingIn)
			return;
		if (tm >= 0f)
			m_fadeInLength = tm;
		m_fadingIn = true;

		if (!m_playing)
			play(true);
	}
	
	public void fadeOut (float tm = -1f)
	{
		if (!m_playing || m_fadingOut)
			return;
		if (tm >= 0f)
			m_fadeOutLength = tm;
		m_fadingOut = true;
		m_fadingIn = false;
	}

	public void skipTo (float tm)
	{
		audio.time = tm;
	}

	public void play (bool playstop)
	{
		m_playing = playstop;

		if (playstop)
		{
			m_fadingIn = true;
			audio.Play();
		}
		else
			audio.Stop();
	}

	public CAudioMusicSource duplicate ()
	{
		GameObject go = Instantiate(gameObject) as GameObject;
		if (go)
		{
			go.transform.parent = transform.parent;
			go.name = name;

			print("duplicated " + name);

			return go.GetComponent<CAudioMusicSource>();
		}
		return null;
	}

	public void applyVolume ()
	{
		audio.volume = m_baseVolume * m_origVolume * CAudioManager.g.m_musicVolume;
	}
	
	// Update is called once per frame
	void Update () 
	{
#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
		if (m_playing && Input.GetKeyDown(KeyCode.RightArrow))
		{
			audio.time += 5f;
		}
#endif
		if (!m_playing)
			return;

		float atime = audio.time;
		if (atime < m_fadeInAt)
		{
			atime = audio.time = m_fadeInAt;
			if (!audio.isPlaying)
				audio.Play();
		}

		if (m_fadingIn)
		{
			if ((m_baseVolume += Time.deltaTime / m_fadeInLength) >= 1f)
			{
				m_fadingIn = false;
				m_baseVolume = 1f;
			}
			audio.volume = m_baseVolume * m_origVolume * CAudioManager.g.m_musicVolume;
		}
		else if (m_fadingOut)
		{
			if ((m_baseVolume -= Time.deltaTime / m_fadeOutLength) <= 0f)
			{
				m_fadingOut = false;
				m_baseVolume = 0f;
				play(false);
				if (m_destroyWhenComplete)
					Destroy(gameObject);
				else
					gameObject.active = false;
			}
			audio.volume = m_baseVolume * m_origVolume * CAudioManager.g.m_musicVolume;
		}

		float now = Time.time;
		if (now >= m_nextCheck)
		{
			m_nextCheck = now + CHECK_FREQ;

			if (!m_fadingOut &&
			    atime >= m_fadeOutAt)
				m_fadingOut = true;

			if (atime >= m_activateSourceAt && 
			    m_activateSource != null)
			{ 
			 	if (!m_activateSource.gameObject.active)
					m_activateSource.gameObject.active = true;
				else if (m_activateSource == this && m_playImmediately)
					duplicate();

				m_activateSource = null;
			}

#if UNITY_EDITOR
			m_currentTime = audio.time;
#endif
		}
	}
}
