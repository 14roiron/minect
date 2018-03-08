using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Easy_trajectory : MonoBehaviour {
	public enum trajectoryTypes {
		None,
		Sin,
		Cos,
		Triangle
	}

	public trajectoryTypes xTrajectory;
	public float xFreq;
	public float xAmpli;
	public trajectoryTypes yTrajectory;
	public float yFreq;
	public float yAmpli;
	public trajectoryTypes zTrajectory;
	public float zFreq;
	public float zAmpli;

	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		float x, y, z = 0;
		x = xAmpli * getTrajectory (xTrajectory, xFreq);
		y = yAmpli * getTrajectory (yTrajectory, yFreq);
		z = zAmpli * getTrajectory (zTrajectory, zFreq);
		gameObject.transform.position = new Vector3 (x, y, z) + offset;
	}

	float getTrajectory(trajectoryTypes type, float freq) {
		if (type == trajectoryTypes.Cos) {
			return Mathf.Cos (2 * freq * Mathf.PI * Time.time);
		}
		if (type == trajectoryTypes.Sin) {
			return Mathf.Sin (2 * freq * Mathf.PI * Time.time);
		} 
		if (type == trajectoryTypes.Triangle) {
			float t = Time.time;
			return 4 * Mathf.Abs(t * freq - Mathf.Floor (t * freq - 1) - 1.5f) - 1;
		} 
		if (type == trajectoryTypes.None) {
			return 0;
		} else {
			return 0;
		}
	}
}
