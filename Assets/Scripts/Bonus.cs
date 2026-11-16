using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

using static Bonus.BonusStatus;

public class Bonus : MonoBehaviour
{
    public static string[] types = { "BB", "FF", "SB", "LB", "MG", "PE", "FL", "MT" };
    public static string[] labels = { "Balls Burst", "Force Field", "Safety Bar", "Laser Ball", "Magnifying Glass", "Paddle Enlargement", "Flush", "Magnet" };
    static float[] durations = { 1f, 5f, 5f, 5f, 5f, 5f, 5f, 5f };
    public static int[] levels = { 1, 1, 1, 1, 1, 1, 1, 1 };

    private int type;
    private float duration;

    public enum BonusStatus { FALLING, DOCKED, ACTIVE, GONE }
    BonusStatus status = FALLING;

    static GameObject lastDocked, lastActive;
    public GameObject previousDocked, previousActive;
    public TextMeshPro label;

    public Material whiteBall, redBall;

    void Start()
    {
        status = FALLING;

        //SetType(Random.Range(0, types.Length));
    }

    public void SetType(int type)
    {
        this.type = type;
        duration = durations[type];
        label.text = labels[type];
        SetText(types[type]);
    }

    void SetText(string s)
    {
        Transform text1 = transform.Find("Text 1");
        text1.GetComponent<TextMeshPro>().text = s;
        text1.GetComponent<WrappedText>().Init();

        Transform text2 = transform.Find("Text 2");
        text2.GetComponent<TextMeshPro>().text = s;
        text2.GetComponent<WrappedText>().Init();
    }

    private void FixedUpdate()
    {
        Vector3 origin = transform.position;
        Vector3 target = Vector3.zero;

        switch (status)
        {
            case FALLING:
                GameObject paddle = GameObject.Find("Paddle");
                if (paddle.GetComponent<Paddle>().magnet)
                {
                    Vector3 targett = paddle.transform.position;
                    if (GetComponent<Rigidbody>().velocity.y < 0f)
                    {
                        GetComponent<Rigidbody>().AddForce((targett - transform.position) / Vector3.Distance(targett, transform.position) * 100f);
                    }
                }
                //transform.Translate(new Vector3(0f, -0.1f));
                break;

            case DOCKED:
                target = previousDocked.transform.position + new Vector3(1.5f, 0);
                break;

            case ACTIVE:
                target = previousActive.transform.position + new Vector3(0f, -1.25f);
                break;

            case GONE:
                target = new Vector3(-4.1f, 20f);
                break;
        }

        if (target != Vector3.zero)
        {
            GetComponent<Rigidbody>().velocity = (target - origin) * 3f;
        }

        transform.Rotate(new Vector3(0f, 5f, 0f));

        if (transform.position.y < -5f || transform.position.y > 19f)
        {
            Destroy(gameObject);
        }

        label.gameObject.transform.rotation = Quaternion.Euler(0f, transform.rotation.y, 0f);
    }

    void Activate()
    {
        if (types[type] == "FF")
        {
            BreakableSurface.forceField = levels[type];
        }

        if (types[type] == "BB")
        {
            GameObject.Find("Paddle").GetComponent<Paddle>().FireBallsBurst();
        }

        if (types[type] == "SB")
        {
            GameObject.Find("Paddle").GetComponent<Paddle>().SetSafetyBar(true);
        }

        if (types[type] == "LB")
        {
            GameObject[] bricks = GameObject.FindGameObjectsWithTag("Brick");
            foreach (GameObject brick in bricks)
            {
                brick.GetComponent<MeshCollider>().isTrigger = true;
            }

            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
            foreach (GameObject ball in balls)
            {
                ball.GetComponent<MeshRenderer>().material = redBall;
                ball.GetComponent<Projectile>().whiteTrail.gameObject.SetActive(false);
                ball.GetComponent<Projectile>().redTrail.gameObject.SetActive(true);
            }
        }

        if (types[type] == "MG")
        {
            GameObject[] bricks = GameObject.FindGameObjectsWithTag("Brick");
            foreach (GameObject brick in bricks)
            {
                brick.GetComponent<BreakableSurface>().label.gameObject.SetActive(true);
            }
        }

        if (types[type] == "PE")
        {
            GameObject.Find("Paddle").GetComponent<Paddle>().Stretch(0.5f);
        }

        if (types[type] == "FL")
        {
            Physics.IgnoreLayerCollision(7, 12, true);
        }

        if (types[type] == "MT")
        {
            GameObject.Find("Paddle").GetComponent<Paddle>().magnet = true;
        }
    }

    void DeActivate() {
        if (types[type] == "FF")
        {
            BreakableSurface.forceField = 0f;
        }

        if (types[type] == "SB")
        {
            GameObject.Find("Paddle").GetComponent<Paddle>().SetSafetyBar(false);
        }

        if (types[type] == "LB")
        {
            GameObject[] bricks = GameObject.FindGameObjectsWithTag("Brick");
            foreach (GameObject brick in bricks)
            {
                brick.GetComponent<MeshCollider>().isTrigger = false;
            }

            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
            foreach (GameObject ball in balls)
            {
                ball.GetComponent<MeshRenderer>().material = whiteBall;
                ball.GetComponent<Projectile>().whiteTrail.gameObject.SetActive(true);
                ball.GetComponent<Projectile>().redTrail.gameObject.SetActive(false);
            }
        }

        if (types[type] == "MG")
        {
            GameObject[] bricks = GameObject.FindGameObjectsWithTag("Brick");
            foreach (GameObject brick in bricks)
            {
                brick.GetComponent<BreakableSurface>().label.gameObject.SetActive(false);
            }
        }

        if (types[type] == "PE")
        {
            GameObject.Find("Paddle").GetComponent<Paddle>().Stretch(-0.5f);
        }

        if (types[type] == "FL")
        {
            Physics.IgnoreLayerCollision(7, 12, false);
        }

        if (types[type] == "MT")
        {
            GameObject.Find("Paddle").GetComponent<Paddle>().magnet = false;
        }
    }

    void Update()
    {
        Vector3 position = Vector3.zero;

        if (Input.mousePresent && Input.GetMouseButtonDown(0))
        {
            position = Input.mousePosition;
        }

        if (Input.touchSupported && Input.touchCount == 1)
        {
            Touch touch =  Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                position = touch.position;
            }
        }

        if (position != Vector3.zero)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(position);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    if (status == DOCKED)
                    {
                        Activate();

                        Input.ResetInputAxes();

                        GameObject[] docked = GameObject.FindGameObjectsWithTag("Docked");
                        foreach (GameObject obj in docked)
                        {
                            Bonus bonus = obj.GetComponent<Bonus>();
                            if (bonus.previousDocked == gameObject)
                            {
                                bonus.previousDocked = previousDocked;
                            }
                        }

                        if (lastActive == null)
                        {
                            lastActive = GameObject.Find("Active Bonus Dock");
                        }

                        previousActive = lastActive;

                        lastActive = gameObject;

                        status = ACTIVE;
                        tag = "Active";
                        label.gameObject.SetActive(true);

                        if (lastDocked == gameObject)
                        {
                            lastDocked = previousDocked;
                        }

                        GetComponent<SphereCollider>().enabled = false;

                        StartCoroutine(DelayAndDeath());
                    }
                }
            }
        }
    }

    IEnumerator DelayAndDeath()
    {
        yield return new WaitForSeconds(duration);

        status = GONE;

        GameObject[] active = GameObject.FindGameObjectsWithTag("Active");
        foreach (GameObject obj in active)
        {
            Bonus bonus = obj.GetComponent<Bonus>();
            if (bonus.previousActive == gameObject)
            {
                bonus.previousActive = previousActive;
            }
        }

        if (lastActive == gameObject)
        {
            lastActive = previousActive;
        }

        DeActivate();
    }

    public void Catch()
    {
        if (status == FALLING)
        {
            gameObject.layer = 14;

            //transform.position = sphere.transform.position;
            //sphere.transform.localPosition = Vector3.zero;

            GetComponent<Rigidbody>().useGravity = false;
            //Destroy(GetComponent<Rigidbody>());

            status = DOCKED;

            if (lastDocked == null)
            {
                lastDocked = GameObject.Find("Bonus Dock");
            }

            previousDocked = lastDocked;

            lastDocked = gameObject;

            tag = "Docked";

            //GameObject.Find("Paddle").GetComponent<Paddle>().PlayCatchSFX(transform.position.x / 5.5f);

            /*SmoothAction action = (float t) => {
                transform.position = transform.position + (previous.transform.position - transform.position) * t;
            };*/
            //StartCoroutine(SmoothCoroutine(action, 1f));
        }
    }

    public static IEnumerator SmoothCoroutine(SmoothAction action, float delay)
    {
        float startTime = Time.time;

        AnimationCurve easeInOut = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        while (Time.time - startTime < delay)
        {
            float t = easeInOut.Evaluate((Time.time - startTime) / delay);
            action(t);
            yield return null;
        }
        action(1f);
    }

    public delegate void SmoothAction(float f);

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.tag == "Paddle")
        {
            Catch();
        }
    }
}
