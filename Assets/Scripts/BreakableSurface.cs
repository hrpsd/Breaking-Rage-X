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
using System.Collections.Generic;
using GK;
using TMPro;
using UnityEngine;


public class BreakableSurface : MonoBehaviour {
	public MeshFilter Filter     { get; private set; }
	public MeshRenderer Renderer { get; private set; }
	public MeshCollider Collider { get; private set; }
	public Rigidbody Rigidbody   { get; private set; }

	public PhysicMaterial lowBounceMaterial;

	public List<Vector2> Polygon;
	public float Thickness = 1.0f;
	public float MinBreakArea = 0.01f;
	public float MinImpactToBreak = 50.0f;

	float _Area = -1.0f;

	int age;

	public GameObject bonus;
	public GameObject[] coins;
    public GameObject gift;

    public static float forceField = 0f;

	public TextMeshPro label;
	private int bonusType;

	public float Area {
		get {
			if (_Area < 0.0f) {
				_Area = Geom.Area(Polygon);
			}

			return _Area;
		}
	}

	void Start() {
		age = 0;

		Reload();

		if (Random.Range(0, 50) == 0)
		{
			bonusType = Random.Range(0, Bonus.types.Length);
            label.text = Bonus.types[bonusType];
        }
    }

	public void Reload() {
		var pos = transform.position;

		if (Filter == null) Filter = GetComponent<MeshFilter>();
		if (Renderer == null) Renderer = GetComponent<MeshRenderer>();
		if (Collider == null) Collider = GetComponent<MeshCollider>();
		if (Rigidbody == null) Rigidbody = GetComponent<Rigidbody>();

		if (Polygon.Count == 0) {
			// Assume it's a cube with localScale dimensions
			var scale = 0.5f * transform.localScale;

			Polygon.Add(new Vector2(-scale.x, -scale.y));
			Polygon.Add(new Vector2(scale.x, -scale.y));
			Polygon.Add(new Vector2(scale.x, scale.y));
			Polygon.Add(new Vector2(-scale.x, scale.y));

			Thickness = 2.0f * scale.z;

			transform.localScale = Vector3.one;
		}

		var mesh = MeshFromPolygon(Polygon, 1f);
        var mesh2 = MeshFromPolygon(Polygon, 1.3f);

        Filter.sharedMesh = mesh;
		Collider.sharedMesh = mesh2;
	}

	void FixedUpdate() {
		if (gameObject.layer == LayerMask.NameToLayer("Wreakage"))
		{
			Rigidbody paddle = GameObject.Find("Paddle").GetComponent<Rigidbody>();

			Rigidbody rb = GetComponent<Rigidbody>();

			Vector3 delta = rb.position - paddle.position;

			delta /= (delta.sqrMagnitude * delta.sqrMagnitude);

            rb.AddForce(delta * 2000000f * Mathf.Pow(2, forceField));
		}

		var pos = transform.position;

		age++;
		if (pos.magnitude > 1000.0f) {
			DestroyImmediate(gameObject);
		}

            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (age > 5)
        {
            var pnt = other.transform.position;
            Break((Vector2)transform.InverseTransformPoint(pnt));
        }
    }

    void OnCollisionEnter(Collision coll) {
		if (age > 5 && coll.relativeVelocity.magnitude > MinImpactToBreak) {
			var pnt = coll.contacts[0].point;
			Break((Vector2)transform.InverseTransformPoint(pnt));
		}
	}

	static float NormalizedRandom(float mean, float stddev) {
		var u1 = UnityEngine.Random.value;
		var u2 = UnityEngine.Random.value;

		var randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
			Mathf.Sin(2.0f * Mathf.PI * u2);

		return mean + stddev * randStdNormal;
	}

	public void Break(Vector2 position) {
		var area = Area;
		if (area > MinBreakArea) {

			if (tag == "Brick")
			{
				GameObject.Find("Paddle").GetComponent<Paddle>().PlayGlassSFX(transform.position.x / 5.5f);
            }

            Paddle.breakedBricks++;

			if (label.text != "")
            {
                GameObject newBonus = Instantiate(bonus);
				newBonus.GetComponent<Bonus>().SetType(bonusType);
				newBonus.transform.position = transform.position;

				label.gameObject.SetActive(false);
            }
			else if (Random.Range(0, 30) == 0)
            {
                GameObject newCoin = Instantiate(coins[Random.Range(0, 3)]);
                newCoin.transform.position = transform.position;
                float torque = 1000000f;
                newCoin.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-torque, torque), Random.Range(-torque, torque), Random.Range(-torque, torque)), ForceMode.Impulse);
            }
            else if (Random.Range(0, 120) == 0)
            {
                GameObject newCoin = Instantiate(gift);
                bool b = Random.Range(0, 2) == 0;
                gift.transform.Find("Present_Box_Pattern_17").gameObject.SetActive(b);
                gift.transform.Find("Present_Box_Pattern_18").gameObject.SetActive(!b);

                newCoin.transform.position = transform.position;
                float torque = 1000000f;
                newCoin.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-torque, torque), Random.Range(-torque, torque), Random.Range(-torque, torque)), ForceMode.Impulse);
            }

                var calc = new VoronoiCalculator();
			var clip = new VoronoiClipper();

			var sites = new Vector2[Random.Range(4, 9)];

			for (int i = 0; i < sites.Length; i++) {
				var dist = Mathf.Abs(NormalizedRandom(0.5f, 1.0f/2.0f));
				var angle = 2.0f * Mathf.PI * Random.value;

				sites[i] = position + new Vector2(
						dist * Mathf.Cos(angle),
						dist * Mathf.Sin(angle));
			}

			var diagram = calc.CalculateDiagram(sites);

			var clipped = new List<Vector2>();

			for (int i = 0; i < sites.Length; i++) {
				clip.ClipSite(diagram, Polygon, i, ref clipped);

				if (clipped.Count > 0) {
						
                    var newGo = Instantiate(gameObject, transform.parent);

					newGo.transform.localPosition = transform.localPosition;
					newGo.transform.localRotation = transform.localRotation;

					var bs = newGo.GetComponent<BreakableSurface>();

					bs.Thickness = Thickness;
					bs.Polygon.Clear();
					bs.Polygon.AddRange(clipped);

					var childArea = bs.Area;

					var rb = bs.GetComponent<Rigidbody>();

					rb.useGravity = true;
					rb.mass = Rigidbody.mass * (childArea / area);

					float speed = Random.Range(5f, 20f) * Mathf.Sign(Random.Range(-1, 1f));
                    float angle = Random.Range(0f, 2.0f * Mathf.PI);

                    rb.velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
					rb.angularVelocity = new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), Random.Range(-20f, 20f));

					Paddle.wreckage++;

                    newGo.layer = LayerMask.NameToLayer("Wreckage");
					newGo.GetComponent<MeshCollider>().material = lowBounceMaterial;
					newGo.GetComponent<BreakableSurface>().MinImpactToBreak = 20000f;
					newGo.tag = "Wreckage";

					newGo.GetComponent<BreakableSurface>().label.gameObject.SetActive(false);
                }
			}

			Destroy(gameObject);
		}
	}

	static Mesh MeshFromPolygon(List<Vector2> polygon, float scale) {
		var count = polygon.Count;
		// TODO: cache these things to avoid garbage
		var verts = new Vector3[6 * count];
		var norms = new Vector3[6 * count];
		var tris = new int[3 * (4 * count - 4)];
		// TODO: add UVs

		var vi = 0;
		var ni = 0;
		var ti = 0;

		var ext = 0.25f * scale;

		// Top
		for (int i = 0; i < count; i++) {
			verts[vi++] = new Vector3(polygon[i].x * scale, polygon[i].y * scale, ext);
			norms[ni++] = Vector3.forward;
		}

		// Bottom
		for (int i = 0; i < count; i++) {
			verts[vi++] = new Vector3(polygon[i].x * scale, polygon[i].y * scale, -ext);
			norms[ni++] = Vector3.back;
		}

		// Sides
		for (int i = 0; i < count; i++) {
			var iNext = i == count - 1 ? 0 : i + 1;

			verts[vi++] = new Vector3(polygon[i].x * scale, polygon[i].y * scale, ext);
			verts[vi++] = new Vector3(polygon[i].x * scale, polygon[i].y * scale, -ext);
			verts[vi++] = new Vector3(polygon[iNext].x * scale, polygon[iNext].y * scale, -ext);
			verts[vi++] = new Vector3(polygon[iNext].x * scale, polygon[iNext].y * scale, ext);

			var norm = Vector3.Cross(polygon[iNext] - polygon[i], Vector3.forward).normalized;

			norms[ni++] = norm;
			norms[ni++] = norm;
			norms[ni++] = norm;
			norms[ni++] = norm;
		}


		for (int vert = 2; vert < count; vert++) {
			tris[ti++] = 0;
			tris[ti++] = vert - 1;
			tris[ti++] = vert;
		}

		for (int vert = 2; vert < count; vert++) {
			tris[ti++] = count;
			tris[ti++] = count + vert;
			tris[ti++] = count + vert - 1;
		}

		for (int vert = 0; vert < count; vert++) {
			var si = 2*count + 4*vert;

			tris[ti++] = si;
			tris[ti++] = si + 1;
			tris[ti++] = si + 2;

			tris[ti++] = si;
			tris[ti++] = si + 2;
			tris[ti++] = si + 3;
		}

		Debug.Assert(ti == tris.Length);
		Debug.Assert(vi == verts.Length);

		var mesh = new Mesh();


		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.normals = norms;

		return mesh;
	}
}
