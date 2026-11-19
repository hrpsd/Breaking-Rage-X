// Copyright Sylvain Michel René Nowé 2026

using UnityEngine;
using System.Collections;

public class Common : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
			
	}

	public static void PlayClipWithPan(AudioClip clip, float pan)
	{
		pan = Mathf.Clamp(pan, -1f, 1f);
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position + new Vector3(Mathf.Cos(Mathf.PI * 0.5f * (1f - pan)), 0f, Mathf.Sin(Mathf.PI * 0.5f * (1f - pan))));
    }
}

