using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyController : AbstractEnemy
{
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;
    
    private Transform castPoint;
    private Vector3 currentDirection = Vector3.down;
    private float remainingTimeForDirectionChange = 3f;
    private float remainingTimeForFire = 1f;
    private const float DIRECTION_CHANGE_TIME = 3f;
    private List<Vector3> availableDirections = new List<Vector3> { Vector2.up, Vector3.down, Vector3.right, Vector3.left };
    private Dictionary<Vector3, float> directionsMap = new Dictionary<Vector3, float>() { { Vector3.up, 0f }, { Vector3.down, 180f }, { Vector3.right, -90f }, { Vector3.left, 90f } };
    private Rigidbody2D rb;
    private Transform tipTransform;
    private GameObject currentBullet;
    private bool isFreezed = false;
    private bool shieldActive = false;
    private bool canShotMultiple = false;

    // Start is called before the first frame update
    void Start()
    {
        //castPoint = transform.Find("CastPoint");
        rb = GetComponent<Rigidbody2D>();
        tipTransform = gameObject.transform.Find("Tip");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFreezed)
        {
            MoveGameObject();

            remainingTimeForDirectionChange -= Time.deltaTime;
            if(remainingTimeForDirectionChange <= 0)
            {
                ChangeDirection();
                remainingTimeForDirectionChange = DIRECTION_CHANGE_TIME;
            }

            FireBullet();
        }
    }

    private void FixedUpdate()
    {
        /*
        Vector2 endPos = castPoint.position + currentDirection * 2f;
        RaycastHit2D hit = Physics2D.Linecast(castPoint.position, endPos);

        if (hit.collider != null)
        {
            Debug.DrawRay(castPoint.position, currentDirection * 2f, Color.red);
        }
        else
        {
            Debug.DrawRay(castPoint.position, currentDirection * 2f, Color.blue);
        }
        if (hit && hit.collider && hit.collider.name != "Enemy")
        {
            //Debug.Log(hit.collider.name);
        }
        */
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy")
        {
            ChangeDirection();
            remainingTimeForDirectionChange = DIRECTION_CHANGE_TIME;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!shieldActive && collision.gameObject.tag == "Bullet")
        {
            HandleBulletCollision();
        }
    }

    private void HandleBulletCollision()
    {
        if((int) level.Color == 0)
        {
            DestroySelf();
            return;
        }
        if (isSpecial)
        {
            GameManager.instance.CreatePowerUp();
        }
        SetColor(level.Color - 1);
        UpdateSprite();
    }

    public void IncreaseEnemyLevel()
    {
        SetColor(level.Color + 1);
        UpdateSprite();
    }

    public void DestroySelf()
    {
        // Destroy
        Vector3 explosionPosition = new Vector3(transform.position.x, transform.position.y, 1f);
        GameObject expolsion = Instantiate(explosionPrefab, explosionPosition, transform.rotation) as GameObject;
        expolsion.GetComponent<ParticleSystem>().Play();
        
        GameManager.instance.EnemyDestroyed(gameObject);

        Destroy(gameObject);
        Destroy(expolsion, .5f);
    }

    private void ChangeDirection()
    {
        Vector3 newDirection = CalculateNewDirection();
        
        currentDirection = newDirection;
        RotateGameObject();
    }

    private Vector3 CalculateNewDirection()
    {
        List<Vector3> otherDirections = new List<Vector3>(availableDirections);
        otherDirections.Remove(currentDirection);
        int index = Random.Range(0, otherDirections.Count);
        return otherDirections[index];
        
    }

    private void RotateGameObject()
    {
        rb.rotation = directionsMap[currentDirection];
    }

    private void MoveGameObject()
    {
        rb.MovePosition(rb.position + new Vector2(currentDirection.x, currentDirection.y) * speed * Time.fixedDeltaTime);
    }

    private void FireBullet()
    {
        remainingTimeForFire -= Time.deltaTime;
        if (remainingTimeForFire <= 0)
        {
            if (canShotMultiple || currentBullet == null)
            {
                currentBullet = Instantiate(bulletPrefab, tipTransform.position, tipTransform.rotation) as GameObject;
                currentBullet.GetComponent<EnemyBulletController>().AddForce(currentDirection);
                remainingTimeForFire = 2f;
            }
        }
    }

    public void FreezeGameObject(float seconds)
    {
        StartCoroutine(FreezeForSeconds(seconds));
    }

    IEnumerator FreezeForSeconds(float seconds)
    {
        isFreezed = true;

        yield return new WaitForSeconds(seconds);

        isFreezed = false;
    }

    public void EnableMultipleShots(bool enabled)
    {
        canShotMultiple = enabled;
    }

    public void EnableShield(bool enabled)
    {
        gameObject.transform.Find("EnemyShield").gameObject.SetActive(enabled);
        shieldActive = enabled;
    }
}
