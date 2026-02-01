using System;
using System.Linq;
using DefaultNamespace;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PowerNode : MonoBehaviour
{
    public enum NodeState { Floating, Dragging, Docked }

    public float springStrength = 15f;
    public float damping = 0.75f;
    

    public float floatAmplitude = 0.1f;
    public float floatSpeed = 1f;
    public float floatingDamping = 0.98f;
    
    public float edgeMargin = 0.5f;
    public float edgeForceStrength = 10f;

    public float separationDistance = 1f;
    public float separationForce = 5f;

    public string nodeName;
    public string description;
    public float repeatDelay;
    public float repeatMultiplier = 1f;

    public Camera mainCamera;
    public SpriteRenderer selectionSprite;
    public SpriteRenderer dockedSprite;
    public Transform sceneParent;
    public SpriteRenderer[] clippedRenderers;
    

    public RectTransform rectTransform;
    public NodeState currentState = NodeState.Floating;
    private Vector3 targetWorldPosition;
    private Vector3 velocity;
    private Vector3 anchorPosition;
    private Vector2 noiseOffset;
    private Vector3 floatingVelocity;
    private bool wasDocked;
    private MaterialPropertyBlock propertyBlock;
    private static readonly int ClipRectID = Shader.PropertyToID("_ClipRect");
    private Vector2 dragOffset;
    
    // deckboard simulation stuff

    public enum Rarity { Common, Rare, Legendary }
    public string NodeName;
    public string Description;
    public Rarity rarity = Rarity.Common; // 

    public GameObject PulseObject;
    public GameObject PulsedirectionIndicator;
    public Vector2[] PulseDirections = new Vector2[] {};

    public RectTransform[] pulseIndicators = new RectTransform[] {};
    
    // common base stats
    public float FireRate = 2.0f;  // for generators this translates to how often it generates a pulse
    public float NodePower = 1.0f;
    public float RepeatDelay = 0.1f;
    public float powerFactor = 0.1f;
    public float minPulsePower = 0.2f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        anchorPosition = transform.position;
        noiseOffset = new Vector2(Random.Range(0f, 100f), Random.Range(0f, 100f));
        propertyBlock = new MaterialPropertyBlock();
        AwakeInner();
    }

    public virtual void AwakeInner()
    {
        
    }

    public virtual void StartInner()
    {
        
    }

    private void Start()
    {
        if (sceneParent == null)
            sceneParent = transform.parent;
        
        UpdatePulseDirections();
        
        Deckbuilder.GetInstance()?.RegisterFloatingNode(this);
        StartInner();
    }
    
    public void UpdatePulseDirections()
    {
        foreach (var dir in PulseDirections)
        {
            GameObject go = Instantiate(PulsedirectionIndicator, Vector3.zero, Quaternion.identity);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(transform);
            rt.localPosition = new Vector3(0,0,-2);
            rt.localScale = Vector3.one;
            float deg = (float) ((180.0 / Math.PI) * Math.Atan2(dir.y, dir.x));
            rt.rotation = Quaternion.Euler(0f, 0f, deg);
            
            
            SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();
            ApplyClipRectSingle(Deckbuilder.GetInstance().gridBounds, sr);
        }
    }

    private void OnDestroy()
    {
        Deckbuilder.GetInstance()?.UnregisterFloatingNode(this);
    }

    private void Update()
    {
        UpdateInner();
    }

    public void UpdateInner()
    {
        if (Mouse.current == null) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();

        bool isHovered = IsPointerOverNode(screenPos);

        // lmao this is a really disgusting hack but unity scene order always seems to update deterministically
        if (isHovered)
        {
            Deckbuilder db = Deckbuilder.GetInstance();
            db.hoveredNode = this;
            db.hasHover = true;
        }
        
        // Submit self as drag candidate if clicked
        if (Mouse.current.leftButton.wasPressedThisFrame && currentState != NodeState.Dragging)
        {
            if (isHovered)
            {
                if (currentState == NodeState.Floating)
                {
                    Deckbuilder.GetInstance().SubmitDragCandidate(this);
                }
                else if (currentState == NodeState.Docked)
                {
                    if (Deckbuilder.GetInstance().IsMouseInGridBounds(screenPos))
                    {
                        Deckbuilder.GetInstance().SubmitDragCandidate(this);
                    }
                }
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (currentState == NodeState.Dragging)
            {
                HandleDragRelease(screenPos);
            }
        }

        if (currentState == NodeState.Dragging)
        {
            targetWorldPosition = ScreenToWorldOnNodePlane(screenPos - dragOffset);
            ApplySpringPhysics();
            selectionSprite.enabled = true;
            dockedSprite.enabled = false;
        }
        else if (currentState == NodeState.Floating)
        {
            selectionSprite.enabled = false;
            dockedSprite.enabled = false;
            HandleFloating();
        }
        else if (currentState == NodeState.Docked)
        {
            selectionSprite.enabled = false;
            dockedSprite.enabled = true;
        }

        TickNode();
    }

    private void HandleFloating()
    {
        Vector3 totalForce = Vector3.zero;

        // Perlin noise drift force
        float time = Time.time * floatSpeed;
        float noiseX = (Mathf.PerlinNoise(time, noiseOffset.x) - 0.5f) * 2f;
        float noiseY = (Mathf.PerlinNoise(noiseOffset.y, time) - 0.5f) * 2f;
        totalForce += new Vector3(noiseX, noiseY, 0f) * floatAmplitude;

        // Screen edge repulsion force
        totalForce += HandleEdgeForce();

        // Flocking separation force
        totalForce += HandleAntiFlockForce();

        floatingVelocity += totalForce * Time.deltaTime;
        
        floatingVelocity *= floatingDamping;

        transform.position += floatingVelocity * Time.deltaTime;
    }

    private Vector3 HandleEdgeForce()
    {
        Vector3 force = Vector3.zero;
        Vector3 worldPos = transform.position;
        
        // Does this work?
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, Mathf.Abs(mainCamera.transform.position.z - worldPos.z)));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, Mathf.Abs(mainCamera.transform.position.z - worldPos.z)));

        float minX = bottomLeft.x + edgeMargin;
        float maxX = topRight.x - edgeMargin;
        float minY = bottomLeft.y + edgeMargin;
        float maxY = topRight.y - edgeMargin;

        // Left
        if (worldPos.x < minX)
        {
            float penetration = minX - worldPos.x;
            force.x += penetration * edgeForceStrength;
        }
        // Right
        else if (worldPos.x > maxX)
        {
            float penetration = worldPos.x - maxX;
            force.x -= penetration * edgeForceStrength;
        }

        // Bottom
        if (worldPos.y < minY)
        {
            float penetration = minY - worldPos.y;
            force.y += penetration * edgeForceStrength;
        }
        // Top
        else if (worldPos.y > maxY)
        {
            float penetration = worldPos.y - maxY;
            force.y -= penetration * edgeForceStrength;
        }

        return force;
    }

    private Vector3 HandleAntiFlockForce()
    {
        Vector3 force = Vector3.zero;
        var nodes = Deckbuilder.GetInstance()?.GetFloatingNodes();
        if (nodes == null) return force;

        foreach (PowerNode other in nodes)
        {
            if (other == this) continue;
            
            Vector3 toSelf = transform.position - other.transform.position;
            float dist = toSelf.magnitude;
            
            if (dist < separationDistance)
            {
                Vector3 pushDir;
                if (dist < 0.01f)
                {
                    float angle = Random.Range(0f, Mathf.PI * 2f);
                    pushDir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                }
                else
                {
                    pushDir = toSelf.normalized;
                }
                
                float strength = (separationDistance - dist) / separationDistance;
                force += separationForce * strength * pushDir;
            }
        }

        return force;
    }

    public void StartDragging()
    {
        wasDocked = (currentState == NodeState.Docked);
        
        if (wasDocked)
        {
            UndockToScene();
        }
        
        currentState = NodeState.Dragging;
        velocity = Vector3.zero;
        transform.SetAsLastSibling();

        // We keep an offset of where we "mouse down" into the rect. This will make dragging stuff feel more natural.
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 uiScreenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, rectTransform.position);
        dragOffset = mouseScreenPos - uiScreenPos;
    }

    private void HandleDragRelease(Vector2 screenPos)
    {
        Deckbuilder db = Deckbuilder.GetInstance();
        db.StopDragging(this);
        dragOffset = Vector2.zero;
        
        Vector3 thisScreenPos = mainCamera.WorldToScreenPoint(transform.position);
        
        if (db.IsMouseInGridBounds(screenPos) && db.IsMouseInGridBounds(new Vector2 (thisScreenPos.x, thisScreenPos.y)))
        {
            DockToDeckBuilder();
        }
        else
        {
            currentState = NodeState.Floating;
            floatingVelocity = velocity;
        }
    }

    private void DockToDeckBuilder()
    {
        Deckbuilder db = Deckbuilder.GetInstance();
        
        db.UnregisterFloatingNode(this);
        db.RegisterDockedNode(this);
        transform.SetParent(db.graphRoot, true);
        currentState = NodeState.Docked;
        velocity = Vector3.zero;
        
        ApplyClipRect(db.gridBounds);
    }

    private void UndockToScene()
    {
        Deckbuilder db = Deckbuilder.GetInstance();
        db.RegisterFloatingNode(this);
        db.UnregisterDockedNode(this);
        transform.SetParent(sceneParent, true);
        
        ClearClipRect();
    }

    private void ApplyClipRectSingle(RectTransform clip, SpriteRenderer sr)
    {
        Vector3[] corners = new Vector3[4];
        clip.GetWorldCorners(corners);
        Vector4 clipRect = new Vector4(corners[0].x, corners[0].y, corners[2].x, corners[2].y);
        
        sr.GetPropertyBlock(propertyBlock);
        propertyBlock.SetVector(ClipRectID, clipRect);
        sr.SetPropertyBlock(propertyBlock);
    }

    private void ApplyClipRect(RectTransform clipSource)
    {
        if (clippedRenderers == null) return;
        
        Vector3[] corners = new Vector3[4];
        clipSource.GetWorldCorners(corners);
        Vector4 clipRect = new Vector4(corners[0].x, corners[0].y, corners[2].x, corners[2].y);
        
        foreach (var sr in clippedRenderers)
        {
            sr.GetPropertyBlock(propertyBlock);
            propertyBlock.SetVector(ClipRectID, clipRect);
            sr.SetPropertyBlock(propertyBlock);
        }
    }

    private void ClearClipRect()
    {
        if (clippedRenderers == null) return;
        
        Vector4 noClip = new Vector4(-10000, -10000, 10000, 10000);
        foreach (var sr in clippedRenderers)
        {
            sr.GetPropertyBlock(propertyBlock);
            propertyBlock.SetVector(ClipRectID, noClip);
            sr.SetPropertyBlock(propertyBlock);
        }
    }

    public bool IsPointerOverNode(Vector2 screenPos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPos, mainCamera);
    }

    private Vector3 ScreenToWorldOnNodePlane(Vector2 screenPos)
    {
        float distanceToCamera = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distanceToCamera));
        worldPos.z = transform.position.z;
        return worldPos;
    }

    private void ApplySpringPhysics()
    {
        Vector3 displacement = targetWorldPosition - transform.position;
        Vector3 springForce = displacement * springStrength;
        velocity += springForce * Time.deltaTime;
        velocity *= damping;
        transform.position += velocity * Time.deltaTime;
    }

    public void GeneratePulse(Vector2 direction, float pulsePower)
    {
        Deckbuilder builder = Deckbuilder.GetInstance();
        builder.SpawnPulse(this, direction, pulsePower);
    }

    public virtual void OnPulse(float pulseStrength)
    {
        if (pulseStrength < minPulsePower)
        {
            return;
        }
            
        foreach (var dir in PulseDirections)
        {
            GeneratePulse(dir, pulseStrength * powerFactor);
        }
    }

    public virtual void TickNode()
    {
        
    }
}
