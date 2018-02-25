using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaballsPostProcess : MonoBehaviour
{

	public GameObject	metaballCameraPrefab;
	public Color		metaballColor;

	Camera				mainCamera;
	Camera				metaballCamera;

	RenderTexture		metaballTexture;

	Material			metaballMaterial;

	int					metaballLayer;

	void Start ()
	{
		mainCamera = Camera.main;

		var g = GameObject.Instantiate(metaballCameraPrefab, mainCamera.transform);
		metaballCamera = g.GetComponent< Camera >();

		metaballCamera.orthographicSize = mainCamera.orthographicSize;

		metaballTexture = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 0, RenderTextureFormat.ARGB32);
		metaballCamera.targetTexture = metaballTexture;

		metaballMaterial = new Material(Shader.Find("Hidden/MetaballEffect"));
        metaballMaterial.hideFlags = HideFlags.HideAndDontSave;

		metaballLayer = LayerMask.NameToLayer("Metaball");
	}
	
    void OnPreCull()
	{
		//Update metaball size
		if (metaballTexture.width != mainCamera.pixelWidth || metaballTexture.height != mainCamera.pixelHeight)
		{
			metaballTexture = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 16, RenderTextureFormat.Default);
			metaballCamera.targetTexture = metaballTexture;
		}

		UpdateMetaballCameraSettings();
	}

	void UpdateMetaballCameraSettings()
	{
		metaballCamera.CopyFrom(mainCamera);
		metaballCamera.renderingPath = RenderingPath.Forward;
		metaballCamera.backgroundColor = new Color(0, 0, 0, 0);
		metaballCamera.clearFlags = CameraClearFlags.SolidColor;
		metaballCamera.cullingMask = 1 << metaballLayer;
		metaballCamera.rect = new Rect(0, 0, 1, 1);
		metaballCamera.enabled = true;
		metaballCamera.targetTexture = metaballTexture;
	}
	
    void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		metaballMaterial.SetTexture("_MetaballTex", metaballTexture);
		metaballMaterial.SetColor("_Color", metaballColor);
		Graphics.Blit(source, destination, metaballMaterial);
	}

	void OnDestroy()
	{
		DestroyImmediate(metaballMaterial);
	}
}
