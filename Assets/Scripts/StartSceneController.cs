using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Text highScore;
    private int defaultFontSize;

    // Start is called before the first frame update
    void Start()
    {
        int score = PlayerPrefs.GetInt("HighScore");
        string scoreText = "";
        if (score == 0)
        {
            scoreText = "000000";
        } else
        {
            scoreText = score.ToString();
        }
        highScore.text = scoreText;
        defaultFontSize = gameObject.GetComponent<Text>().fontSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.GetComponent<Text>().fontSize = defaultFontSize + 10;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<Text>().fontSize = defaultFontSize;
    }
}
