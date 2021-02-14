using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;
    public GameObject enemyPrefab;
    public Text remainingEnemyCountText;
    public Text remainingLivesCountText;
    public Text scoreText;
    public GameObject enemyImage;
    public GameObject explosionPrefab;
    public GameObject castle;
    public GameObject powerUpPrefab;
    public GameObject protector;
    public GameObject marbleProtector;
    public Text powerUpInfoText;
    public GameObject powerUpImage;
    public GameObject[] enemyBaseList;
    public GameObject brickPrefab;
    public GameObject steelPrefab;
    public GameObject grassPrefab;
    public GameObject icePrefab;
    public GameObject waterPrefab;
    public GameObject infoCamera;

    private GameObject currentPowerUp;
    private int score = 0;
    private int remainingEnemyCount = 1;
    private int totalRemainingEnemyCount = 1;
    private int remainingLives = 3;
    private int maxEnemyLevel = 4;

    private LevelManager levelManager;
    private static int currentLevel = 1;
    private static int maxLevel = 3;
    private Dictionary<AbstractEnemy.EnemyColor, int> destroyedEnemyCount;

    private float timer = -1f;
    private int PowerUpTimer
    {
        get { return (int) timer; }
        set { timer = value; }
    }

    private int Score
    {
        get { return score;}
        set
        {
            score = value;
            UpdateScoreText();
        }
    }

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        LoadLevel();
    }

    public void LoadNextLevel()
    {
        infoCamera.SetActive(false);
        gameObject.SetActive(true);
        LoadLevel();
    }

    private void LoadLevel()
    {
        if(levelManager == null)
        {
            levelManager = new LevelManager();
        }

        destroyedEnemyCount = new Dictionary<AbstractEnemy.EnemyColor, int>();
        levelManager.LoadLevel(currentLevel);
        UpdateEnemyCountText();
        SpawnFirstThreeEnemies();
        StartCoroutine(RespawnPlayer(0f));
        Time.timeScale = 1;
    }

    public void DestroyObject(GameObject objectToDestroy)
    {
        Destroy(objectToDestroy);
    }

    public int EnemyCount
    {
        get { return remainingEnemyCount; }
        set {
            remainingEnemyCount = value - 3;
            totalRemainingEnemyCount = value;
        }
    }

    public int MaxEnemyLevel
    {
        get { return maxEnemyLevel; }
        set { maxEnemyLevel = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateEnemyCountText();
        UpdateRemainingLivesText();

        Score = 0;
    }

    private void SpawnFirstThreeEnemies()
    {
        CreateNewEnemy(enemyBaseList[0]);
        CreateNewEnemy(enemyBaseList[1]);
        CreateNewEnemy(enemyBaseList[2]);
    }

    public void RenderWall(WallType wallType, Vector2 position)
    {
        GameObject newObject = null;
        switch (wallType)
        {
            case WallType.BRICK:
                newObject = brickPrefab;
                break;
            case WallType.STEEL:
                newObject = steelPrefab;
                break;
            case WallType.GRASS:
                newObject = grassPrefab;
                break;
            case WallType.ICE:
                newObject = icePrefab;
                break;
            case WallType.WATER:
                newObject = waterPrefab;
                break;
        }
        newObject.transform.position = position;
        Instantiate(newObject, newObject.transform.position, newObject.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if(PowerUpTimer > -1)
        {
            timer -= Time.deltaTime;
            powerUpInfoText.text = PowerUpTimer.ToString();
        }
        else
        {
            PowerUpTimer = -1;
        }

        if (PowerUpTimer < 0)
        {
            powerUpInfoText.gameObject.SetActive(false);
            powerUpImage.SetActive(false);
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        currentLevel = 1;
        gameObject.SetActive(false);
        infoCamera.GetComponent<InfoSceneManager>().GameOver();
    }

    private void EndLevel()
    {
        if(currentLevel == maxLevel)
        {
            GameWon();
        } else
        {
            gameObject.SetActive(false);
            infoCamera.GetComponent<InfoSceneManager>().ShowInfo(destroyedEnemyCount, currentLevel, score);
            currentLevel++;
        }
    }

    public void GameWon()
    {
        currentLevel = 1;
        infoCamera.GetComponent<InfoSceneManager>().GameCompleted(score);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void EnemyDestroyed(GameObject enemy)
    {
        totalRemainingEnemyCount--;

        Score += enemy.GetComponent<EnemyController>().GetEnemyPoint();

        EnemyLevel enemyLevel = enemy.GetComponent<EnemyController>().GetEnemyLevel();
        
        if (!destroyedEnemyCount.ContainsKey(enemyLevel.GetStartColor()))
        {
            destroyedEnemyCount.Add(enemyLevel.GetStartColor(), 0);
        }
        destroyedEnemyCount[enemyLevel.GetStartColor()] = destroyedEnemyCount[enemyLevel.GetStartColor()] + 1;

        if (remainingEnemyCount > 0)
        {
            StartCoroutine(WaitSeconds(1f));
            remainingEnemyCount--;
            UpdateEnemyCountText();
        }

        StartCoroutine(CheckForGameWon(1f));
    }

    IEnumerator CheckForGameWon(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (totalRemainingEnemyCount == 0)
        {
            EndLevel();
        }
    }

    IEnumerator WaitSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        CreateNewEnemy(null);
    }

    public void PlayerHit()
    {
        player.gameObject.SetActive(false);
        remainingLives--;
        UpdateRemainingLivesText();
        GameObject expolsion = Instantiate(explosionPrefab, player.transform.position, player.transform.rotation) as GameObject;
        expolsion.GetComponent<ParticleSystem>().Play();
        Destroy(expolsion, .5f);

        if (remainingLives <= 0)
        {
            Destroy(player);
            GameOver();
            return;
        }
        StartCoroutine(RespawnPlayer(1f));
    }

    IEnumerator RespawnPlayer(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        player.transform.position = Constants.playerStartPosition;
        player.gameObject.SetActive(true);
        player.GetComponent<BoxCollider2D>().enabled = false;
        player.GetComponent<PlayerController>().RespawnPlayer();

        StartCoroutine(WaitAfterRespawn(2f));
    }

    IEnumerator WaitAfterRespawn(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        player.GetComponent<BoxCollider2D>().enabled = true;
    }

    private void UpdateEnemyCountText()
    {
        remainingEnemyCountText.text = "x " + remainingEnemyCount;
    }
    private void UpdateRemainingLivesText()
    {
        remainingLivesCountText.text = "x " + remainingLives;
    }

    private void CreateNewEnemy(GameObject baseObject)
    {
        StartCoroutine(StartEnemySpawnAnimation(1f, baseObject));
    }

    IEnumerator StartEnemySpawnAnimation(float delaySeconds, GameObject baseObject)
    {
        GameObject startBase = enemyBaseList[Random.Range(0, 3)];
        startBase.GetComponent<Animator>().SetBool("PlayAnimation", true);
        
        yield return new WaitForSeconds(delaySeconds);

        startBase.GetComponent<Animator>().SetBool("PlayAnimation", false);
        StartCoroutine(SpawnEnemy(delaySeconds - .5f, baseObject, startBase));
    }

    IEnumerator SpawnEnemy(float delaySeconds, GameObject baseObject, GameObject startBase)
    {
        yield return new WaitForSeconds(delaySeconds);

        Vector2 position = baseObject != null ? baseObject.transform.position : Vector3.zero;
        if (baseObject == null) position = startBase.transform.position;
        GameObject newEnemy = Instantiate(enemyPrefab, position, enemyPrefab.transform.rotation);
        FindObjectOfType<EnemyController>().SetRandomEnemy(newEnemy, MaxEnemyLevel);
    }

    private void UpdateScoreText()
    {
        string scoreStr = score.ToString();
        if (score < 100000) scoreStr = "0" + score;
        if (score < 10000) scoreStr = "00" + score;
        if (score < 1000) scoreStr = "000" + score;
        if (score < 100) scoreStr = "0000" + score;
        if (score < 10) scoreStr = "00000" + score;
        scoreText.text = scoreStr;
    }

    public void PowerUpTaken(GameObject taker)
    {
        if ("Enemy".Equals(taker.tag))
        {
            HandleEnemyPowerUp(taker);
        }
        else if ("Player".Equals(taker.tag))
        {
            HandlePlayerPowerUp();
        }

        ShowPowerUpInfo();
    }

    private void HandlePlayerPowerUp()
    {
        switch (currentPowerUp.GetComponent<PowerUpController>().Type)
        {
            case PowerUp.BOMB:
                PowerUpManager.DestroyAllEnemies();
                break;
            /*case PowerUp.GUN:
                break;*/
            case PowerUp.HELMET:
                MakePlayerShieldActive();
                break;
            case PowerUp.SHOVEL:
                SetMarbleProtector();
                break;
            case PowerUp.STAR:
                MakePlayerShotMultipleBullets();
                break;
            case PowerUp.TANK:
                remainingLives += 1;
                UpdateRemainingLivesText();
                break;
            case PowerUp.TIMER:
                PowerUpTimer = (int) Constants.freezeTime;
                PowerUpManager.FreezeAllEnemies();
                break;
        }
    }

    private void HandleEnemyPowerUp(GameObject enemy)
    {
        switch (currentPowerUp.GetComponent<PowerUpController>().Type)
        {
            case PowerUp.BOMB:
                player.GetComponent<PlayerController>().PlayerHit();
                break;
            /*case PowerUp.GUN:
                break;*/
            case PowerUp.HELMET:
                MakeEnemyShieldActive(enemy);
                break;
            case PowerUp.SHOVEL:
                RemoveProtector();
                break;
            case PowerUp.STAR:
                MakeEnemiesShotMultipleBullets();
                break;
            case PowerUp.TANK:
                PowerUpManager.UpdateEnemyLevels();
                break;
            case PowerUp.TIMER:
                PowerUpTimer = (int) Constants.freezeTime;
                FreezePlayer();
                break;
        }
    }

    private void ShowPowerUpInfo()
    {
        powerUpImage.SetActive(true);
        powerUpImage.GetComponent<SpriteRenderer>().sprite = currentPowerUp.GetComponent<SpriteRenderer>().sprite;
        if(PowerUpTimer > -1)
        {
            powerUpInfoText.gameObject.SetActive(true);
            powerUpInfoText.text = PowerUpTimer.ToString();
        }
    }

    public void CreatePowerUp()
    {
        currentPowerUp = PowerUpManager.CreatePowerUp(currentPowerUp, powerUpPrefab);
    }

    private void SetMarbleProtector()
    {
        PowerUpTimer = 5;
        StartCoroutine(SetMarbleProtectorForSeconds(5f));
    }

    IEnumerator SetMarbleProtectorForSeconds(float seconds)
    {
        protector.transform.Find("Walls").gameObject.SetActive(false);
        marbleProtector.transform.Find("Walls").gameObject.SetActive(true);

        yield return new WaitForSeconds(seconds);

        protector.transform.Find("Walls").gameObject.SetActive(true);
        marbleProtector.transform.Find("Walls").gameObject.SetActive(false);
    }

    private void RemoveProtector()
    {
        PowerUpTimer = 5;
        StartCoroutine(RemoveProtectorForSeconds(5f));
    }

    IEnumerator RemoveProtectorForSeconds(float seconds)
    {
        protector.transform.Find("Walls").gameObject.SetActive(false);
        marbleProtector.transform.Find("Walls").gameObject.SetActive(false);

        yield return new WaitForSeconds(seconds);

        protector.transform.Find("Walls").gameObject.SetActive(true);
    }

    private void MakePlayerShotMultipleBullets()
    {
        PowerUpTimer = 5;
        StartCoroutine(PlayerCanShotMultipleBulletsForSeconds(5f));
    }

    IEnumerator PlayerCanShotMultipleBulletsForSeconds(float seconds)
    {
        player.GetComponent<PlayerController>().EnableMultipleShots(true);

        yield return new WaitForSeconds(seconds);

        player.GetComponent<PlayerController>().EnableMultipleShots(false);
    }

    private void MakeEnemiesShotMultipleBullets()
    {
        PowerUpTimer = 5;
        StartCoroutine(EnemiesCanShotMultipleBulletsForSeconds(5f));
    }

    IEnumerator EnemiesCanShotMultipleBulletsForSeconds(float seconds)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyController>().EnableMultipleShots(true);
        }

        yield return new WaitForSeconds(seconds);

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyController>().EnableMultipleShots(false);
        }
    }

    private void MakePlayerShieldActive()
    {
        PowerUpTimer = 5;
        StartCoroutine(PlayerShieldActiveForSeconds(5f));
    }

    IEnumerator PlayerShieldActiveForSeconds(float seconds)
    {
        player.GetComponent<PlayerController>().EnableShield(true);

        yield return new WaitForSeconds(seconds);

        player.GetComponent<PlayerController>().EnableShield(false);
    }

    private void MakeEnemyShieldActive(GameObject enemy)
    {
        PowerUpTimer = 5;
        StartCoroutine(EnemyShieldActiveForSeconds(5f, enemy));
    }

    IEnumerator EnemyShieldActiveForSeconds(float seconds, GameObject enemy)
    {
        enemy.GetComponent<EnemyController>().EnableShield(true);
        
        yield return new WaitForSeconds(seconds);

        enemy.GetComponent<EnemyController>().EnableShield(false);
    }

    private void FreezePlayer()
    {
        player.GetComponent<PlayerController>().FreezePlayer(Constants.freezeTime);
    }
}
