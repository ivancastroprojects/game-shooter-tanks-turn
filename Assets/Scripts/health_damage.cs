using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class health_damage : MonoBehaviour
{
    public Slider slider;
    public Image fillImage;
    Text textSlider;

    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;
    public GameObject explosionPrefab;
    public ParticleSystem explosionParticles;

    protected Rigidbody _rigidBody;
    protected float _tankSpeed;
    protected float _turnSpeed;
    protected float _startingHealth = 100;
    protected float _currentHealth;
    protected float _damage = 30;
    protected float _mass;

    private void Start()
    {
        if (gameObject.tag == ("Player1"))
        {
            textSlider = slider.transform.GetChild(3).GetComponent<Text>();
            textSlider.text = "Player 1";
        }
        else if (gameObject.tag == ("Player2"))
        {
            textSlider = slider.transform.GetChild(3).GetComponent<Text>();
            textSlider.text = "Player 2";
        }
        else if (gameObject.tag == ("Computer"))
        {
            textSlider = slider.transform.GetChild(3).GetComponent<Text>();
            textSlider.text = "Computer";
        }
        _currentHealth = _startingHealth;
        SetHealthUI();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //VERSION 2 PLAYERS
        if (gameObject.tag == ("Player1"))
        {
            if (collision.gameObject.tag.Equals("Bullet"))
            {
                Debug.Log("colision");
                Instantiate(explosionParticles, collision.gameObject.transform.position, Quaternion.identity);
                TakeDamage();
            }
        }
        else if (gameObject.tag == ("Player2"))
        {
            if (collision.gameObject.tag.Equals("Bullet"))
            {
                Debug.Log("colision");
                Instantiate(explosionParticles, collision.gameObject.transform.position, Quaternion.identity);
                TakeDamage();
            }
        }

        //VERSION VS COMPUTER
        else if (gameObject.tag == ("Player1"))
        {
            if (collision.gameObject.tag.Equals("Bullet"))
            {
                Debug.Log("colision");
                Instantiate(explosionParticles, collision.gameObject.transform.position, Quaternion.identity);
                TakeDamage();
            }
        }
        else if (gameObject.tag == ("Computer") && GameObject.FindGameObjectWithTag("Player1").GetComponent<TankController>().enabled == true)
        {
            if (collision.gameObject.tag.Equals("Bullet"))
            {
                Debug.Log("colision");
                Instantiate(explosionParticles, collision.gameObject.transform.position, Quaternion.identity);
                TakeDamage();
            }
        }
    }

    public void TakeDamage() {

        _currentHealth -= _damage;
        SetHealthUI();

        if (_currentHealth <= 0f) {
            OnDeath();
        }
    }

    public void TakeHealth()
    {
        _currentHealth += 40.0f;
        SetHealthUI();

        if (_currentHealth >= 100f)
        {
            _currentHealth = 100f;
        }
    }

    private void OnDeath() {

        explosionParticles.transform.position = transform.position;
        explosionParticles.gameObject.SetActive(true);
        explosionParticles.Play();

        GameObject.FindGameObjectWithTag("Canvas").GetComponent<TurnPlayer>().WinnerGame();

        StartCoroutine(DelayDeath(explosionParticles));
    }

    IEnumerator DelayDeath(ParticleSystem explosionParticles) {
        yield return new WaitForSeconds(explosionParticles.main.duration);
        Destroy(explosionParticles.gameObject);
        gameObject.SetActive(false);
    }

    public void SetHealthUI() {

        slider.value = _currentHealth;
        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, _currentHealth / _startingHealth);
    }
}
