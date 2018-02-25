using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Light2D : MonoBehaviour
{

	public float	radius = 1;
	public int		resolution = 360;

	MeshFilter				meshFilter;
	
	Collider2D[] 		results = new Collider2D[20];
	ContactFilter2D		contactFilter;

	Mesh				lightMesh;
	List< Vector3 >		lightVertices = new List< Vector3 >();
	List< int >			lightTriangles = new List< int >();
	
	int					oldResolution;

	Dictionary< Collider2D, SpriteRenderer > spriteRenderers = new Dictionary< Collider2D, SpriteRenderer >();

	List< Vector2 >		spriteVertices = new List< Vector2 >();

	void Start()
	{
		meshFilter = GetComponent< MeshFilter >();

		lightMesh = new Mesh();
		
		contactFilter = new ContactFilter2D();
		contactFilter.useTriggers = false;
	}

	void Update ()
	{
		if (meshFilter == null)
			Start();
		
		UpdateVisibleVertices();

		UpdateLightMesh();
		
		meshFilter.sharedMesh = lightMesh;

		oldResolution = resolution;
	}

	void UpdateVisibleVertices()
	{
		int nbCollider = Physics2D.OverlapCircleNonAlloc(transform.position, radius, results);

		spriteVertices.Clear();
		for (int i = 0; i < nbCollider; i++)
		{
			var col = results[i];

			if (!spriteRenderers.ContainsKey(col))
			{
				var obj = col.gameObject;
				spriteRenderers[col] = obj.GetComponent< SpriteRenderer >();
			}
			
			var sp = spriteRenderers[col];

			if (sp == null)
				continue ;

			var vertices = sp.sprite.vertices;
			
			foreach (var v in vertices)
				spriteVertices.Add(sp.transform.TransformPoint(v));
		}
	}

	RaycastHit2D[] raycastCollisions = new RaycastHit2D[1];

	void UpdateLightMesh()
	{
		int i = 0;

		if (resolution != oldResolution)
		{
			lightVertices.Clear();
			lightTriangles.Clear();
			for (i = 0; i < resolution; i++)
			{
				lightVertices.Add(Vector3.zero);
				lightVertices.Add(Vector3.zero);
				lightVertices.Add(Vector3.zero);
				lightTriangles.Add(0);
				lightTriangles.Add(i * 3 + 1);
				lightTriangles.Add(i * 3 + 2);
			}
		}

		float angle = 0;
		float rres = 360f / resolution;
		float ra = rres * Mathf.Deg2Rad;
		int layerMask = 1 << LayerMask.NameToLayer("Default");
		
		for (i = 0; i < resolution; i++)
		{
			float a = angle * Mathf.Deg2Rad;
			Vector2 dir1 = new Vector2(Mathf.Sin(a), Mathf.Cos(a));
			Vector2 dir2 = new Vector2(Mathf.Sin(a + ra), Mathf.Cos(a + ra));
			int index = i * 3;

			lightVertices[index + 0] = Vector3.zero;
			lightVertices[index + 1] = dir1 * radius;
			lightVertices[index + 2] = dir2 * radius;

			angle += rres;
		}
		
		Vector2 pos = transform.position;
		for (i = 0; i < resolution; i++)
		{
			int index = i * 3;

			int nColl = Physics2D.RaycastNonAlloc(transform.position, lightVertices[index + 1], raycastCollisions, radius, layerMask);

			if (nColl > 0)
			{
				Vector2 v = raycastCollisions[0].point - pos;
				lightVertices[index + 1] = v;
				if (index - 2 > 0)
					lightVertices[index - 1] = v;
			}
		}

		#if UNITY_EDITOR
		lightMesh.Clear();
		#endif
		lightMesh.SetVertices(lightVertices);
		lightMesh.SetTriangles(lightTriangles, 0);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, radius);

		Gizmos.color = Color.white;
		foreach (var vertice in spriteVertices)
		{
			Gizmos.DrawSphere(vertice, .02f);
			Gizmos.DrawLine(transform.position, vertice);
		}
	}
}
