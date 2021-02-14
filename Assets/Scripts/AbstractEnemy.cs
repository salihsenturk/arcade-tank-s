using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractEnemy : MonoBehaviour
{
    public int speed = 3;
    protected EnemySize size;
    protected EnemyLevel level;
    protected bool isSpecial = false;

    public enum EnemySize
    {
        SIZE_0, SIZE_1, SIZE_2, SIZE_3, SIZE_4, SIZE_5
    };

    public enum EnemyColor
    {
        GRAY, YELLOW, GREEN, RED
    };

    public EnemyLevel GetEnemyLevel()
    {
        return level;
    }

    public void SetSize(EnemySize size)
    {
        this.size = size;

        if ((int) size < 2) speed = 1;
        if ((int) size < 4) speed = 2;
        if ((int) size < 5) speed = 3;
    }

    public void SetColor(EnemyColor color)
    {
        if(this.level == null)
        {
            this.level = new EnemyLevel(color);
        } else if(color > EnemyColor.RED)
        {
            color = EnemyColor.RED;
        } else
        {
            this.level.Color = color;
        }
        isSpecial = level.Color.Equals(EnemyColor.RED);
    }

    public void SetRandomEnemy(GameObject newEnemy, int maxLevel)
    {
        int randomValue = Random.Range(0, maxLevel);
        EnemyColor color = (EnemyColor) randomValue;
        string colorStr = color.ToString() + "_";
        if ((int) color == 0)
        {
            colorStr = "";
        }

        EnemySize size = (EnemySize)Random.Range(0, 6);
        string enemySize = size.ToString();
        string spriteName = colorStr + enemySize;

        newEnemy.GetComponent<SpriteRenderer>().sprite = findEnemySpriteByName(spriteName.ToLower());
        newEnemy.GetComponent<EnemyController>().SetColor(color);
        newEnemy.GetComponent<EnemyController>().SetSize(size);
    }

    protected void UpdateSprite()
    {
        string newLevelStr = level.Color.ToString() + "_";
        if ((int) level.Color == 0)
        {
            newLevelStr = "";
        }
        string spriteName = newLevelStr + size.ToString();
        GetComponent<SpriteRenderer>().sprite = findEnemySpriteByName(spriteName.ToLower());
    }

    private Sprite findEnemySpriteByName(string name)
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

    public int GetEnemyPoint()
    {
        return level.Point;
    }
}
