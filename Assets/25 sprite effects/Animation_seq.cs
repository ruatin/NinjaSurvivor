using System;
using UnityEngine;
using System.Collections;

public class Animation_seq: MonoBehaviour
{
    public float fps = 24.0f;
    public Texture2D[] frames;

    private int frameIndex;
    private MeshRenderer rendererMy;

    void Start()
    {
        rendererMy = GetComponent<MeshRenderer>();
        NextFrame();
        InvokeRepeating("NextFrame", 1 / fps, 1 / fps);
    }

    private void OnDisable()
    {
        rendererMy.sharedMaterial.SetTexture("_MainTex", frames[0]);
        frameIndex = 0;
    }
    
    private float currentTime;

    private void Update()
    {
    }

    void NextFrame()
    {
        rendererMy.sharedMaterial.SetTexture("_MainTex", frames[frameIndex]);
        frameIndex = (frameIndex + 0001) % frames.Length;
    }
}