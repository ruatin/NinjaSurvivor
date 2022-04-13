using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainTitle : MonoBehaviour
{
    private bool isStart;
    public TMP_Text maxScoreText;

    private void Awake()
    {
        var maxScore = PlayerPrefs.GetInt("PlayerMaxScore", 0);
        if (maxScore > 0)
        {
            maxScoreText.text = maxScore.ToString();
            maxScoreText.gameObject.SetActive(true);
        }
        else
        {
            maxScoreText.gameObject.SetActive(false);
        }
    }

    public void StarGame()
    {
        if (isStart)
        {
            return;
        }
        
        AudioManager.instance.PlayGameStart();
        StartCoroutine(LoadGameScene());
    }
    
    private IEnumerator LoadGameScene()
    {
        isStart = true;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainGame");
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }
}
