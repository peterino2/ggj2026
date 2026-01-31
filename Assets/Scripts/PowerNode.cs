using UnityEngine;
using UnityEngine.InputSystem;

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

    public Camera mainCamera;
    public SpriteRenderer selectionSprite;
    

    private RectTransform rectTransform;
    private NodeState currentState = NodeState.Floating;
    private Vector3 targetWorldPosition;
    private Vector3 velocity;
    private Vector3 anchorPosition;
    private Vector2 noiseOffset;
    private Vector3 floatingVelocity;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        anchorPosition = transform.position;
        noiseOffset = new Vector2(Random.Range(0f, 100f), Random.Range(0f, 100f));
    }

    private void Start()
    {
        Deckbuilder.GetInstance()?.RegisterFloatingNode(this);
    }

    private void OnDestroy()
    {
        Deckbuilder.GetInstance()?.UnregisterFloatingNode(this);
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        
        // Submit self as drag candidate if clicked
        if (Mouse.current.leftButton.wasPressedThisFrame && currentState != NodeState.Dragging)
        {
            if (IsPointerOverNode(screenPos))
            {
                Deckbuilder.GetInstance()?.SubmitDragCandidate(this);
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (currentState == NodeState.Dragging)
            {
                currentState = NodeState.Floating;
                floatingVelocity = velocity;
                Deckbuilder.GetInstance()?.StopDragging(this);
            }
        }

        if (currentState == NodeState.Dragging)
        {
            selectionSprite.enabled = true;
            targetWorldPosition = ScreenToWorldOnNodePlane(screenPos);
            ApplySpringPhysics();
        }
        else if(currentState == NodeState.Floating)
        {
            selectionSprite.enabled = false;
            HandleFloating();
        }
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
        currentState = NodeState.Dragging;
        velocity = Vector3.zero;
        transform.SetAsLastSibling();
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
}
