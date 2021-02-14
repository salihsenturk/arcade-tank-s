using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    private int timer = 0;
    private bool alphaUp = false;

    private PowerUp type;

    public PowerUp Type {
        get { return type; }
        set {
            this.type = value;
            gameObject.GetComponent<SpriteRenderer>().sprite = findPowerUpSpriteByName("pu_" + this.type.ToString().ToLower());
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += 1;
        if(timer >= 50)
        {
            UpdateSpriteAlpha();
            timer = 0;
        }
    }

    private void UpdateSpriteAlpha()
    {
        Color spriteColor = gameObject.GetComponent<SpriteRenderer>().color;
        float newAlpha = spriteColor.a;
        if(alphaUp && newAlpha >= 1)
        {
            alphaUp = false;
        } else if(!alphaUp && newAlpha <= .3)
        {
            alphaUp = true;
        }

        if (alphaUp)
        {
            newAlpha += .05f;
        } else
        {
            newAlpha -= .05f;
        }
        Color newSpriteColor = new Color(spriteColor.r, spriteColor.g, spriteColor.b, newAlpha);
        gameObject.GetComponent<SpriteRenderer>().color = newSpriteColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Player")
        {
            GameManager.instance.PowerUpTaken(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private Sprite findPowerUpSpriteByName(string name)
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>("battle_city_sprites");
        if (allSprites != null)
        {
            foreach (Sprite sprite in allSprites)
            {
                if (sprite.name == name)
                {
                    return sprite;
                }
            }
        }

        return null;
    }
}
