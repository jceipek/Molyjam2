using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CCellSprite : MonoBehaviour 
{	
	protected const float OVERLAY_EPSILON = 0.001f;
	
	protected Mesh m_mesh = null;
	public Mesh mesh {
		get { return m_mesh; }
	}
	protected MeshFilter m_mf;

	protected Vector2[] m_uv;
	
	public bool m_centered = true;
	public Vector3 m_offset = Vector3.zero;
	public Vector2 m_scale = Vector2.one;
	
	public Vector2 m_atlasSize = new Vector2 (512, 512);	// Total pixels
	public Vector2 m_cellSize = new Vector2 (32, 32);		// Cell pixels
	
	[HideInInspector]
	public Vector2 m_numCells = new Vector2 (1, 1);
	
	public int m_currentCell = 0;							// From top, left to right, then top to bottom
	
	public Vector2 m_cellSpan = new Vector2 (1, 1);

	[HideInInspector]
	public Transform m_xform;

	protected virtual void Awake () 
	{
		m_xform = transform;

		m_mf = gameObject.GetComponent<MeshFilter>();
		if (m_mf == null)
			m_mf = gameObject.AddComponent<MeshFilter>();

		if (Application.isPlaying &&
		    m_mf.sharedMesh != null)
		{
			m_mesh = m_mf.sharedMesh;
			setMesh();
		}
		else
		{
			m_mesh = new Mesh ();
			setMesh(true);
			m_mf.sharedMesh = m_mesh;
		}

		tintCell(Color.gray);
	}

	public Mesh getMesh ()
	{
		return m_mesh;
	}

	public void tintCell (Color color)
	{
		int numv = m_mesh.vertexCount;
		Color[] cs = new Color [numv];
		for (int i = 0; i < numv; ++i)
			cs[i] = color;
		m_mesh.colors = cs;
	}

	protected virtual void setMesh (bool mesh_changed = false)
	{
		m_numCells = new Vector2 ((float)m_atlasSize.x / m_cellSize.x,
							 	  (float)m_atlasSize.y / m_cellSize.y);

		//m_mesh.colors = null;

		if (mesh_changed)
		{
			Vector3 off = m_centered ? 
				m_offset - new Vector3 (m_scale.x * m_cellSpan.x * 0.5f, m_scale.y * m_cellSpan.y * 0.5f): 
				m_offset; // Top left
			
	        m_mesh.vertices = new Vector3[] { 
	                off + new Vector3(-OVERLAY_EPSILON, 
									  m_scale.y * m_cellSpan.y + OVERLAY_EPSILON),	// topleft
	                off + new Vector3(m_scale.x * m_cellSpan.x + OVERLAY_EPSILON, 
									  m_scale.y * m_cellSpan.y + OVERLAY_EPSILON),	// topright
	                off + new Vector3(m_scale.x * m_cellSpan.x + OVERLAY_EPSILON, 
									  -OVERLAY_EPSILON),	// botright
	                off + new Vector3(-OVERLAY_EPSILON, 
								      -OVERLAY_EPSILON)	// botleft
	        };        
			
			Vector3 eu = m_xform.eulerAngles;
			float angx = (eu.x + 3600) % 360;
			float angy = (eu.y + 3600) % 360;
			bool flipped = (angx > 90f && angx < 270f) ^ 
						   (angy > 90f && angy < 270f);
			
			// Flip triangles to allow editor click
			if (flipped)
				m_mesh.triangles = new int[] { 
	                0,2,1,
					2,0,3
	    	    };
			else	
				m_mesh.triangles = new int[] { 
	                0,1,2,
					2,3,0
	        	};
		}

		m_mesh.normals = new Vector3[] { 
                Vector3.back,
                Vector3.back,
                Vector3.back,
                Vector3.back
        };      

		if (m_uv == null)
		{
			m_uv = new Vector2 [4];
			setCell(m_currentCell);
		}
	}
		
	public Vector2 pixelToUV (Vector2 pix)
	{
		return new Vector2 (Mathf.Round(pix.x) / m_atlasSize.x,
		                    Mathf.Round(pix.y) / m_atlasSize.y);
	}
		
	public virtual void setCell (int cell_input)
	{
		if (m_mesh != null)
		{
			Vector2 cell = new Vector2 (cell_input % m_numCells.y,
										m_numCells.y - Mathf.Floor(cell_input / m_numCells.y) - m_cellSpan.y);
	
			Vector2 pixelh = new Vector2 (0.5f / m_atlasSize.x,
			                              0.5f / m_atlasSize.y);
			m_uv[0] = new Vector2 (cell.x / m_numCells.x + pixelh.x, 
								  (cell.y + m_cellSpan.y) / m_numCells.y - pixelh.y); 
			m_uv[1] = new Vector2 ((cell.x + m_cellSpan.x) / m_numCells.x - pixelh.x, 
								  (cell.y + m_cellSpan.y) / m_numCells.y - pixelh.y); 
			m_uv[2] = new Vector2 ((cell.x + m_cellSpan.x) / m_numCells.x - pixelh.x, 
								  cell.y / m_numCells.y + pixelh.y); 
			m_uv[3] = new Vector2 (cell.x / m_numCells.x + pixelh.x, 
								  cell.y / m_numCells.y + pixelh.y);
			m_mesh.uv = m_uv;			
		}
		
		m_currentCell = cell_input;
	}
	
	public void snap ()
	{
		Vector3 lp = m_xform.localPosition;
		m_xform.localPosition = new Vector3 (Mathf.Round(lp.x), Mathf.Round(lp.y), 0);
		float z = m_xform.position.z;

		foreach (Transform xf in m_xform)
		{
			Vector3 curpos = xf.position;
			xf.position = new Vector3 (curpos.x, curpos.y, z);
		}
	}
	
	public void changeZ (float z)
	{
		m_xform.localPosition += new Vector3 (0, 0, z);
		
		foreach (Transform xf in m_xform)
			xf.localPosition -= new Vector3 (0, 0, z);
	}
	
	public void rotate (bool clock)
	{
		Vector3 eu = m_xform.localEulerAngles;
		if (clock)
			eu.z = (eu.z + 270f) % 360f;
		else
			eu.z = (eu.z + 90f) % 360f;
		m_xform.localEulerAngles = eu;
		setMesh(true);
	}
	
	public void flip (bool horiz, bool vert)
	{
		Vector3 eu = m_xform.localEulerAngles;
		if (horiz)
		{
			eu.y = (eu.y + 180f) % 360f;
			m_offset.z = -m_offset.z;
		}
		if (vert)
		{
			eu.x = (eu.x + 180f) % 360f;
			m_offset.z = -m_offset.z;
		}
		m_xform.localEulerAngles = eu;
		setMesh(true);
	}
	
	public void resizeAtlas (int ax, int ay)
	{
		int y = m_currentCell / Mathf.RoundToInt(m_numCells.x);
		int x = m_currentCell % Mathf.RoundToInt(m_numCells.x);
		//print ("cell " + x + "," + y);
		m_atlasSize = new Vector2 (ax, ay);	
		m_numCells = new Vector2 ((float)m_atlasSize.x / m_cellSize.x,
							 	  (float)m_atlasSize.y / m_cellSize.y);
		setCell(y * Mathf.RoundToInt(m_numCells.x) + x);
	}
	
#if UNITY_EDITOR
	protected virtual void Update ()
	{
		// Only run in editing mode
		if (Application.isPlaying)
			return;

		// Force setCell
		m_uv = null;

		if (m_mesh == null || GetComponent<MeshFilter>() == null)
			Awake();
		else
			m_xform = transform;
			
		if (renderer == null)
			gameObject.AddComponent<MeshRenderer>();	

		setMesh(true);
	}

	protected void OnDrawGizmosSelected ()
	{
		Gizmos.color = new Color (1f, 0.5f, 0);
		Vector3 pos = transform.position;
		Gizmos.DrawLine(pos + new Vector3 (0.05f, 0.05f), pos - new Vector3 (0.05f, 0.05f));
		Gizmos.DrawLine(pos + new Vector3 (-0.05f, 0.05f), pos - new Vector3 (-0.05f, 0.05f));
	}
#endif
	
}