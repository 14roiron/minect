using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Easy_trajectory : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position = new Vector3 (Mathf.Sin (2 * Time.time), Mathf.Sin (3 * Time.time), 0);
	}
}
