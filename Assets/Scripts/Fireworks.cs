// Copyright Sylvain Michel René Nowé 2026

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

using static Common;

public class Fireworks : MonoBehaviour
{
    public GameObject fireworks;
    public static bool run = false;

    public AudioClip explosionSFX;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (run && Random.Range(0, 15) == 0)
        {
            GameObject newFireworks = Instantiate(fireworks);
            newFireworks.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(4f, 14f), -5f);

            ParticleSystem particles = newFireworks.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = particles.main;
            particles.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            main.startColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            main.startSize = Random.Range(0.1f, 0.2f);
            main.startSpeed = Random.Range(5f, 10f);

            particles.emission.SetBurst(0, new ParticleSystem.Burst(0f, new ParticleSystem.MinMaxCurve(Random.Range(100, 250))));

            PlayClipWithPan(explosionSFX, newFireworks.transform.position.x / 5f);

            Destroy(newFireworks, main.duration);
        }
    }
}
