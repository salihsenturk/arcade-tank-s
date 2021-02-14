using System;
using UnityEngine;

public class LevelManager
{
    public void LoadLevel(int levelNumber)
    {
        ClearExistingObstacles();

        LevelInfo levelInfo = JsonUtility.FromJson<LevelInfo>(Resources.Load("level" + levelNumber).ToString());
        GameManager.instance.EnemyCount = levelInfo.enemyCount;
        if(levelInfo.maxEnemyLevel >= 0)
        {
            GameManager.instance.MaxEnemyLevel = levelInfo.maxEnemyLevel;
        }
        
        if(levelInfo.walls != null)
        {
            foreach(Wall wall in levelInfo.walls)
            {
                if(wall.positions != null)
                {
                    foreach(Position position in wall.positions)
                    {
                        WallType type = (WallType) Enum.Parse(typeof(WallType), wall.type);
                        GameManager.instance.RenderWall(type, CalculatePosition(type, position));
                    }
                }
            }
        }
    }

    private void ClearExistingObstacles()
    {
        GameObject[] currentObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        if(currentObstacles != null)
        {
            foreach(GameObject obstacle in currentObstacles)
            {
                GameManager.instance.DestroyObject(obstacle);
            }
        }
    }

    private Vector2 CalculatePosition(WallType wallType, Position position)
    {
        float x = (float) (Constants.WALL_LEFT_X + position.col * .8);
        float y = (float) (Constants.WALL_TOP_Y - position.row * .8);
        if (!WallType.BRICK.Equals(wallType) && !WallType.STEEL.Equals(wallType))
        {
            x += .3f;
            y += .3f;
        }

        return new Vector2(x, y);
    }
}