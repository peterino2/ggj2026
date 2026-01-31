using UnityEngine;
using UnityEngine.InputSystem;

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

    public Deckbuilder GetInstance()
    {
        return gDeckBuilder;
    }

    void Awake()
    {
        gDeckBuilder = this;
        _graphRootPosition = graphRoot.position;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is create
    void Start()
    {
        
    }

    public bool MouseDown = false;

    #if FALSE
    void HandleMouseDown(Vector2 mouse2d)
    {
        if (!MouseDown)
        {
            return;
        }

        Vector2 offset = mouse2d - _mouseDownPosition;
        _graphOffsetPosition = offset + _graphOffsetPositionBase;
        
        if (Input.GetMouseButtonUp(0))
        {
            _graphOffsetPositionBase = offset;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouse2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        
        if (Input.GetMouseButtonDown(0))
        {
            MouseDown = true;
            _mouseDownPosition = mouse2d;
        }

        _graphOffsetPosition = _graphOffsetPositionBase;
        HandleMouseDown(mouse2d);
        
        graphRoot.position = _graphRootPosition;
    }
    #endif
    
    void HandleMouseDown(Vector2 mouse2d)
    {
        if (!MouseDown)
            return;

        Vector2 offset = mouse2d - _mouseDownPosition;
        _graphOffsetPosition = _graphOffsetPositionBase + offset;
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouse2d = Mouse.current.position.ReadValue();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            MouseDown = true;
            _mouseDownPosition = mouse2d;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            MouseDown = false;
            _graphOffsetPositionBase = _graphOffsetPosition;
        }

        HandleMouseDown(mouse2d);

        graphRoot.position = _graphRootPosition + _graphOffsetPosition;
    }

}
