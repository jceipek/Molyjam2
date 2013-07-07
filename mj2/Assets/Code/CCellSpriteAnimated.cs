using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CCellSpriteAnimated : CCellSprite 
{		
	
	public string m_animationName;
	public string m_animationsJson;
	
	public bool isAnimating {
		get { return m_currentAnimation != null; }
	}		
	
	protected bool m_invis = false;
	protected Renderer m_renderer;
	protected int[] m_triangles;

	// Example: {"anim1":{"cells":[12,20,12],"loop":true,"freq":0.1}}
	protected class CAnimationData {
		public List<int> m_cells;
		public int m_currentCellIndex = 0;
		public float m_currentFreq = 0.1f;
		public bool m_loop = true;
		
		public CAnimationData (object data) {
			m_cells = new List<int> ();
			
			if (data is Hashtable)
			{
				foreach (DictionaryEntry de in (data as Hashtable))
				{
					string key = de.Key.ToString();
					switch (key) 
					{
						case "cells":
							if (de.Value is ArrayList)
							{
								foreach (object instance in (de.Value as ArrayList))
									m_cells.Add(int.Parse(instance.ToString()));
							}
							break;
							
						case "freq":
							m_currentFreq = float.Parse(de.Value.ToString());							
							break;
						
						case "loop":
							try
							{
								m_loop = bool.Parse(de.Value.ToString());
							}
							catch (System.Exception)
							{
								try
								{
									 m_loop = int.Parse(de.Value.ToString()) == 1;
								}
								catch (System.Exception ee)
								{
									Debug.LogError("Error " + ee);
								}
							}
							break;
					}							
				}
			}
		}
	}
	protected Dictionary<string, CAnimationData> m_animations;
	protected CAnimationData m_currentAnimation;
	
	protected string m_currentAnimationName;
	public string currentAnimationName {
		get { return m_currentAnimationName; }
	}		

	protected float m_timeForNextFrame;
	
	protected override void Awake ()
	{
		base.Awake ();

		if (m_mesh.triangles == null ||
		    m_mesh.triangles.Length == 0)
			setMesh(true);

		m_renderer = renderer;
		
		if (m_animationsJson != null && m_animationsJson != "")
		{
			loadAnimations(m_animationsJson);
			if (m_animationName != null && m_animationName != "")
				runAnimation(m_animationName);
		}		
	}

	protected override void setMesh(bool mesh_changed)
	{
		base.setMesh(mesh_changed);

		//print(name + " " + (m_mesh.triangles == null ? "null" : m_mesh.triangles.Length.ToString()));
		if (m_mesh.triangles != null &&
		    m_mesh.triangles.Length > 0)
			m_triangles = m_mesh.triangles;
	}

	public override void setCell(int cell_input)
	{
		if (cell_input < 0)
		{
			if (!m_invis)
			{
				m_invis = true;
				m_mesh.triangles = null;
			}
			m_mesh.uv = m_uv;
			m_currentCell = cell_input;
		}
		else
		{
			if (m_invis)
			{
				m_invis = false;
				m_mesh.triangles = m_triangles;
			}
			base.setCell(cell_input);
		}
	}

	/*public void makeVisible (bool onoff)
	{
		m_mesh.triangles = onoff ? m_triangles : null;
		m_invis = !onoff;
	}*/
	
	protected void loadAnimations (string json)
	{
		object json_obj = MiniJSON.jsonDecode(json);
		if (json_obj is Hashtable)
		{
			foreach (DictionaryEntry de in (json_obj as Hashtable))
			{
				CAnimationData data = new CAnimationData (de.Value);
				if (m_animations == null)
					m_animations = new Dictionary<string, CAnimationData> ();
				m_animations.Add(de.Key.ToString(), data);
			}
		}
	}
	
	public bool runAnimation (string anim)
	{
		CAnimationData data = null;
		if (m_animations == null || 
		    !m_animations.TryGetValue(anim, out data))
			return false;
			
		m_currentAnimationName = anim;	
			
		m_currentAnimation = data;
		m_currentAnimation.m_currentCellIndex = 0;
		m_timeForNextFrame = 0;
			
		int cell = m_currentAnimation.m_cells[m_currentAnimation.m_currentCellIndex];
		//print(name + " anim " + anim + " cell " + cell);
		setCell(cell);

		return true;
	}

#if UNITY_EDITOR
	protected override void Update ()
	{
		base.Update();
#else	
	protected void Update ()
	{
#endif
		// Don't animate if there's no animation, or if renderer is not visible
		if (m_currentAnimation == null || (!m_renderer.isVisible && !m_invis))
			return;
		
		float now = Time.time;
		if (now >= m_timeForNextFrame)
		{
			if (++m_currentAnimation.m_currentCellIndex >= m_currentAnimation.m_cells.Count)
			{
				if (m_currentAnimation.m_loop)
					m_currentAnimation.m_currentCellIndex = 0;
				else
				{
					m_currentAnimation = null;
					return;
				}
			}				
			m_timeForNextFrame = now + m_currentAnimation.m_currentFreq;			
			
			int cell = m_currentAnimation.m_cells[m_currentAnimation.m_currentCellIndex];
			if (cell != m_currentCell)
				setCell(cell);
		}	
	}
	
}
