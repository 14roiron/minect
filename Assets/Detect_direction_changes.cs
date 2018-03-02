using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotVelocityArray {
	private Vector3[] _velocityArray;
	private Vector3[] _positionArray;
	private float[] _timeArray;
	private int _index;
	private int _size;
	private int _indexMaxY;
	private float _range;

	public RotVelocityArray(int size){
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
		_indexMaxY = 0;
	}

	public void append(Vector3 position) {
		var velocity = (position - _positionArray[_index]) / Time.deltaTime;

		if (++_index >= _size) _index -= _size;

	  _positionArray[_index] = position;
	  _velocityArray[_index] = velocity;
	  _timeArray[_index] = Time.time;
	}

	public int getNextI(int i) {
	//get the ith element, time based, with positionArray[i+1] is the oldest
		int position;
		position = _index + 1 + i;
		if(position >= _size) position -= _size;
		if(position < 0) position += _size;
		return position;
	}

	public bool getRangeMax(){
		float minY, maxY, timepourcentage;
		int indexMax;
		Vector3 currentPos;
		minY = float.PositiveInfinity;
		maxY = float.NegativeInfinity;
		indexMax = 0;

		for(int i = 0; i < _size; i++) {
			currentPos = _positionArray[getNextI(i)];
				if(maxY < currentPos.y) {
					maxY = currentPos.y;
					indexMax = i;
				}
				if(minY > currentPos.y) {
					minY = currentPos.y;
				}
		}
		_range = maxY - minY;
		_indexMaxY = indexMax;

		 //get the max position
		timepourcentage = (_timeArray[getNextI(indexMax)] - _timeArray[getNextI(0)]) / (_timeArray[getNextI(_size-1)] - _timeArray[getNextI(0)]);
		if(_range > 0.5 && timepourcentage > 0.4 && timepourcentage < 0.85 ) return true;
		else return false;
	}

	public Vector3 getPreviousVelocity()
	{
		Vector3 mean;
		mean = new Vector3 ();
		for (int i = 1 + _indexMaxY; i > _indexMaxY - 10 - 1; i--) mean += _velocityArray [getNextI(i)];
		return  mean / 10;
	}

	public Vector3 getMaxPosition() {
		return _positionArray[getNextI(_indexMaxY)]
	}
}


public class Detect_direction_changes : MonoBehaviour {

	public bool changeDetected;
	public GameObject particles;
	public Transform handTransfrom;
	public float pauseTime;

	RotVelocityArray rotArray;
	/*
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
	*/

	private GameObject currentTrail;
	private TrailRenderer currentTrailRenderer;

	List<GameObject> ListOfTrail;

	// Use this for initialization
	void Start () {
		changeDetected = false;

		rotArray = new RotVelocityArray();
		/*
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
		*/

		//create trail list
		ListOfTrail = new List<GameObject>();
	}

	// Update is called once per frame
	void Update () {
		rotArray.append(gameObject.transform.position)
		/*
    var currentVelocity = (gameObject.transform.position - oldPosition) / Time.deltaTime;
		oldVelocity = currentVelocity;
		oldPosition = gameObject.transform.position;

        //update the position array
		if (++indice >= size)
			indice -= size;
        positionArray[indice] = gameObject.transform.position;
        velocityArray[indice] = currentVelocity;
        timeArray[indice] = Time.time;
		*/

		if (changeDetected) {
			if (Time.time > pauseTime + lastChangeTime) {
				changeDetected = false;
				particles.SetActive (false);
			}
		}
		else {
			gameObject.transform.position = handTransfrom.position;

			//if(GetRangeMax()){
			if(rotArray.getRangeMax()){
				changeDetected = true;
				lastChangeTime = Time.time;
				particles.SetActive (true);
				particles.transform.position = rotArray.getMaxPosition();
				//particles.transform.position = positionArray[getIn(indiceMaxY)]; //gameObject.transform.position;
				particles.transform.rotation = Quaternion.LookRotation(rotArray.getPreviousVelocity().normalized);
				//particles.transform.rotation = Quaternion.LookRotation(getPreviousVelocity().normalized);//(currentVelocity.normalized);

				currentTrail = Instantiate (gameObject);
				ListOfTrail.Add(currentTrail);
				//currentTrail.transform.SetParent = null;
				currentTrailRenderer = currentTrail.GetComponent<TrailRenderer>();
				currentTrailRenderer.time = 50;
				currentTrail.GetComponent<Detect_direction_changes>().enabled = false;
				currentTrail.GetComponentInChildren<ParticleSystem> ().Clear ();
				currentTrail.GetComponentInChildren<ParticleSystem> ().enableEmission = false;
			}
		}
	}
	/*
    int getIn(int i)//get the ith element, time based, with positionArray[i+1] is the oldest
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
            currentPos = positionArray[getIn(i)];
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
        timepourcentage= (timeArray[getIn(indiceMax)] - timeArray[getIn(0)])/(timeArray[getIn(size-1)] -timeArray[getIn(0)] ); //get the max position
    if(range>0.5 && timepourcentage>0.4 && timepourcentage<0.90 )
        return true;
    else
        return false;

	}
	Vector3 getPreviousVelocity()
	{
		Vector3 mean;
		mean = new Vector3 ();
		for (int i = 1+indiceMaxY; i > indiceMaxY-10-1; i--) {
			mean += velocityArray [getIn( i)];
		}
		//mean.y = -mean.y;
		return  mean / 10;

	}
	*/
}
