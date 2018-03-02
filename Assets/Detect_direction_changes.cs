using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotVelocityArray {
	private Vector3[] _velocityArray;
	private Vector3[] _positionArray;
	private float[] _time;
	private int _index;
	private int _size;
	private float _range;

	public RotVelocityArray(size){
		_size = size;
		_positionArray = new Vector3[size];
		_velocityArray = new Vector3[size];
		_timeArray = new float[size];
		for (int i = 0; i < size; i++)
		{
				_positionArray[i] = new Vector3(0.0f, 0.0f, 0.0f);
				_velocityArray[i] = new Vector3(0.0f, 0.0f, 0.0f);
				_timeArray[i] = 0.0f;
		}
		_index = 0;
	}

	public append(position) {
		var velocity = (position - _positionArray[_index]) / Time.deltaTime;

		if (++indice >= size) indice -= size;

    positionArray[_index] = position;
    velocityArray[_index] = velocity;
    timeArray[_index] = Time.time;
	}

	public int getIn(int i) {
	//get the ith element, time based, with positionArray[i+1] is the oldest
		int position;
		position = _index + 1 + i;
		if(position >= size) position -= _size;
		if(position < 0) position += _size;
		return position;
	}

	public bool GetRangeMax(){
		float minY, maxY, timepourcentage;
		int indexMax;
		Vector3 currentPos;
		minY = float.PositiveInfinity;
		maxY = float.NegativeInfinity;
		indexMax = 0;

		for(int i = 0; i < size; i++) {
			currentPos = positionArray[get(i)];
				if(maxY < currentPos.y) {
					maxY = currentPos.y;
					indexMax = i;
				}
				if(minY > currentPos.y) {
					minY = currentPos.y;
				}
		}
		range = maxY-minY;
		indiceMaxY = indexMax;
		timepourcentage = (_timeArray[get(indexMax)] - _timeArray[get(0)]) / (_timeArray[get(size-1)] - _timeArray[get(0)]); //get the max position
		if(range>0.5 && timepourcentage>0.4 && timepourcentage<0.85 ) return true;
		else return false;
	}
}


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
    for (int i = 0; i < size; i++) {
			positionArray[i] = new Vector3(0.0f, 0.0f, 0.0f);
			velocityArray[i] = new Vector3(0.0f, 0.0f, 0.0f);
      timeArray[i]=0.0f;
    }
	}

	// Update is called once per frame
	void Update () {
    var currentVelocity = (gameObject.transform.position - oldPosition) / Time.deltaTime;
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
				particles.transform.position = positionArray[indiceMaxY]; //gameObject.transform.position;


				particles.transform.rotation = Quaternion.LookRotation (getPreviousVelocity().normalized);//(currentVelocity.normalized);
			}
		}

	}
    int get(int i)//get the ith element, time based, with positionArray[i+1] is the oldest
    {
        int position;
        position = indice + 1 +i;
        if(position>=size)
            position -=size;
		if(position<0)
			position +=size;
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
        timepourcentage= (timeArray[get(indiceMax)] - timeArray[get(0)])/(timeArray[get(size-1)] -timeArray[get(0)] ); //get the max position
    if(range>0.5 && timepourcentage>0.4 && timepourcentage<0.85 )
        return true;
    else
        return false;

	}
	Vector3 getPreviousVelocity()
	{
		Vector3 mean;
		mean = new Vector3 ();
		for (int i = 1+indiceMaxY; i > indiceMaxY-10-1; i--) {
			mean += velocityArray [get ( i)];
		}
		//mean.y = -mean.y;
		return  mean / 10;

	}

}
