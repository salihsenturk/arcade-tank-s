using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Wall" || collider.gameObject.tag == "Brick" || collider.gameObject.tag == "Bullet" || collider.gameObject.tag == "EnemyBullet")
        {
            Destroy(gameObject);
        }
    }

    public void AddForce(Vector2 direction)
    {
        GetComponent<Rigidbody2D>().AddForce(direction * 500);
    }


}
