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
	Vector3[] velocityArray;
	float[] timeArray;
    int indice;
    int size;

    float range;
    int indiceMaxY;

	// Use this for initialization
	void Start () {
		changeDetected = false;
		oldVelocity = new Vector3 (0, 0, 0);
		lastChangeTime = 0;
		oldPosition = new Vector3 (0, 0, 0);
        indice = 0;
        size = 50;
        positionArray = new Vector3[size];
		velocityArray = new Vector3[size];
        timeArray = new float[size];
        for (int i = 0; i < size; i++)
        {
            positionArray[i] = new Vector3(0.0f, 0.0f, 0.0f);      
			velocityArray[i] = new Vector3(0.0f, 0.0f, 0.0f);
            timeArray[i]=0.0f;
        }
	}
	
	// Update is called once per frame
	void Update () {
        var currentVelocity = (gameObject.transform.position - oldPosition) / Time.deltaTime;
		//Vector3 acceleration = ((currentVelocity - oldVelocity) / Time.deltaTime).magnitude;
		oldVelocity = currentVelocity;
		oldPosition = gameObject.transform.position;

        //update the position array
		if (++indice >= size)
			indice -= size;
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
			if(GetRangeMax()){
            //if (Time.time - lastChangeTime > 5 + pauseTime) {
				changeDetected = true;
				lastChangeTime = Time.time;
				particles.SetActive (true);
				particles.transform.position = gameObject.transform.position;
				particles.transform.rotation = Quaternion.LookRotation (currentVelocity.normalized);
			}
		}
        
	}
    int get(int i)//get the ith element, time based, with positionArray[i+1] is the oldest
    {
        int position;
        position = indice + 1 +i;
        if(position>=size)
            position -=size;
        return position;
    }
    bool GetRangeMax(){
        float minY;
        float maxY;
        int indiceMax;
        float timepourcentage;
        Vector3 currentPos;
		minY=float.PositiveInfinity;
		maxY=float.NegativeInfinity;
		indiceMax = 0;
        for(int i=0;i<size;i++)
        {
            currentPos = positionArray[get(i)];
            if(maxY<currentPos.y)
            {
                maxY=currentPos.y;
                indiceMax=i;
            }
            if(minY>currentPos.y)
            {
                minY=currentPos.y;
            }
        }
        range=maxY-minY;
        indiceMaxY = indiceMax;
        timepourcentage= (timeArray[get(indiceMax)] - timeArray[get(0)])/(timeArray[get(size-1)] -timeArray[get(0)] );

<<<<<<< HEAD
    if(range>1.5 && timepourcentage>0.7 && timepourcentage<0.8 )
=======
    if(range>5 && timepourcentage>0.7 && timepourcentage<0.8 )
>>>>>>> 72ed9e8a3c52876213329e9ba8efd40d309d6178
        return true;
    else
        return false;

}
}

