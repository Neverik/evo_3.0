using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

	public Transform ball;
	public float t;
	public Vector3 offset;

	void Start () {
		offset = transform.position - GameObject.Find ("Car").transform.position;
	}

	void Update () {
		Scientist scientist = GameObject.FindObjectOfType<Scientist> ();
		if (scientist.Find(GameObject.FindObjectOfType<Inner>().gameObject, "Racer") != null) {
			ball = scientist.Find(GameObject.FindObjectOfType<Inner>().gameObject, "Racer").transform;
			transform.position = Vector3.Lerp (transform.position, ball.position + offset, t);
		}
	}
}
