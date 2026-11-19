// Copyright Sylvain Michel René Nowé 2026

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Common;
using static Bonus;
using static UnityEngine.GraphicsBuffer;

public class Coin : MonoBehaviour
{
    public Texture[] coinFrames;
    public bool falling;

    public AudioClip getCoinSFX;

    public static bool done;

    void Start()
    {
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Transform parent = transform.parent;

        if (falling)
        {
            GameObject paddle = GameObject.Find("Paddle");
            if (paddle.GetComponent<Paddle>().magnet)
            {
                Vector3 target = paddle.transform.position;
                if (GetComponent<Rigidbody>().velocity.y < 0f)
                {
                    GetComponent<Rigidbody>().AddForce((target - transform.position) / Vector3.Distance(target, transform.position) * 100f);
                }
            }
        }

        //parent.Rotate(new Vector3(0f, 10f, 0f));

        if (GetComponent<Rigidbody>().transform.position.y < -2f)
        {
            if (GetComponent<MeshCollider>() != null)
            {
                GetComponent<MeshCollider>().enabled = false;
            }
        }

        if (transform.position.y < -5f)
        {
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (falling && collision.gameObject.tag == "Paddle")
        {
            falling = false;

            PlayClipWithPan(getCoinSFX, transform.position.x / 5.5f);

            Paddle.money++;


            Vector3 origin = transform.position;
            Vector3 target = GameObject.Find("Gold Dock").transform.position;
            Vector3 scale = transform.localScale;

            SmoothAction moveAction = (float f) =>
            {    
                transform.position = origin + (target - origin) * f;
                transform.localScale = scale * (1f + Mathf.Sin(f * Mathf.PI) * 0.75f);
                if (f == 1f)
                {
                    Destroy(gameObject);
                }
            };

            StartCoroutine(SmoothCoroutine(moveAction, 0.5f));

            
        }
    }

    private void OnDestroy()
    {
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        Destroy(gameObject);
    }
}