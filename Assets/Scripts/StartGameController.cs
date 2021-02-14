using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartGameController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    int defaultFontSize;

    void Start()
    {
        defaultFontSize = gameObject.GetComponent<Text>().fontSize;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.RestartGame();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.GetComponent<Text>().fontSize = 52;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<Text>().fontSize = defaultFontSize;
    }
}
