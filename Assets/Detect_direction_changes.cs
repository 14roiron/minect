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
    Vector3[] positionArray;
    int indice;
    int size;

    int range;
    int indiceMaxY;

	// Use this for initialization
	void Start () {
		changeDetected = false;
		oldVelocity = new Vector3 (0, 0, 0);
		lastChangeTime = 0;
		oldPosition = new Vector3 (0, 0, 0);
        indice = 0;
        size = 100;
        Vector3[] positionArray = new Vector3[size];
i       float[] velocityArray = new float[size];
        float[] timeArray = new float[size];
        for (int i = 0; i <= size; i++)
        {
            positionArray[i] = new Vector3(0.0f,0.0f,0.0f);      
            velocityArray[i] = 0.0;
            timeArray[i]=0.0;
        }
	}
	
	// Update is called once per frame
	void Update () {
        var currentVelocity = (gameObject.transform.position - oldPosition) / Time.deltaTime;
		//Vector3 acceleration = ((currentVelocity - oldVelocity) / Time.deltaTime).magnitude;
		oldVelocity = currentVelocity;
		oldPosition = gameObject.transform.position;

        //update the position array``
        indice = (++indice>=size) ? indice : 0;
        positionArray[indice] = gameObject.transform.position;
        velocityArray[indice] = currentVelocity;
        timeArray[indice] = Time.time;


		if (changeDetected) {
			if (Time.time > pauseTime + lastChangeTime) {
				changeDetected = false;
				particles.SetActive (false);
			}
		}
		else {
			gameObject.transform.position = handTransfrom.position;
			if (Time.time - lastChangeTime > 5 + pauseTime) {
				changeDetected = true;
				lastChangeTime = Time.time;
				particles.SetActive (true);
				particles.transform.position = gameObject.transform.position;
				particles.transform.rotation = Quaternion.LookRotation (currentVelocity.normalized);
			}
		}
        
	}
    Vector3 get(int i)//get the ith element, time based, with positionArray[i+1] is the oldest
    {
        int position;
        position = indice + 1 +i;
        if(position>=size)
            position -=size;
        return positionArray[position];
    }
    void GetRangeMax(){
        float minY;
        float maxY;
        float indiceMax;
        
}
}

