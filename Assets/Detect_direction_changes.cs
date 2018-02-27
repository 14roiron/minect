using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detect_direction_changes : MonoBehaviour {
	
	public bool changeDetected;
	public GameObject particles;
	public Transform handTransfrom;
	public float pauseTime;

	Vector3 oldVelocity;
	Vector3 oldPosition;
	float lastChangeTime;

	// Use this for initialization
	void Start () {
		changeDetected = false;
		oldVelocity = new Vector3 (0, 0, 0);
		lastChangeTime = 0;
		oldPosition = new Vector3 (0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (changeDetected) {
			if (Time.time > pauseTime + lastChangeTime) {
				changeDetected = false;
				particles.SetActive (false);
			}
		}
		else {
			gameObject.transform.position = handTransfrom.position;
			var currentVelocity = (gameObject.transform.position - oldPosition) / Time.deltaTime;
			//Vector3 acceleration = ((currentVelocity - oldVelocity) / Time.deltaTime).magnitude;
			oldVelocity = currentVelocity;
			oldPosition = gameObject.transform.position;
			if (Time.time - lastChangeTime > 5 + pauseTime) {
				changeDetected = true;
				lastChangeTime = Time.time;
				particles.SetActive (true);
				particles.transform.position = gameObject.transform.position;
				particles.transform.rotation = Quaternion.LookRotation (currentVelocity.normalized);
			}
		}
	}
}
