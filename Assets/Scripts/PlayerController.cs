using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject bulletPrefab;
    private int speed = 3;

    private Vector2 playerDirection = Vector2.up;
    private Rigidbody2D myRigidbody;
    private Transform tipTransform;
    private bool isMoving = false;
    private Animator[] animators;
    private GameObject currentBullet;
    private bool canShotMultiple = false;
    private bool shieldActive = false;
    private bool isFreezed = false;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        tipTransform = gameObject.transform.Find("Tip");
        animators = GetComponentsInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFreezed)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (myRigidbody.rotation != 0f)
                {
                    myRigidbody.rotation = 0f;
                    playerDirection = Vector2.up;
                }
                MovePlayer();
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                if (myRigidbody.rotation != 180f)
                {
                    myRigidbody.rotation = 180f;
                    playerDirection = Vector2.down;
                }
                MovePlayer();
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (myRigidbody.rotation != 90f)
                {
                    myRigidbody.rotation = 90f;
                    playerDirection = Vector2.left;
                }
                MovePlayer();
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                if (myRigidbody.rotation != -90f)
                {
                    myRigidbody.rotation = -90f;
                    playerDirection = Vector2.right;
                }
                MovePlayer();
            } else
            {
                isMoving = false;
                UpdateAnimators();
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                ShotBullet();
            }
        }
    }

    private void ShotBullet()
    {
        if (canShotMultiple)
        {
            Vector3 bulletPosition = new Vector3(tipTransform.position.x, tipTransform.position.y, 1f);
            GameObject newBullet = Instantiate(bulletPrefab, bulletPosition, tipTransform.rotation);
            newBullet.GetComponent<BulletController>().AddForce(playerDirection);
        }
        else if (currentBullet == null)
        {
            Vector3 bulletPosition = new Vector3(tipTransform.position.x, tipTransform.position.y, 1f);
            currentBullet = Instantiate(bulletPrefab, bulletPosition, tipTransform.rotation);
            currentBullet.GetComponent<BulletController>().AddForce(playerDirection);
        }
    }

    private void UpdateAnimators()
    {
        animators[0].SetBool("IsMoving", isMoving);
        animators[1].SetBool("IsMoving", isMoving);
    }

    private void MovePlayer()
    {
        myRigidbody.MovePosition(myRigidbody.position + playerDirection * speed * Time.fixedDeltaTime);
        if(!isMoving)
        {
            isMoving = true;
            UpdateAnimators();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!shieldActive && other.gameObject.tag == "EnemyBullet")
        {
            PlayerHit();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!shieldActive && collision.gameObject.tag == "Enemy")
        {
            PlayerHit();
        }
    }

    public void PlayerHit()
    {
        playerDirection = Vector2.up;
        transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w);
        MovePlayer();
        GameManager.instance.PlayerHit();
    }

    public void RespawnPlayer()
    {
        playerDirection = Vector2.up;
    }

    public void EnableMultipleShots(bool enabled)
    {
        canShotMultiple = enabled;
    }

    public void EnableShield(bool enabled)
    {
        gameObject.transform.Find("Shield").gameObject.SetActive(enabled);
        shieldActive = enabled;
    }

    public void FreezePlayer(float seconds)
    {
        StartCoroutine(FreezeForSeconds(seconds));
    }

    IEnumerator FreezeForSeconds(float seconds)
    {
        isFreezed = true;
        isMoving = false;

        yield return new WaitForSeconds(seconds);

        isFreezed = false;
    }
}
