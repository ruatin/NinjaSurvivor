using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using CAH.GameSystem.BigNumber;
using TMPro;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public enum GameState
{
    GameReady,
    GameStart,
    GamePause,
    GamePlaying,
    GameOver
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player player;

    public int waveCount;
    public GameState GameState = GameState.GameReady;

    public delegate void GameOver();
    public event GameOver gameOverEvent;
    public delegate void GoldChange(BigInteger gold);
    public event GoldChange goldChangeEvent;

    private float _sec;
    private int _min;

    public SpriteRenderer battleField;
    public Transform upWallTr;
    public Transform downWallTr;
    public Transform rightWallTr;
    public Transform leftWallTr;

    private void GameTimer()
    {
        if (GameState == GameState.GameOver)
        {
            return;
        }
        
        _sec += Time.deltaTime;
        
        if ((int) _sec > 59)
        {
            _sec = 0;
            _min++;
        }
        
        UiManager.instance.gameTimerText.text = string.Format("{0:D2} : {1:D2}", _min, (int) _sec);
    }

    public int GetTimerScore()
    {
        return _min * 10 + (int)_sec * 5;
    }

    public int MonsterScore
    {
        get => _monsterScore;
        set => _monsterScore = value;
    }

    private int _monsterScore;


    public BigInteger Gold
    {
        get => _gold;
        set
        {
            _gold = value;
            goldText.text = string.Format(BigIntegerManager.GetUnit(_gold));
            // UpgradeManager.instance.goldText.text = string.Format(BigIntegerManager.GetUnit(_gold));
            goldChangeEvent?.Invoke(_gold);
        }
    }

    private BigInteger _gold;

    [SerializeField]
    private TMP_Text goldText;
    
    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }


        UpdateScreenSize();
    }

    public float spawnScale = 0.75f;
    private void UpdateScreenSize()
    {
        var sprite = battleField.sprite;
        float size = Camera.main.orthographicSize;
        float sizeX = sprite.bounds.size.x;
        float sizeY = sprite.bounds.size.y;
        float screenY = size * 2;
        float screenX = screenY / Screen.height * Screen.width;
        var scaleX = screenX / sizeX;
        var scaleY = screenY/sizeY;

        
        upWallTr.localScale = new Vector3(scaleX,spawnScale,0);
        downWallTr.localScale = new Vector3(scaleX,spawnScale,0);
        rightWallTr.localScale = new Vector3(scaleX,scaleY,0);
        leftWallTr.localScale = new Vector3(scaleX,scaleY,0);
        battleField.transform.localScale = new Vector3(scaleX,scaleY,2);
        
        upWallTr.position = new Vector3(0,sizeY/2 * scaleY + sizeY/2 * spawnScale,0);
        downWallTr.position = new Vector3(0,-(sizeY/2 * scaleY + sizeY/2 * spawnScale),0);
        rightWallTr.position = new Vector3(sizeX*scaleX,0,0);
        leftWallTr.position = new Vector3(-sizeX*scaleX,0,0);
    }

    private void Start()
    {
        GameState = GameState.GamePlaying;
        EnemySpawner.instance.UpdateWaveData(waveCount);
        AdManager.instance.adContinueGameEvent += ContinueGame;
    }
    

    public void SetGameOver()
    {
        GameState = GameState.GameOver;
        EnemySpawner.instance.StopAllSpawn();
        gameOverEvent?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UiManager.instance.QuitPanelActive();
        }
        
        if (GameState == GameState.GameOver)
        {
            EnemySpawner.instance.StopAllSpawn();
        }

        GameTimer();
    }
    
    
    public void QuitGame()
    {
        Application.Quit();
    }

    [HideInInspector]
    public bool isContinueGame;
    private void ContinueGame(bool isSuccess)
    {
        if (!isSuccess)
        {
            return;
        }
        isContinueGame = true;
        GameState = GameState.GamePlaying;
        EnemySpawner.instance.UpdateWaveData(waveCount);
    }
}
