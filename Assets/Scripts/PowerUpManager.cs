using UnityEngine;
public class PowerUpManager : MonoBehaviour
{
    public static void DestroyAllEnemies()
    {
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyList != null)
        {
            foreach (GameObject enemy in enemyList)
            {
                enemy.GetComponent<EnemyController>().DestroySelf();
            }
        }
    }

    public static void FreezeAllEnemies()
    {
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyList != null)
        {
            foreach (GameObject enemy in enemyList)
            {
                enemy.GetComponent<EnemyController>().FreezeGameObject(Constants.freezeTime);
            }
        }
    }

    public static GameObject CreatePowerUp(GameObject currentPowerUp, GameObject powerUpPrefab)
    {
        if (currentPowerUp != null) Destroy(currentPowerUp);
        
        float powerUpX = Random.Range(Constants.powerUpMinX, Constants.powerUpMaxX);
        float powerUpY = Random.Range(Constants.powerUpMinY, Constants.powerUpMaxY);

        currentPowerUp = Instantiate(powerUpPrefab) as GameObject;
        currentPowerUp.transform.position = new Vector3(powerUpX, powerUpY, currentPowerUp.transform.position.z);
        currentPowerUp.GetComponent<PowerUpController>().Type = (PowerUp) Random.Range(0, 6);

        return currentPowerUp;
    }

    internal static void UpdateEnemyLevels()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyController>().IncreaseEnemyLevel();
        }
    }
}