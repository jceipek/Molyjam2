using UnityEngine;
using System.Collections;

public class CAudioEffectSource : MonoBehaviour 
{

	public int m_parallel = 1;
	public float m_pitchRandom = 0.15f;

	AudioSource[,] m_gameObjectQueue;
	AudioSource[] m_allAudio;
	int[] m_queuePos;
	int m_numClipsTotal;

	float m_baseVolume;
	float m_origVolume;
	float m_origPitch;

	// Use this for initialization
	void Awake () 
	{
		createGameObjects();
	}

	void createGameObjects ()
	{
		m_allAudio = GetComponentsInChildren<AudioSource>() as AudioSource[];

		if (m_allAudio == null || m_allAudio.Length == 0)
		{
			m_numClipsTotal = 0;
			return;
		}

		m_gameObjectQueue = new AudioSource [m_allAudio.Length, m_parallel];
		m_queuePos = new int [m_allAudio.Length];

		int j = 0;
		foreach (AudioSource asrc in m_allAudio)
		{
			if (asrc.gameObject == gameObject)
				continue;

			m_queuePos[j] = 0;
			m_gameObjectQueue[j, 0] = asrc;
			m_origVolume = asrc.volume;
			m_baseVolume = asrc.volume = 0f;
			m_origPitch = asrc.pitch;

			// Silent play
			//asrc.Play();

			// Duplicate if >2
			for (int i = 1; i < m_parallel; ++i)
			{
				GameObject go = Instantiate(asrc.gameObject) as GameObject;
				go.transform.parent = transform;
				go.name = asrc.name;

				m_gameObjectQueue[j, i] = go.GetComponent<AudioSource>();
				m_gameObjectQueue[j, i].volume = 0f;
				m_gameObjectQueue[j, i].Play();
			}
			++j;
		}
		m_numClipsTotal = j;
	}

	public void play (float vol, Vector3 pos)
	{
		if (m_numClipsTotal == 0)
			return;
		int j = Random.Range(0, m_numClipsTotal);
		AudioSource asrc = m_gameObjectQueue[j, m_queuePos[j]];
		if (++m_queuePos[j] == m_parallel)
			m_queuePos[j] = 0;
		asrc.transform.position = pos;
		asrc.pitch = ((UnityEngine.Random.value - 0.5f) * m_pitchRandom + 1f) * m_origPitch;  // Randomizing pitch around 1
		m_baseVolume = Mathf.Clamp(vol, 0, 1);
		asrc.volume = m_baseVolume * m_origVolume * CAudioManager.g.m_effectsVolume;
		asrc.Play();
	}
	
	public void play (float vol, Vector3 pos, float pitch)
	{
		if (m_numClipsTotal == 0)
			return;
		int j = Random.Range(0, m_numClipsTotal);
		AudioSource asrc = m_gameObjectQueue[j, m_queuePos[j]];
		if (++m_queuePos[j] == m_parallel)
			m_queuePos[j] = 0;
		asrc.transform.position = pos;
		asrc.pitch = pitch; 
		m_baseVolume = vol;
		asrc.volume = m_baseVolume * m_origVolume * CAudioManager.g.m_effectsVolume;
		asrc.Play();
	}

	public void applyVolume ()
	{
		foreach (AudioSource asrc in m_allAudio)
			asrc.volume = m_baseVolume * m_origVolume * CAudioManager.g.m_effectsVolume;
	}

}
