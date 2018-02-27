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
	public float xFreq = 1;
	public trajectoryTypes yTrajectory;
	public float yFreq = 1;
	public trajectoryTypes zTrajectory;
	public float zFreq = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float x, y, z = 0;
		x = getTrajectory (xTrajectory, xFreq);
		y = getTrajectory (yTrajectory, yFreq);
		z = getTrajectory (zTrajectory, zFreq);
		gameObject.transform.position = new Vector3 (x, y, z);
	}

	float getTrajectory(trajectoryTypes type, float freq) {
		if (type == trajectoryTypes.Cos) {
			return Mathf.Cos (2 * freq * Mathf.PI * Time.time);
		}
		if (type == trajectoryTypes.Sin) {
			return Mathf.Sin (2 * freq * Mathf.PI * Time.time);
		} 
		if (type == trajectoryTypes.Triangle) {
			return Mathf.Cos (2 * freq * Mathf.PI * Time.time);
		} 
		if (type == trajectoryTypes.None) {
			return 0;
		} else {
			return 0;
		}
	}
}
