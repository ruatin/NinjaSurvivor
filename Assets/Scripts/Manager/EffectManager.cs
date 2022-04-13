using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public GameObject explosionPrefab;
    public GameObject explosionHitPrefab;
    
    private ParticleSystem explosion;
    private ParticleSystem explosionHit;

    public static EffectManager instance;
    
    private void Awake()
    {
        instance = this;

        explosion = Instantiate(explosionPrefab, transform).GetComponent<ParticleSystem>();
        explosionHit = Instantiate(explosionHitPrefab, transform).GetComponent<ParticleSystem>();
    }

    public void PlayExplosion(Vector3 pos , float size)
    {
        explosion.transform.position = pos;
        var explosionMain = explosion.main;
        explosionMain.startSize = size;
        explosion.Play();
    }
    
    public void PlayExplosionHit(Vector3 pos , float size)
    {
        explosionHit.transform.position = pos;
        var explosionHitMain = explosionHit.main;
        explosionHitMain.startSize = size;
        explosionHit.Play();
    }
}
