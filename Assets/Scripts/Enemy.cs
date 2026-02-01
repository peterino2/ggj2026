using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D body;
    [Header("Ship Parameters")]
    [SerializeField] private float velocity = 5f;
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float curHP = 100f;
    [SerializeField] private float damage = 100f;
    [SerializeField] private float circleRadius = 5;
    [SerializeField] private float waveAmp = 2;
    [SerializeField] private float arcAmp = 0.1f;
    [SerializeField] private movementType movementOption = movementType.Straight;
    private float internalTimer = 0f;
    private Rigidbody2D.SlideMovement SlideMovement = new Rigidbody2D.SlideMovement();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public enum movementType
        {
        Straight,
        Wave,
        CircularCW,
        CircularCCW,
        UpwardArc,
        DownwardArc
    }
    void Start()
    {
        body = gameObject.GetComponent<Rigidbody2D>();
        curHP +=  Gamemode.Instance.GetDifficulty() * 20;

    }

    void Update() 
    {
        internalTimer += Time.deltaTime;
        if (movementOption == movementType.Straight)
        {
            movementStraight();
        }
        else if (movementOption == movementType.Wave)
        {
            movementWave();
        }
        else if (movementOption == movementType.CircularCW)
        {
            movementCircularCW();
        }
        else if (movementOption == movementType.CircularCCW)
        {
            movementCircularCCW();
        }
        else if (movementOption == movementType.UpwardArc)
        {
            movementUpwardArc();
        }
        else if (movementOption == movementType.DownwardArc) {
            movementDownwardArc();
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, -5.0f);
        
        if (isOutOfBounds())
        {
            Destroy(gameObject);
        }
    }

    void movementStraight()
    {
        transform.Translate(new Vector2(-1, 0) * velocity * Time.deltaTime);
    }

    void movementWave()
    {
        Vector3 curPosition = transform.position;
        Vector3 newPosition = new Vector3(transform.position.x + Vector2.left.x * velocity * Time.deltaTime, Mathf.Sin(internalTimer + Time.deltaTime) * waveAmp, transform.position.z);
        Vector3 newDirection = newPosition - curPosition;
        transform.Translate(newDirection * velocity * Time.deltaTime);
    }
    //rotates clockwise in a circle starting from 3
    void movementCircularCW()
    {
        transform.Translate(new Vector2(Mathf.Sin(Time.fixedTime), Mathf.Cos(Time.fixedTime)) * -1 * circleRadius * Time.deltaTime);
    }

    //rotates counter clockwise from 3
    void movementCircularCCW()
    {
        transform.Translate(new Vector2(Mathf.Sin(Time.fixedTime) * -1, Mathf.Cos(Time.fixedTime)) * circleRadius * Time.deltaTime);
        transform.Translate(Vector2.left * velocity * Time.deltaTime);
    }

    void movementUpwardArc()
    {
        Vector3 curPosition = transform.position;
        Vector3 newPosition = new Vector3(transform.position.x + Vector2.left.x * velocity * Time.deltaTime * 3, arcAmp*Mathf.Pow(internalTimer + Time.deltaTime - 2, 3), transform.position.z);
        Vector3 newDirection = newPosition - curPosition;
        transform.Translate(newDirection * velocity * Time.deltaTime);

    }

    void movementDownwardArc()
    {
        Vector3 curPosition = transform.position;
        Vector3 newPosition = new Vector3(transform.position.x + Vector2.left.x * velocity * Time.deltaTime * 3, arcAmp * -Mathf.Pow(internalTimer + Time.deltaTime - 2, 3), transform.position.z);
        Vector3 newDirection = newPosition - curPosition;
        transform.Translate(newDirection * velocity * Time.deltaTime);

    }

    bool isOutOfBounds() {
        return (transform.position.x > 4000 || transform.position.x < -4000 || transform.position.y > 3000 || transform.position.y < -3000);
     
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            //takeDamage
            curHP -= 10;
            AudioPlayer.Instance.Play(SoundType.Hit);
            if (curHP <= 0)
            { 
                XPBarSystem.GetInstance().AddXP(3);
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("Player"))
        {
            //SpriteController.takeDamage(20);
            Destroy(gameObject);
        }
    }
}

