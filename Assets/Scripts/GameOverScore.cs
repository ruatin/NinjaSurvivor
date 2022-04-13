using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameOverScore : MonoBehaviour
{
    public TMP_Text timerText;
    public TMP_Text monsterScoreText;
    public TMP_Text upgradeLevelText;
    public TMP_Text totalGameScoreText;

    public DOTweenAnimation timerDoTween;
    public DOTweenAnimation monsterScoreDoTween;
    public DOTweenAnimation upgradeLevelDoTween;

    public GameObject lockObj;
    public GameObject tapToStartObj;
    
    private int timerScore;
    private int monsterScore;
    private int upgradeLevelScore;
    private int totalScore;


    private void Awake()
    {
        timerDoTween.DOPlay();
    }

    private void OnEnable()
    {
        lockObj.SetActive(true);
        tapToStartObj.SetActive(false);
        
        timerScore = GameManager.instance.GetTimerScore();
        monsterScore = GameManager.instance.MonsterScore;
        upgradeLevelScore = UpgradeManager.instance.GetTotalUpgradeValue();
        
        //게임 타임 가져와야함
        timerText.text = string.Format("{0:D2} : {1:D2}", 0, 0);
        monsterScoreText.text = 0.ToString();
        upgradeLevelText.text = 0.ToString();
        totalGameScoreText.text = 0.ToString();
    }

    public void UpdateGameScore(int num)
    {
        if (num == 0)
        {
            timerText.text = UiManager.instance.gameTimerText.text;
        }
        else if( num == 1)
        {
            monsterScoreText.text = monsterScore.ToString();
        }
        else if(num == 2)
        {
            upgradeLevelText.text = upgradeLevelScore.ToString();
        }
        
        AudioManager.instance.PlayScoreUpdate();
    }

    public void UpdateTotalGameScore(int num)
    {
        var score = upgradeLevelScore + monsterScore + timerScore;
        var maxScore = PlayerPrefs.GetInt("PlayerMaxScore", 0);
        if (score > maxScore)
        {
            PlayerPrefs.SetInt("PlayerMaxScore",score);
        }

        StartCoroutine(AnimTotalScore(score));
    }

    private IEnumerator AnimTotalScore(int score)
    {
        yield return new WaitForSeconds(0.01f);
        totalGameScoreText.text = score.ToString();
        AudioManager.instance.PlayTotalScoreUpdate();
    }

    public void ActiveTapToStart()
    {
        lockObj.SetActive(false);
        tapToStartObj.SetActive(true);
    }
}
