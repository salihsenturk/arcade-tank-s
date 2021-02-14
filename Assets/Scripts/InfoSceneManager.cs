using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class InfoSceneManager : MonoBehaviour
{
    public GameObject infoCanvas;
    public GameObject gameOverCanvas;
    public GameObject winCanvas;
    public Text highScoreText;
    public Text currentScoreText;
    public Text levelName;

    private Text grayEnemyScore;
    private Text grayEnemyCount;
    private Text yellowEnemyScore;
    private Text yellowEnemyCount;
    private Text greenEnemyScore;
    private Text greenEnemyCount;
    private Text redEnemyScore;
    private Text redEnemyCount;
    private Text totalEnemyCountText;

    void Awake()
    {
        grayEnemyScore = GameObject.Find("GrayEnemyScore").GetComponent<Text>();
        grayEnemyCount = GameObject.Find("GrayEnemyCount").GetComponent<Text>();
        yellowEnemyScore = GameObject.Find("YellowEnemyScore").GetComponent<Text>();
        yellowEnemyCount = GameObject.Find("YellowEnemyCount").GetComponent<Text>();
        greenEnemyScore = GameObject.Find("GreenEnemyScore").GetComponent<Text>();
        greenEnemyCount = GameObject.Find("GreenEnemyCount").GetComponent<Text>();
        redEnemyScore = GameObject.Find("RedEnemyScore").GetComponent<Text>();
        redEnemyCount = GameObject.Find("RedEnemyCount").GetComponent<Text>();
        totalEnemyCountText = GameObject.Find("TotalEnemyCount").GetComponent<Text>();
    }

    public void GameOver()
    {
        gameObject.SetActive(true);
        gameOverCanvas.SetActive(true);
        infoCanvas.SetActive(false);
        int highScore = PlayerPrefs.GetInt("HighScore");
        highScoreText.text = highScore.ToString();
    }

    public void GameCompleted(int score)
    {
        gameObject.SetActive(true);
        winCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        infoCanvas.SetActive(false);
        int highScore = PlayerPrefs.GetInt("HighScore");
        highScore += score;
        PlayerPrefs.SetInt("HighScore", highScore);
        highScoreText.text = highScore.ToString();
    }

    public void ShowInfo(Dictionary<AbstractEnemy.EnemyColor, int> destroyedEnemyCount, int currentLevel, int currentScore)
    {
        gameObject.SetActive(true);
        gameOverCanvas.SetActive(false);
        infoCanvas.SetActive(true);

        GameObject grayEnemy = GameObject.Find("GrayEnemyInfo");
        GameObject yellowEnemy = GameObject.Find("YellowEnemyInfo");
        GameObject greenEnemy = GameObject.Find("GreenEnemyInfo");
        GameObject redEnemy = GameObject.Find("RedEnemyInfo");
        GameObject dashes = GameObject.Find("Dashes");
        GameObject totalText = GameObject.Find("TotalText");
        GameObject continueButton = GameObject.Find("ContinueButton");

        SetInfoVisible(grayEnemy, false);
        SetInfoVisible(yellowEnemy, false);
        SetInfoVisible(greenEnemy, false);
        SetInfoVisible(redEnemy, false);
        SetInfoVisible(dashes, false);
        SetInfoVisible(totalText, false);
        SetInfoVisible(totalEnemyCountText.gameObject, false);
        SetInfoVisible(continueButton, false);

        int highScore = PlayerPrefs.GetInt("HighScore");
        if(highScore == 0)
        {
            highScoreText.text = "000000";
        } else
        {
            highScoreText.text = highScore.ToString();
        }

        currentScoreText.text = currentScore.ToString();
        levelName.text = "Level " + currentLevel;

        int oldHighScore = PlayerPrefs.GetInt("HighScore");
        if (currentScore > oldHighScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
        }

        int totalEnemyCount = 0;

        foreach (AbstractEnemy.EnemyColor color in (AbstractEnemy.EnemyColor[]) Enum.GetValues(typeof(AbstractEnemy.EnemyColor)))
        {
            int score = EnemyLevel.GetScoreByColor(color);
            int count = 0;
            if (destroyedEnemyCount.ContainsKey(color))
            {
                count = destroyedEnemyCount[color];
            }

            string scoreText = (score * count).ToString();
            string countText = count.ToString();
            SetScoreValues(color, scoreText, countText);
            totalEnemyCount += count;
        }

        totalEnemyCountText.text = totalEnemyCount.ToString();

        StartCoroutine(SetInfoVisibleDelayed(1f, grayEnemy, true));
        StartCoroutine(SetInfoVisibleDelayed(2f, yellowEnemy, true));
        StartCoroutine(SetInfoVisibleDelayed(3f, greenEnemy, true));
        StartCoroutine(SetInfoVisibleDelayed(4f, redEnemy, true));
        StartCoroutine(SetInfoVisibleDelayed(5f, dashes, true));
        StartCoroutine(SetInfoVisibleDelayed(6f, totalText, true));
        StartCoroutine(SetInfoVisibleDelayed(6f, totalEnemyCountText.gameObject, true));
        StartCoroutine(SetInfoVisibleDelayed(7f, continueButton, true));
    }

    private void SetScoreValues(AbstractEnemy.EnemyColor color, string scoreText, string countText)
    {
        switch (color)
        {
            case AbstractEnemy.EnemyColor.GRAY:
                grayEnemyScore.text = scoreText;
                grayEnemyCount.text = countText + " X";
                break;
            case AbstractEnemy.EnemyColor.YELLOW:
                yellowEnemyScore.text = scoreText;
                yellowEnemyCount.text = countText + " X";
                break;
            case AbstractEnemy.EnemyColor.GREEN:
                greenEnemyScore.text = scoreText;
                greenEnemyCount.text = countText + " X";
                break;
            case AbstractEnemy.EnemyColor.RED:
                redEnemyScore.text = scoreText;
                redEnemyCount.text = countText + " X";
                break;
        }
    }

    private IEnumerator SetInfoVisibleDelayed(float delaySecond, GameObject gameObject, bool visible)
    {
        yield return new WaitForSeconds(delaySecond);

        SetInfoVisible(gameObject, visible);
    }

    private void SetInfoVisible(GameObject info, bool visible)
    {
        info.SetActive(visible);
    }
}
