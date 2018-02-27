using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

/// <inheritdoc />
/// <summary>
/// Listens for input from the mouse, where shapes are created and updated by 
/// the current cursor position.
/// </summary>
public class DrawController : MonoBehaviour
{
    public DrawMode Mode = DrawMode.Rectangle;

    public DrawShape RectanglePrefab;
    public DrawShape CirclePrefab;

    // Associates a draw mode to the prefab to instantiate
    private Dictionary<DrawMode, DrawShape> _drawModeToPrefab;

    private readonly List<DrawShape> _allShapes = new List<DrawShape>();

    private DrawShape CurrentShapeToDraw { get; set; }
    private bool IsDrawingShape { get; set; }

	CinemachineVirtualCamera	vcam;
	CinemachineVirtualCamera	playerVcam;
	Transform					playerTransform;
	Transform					lastPlayerTransform;
    CinemachineBrain brain;
    
    private void Awake()
    {
        _drawModeToPrefab = new Dictionary<DrawMode, DrawShape> {
            {DrawMode.Rectangle, RectanglePrefab},
            {DrawMode.Circle, CirclePrefab}
        };
		vcam = GameObject.Find("PlayerCameraDrawing").GetComponent< CinemachineVirtualCamera >();
		playerVcam = GameObject.Find("PlayerCamera").GetComponent< CinemachineVirtualCamera >();
        brain = GameObject.FindObjectOfType<CinemachineBrain>();
        vcam.m_Lens.OrthographicSize = playerVcam.m_Lens.OrthographicSize;
		playerTransform = GameObject.Find("Player").transform;
		lastPlayerTransform = new GameObject("PlayerTransform").transform;
		vcam.Follow = lastPlayerTransform;
		vcam.gameObject.SetActive(false);
    }

    private void Update()
    {
        var mousePos = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var clickDown = Input.GetKeyDown(KeyCode.Mouse0) && CurrentShapeToDraw == null &&
                    !EventSystem.current.IsPointerOverGameObject();
        var clickUp = Input.GetKeyUp(KeyCode.Mouse0) && CurrentShapeToDraw != null &&
                    !EventSystem.current.IsPointerOverGameObject();
        var canUpdateShape = CurrentShapeToDraw != null && IsDrawingShape;

        if (GameManager.instance.gameState == GameManager.GameState.Pause)
            return ;

        if (!IsDrawingShape && !brain.IsBlending)
            lastPlayerTransform.position = playerVcam.transform.position;

        if (clickDown || clickUp) {
            AddShapeVertex(mousePos);
        } else if (canUpdateShape) {
            UpdateShapeVertex(mousePos);
        }
    }

    /// <summary>
    /// Adds a new vertex to the current shape at the given position, 
    /// or creates a new shape if it doesn't exist
    /// </summary>
    private void AddShapeVertex(Vector2 position)
    {
        if (CurrentShapeToDraw == null) {
            lastPlayerTransform.position = playerVcam.transform.position;
            // No current shape -> instantiate a new shape and add two vertices:
            // one for the initial position, and the other for the current cursor
            var prefab = _drawModeToPrefab[Mode];
            CurrentShapeToDraw = Instantiate(prefab);
            CurrentShapeToDraw.name = "Shape " + _allShapes.Count;

            CurrentShapeToDraw.AddVertex(position);
            CurrentShapeToDraw.AddVertex(position);
            
            CurrentShapeToDraw.SimulatingPhysics = false;

            IsDrawingShape = true;

            _allShapes.Add(CurrentShapeToDraw);
			vcam.gameObject.SetActive(true);
			playerVcam.gameObject.SetActive(false);
        } else {
            // Current shape exists -> add vertex if finished, 
            // otherwise start physics simulation and reset reference
            IsDrawingShape = !CurrentShapeToDraw.ShapeFinished;

            if (IsDrawingShape) {
                CurrentShapeToDraw.AddVertex(position);
            } else {
                CurrentShapeToDraw.Validate();
                CurrentShapeToDraw.SimulatingPhysics = true;
                CurrentShapeToDraw = null;
				vcam.gameObject.SetActive(false);
				playerVcam.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Updates the current shape's latest vertex position to allow
    /// a shape to be updated with the mouse cursor and redrawn
    /// </summary>
    private void UpdateShapeVertex(Vector2 position)
    {
        if (CurrentShapeToDraw == null) {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
            CurrentShapeToDraw.Remove();

        CurrentShapeToDraw.UpdateShape(position);
    }

    /// <summary>
    /// Controlled via Unity GUI button
    /// </summary>
    public void SetDrawMode(string mode)
    {
        Mode = (DrawMode) Enum.Parse(typeof(DrawMode), mode);
    }

    /// <summary>
    /// The types of shapes that can be drawn, useful for
    /// selecting shapes to draw
    /// </summary>
    public enum DrawMode
    {
        Rectangle,
        Circle
    }
}