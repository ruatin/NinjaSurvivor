using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using CAH.GameSystem.BigNumber;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeNum
{
    MaxHealth = 0,
    AttackRange = 1,
    AttackDamage = 2,
    AttackDelayTime = 3,
    MoveSpeed = 4,
    SatelliteAttackDamage = 5,
    SatelliteAttackSpeed = 6, 
    SatelliteAttackRadius = 7,
    SatelliteAttackCount = 8,
    MultipleShotCount = 9,
    Defence = 10,
    MultipleShotPercent = 11,
    RecoveryAmount = 12,
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;
    private string[] upgradeNumStrings = new string[]
    {
        "Max Health",
        "Attack Range",
        "Attack Damage",
        "Attack DelayTime",
        "Move Speed",
        "Satellite Damage",
        "Satellite Speed",
        "Satellite Radius",
        "Satellite Count",
        "MultipleShot Count",
        "Defence",
        "MultipleShot Percent",
        "Hp Recovery",
    };
    
    //넘버링값
    public Player player;

    [HideInInspector]
    public int maxHealthUpgrade;
    [HideInInspector]
    public int normalAttackRangeUpgrade;
    [HideInInspector]
    public int normalAttackDamageUpgrade;
    [HideInInspector]
    public int normalAttackDelayTimeUpgrade;
    [HideInInspector]
    public int moveSpeedUpgrade;
    [HideInInspector]
    public int satelliteAttackDamageUpgrade;
    [HideInInspector]
    public int satelliteAttackSpeedUpgrade;
    [HideInInspector]
    public int satelliteAttackRadiusUpgrade;
    [HideInInspector]
    public int satelliteAttackCountUpgrade;
    [HideInInspector]
    public int multipleShotCountUpgrade;
    [HideInInspector]
    public int defenceUpgrade;
    [HideInInspector]
    public int multipleShotPercentUpgrade;
    [HideInInspector]
    public int recoveryAmountUpgrade;
    //레벨 및 업그레이드 패널 이름

    //처음에 만들때 레벨+이넘 , 업그레이드 버튼 , 현재 수치, 패널 생성해서 만들기

    private List<TMP_Text> upgradeLevelText = new List<TMP_Text>();
    private List<TMP_Text> upgradeCurrentValue = new List<TMP_Text>();
    private List<TMP_Text> upgradeCost = new List<TMP_Text>();
    private List<Button> upgradeButton = new List<Button>();
    private List<UpgradePanelInfo> _upgradePanelInfos = new List<UpgradePanelInfo>();

    public GameObject upgradePanelRoot;
    public UpgradePanelInfo upgradePanelSlotInfo;
    public GameObject upgradePanelScroll;
    public TMP_Text goldText;
    public GameObject upgradeCanEffect;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        GameManager.instance.goldChangeEvent += GoldChange;
        
        goldText.text = GameManager.instance.Gold.ToString();
        
        var enumCount = Enum.GetValues(typeof(UpgradeNum)).Length;
        for (int i = 0; i < enumCount; i++)
        {
            var upgradePanel = Instantiate(upgradePanelSlotInfo, upgradePanelScroll.transform);
            var i1 = i;
            _upgradePanelInfos.Add(upgradePanel);
            upgradeLevelText.Add(upgradePanel.upgradeLevelText);
            upgradeCurrentValue.Add(upgradePanel.upgradeCurrentValue);
            upgradeButton.Add(upgradePanel.upgradeButton);
            upgradeCost.Add(upgradePanel.upgradeCost);
            ShowUpgradeNumber(i);
            upgradePanel.upgradeButton.onClick.AddListener(() => UpgradeButtonFunc(i1));
        }
        
        UpdateUpgradeLevel();
    }

    public void OpenUpgradePanel()
    {
        UpdateUpgradeButton();
        AudioManager.instance.PlayUiOpen();
        upgradePanelRoot.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ExitUpgradePanel()
    {
        AudioManager.instance.PlayUiClose();
        upgradePanelRoot.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ShowUpgradeNumber(int upgradeNum)
    {
        upgradeLevelText[upgradeNum].text = upgradeNumStrings[upgradeNum] + " : " + GetUpgradeValue(upgradeNum);
        upgradeCurrentValue[upgradeNum].text = player.GetUpgradeCurrentValue(upgradeNum,GetUpgradeValue(upgradeNum)).ToString();
        upgradeCost[upgradeNum].text = BigIntegerManager.GetUnit(UpdateUpgradeCost(upgradeNum));
    }

    private void UpgradeButtonFunc(int upgradeNum)
    {
        var cost = UpdateUpgradeCost(upgradeNum);
        if (GameManager.instance.Gold < cost)
        {
            AudioManager.instance.PlayUpgradeFail();
            return;
        }

        AudioManager.instance.PlayUpgradeSuccess();
        GameManager.instance.Gold -= cost;
        
        UpdateUpgradeValue(upgradeNum);
        ShowUpgradeNumber(upgradeNum);
        UpdateUpgradeLevel();
        UpdateUpgradeButton();
    }

    public int UpdateUpgradeCost(int upgradeNum)
    {
        // 업그레이드 비용 조절함

        int cost = 0;
        // 가중치 개념 넣음
        int baseCost = 500;
        float costWeight = 1.0f;
        float costWeight2 = 2.0f;
        int upgrade_value = 0;

        switch (upgradeNum)
        {
            case (int)UpgradeNum.MaxHealth:
                baseCost = 1000;
                costWeight = 2f;
                costWeight2 = 1.3f;
                upgrade_value = maxHealthUpgrade;
                break;
            case (int)UpgradeNum.AttackRange:
                costWeight = 1.5f;
                costWeight2 = 1.65f;
                upgrade_value = normalAttackRangeUpgrade;
                break;
            case (int)UpgradeNum.AttackDamage:
                upgrade_value = normalAttackDamageUpgrade;
                break;
            case (int)UpgradeNum.AttackDelayTime:
                baseCost = 1000;
                costWeight = 0.5f;
                costWeight2 = 2.7f;
                upgrade_value = normalAttackDelayTimeUpgrade;
                break;
            case (int)UpgradeNum.MoveSpeed:
                baseCost = 200;
                upgrade_value = moveSpeedUpgrade;
                break;
            case (int)UpgradeNum.SatelliteAttackDamage:
                baseCost = 600;
                upgrade_value = satelliteAttackDamageUpgrade;
                break;
            case (int)UpgradeNum.SatelliteAttackSpeed:
                baseCost = 1200;
                costWeight = 0.5f;
                costWeight2 = 2.7f;
                upgrade_value = satelliteAttackSpeedUpgrade;
                break;
            case (int)UpgradeNum.SatelliteAttackRadius:
                baseCost = 1000;
                costWeight = 1.5f;
                costWeight2 = 1.65f;
                upgrade_value = satelliteAttackRadiusUpgrade;
                break;
            case (int)UpgradeNum.SatelliteAttackCount:
                baseCost = 2000;
                upgrade_value = satelliteAttackCountUpgrade;
                break;
            case (int)UpgradeNum.MultipleShotCount:
                baseCost = 2000;
                upgrade_value = multipleShotCountUpgrade;
                break;
            case (int)UpgradeNum.Defence:
                upgrade_value = defenceUpgrade;
                break;
            case (int)UpgradeNum.MultipleShotPercent:
                baseCost = 1000;
                costWeight = 0.5f;
                costWeight2 = 2.7f;
                upgrade_value = multipleShotPercentUpgrade;
                break;
            case (int)UpgradeNum.RecoveryAmount:
                baseCost = 1000;
                upgrade_value = recoveryAmountUpgrade;
                break;
        }

        // 나중에 최적화 하세요
        cost = baseCost + (int)Math.Round(Math.Pow(upgrade_value * costWeight, costWeight2) * baseCost);
        
        return cost;
    }
    
    public void UpdateUpgradeValue(int upgradeNum)
    {
        switch (upgradeNum)
        {
            case (int)UpgradeNum.MaxHealth:
                maxHealthUpgrade++;
                break;
            case (int)UpgradeNum.AttackRange:
                normalAttackRangeUpgrade++;
                break;
            case (int)UpgradeNum.AttackDamage:
                normalAttackDamageUpgrade++;
                break;
            case (int)UpgradeNum.AttackDelayTime:
                normalAttackDelayTimeUpgrade++;
                break;
            case (int)UpgradeNum.MoveSpeed:
                moveSpeedUpgrade++;
                break;
            case (int)UpgradeNum.SatelliteAttackDamage:
                satelliteAttackDamageUpgrade++;
                break;
            case (int)UpgradeNum.SatelliteAttackSpeed:
                satelliteAttackSpeedUpgrade++;
                break;
            case (int)UpgradeNum.SatelliteAttackRadius:
                satelliteAttackRadiusUpgrade++;
                break;
            case (int)UpgradeNum.SatelliteAttackCount:
                satelliteAttackCountUpgrade++;
                break;
            case (int)UpgradeNum.MultipleShotCount:
                multipleShotCountUpgrade++;
                break;
            case (int)UpgradeNum.Defence:
                defenceUpgrade++;
                break;
            case (int)UpgradeNum.MultipleShotPercent:
                multipleShotPercentUpgrade++;
                break;
            case (int)UpgradeNum.RecoveryAmount:
                recoveryAmountUpgrade++;
                break;
        }
    }

    public int GetUpgradeValue(int upgradeNum)
    {
        int result = -1;
        switch (upgradeNum)
        {
            case (int)UpgradeNum.MaxHealth:
                result = maxHealthUpgrade;
                break;
            case (int)UpgradeNum.AttackRange:
                result = normalAttackRangeUpgrade;
                break;
            case (int)UpgradeNum.AttackDamage:
                result = normalAttackDamageUpgrade;
                break;
            case (int)UpgradeNum.AttackDelayTime:
                result = normalAttackDelayTimeUpgrade;
                break;
            case (int)UpgradeNum.MoveSpeed:
                result = moveSpeedUpgrade;
                break;
            case (int)UpgradeNum.SatelliteAttackDamage:
                result = satelliteAttackDamageUpgrade;
                break;
            case (int)UpgradeNum.SatelliteAttackSpeed:
                result = satelliteAttackSpeedUpgrade;
                break;
            case (int)UpgradeNum.SatelliteAttackRadius:
                result = satelliteAttackRadiusUpgrade;
                break;
            case (int)UpgradeNum.SatelliteAttackCount:
                result = satelliteAttackCountUpgrade;
                break;
            case (int)UpgradeNum.MultipleShotCount:
                result = multipleShotCountUpgrade;
                break;
            case (int)UpgradeNum.Defence:
                result = defenceUpgrade;
                break;
            case (int)UpgradeNum.MultipleShotPercent:
                result = multipleShotPercentUpgrade;
                break;
            case (int)UpgradeNum.RecoveryAmount:
                result = recoveryAmountUpgrade;
                break;
        }

        return result;
    }

    public int GetTotalUpgradeValue()
    {
        var total = 0;
        for (int i = 0; i < Enum.GetValues(typeof(UpgradeNum)).Length; i++)
        {
            total += GetUpgradeValue(i);
        }
        return total;
    }

    private void UpdateUpgradeLevel()
    {
        var totalUpgradeValue = GetTotalUpgradeValue();
        UiManager.instance.playerLevel.text = "Lv "+totalUpgradeValue;
    }
    
    private void GoldChange(BigInteger gold)
    {
        instance.goldText.text = string.Format(BigIntegerManager.GetUnit(gold));
        if (FindCanUpgradeSlot() > 0)
        {
            upgradeCanEffect.SetActive(true);
        }
        else
        {
            upgradeCanEffect.SetActive(false);
        }
    }

    private int FindCanUpgradeSlot()
    {
        //모든 업그레이드 패널중에서 업그레이드 가능한 패널이 있다면 트루를 반환
        var enumCount = Enum.GetValues(typeof(UpgradeNum)).Length;
        var upCount = 0;
        for (int i = 0; i < enumCount; i++)
        {
            if (GameManager.instance.Gold >= UpdateUpgradeCost(i))
            {
                upCount++;
            }
        }
        return upCount;
    }

    private void UpdateUpgradeButton()
    {
        var enumCount = Enum.GetValues(typeof(UpgradeNum)).Length;
        for (int i = 0; i < enumCount; i++)
        {
            var colors = upgradeButton[i].colors;
            if (GameManager.instance.Gold >= UpdateUpgradeCost(i))
            {
                colors.normalColor = new Color32(255, 255, 255,255);
                colors.highlightedColor = new Color32(245, 245, 245,245);
                colors.pressedColor = new Color32(200, 200, 200,200);
                colors.selectedColor = new Color32(245, 245, 245,245);
                colors.disabledColor = new Color32(138, 138, 138,138);
                upgradeButton[i].colors = colors;
            }
            else
            {
                colors.normalColor = new Color32(138, 138, 138,138);
                colors.highlightedColor = new Color32(138, 138, 138,138);
                colors.pressedColor = new Color32(138, 138, 138,138);
                colors.selectedColor = new Color32(138, 138, 138,138);
                colors.disabledColor = new Color32(138, 138, 138,138);
                upgradeButton[i].colors = colors;
            }
        }
    }
}
