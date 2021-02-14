using System.Collections;
using UnityEngine;
using static AbstractEnemy;

public class EnemyLevel
{
    private EnemyColor color;
    private EnemyColor startColor;
    private int point;

    public EnemyColor Color
    {
        get { return color; }
        set { color = value; }
    }

    public EnemyColor GetStartColor()
    {
        return startColor;
    }

    public int Point
    {
        get
        {
            if (EnemyColor.GRAY.Equals(startColor)) return 100;
            if (EnemyColor.YELLOW.Equals(startColor)) return 200;
            if (EnemyColor.GREEN.Equals(startColor)) return 300;
            if (EnemyColor.RED.Equals(startColor)) return 400;
            return 0;
        }
    }

    public EnemyLevel(EnemyColor color)
    {
        this.Color = color;
        this.startColor = color;
    }

    public static int GetScoreByColor(EnemyColor enemyColor)
    {
        switch (enemyColor)
        {
            case EnemyColor.GRAY:
                return 100;
            case EnemyColor.YELLOW:
                return 200;
            case EnemyColor.GREEN:
                return 300;
            case EnemyColor.RED:
                return 400;
            default:
                return 0;
        }
    }
}