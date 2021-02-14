using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public Sprite baseDemolishedSprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Bullet") || collision.tag.Equals("EnemyBullet"))
        {
            GetComponent<SpriteRenderer>().sprite = baseDemolishedSprite;
            GameManager.instance.GameOver();
        }
    }
}
