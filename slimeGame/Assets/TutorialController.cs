using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField]
    private Transform dotHolder;
    [SerializeField]
    private Transform imageHolder;

    private int index = 1;
    private int imageCount = 0;

    void Start()
    {
        foreach (Transform child in imageHolder)
        {
            imageCount++;
        }
        ShowPage();
    }

    public void Increase()
    {
        index++;
        if(index > imageCount) index = imageCount;
        ShowPage();
    }
    public void Decrease()
    {
        index--;
        if(index < 1) index = 1;
        ShowPage();
    }

    private void ShowPage()
    {
        for (int i = 0; i < imageHolder.childCount; i++)
        {
            imageHolder.GetChild(i).gameObject.SetActive(i == index - 1);
        }

        for (int i = 0; i < dotHolder.childCount; i++)
        {
            Image dot = dotHolder.GetChild(i).GetComponent<Image>();
            dot.color = (i == index - 1) ? Color.white : Color.grey;
        }
    }
}
