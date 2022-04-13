using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    public GameObject gameOverAdUiPanel;
    public GameObject gameOverUiPanel;
    public GameObject quitPanel;
    public GameObject settingPanel;
    public TMP_Text playerLevel;
    public TMP_Text gameTimerText;

    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        InitBgmActive();
        InitSfxActive();
        GameManager.instance.gameOverEvent += OpenGameOver;
        AdManager.instance.adContinueGameEvent += ContinueGame;
    }

    private void OpenGameOver()
    {
        AudioManager.instance.PlayGameOver();
        AudioManager.instance.StopBgm();
        if (GameManager.instance.isContinueGame)
        {
            gameOverAdUiPanel.SetActive(false);
            gameOverUiPanel.SetActive(true);
        }
        else
        {
            gameOverAdUiPanel.SetActive(true);
            gameOverUiPanel.SetActive(false);
        }

    }
    
    private void ContinueGame(bool isSuccess)
    {
        if (!isSuccess)
        {
            return;
        }
        if (PlayerPrefs.GetInt("BgmOn", 1) == 1)
        {
            AudioManager.instance.PlayBgm();
        }
        gameOverAdUiPanel.SetActive(false);
    }

    public void GoStartMenu()
    {
        AudioManager.instance.PlayGoTitle();
        SceneManager.LoadScene("Title");
    }

    public void QuitPanelActive()
    {
        if (quitPanel.activeSelf)
        {
            AudioManager.instance.PlayUiClose();
            quitPanel.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            AudioManager.instance.PlayUiOpen();
            quitPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    
    public void SettingPanelActive()
    {
        if (settingPanel.activeSelf)
        {
            AudioManager.instance.PlayUiClose();
            settingPanel.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            AudioManager.instance.PlayUiOpen();
            settingPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public GameObject bgmOn;
    public GameObject bgmOff;
    public GameObject sfxOn;
    public GameObject sfxOff;
    
    private void InitBgmActive()
    {
        var isBgmOn = PlayerPrefs.GetInt("BgmOn", 1);
        if (isBgmOn == 1)
        {
            bgmOff.SetActive(false);
            bgmOn.SetActive(true);
            AudioManager.instance.PlayBgm();
        }
        else if(isBgmOn == 0)
        {
            bgmOff.SetActive(true);
            bgmOn.SetActive(false);
            AudioManager.instance.StopBgm();
        }
    }
    
    private void InitSfxActive()
    {
        var isSfxOn = PlayerPrefs.GetInt("SfxOn", 1);
        if (isSfxOn == 1)
        {
            sfxOff.SetActive(false);
            sfxOn.SetActive(true);
            AudioManager.instance.PlaySfx();
        }
        else if(isSfxOn == 0)
        {
            sfxOff.SetActive(true);
            sfxOn.SetActive(false);
            AudioManager.instance.StopSfx();
        }
    }

    public void SetBgmActive()
    {
        var isBgmOn = PlayerPrefs.GetInt("BgmOn", 1);
        if (isBgmOn == 1)
        {
            bgmOff.SetActive(true);
            bgmOn.SetActive(false);
            AudioManager.instance.StopBgm();
            PlayerPrefs.SetInt("BgmOn", 0);
        }
        else
        {
            bgmOff.SetActive(false);
            bgmOn.SetActive(true);
            AudioManager.instance.PlayBgm();
            PlayerPrefs.SetInt("BgmOn", 1);
        }
    }
    
    public void SetSfxActive()
    {
        var isSfxOn = PlayerPrefs.GetInt("SfxOn", 1);
        if (isSfxOn == 1)
        {
            sfxOff.SetActive(true);
            sfxOn.SetActive(false);
            AudioManager.instance.StopSfx();
            PlayerPrefs.SetInt("SfxOn", 0);
        }
        else
        {
            sfxOff.SetActive(false);
            sfxOn.SetActive(true);
            AudioManager.instance.PlaySfx();
            PlayerPrefs.SetInt("SfxOn", 1);
        }
    }
}
