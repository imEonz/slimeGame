using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public enum UIState {idle, pause, tab, level}
    [SerializeField]
    private UIState uIState;
    [SerializeField]
    private Transform pauseCanvas;
    [SerializeField]
    private Transform tabCanvas;
    [SerializeField]
    private Transform levelCanvas;

    private PlayerController player;
    private PlayerLeveler playerLeveler;

    void Start()
    {
        player = GameObject.FindAnyObjectByType<PlayerController>();
        playerLeveler = FindAnyObjectByType<PlayerLeveler>();
    }

    void Update()
    {
        if(uIState != UIState.idle) return;
    }

    private void ActivatePause()
    {
        if(!pauseCanvas.gameObject.activeSelf)
        {
            pauseCanvas.gameObject.SetActive(true);
            Time.timeScale = 0f;
            uIState = UIState.pause;
        }
        else
        {
            pauseCanvas.gameObject.SetActive(false);
            if(player.GetPlayerState() != PlayerController.State.Consuming) Time.timeScale = 1f;
            uIState = UIState.idle;
        }
    }

    private void ActivateTab()
    {
        if(!tabCanvas.gameObject.activeSelf)
        {
            tabCanvas.gameObject.SetActive(true);
            Time.timeScale = 0f;
            uIState = UIState.tab;
        }
        else
        {
            tabCanvas.gameObject.SetActive(false);
            if(player.GetPlayerState() != PlayerController.State.Consuming) Time.timeScale = 1f;
            uIState = UIState.idle;
        }
    }
    public void ActivateLevel()
    {
        if(!levelCanvas.gameObject.activeSelf)
        {
            levelCanvas.gameObject.SetActive(true);
            Time.timeScale = 0f;
            uIState = UIState.level;
        }
        else
        {
            levelCanvas.gameObject.SetActive(false);
            if(player.GetPlayerState() != PlayerController.State.Consuming) Time.timeScale = 1f;
            uIState = UIState.level;
        }
    }

    public void RegisterEscPress()
    {
        if(uIState != UIState.level && uIState != UIState.tab) ActivatePause();
    }
    public void RegisterTabPress()
    {
        if(uIState != UIState.level && uIState != UIState.pause) ActivateTab();
    }
}
