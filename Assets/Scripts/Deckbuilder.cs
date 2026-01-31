using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Deckbuilder : MonoBehaviour
{
    private static Deckbuilder gDeckBuilder;
    
    // to use me 
    // do Deckbuilder.GetInstance().GraphUpdates += this.graphUpdate
    public delegate void GraphUpdate(Deckbuilder deckbuilder);

    public GraphUpdate GraphUpdates;

    public Vector2 _graphRootPosition = Vector2.zero;
    public Vector2 _graphOffsetPosition;
    public Vector2 _graphOffsetPositionBase;
    public Vector2 _mouseDownPosition;
    public RectTransform graphRoot;
    public RectTransform gridBounds;
    public Camera canvasCamera;

    private List<PowerNode> floatingNodes = new List<PowerNode>();
    private List<PowerNode> dragCandidates = new List<PowerNode>();
    private PowerNode currentlyDragging;

    public static Deckbuilder GetInstance()
    {
        return gDeckBuilder;
    }

    public void RegisterFloatingNode(PowerNode node)
    {
        if (!floatingNodes.Contains(node))
            floatingNodes.Add(node);
    }

    public void UnregisterFloatingNode(PowerNode node)
    {
        floatingNodes.Remove(node);
    }

    public List<PowerNode> GetFloatingNodes()
    {
        return floatingNodes;
    }

    public void SubmitDragCandidate(PowerNode node)
    {
        if (currentlyDragging == null && !dragCandidates.Contains(node))
            dragCandidates.Add(node);
    }

    public void StopDragging(PowerNode node)
    {
        if (currentlyDragging == node)
            currentlyDragging = null;
    }

    void Awake()
    {
        gDeckBuilder = this;
        _graphRootPosition = graphRoot.position;
        
        // mlg pro tip for prototyping 2d games, makes physics and custom visual effects
        // mega consistent.
        Application.targetFrameRate = 60; 
    }

    // really a misnomer but i dont wanna do a rename at this point
    public bool IsMouseInGridBounds(Vector2 screenPos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(gridBounds, screenPos, canvasCamera);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is create
    void Start()
    {
        
    }

    public bool MouseDown = false;
    
    void HandleMouseDown(Vector2 mouse2d)
    {
        if (!MouseDown)
            return;

        if (!IsMouseInGridBounds(mouse2d))
        {
            MouseDown = false;
            _graphOffsetPositionBase = _graphOffsetPosition;
            return;
        }

        Vector2 offset = mouse2d - _mouseDownPosition;
        _graphOffsetPosition = _graphOffsetPositionBase + offset;
        ClampOffset();
    }

    void ClampOffset()
    {
        Vector2 limit = gridBounds.rect.size * 0.75f; // half of 1.5x
        _graphOffsetPosition.x = Mathf.Clamp(_graphOffsetPosition.x, -limit.x, limit.x);
        _graphOffsetPosition.y = Mathf.Clamp(_graphOffsetPosition.y, -limit.y, limit.y);
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouse2d = Mouse.current.position.ReadValue();

        if (Mouse.current.middleButton.wasPressedThisFrame && IsMouseInGridBounds(mouse2d))
        {
            MouseDown = true;
            _mouseDownPosition = mouse2d;
        }

        if (Mouse.current.middleButton.wasReleasedThisFrame)
        {
            MouseDown = false;
            _graphOffsetPositionBase = _graphOffsetPosition;
        }

        HandleMouseDown(mouse2d);

        // Select first drag candidate if any
        if (dragCandidates.Count > 0 && currentlyDragging == null)
        {
            currentlyDragging = dragCandidates[0];
            currentlyDragging.StartDragging();
        }
        dragCandidates.Clear();

        graphRoot.position = _graphRootPosition + _graphOffsetPosition;
    }
}
