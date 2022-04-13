using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveSpeed;
    public float alphaSpeed;
    public float destroyTime;
    [HideInInspector]
    public TMP_Text text;
    Color alpha;
    public int damage;
    public int gold;
    public bool isDamaged;
    private float fontSize;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        alpha = text.color;
        fontSize = text.fontSize;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        if (isDamaged)
        {
            text.text = String.Format("{0:#,##0}",damage);
        }
        else
        {
            text.text = String.Format("<color=#BFFF00>${0:#,##0}</color>",gold);
        }
        
        Invoke("DestroyObject", destroyTime);
        alpha.a = 1f;
        text.color = alpha;
    }

    private void OnDisable()
    {
        text.fontSize = fontSize;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0)); // 텍스트 위치

        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed); // 텍스트 알파값
        text.color = alpha;
    }

    private void DestroyObject()
    {
        FloatingTextPool.ReturnObject(this);
    }
}