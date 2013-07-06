using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CCellSpriteMultiQuad : CCellSprite 
{	
	
	public int m_quadsHoriz = 1;
	public int m_quadsVert = 1;

	protected virtual void setVertices ()
	{
		Vector3 off = m_centered ? 
			m_offset - new Vector3 (m_scale.x * m_cellSpan.x * 0.5f, m_scale.y * m_cellSpan.y * 0.5f): 
				m_offset; // Top left
		
		int wd = m_quadsHoriz + 1;
		int ht = m_quadsVert + 1;
		int numv = wd * ht;

		bool tintit = false;
		if (m_mesh.vertices == null ||
			m_mesh.vertices.Length != numv)
			tintit = true;
		
		Vector3[] vs = new Vector3 [numv];
		for (int y = 0; y <= m_quadsVert; ++y)
		{
			for (int x = 0; x <= m_quadsHoriz; ++x)
			{
				vs[y * wd + x] = off + 
					new Vector3 (
						Mathf.Lerp(0, m_scale.x * m_cellSpan.x, (float)x / m_quadsHoriz),
						Mathf.Lerp(0, m_scale.y * m_cellSpan.y, 1f - (float)y / m_quadsVert));
			}
		}
		m_mesh.vertices = vs;

		if (tintit)
			tintCell(Color.gray);
	}

	protected override void setMesh (bool mesh_changed)
	{
		m_numCells = new Vector2 ((float)m_atlasSize.x / m_cellSize.x,
		                          (float)m_atlasSize.y / m_cellSize.y);

		int wd = m_quadsHoriz + 1;
		int ht = m_quadsVert + 1;
		int numv = wd * ht;

		if (mesh_changed)
		{
			setVertices();
			
			Vector3 eu = m_xform.eulerAngles;
			float angx = (eu.x + 3600) % 360;
			float angy = (eu.y + 3600) % 360;
			bool flipped = (angx > 90f && angx < 270f) ^ 
				(angy > 90f && angy < 270f);
			
			int numtr = m_quadsVert * m_quadsHoriz * 2;
			int[] tris = new int [numtr * 3];
			int i = 0;
			for (int y = 0; y < m_quadsVert; ++y)
			{
				for (int x = 0; x < m_quadsHoriz; ++x)
				{
					int v = (y * wd) + x;
					if (flipped)
					{
						tris[i++] = v;
						tris[i++] = v + wd;
						tris[i++] = v + 1;
						tris[i++] = v + wd;
						tris[i++] = v + wd + 1;
						tris[i++] = v + 1;
					}
					else	
					{
						tris[i++] = v;
						tris[i++] = v + 1;
						tris[i++] = v + wd;
						tris[i++] = v + wd + 1;
						tris[i++] = v + wd;
						tris[i++] = v + 1;
					}
				}
			}
			m_mesh.triangles = tris;
			
			Vector3[] norms = new Vector3 [numv];
			for (i = 0; i < numv; ++i)
				norms[i] = Vector3.back;
			m_mesh.normals = norms;
		}

		if (m_uv == null)
		{
			m_uv = new Vector2 [numv];
			setCell(m_currentCell);		
		}
	}
	
	public override void setCell (int cell_input)
	{
		if (m_mesh != null)
		{
			Vector2 cell = new Vector2 (cell_input % m_numCells.y,
			                            m_numCells.y - Mathf.Floor(cell_input / m_numCells.y) - m_cellSpan.y);
			Vector2 pixelh = Vector2.one * (0.5f / m_atlasSize.x);
			
			int wd = m_quadsHoriz + 1;
			//int ht = m_quadsVert + 1;
			for (int y = 0; y <= m_quadsVert; ++y)
			{
				for (int x = 0; x <= m_quadsHoriz; ++x)
				{
					m_uv[y * wd + x] = new Vector2 (
						Mathf.Lerp(cell.x / m_numCells.x + pixelh.x, (cell.x + m_cellSpan.x) / m_numCells.x - pixelh.x, (float)x / m_quadsHoriz),
						Mathf.Lerp(cell.y / m_numCells.y + pixelh.y, (cell.y + m_cellSpan.y) / m_numCells.y - pixelh.y, 1f - (float)y / m_quadsVert));
				}
			}
			m_mesh.uv = m_uv;
		}
		
		m_currentCell = cell_input;
	}

}

