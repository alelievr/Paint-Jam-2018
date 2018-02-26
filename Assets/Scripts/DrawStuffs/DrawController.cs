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
    CinemachineBrain brain = null;    
    private void Awake()
    {
        _drawModeToPrefab = new Dictionary<DrawMode, DrawShape> {
            {DrawMode.Rectangle, RectanglePrefab},
            {DrawMode.Circle, CirclePrefab}
        };
        if (pasbouger == null)
            pasbouger = new GameObject();
        brain = FindObjectOfType<CinemachineBrain>();
    }

    private void Update()
    {
        if (brain)
            Debug.Log(brain.ActiveVirtualCamera.Name);
        var mousePos = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var clickDown = Input.GetKeyDown(KeyCode.Mouse0) && CurrentShapeToDraw == null &&
                    !EventSystem.current.IsPointerOverGameObject();
        var clickUp = Input.GetKeyUp(KeyCode.Mouse0) && CurrentShapeToDraw != null &&
                    !EventSystem.current.IsPointerOverGameObject();
        var canUpdateShape = CurrentShapeToDraw != null && IsDrawingShape;

        if (GameManager.instance.gameState == GameManager.GameState.Pause)
            return ;

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

    GameObject last;
    GameObject tmpcamera = null;
    GameObject pasbouger = null;
    private void AddShapeVertex(Vector2 position)
    {
        if (CurrentShapeToDraw == null) {
            // No current shape -> instantiate a new shape and add two vertices:
            // one for the initial position, and the other for the current cursor
            if (brain)
            {
                // tmpcamera = new GameObject("VirtualCamera").AddComponent<CinemachineVirtualCamera>();

                last = brain.ActiveVirtualCamera.VirtualCameraGameObject;
                if (tmpcamera == null)
                {
                    tmpcamera = GameObject.Instantiate(last);
                    GameObject.Destroy(tmpcamera.GetComponent<CinemachineConfiner>());
                    tmpcamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = 0.5f;
                    tmpcamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = 0.5f;
                    tmpcamera.name = "tmpcamera";
                }
                pasbouger.transform.position = last.GetComponent<CinemachineVirtualCamera>().transform.position;
                //                 pasbouger.transform.position = last.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_AdjustmentMode =;
                tmpcamera.GetComponent<CinemachineVirtualCamera>().Follow = pasbouger.transform;
                last.SetActive(false);
                tmpcamera.SetActive(true);                
;                // last = brain.ActiveVirtualCamera.Follow;
                // brain.ActiveVirtualCamera.Follow = null;
            }
            var prefab = _drawModeToPrefab[Mode];
            CurrentShapeToDraw = Instantiate(prefab);
            CurrentShapeToDraw.name = "Shape " + _allShapes.Count;

            CurrentShapeToDraw.AddVertex(position);
            CurrentShapeToDraw.AddVertex(position);
            
            CurrentShapeToDraw.SimulatingPhysics = false;

            IsDrawingShape = true;

            _allShapes.Add(CurrentShapeToDraw);
        } else {
            if (brain)
            {
                last.SetActive(true);
                tmpcamera.SetActive(false);
                
            }
            // Current shape exists -> add vertex if finished, 
            // otherwise start physics simulation and reset reference
            IsDrawingShape = !CurrentShapeToDraw.ShapeFinished;

            if (IsDrawingShape) {
                CurrentShapeToDraw.AddVertex(position);
            } else {
                CurrentShapeToDraw.Validate();
                CurrentShapeToDraw.SimulatingPhysics = true;
                CurrentShapeToDraw = null;
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