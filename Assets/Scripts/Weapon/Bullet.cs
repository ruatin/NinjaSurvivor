using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletDamage;
    public float bulletSpeed;
    public Transform targetDir;

    private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        StartCoroutine(AutoDestroyBullet(2));
        if (targetDir != null)
        {
            transform.up = targetDir.position-transform.position;
        }
    }

    private void OnDisable()
    {
        
    }

    private void FixedUpdate()
    {
        MoveBullet(targetDir);
    }

    private void MoveBullet(Transform dir)
    {
        if (!dir.gameObject.activeSelf)
        {
            DestroyBullet();
        }

        transform.up = dir.position -transform.position;
        transform.position += (dir.position -transform.position).normalized * (bulletSpeed * Time.deltaTime);
    }

    private IEnumerator AutoDestroyBullet(float sec)
    {
        yield return new WaitForSeconds(sec);
        DestroyBullet();
    }

    private void DestroyBullet()
    {
        BulletPool.ReturnObject(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            other.GetComponent<Enemy>().Damaged(bulletDamage);
            DestroyBullet();
        }
    }
}
