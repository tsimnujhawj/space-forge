using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    // configurations
    [Header("Player Movement")]
    [SerializeField] float moveXSpeed = 25f;
    [SerializeField] float moveYSpeed = 25f;
    [Range(-5, 25)] [SerializeField] float boundaryPadding = 0.5f;
    [SerializeField] float health = 500;

    [Header("Player VFX/SFX")]
    [SerializeField] GameObject deathVFX;
    [SerializeField] float durationOfExplosion = 0f;
    [Range(0, 1)] [SerializeField] float laserSFXVolume = 0.03f;
    [Range(0, 1)] [SerializeField] float deathSFXVolume = 0.8f;
    [Range(0, 1)] [SerializeField] float damageSFXVolume = 0.05f;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip laserSFX;
    [SerializeField] AudioClip laserAutoSFX;
    [SerializeField] AudioClip damageSFX;

    [Header("Player Projectiles")]
    [SerializeField] GameObject laserPrefab;
    [Range(-30, 30)] [SerializeField] float projectileSpeed = 10f;
    [Range(-5, 5)] [SerializeField] float projectileFiringPeriod = 0.2f;

    Coroutine firingCoroutine;
    Level levelLoader;

    // parameters
    float xMin;
    float xMax;
    float yMin;
    float yMax;

    void Start()
    {
        levelLoader = FindObjectOfType<Level>();
        SetUpMoveBoundaries();
    }

    void Update()
    {
        Move();
        Fire();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        PlayDamageAudioClip();
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // TODO: death SFX
        levelLoader.LoadGameOver();
        Destroy(gameObject, durationOfExplosion);
        PlayDeathAudioClip();
        GameObject explosion = Instantiate(
                deathVFX,
                transform.position,
                Quaternion.identity
            ) as GameObject;
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            PlayLaserAudioClip();
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            // TODO: player shooting SFX
            PlayAutoLaserAudioClip();
            GameObject laser = Instantiate(
                laserPrefab,
                transform.position,
                Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveXSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveYSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);

        transform.position = new Vector2(newXPos, newYPos);
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        // horizontal mins and maxes
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + boundaryPadding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - boundaryPadding;

        // vertical mins and maxes
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + boundaryPadding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - boundaryPadding;
    }

    private void PlayDeathAudioClip()
    {
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSFXVolume);
    }

    private void PlayLaserAudioClip()
    {
        AudioSource.PlayClipAtPoint(laserSFX, Camera.main.transform.position, laserSFXVolume);
    }

    private void PlayAutoLaserAudioClip()
    {
        AudioSource.PlayClipAtPoint(laserAutoSFX, Camera.main.transform.position, laserSFXVolume);
    }

    private void PlayDamageAudioClip()
    {
        AudioSource.PlayClipAtPoint(damageSFX, Camera.main.transform.position, damageSFXVolume);
    }

}
