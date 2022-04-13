using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;
    private AudioSource _player;
    private AudioSource _bgm;

    public AudioClip hitClip;

    public AudioClip gameStartClip;
    public AudioClip uiOpenClip;
    public AudioClip uiCloseClip;
    public AudioClip upgradeSuccessClip;
    public AudioClip upgradeFailClip;
    public AudioClip gameOverClip;
    public AudioClip goToTitleButtonClip;
    public AudioClip playerDamagedClip;
    public AudioClip scoreUpdateClip;
    public AudioClip totalScoreUpdateClip;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        
        DontDestroyOnLoad(this);
        
        var bgmObject = GameObject.FindWithTag("Bgm");
        if (bgmObject != null)
        {
            _bgm = bgmObject.GetComponent<AudioSource>();
        }
        
        if (FindObjectOfType<Player>())
        {
            _player = FindObjectOfType<Player>().GetComponent<AudioSource>();
        }
        _audioSource = GetComponent<AudioSource>();
        
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (FindObjectOfType<Player>())
        {
            _player = FindObjectOfType<Player>().GetComponent<AudioSource>();
        }
        
        var bgmObject = GameObject.FindWithTag("Bgm");
        if (bgmObject != null)
        {
            _bgm = bgmObject.GetComponent<AudioSource>();
        }
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayBgm()
    {
        _bgm.Play();
    }
    
    public void StopBgm()
    {
        _bgm.Stop();
    }
    
    public void PlaySfx()
    {
        _audioSource.mute = false;
        _player.mute = false;
    }
    
    public void StopSfx()
    {
        _audioSource.mute = true;
        _player.mute = true;
    }

    public void PlayHit()
    {
        _audioSource.Stop();
        _audioSource.clip = hitClip;
        _audioSource.Play();
    }

    public void PlayGameStart()
    {
        _audioSource.Stop();
        _audioSource.clip = gameStartClip;
        _audioSource.Play();
    }
    
    public void PlayUiOpen()
    {
        _audioSource.Stop();
        _audioSource.clip = uiOpenClip;
        _audioSource.Play();
    }
    
    public void PlayUiClose()
    {
        _audioSource.Stop();
        _audioSource.clip = uiCloseClip;
        _audioSource.Play();
    }
    
    public void PlayUpgradeSuccess()
    {
        _audioSource.Stop();
        _audioSource.clip = upgradeSuccessClip;
        _audioSource.Play();
    }
    
    public void PlayUpgradeFail()
    {
        _audioSource.Stop();
        _audioSource.clip = upgradeFailClip;
        _audioSource.Play();
    }
    
    public void PlayGameOver()
    {
        _audioSource.Stop();
        _audioSource.clip = gameOverClip;
        _audioSource.Play();
    }
    
    public void PlayGoTitle()
    {
        _audioSource.Stop();
        _audioSource.clip = goToTitleButtonClip;
        _audioSource.Play();
    }
    
    public void PlayScoreUpdate()
    {
        _audioSource.Stop();
        _audioSource.clip = scoreUpdateClip;
        _audioSource.Play();
    }
    
    public void PlayTotalScoreUpdate()
    {
        _audioSource.Stop();
        _audioSource.clip = totalScoreUpdateClip;
        _audioSource.Play();
    }
    
    public void PlayDamagedPlayer()
    {
        _player.Stop();
        _player.clip = playerDamagedClip;
        _player.Play();
    }
}
