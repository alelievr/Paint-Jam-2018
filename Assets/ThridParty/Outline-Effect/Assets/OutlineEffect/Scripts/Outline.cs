﻿using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Renderer))]
public class Outline : MonoBehaviour
{
    public bool     fullSpriteGlow = false;

    [Space]
	public Color    color = Color.white;
    public float    lineThickness = 2f;
    [RangeAttribute(0f, 1f)]
    public float    lineIntensity = 1f;
    [RangeAttribute(0f, 1f)]
    public float    alphaCutoff = 0f;
    public bool     pixelSnap = false;

    [Space]
    public bool     flipY = false;
    public bool     allowOutlineOverlap = true;
    public bool     autoColor = false;
    public bool     hideSprite = false;

    [Space]
    public bool     autoOutline = true;
    public bool     lastLinkedToFirst = true;
    public bool     outlineBezier = true;

    public float    outlineStep = .05f;

    public List< OutlineVertice > outlineVertices = new List< OutlineVertice >()
    {
        new OutlineVertice(Vector3.zero),
        new OutlineVertice(Vector3.right),
        new OutlineVertice(Vector3.one)
    };
    
    OutlineEffect   outlineEffect;

	int             originalLayer;
    int             outlineLayer;
	Material        originalMaterial;
    Material        outlineMaterial;
    LineRenderer    lineRenderer;

    new Renderer renderer;

    Material CreateNewOutlineMaterial(Color emissionColor)
    {
        Material m = new Material(Shader.Find("Hidden/OutlineBufferEffect"));
        m.SetColor("_Color", emissionColor);
        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m.SetInt("_ZWrite", 0);
        m.DisableKeyword("_ALPHATEST_ON");
        m.EnableKeyword("_ALPHABLEND_ON");
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m.renderQueue = 3000;
        return m;
    }

    void CreateBezierPoints(int i1, int i2, List< Vector3 > points)
    {
        Vector3 point1 = outlineVertices[i1].position;
        Vector3 point4 = outlineVertices[i2].position;
        Vector3 point3 = outlineVertices[i2].t1 + point4;
        Vector3 point2 = outlineVertices[i1].t2 + point1;
        for (float t = 0.00f; t < 1f; t = t + outlineStep) {
            float xValue = Mathf.Pow((1-t), 3) * point1.x + 3 * Mathf.Pow((1-t), 2) * t * point2.x + 3 * (1-t) * Mathf.Pow(t, 2) * point3.x + Mathf.Pow(t, 3) * point4.x;
            float yValue = Mathf.Pow((1-t), 3) * point1.y + 3 * Mathf.Pow((1-t), 2) * t * point2.y + 3 * (1-t) * Mathf.Pow(t, 2) * point3.y + Mathf.Pow(t, 3) * point4.y;
            points.Add(new Vector3(xValue, yValue, 0));
        }
    }

    public void CreateLinerendererPoints()
    {
        if (outlineVertices.Count <= 1)
            return ;
        List< Vector3 > points = new List< Vector3 >();
        if (outlineBezier)
        {
            if (outlineStep < 0.005f)
                return ;
            
            for (int i = 0; i < outlineVertices.Count - 1; i++)
                CreateBezierPoints(i, i + 1, points);
            if (lastLinkedToFirst)
                CreateBezierPoints(outlineVertices.Count - 1, 0, points);
        }
        else
        {
            Vector3 lastPos = Vector3.zero;
            Vector3 lastVector = Vector3.zero;
            for (int i = 0; i < outlineVertices.Count; i++)
            {
                var p = outlineVertices[i].position;
                if (i != 0)
                {
                    if (i > 1)
                    {
                        float angle = Vector3.Angle(lastVector, p - lastPos);
                    }
                    lastVector = p - lastPos;
                }
                //set simple vertice:
                points.Add(p);
                lastPos = p;
            }
            if (lastLinkedToFirst)
                points.Add(outlineVertices[0].position);
        }
        lineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            //world scale fix:
            Vector3 p = points[i];
            p.x /= transform.localScale.x;
            p.y /= transform.localScale.y;
            p.z /= transform.localScale.z;
            lineRenderer.SetPosition(i, p);
        }
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
    }

	void Start()
    {
        outlineLayer = LayerMask.NameToLayer("Outline");
        renderer = GetComponent< Renderer >();
        outlineMaterial = CreateNewOutlineMaterial(color);
        if (!autoOutline)
        {
            GameObject lineObject = new GameObject("lineRenderer");
            lineObject.transform.parent = transform;
            lineRenderer = lineObject.AddComponent< LineRenderer >();
            lineRenderer.useWorldSpace = false;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = lineThickness / 100;
            lineRenderer.endWidth = lineThickness / 100;
            lineRenderer.positionCount = 0;
            lineRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
            CreateLinerendererPoints();
            
            lineObject.transform.localPosition = Vector3.zero;
            lineObject.transform.localRotation = Quaternion.identity;
            lineObject.transform.localScale = Vector3.one;
        }
    }

    void OnEnable()
    {
		if(outlineEffect == null)
			outlineEffect = Camera.main.GetComponent<OutlineEffect>();
		outlineEffect.AddOutline(this);
    }

    void OnDisable()
    {
        outlineEffect.RemoveOutline(this);
    }

    public void PreRender()
    {
        if (!autoOutline)
            return ;
        //update material datas:
        originalMaterial = renderer.sharedMaterial;
        originalLayer = gameObject.layer;

        // if(eraseRenderer)
        //     renderer.material = outlineEraseMaterial;
        // else
        renderer.material = outlineMaterial;
        //to do better
        renderer.sharedMaterial.SetColor("_Color", color);
        renderer.sharedMaterial.SetFloat("_AlphaCutoff", alphaCutoff);
        renderer.sharedMaterial.SetFloat("_LineThickness", lineThickness);
        renderer.sharedMaterial.SetFloat("_LineIntensity", lineIntensity);
        renderer.sharedMaterial.SetInt("_PixelSnap", pixelSnap ? 1 : 0);
        renderer.sharedMaterial.SetInt("_FullSprite", fullSpriteGlow ? 1 : 0);
        renderer.sharedMaterial.SetInt("_FlipY", flipY ? 1 : 0);
        renderer.sharedMaterial.SetInt("_AllowOutlineOverlap", allowOutlineOverlap ? 1 : 0);
        renderer.sharedMaterial.SetInt("_AutoColor", autoColor ? 1 : 0);

        if (hideSprite)
            renderer.enabled = true;
        if (renderer is MeshRenderer)
            renderer.sharedMaterial.mainTexture = originalMaterial.mainTexture;

        gameObject.layer = outlineLayer;
    }

    public void PostRender()
    {
        if (!autoOutline)
            return ;
        if (renderer is MeshRenderer)
            renderer.sharedMaterial.mainTexture = null;

        renderer.material = originalMaterial;
        gameObject.layer = originalLayer;
        if (hideSprite)
            renderer.enabled = false;
    }

    [System.Serializable]
    public class OutlineVertice
    {
        public Vector3 position;
        public Vector3 t1;
        public Vector3 t2;

        public OutlineVertice(Vector3 pos)
        {
            position = pos;
        }
        
        public OutlineVertice(Vector3 pos, Vector3 t1, Vector3 t2)
        {
            position = pos;
            this.t1 = t1;
            this.t2 = t2;
        }
    }
}
