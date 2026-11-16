using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

using static Common;
using static Bonus;

public class Paddle : MonoBehaviour
{
    public GameObject ball;

    public float InitialSpeed = 10.0f;

    private float inputX = Screen.width * 0.5f, lastX = 0f;

    private Rigidbody rb;

    public static int breakedBricks, wreckage, numBalls, currentLevel, money;

    public TextMeshPro stats, coinsCount;

    public AudioClip[] glassSFX;
    public AudioClip wallSFX, paddleSFX, metalSFX, catchSFX, shootSFX;

    public GameObject safetyBar;

    double startTime;

    public GameObject leftSphere, rightSphere;

    private float size;

    public bool magnet;

    public GameObject spikeBall;

    public GameObject bomb;


    void Start()
    {
        //magnet = true;

        rb = GetComponent<Rigidbody>();

        startTime = Time.time;
        currentLevel = 1;
        money = 0;

        float horizontalFoV = 60.0f;

        float halfWidth = Mathf.Tan(0.5f * horizontalFoV * Mathf.Deg2Rad);

        float ratio = Mathf.Max((Screen.height / Screen.width) - (4f / 3f), 0f);

        Camera.main.fieldOfView = horizontalFoV * (1f + ratio * 0.25f);

        SetCapsuleSize(1f);

        inputX = Screen.width * 0.5f;
    }

    void SetCapsuleSize(float s)
    {
        size = s;
        transform.localScale = new Vector3(transform.localScale.x, size, transform.localScale.z);
        leftSphere.transform.localScale = new Vector3(1f, 0.5f / size, 1f);
        rightSphere.transform.localScale = new Vector3(1f, 0.5f / size, 1f);
    }

    public void Stretch(float d)
    {
        float startSize = size;
        float targetSize = Mathf.Clamp(startSize + d, 1f, 8f);
        SmoothAction action = (float t) => { SetCapsuleSize(startSize + (targetSize - startSize) * t); };
        StartCoroutine(SmoothCoroutine(action, 1f));
    }

    public void PlayGlassSFX(float pan)
    {
        PlayClipWithPan(glassSFX[Random.Range(0, 7)], pan);
    }

    public void PlayWallSFX(float pan)
    {
        PlayClipWithPan(wallSFX, pan);
    }

    public void PlayPaddleSFX(float pan)
    {
        PlayClipWithPan(paddleSFX, pan);
    }

    public void PlayMetalSFX(float pan)
    {
        PlayClipWithPan(metalSFX, pan);
    }

    public void PlayCatchSFX(float pan)
    {
        PlayClipWithPan(catchSFX, pan);
    }

    void Update()
    {
        spikeBall.transform.localRotation = Quaternion.Euler(Time.time * 60f, Time.time * 90f, Time.time * 120f);

        bomb.transform.localRotation = Quaternion.Euler(0f, Time.time * 180f, 0f);

        stats.text = $"Level: { currentLevel }\nPassed Time: { (Time.time - startTime).ToString("n0") }\nLaunched balls: { numBalls }\nWrecked bricks: {breakedBricks}\nWreckage count: {wreckage}";

        coinsCount.text = "x" + money;

        //Debug.Log("Update() " + Input.mousePosition.x);

        if (Input.touchSupported && Input.touchCount == 1)
        {
            inputX = Input.touches[0].position.x;
        }

        if (Input.mousePresent)
        {
            inputX = Input.mousePosition.x;
        }

        lastX = (inputX / Screen.width - 0.5f) * 12f;
        lastX = Mathf.Clamp(lastX, -4.5f + transform.localScale.x * 0.5f, 4.5f - transform.localScale.x * 0.5f);

        
        if ((Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Began)
          || Input.GetMouseButtonDown(0))
        {
            if (Fireworks.run)
            {
                Fireworks.run = false;
                
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Ball")) { Destroy(obj); }
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Metal Brick")) { Destroy(obj); }
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Wreckage")) { Destroy(obj); }
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Bonus")) { Destroy(obj); }

                //wreckage = 0;
                //breakedBricks = 0;
                //numBalls = 0;

                currentLevel++;
                GameObject.Find("Level Loader").GetComponent<LevelLoader>().Start();
            }
            //Debug.Log("Fire!");

            FireBall();
        }

        if (GameObject.FindGameObjectsWithTag("Brick").Length == 0)
        {
            Fireworks.run = true;
        }
    }

    private void FixedUpdate()
    {
        float f = lastX - rb.position.x;

        rb.velocity = Vector3.zero;
        rb.AddForce(new Vector3(f * 100000f, 0f, 0f));
    }

    private IEnumerator BallsBurstCoroutine()
    {
        for (int i = 0; i < 8; i++)
        {
            FireBall();
            yield return new WaitForSeconds(Random.Range(0f, 0.25f));
        }
        
    }

    public void FireBallsBurst()
    {
        StartCoroutine(BallsBurstCoroutine());
    }

    private void FireBall()
    {
        var go = Instantiate(ball, transform.position + new Vector3(0f, 0.5f), transform.rotation);

        float angle = Random.Range(Mathf.PI * 0.25f, Mathf.PI * 0.75f);

        Vector3 velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * InitialSpeed;

        go.GetComponent<Rigidbody>().velocity = velocity * InitialSpeed;
        numBalls++;

        PlayClipWithPan(shootSFX, transform.position.x / 5.5f);
    }

    public void SetSafetyBar(bool b)
    {
        safetyBar.SetActive(b);
    }
}
