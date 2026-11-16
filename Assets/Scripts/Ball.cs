/**
 * Copyright 2019 Oskar Sigvardsson
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System.Collections;
using UnityEngine;



public class Projectile : MonoBehaviour {

    public TrailRenderer whiteTrail, redTrail;

    private void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.velocity = Vector3.Normalize(rb.velocity) * 10f;

        if (rb.transform.position.y < -5f)
        {
            Destroy(gameObject);
        }

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

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Metal Brick")
        {
            GameObject.Find("Paddle").GetComponent<Paddle>().PlayMetalSFX(transform.position.x / 5.5f);
        }

        if (collision.gameObject.tag == "Wall")
        {
            GameObject.Find("Paddle").GetComponent<Paddle>().PlayWallSFX(transform.position.x / 5.5f);
        }

        if (collision.gameObject.tag == "Paddle")
        {
            GameObject.Find("Paddle").GetComponent<Paddle>().PlayPaddleSFX(transform.position.x / 5.5f);
        }
    }
}
