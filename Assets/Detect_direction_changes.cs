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
	private bool isfull;

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
		isfull = false;
	}

	public void append(Vector3 position) {
		var velocity = (position - _positionArray[_index]) / Time.deltaTime;

		if (++_index >= _size) {
			_index -= _size;
			isfull = true;
		}

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
		if (!isfull)
			return false;
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
					
				}
				if(minY > currentPos.y) {
					minY = currentPos.y;
					indexMax = i;
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
		return _positionArray [getNextI (_indexMaxY)];
	}

	public Vector3 getMaxVelocity() {
		return _velocityArray [getNextI (_indexMaxY)];
	}
	public Vector3 getPlanProjection()
	{
		Vector3 maxPosition, MaxVelocity;
		maxPosition = getMaxPosition ();
		MaxVelocity = getPreviousVelocity ();

		float length;
		float dotNumerator;
		float dotDenominator;
		Vector3 vector;
		Vector3 intersection = Vector3.zero;
		Vector3 planePoint, linePoint, planeNormal, lineVec;
		planePoint = new Vector3 (0f, 0f, 0f);
		planeNormal = new Vector3 (0f,1f,0f);
		lineVec = MaxVelocity;
		linePoint = maxPosition;

		//public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)

			//calculate the distance between the linePoint and the line-plane intersection point
			dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
			dotDenominator = Vector3.Dot(lineVec, planeNormal);

			if (dotDenominator != 0.0f)
			{
				length = dotNumerator / dotDenominator;

				vector = lineVec.normalized * length;

				intersection = linePoint + vector;

				Debug.DrawLine (linePoint, linePoint + lineVec.normalized,Color.white, 100000f, false);
				Debug.DrawLine (linePoint, intersection,Color.green, 100000f, false);
				Debug.DrawLine (planePoint, planeNormal,Color.blue, 100000f, false);
				return vector;
			}

			else
				return Vector3.zero;
		



	}

}


public class Detect_direction_changes : MonoBehaviour {

	public bool changeDetected;
	public GameObject particles;
	public GameObject tree;
	public GameObject handTransfrom;
	public float pauseTime;

	RotVelocityArray rotArray;

	private GameObject currentTrail;
	private GameObject currentParticule;
	private TrailRenderer currentTrailRenderer;
	private Transform currentTransform;
	private float lastChangeTime;

	// Use this for initialization
	void Start () {
		changeDetected = false;
		rotArray = new RotVelocityArray(50);
		lastChangeTime = 0;

		lastChangeTime = 0;
	}

	// Update is called once per frame
	void Update () {
		rotArray.append (gameObject.transform.position);
		if (handTransfrom == null) {
			Debug.Log("sphere not defined", gameObject);
			return;

		}
		if (changeDetected) {
			if (Time.time > pauseTime + lastChangeTime) {
				changeDetected = false;
				particles.SetActive (false);
				gameObject.GetComponent<Detect_direction_changes>().enabled = false;


			}
		}
		else {

			gameObject.transform.position = handTransfrom.transform.position;

			if(rotArray.getRangeMax()){
				


				//currentTransform = Instantiate(gameObject.transform);
				//currentParticule = Instantiate(handTransfrom);
				currentTrail = Instantiate(gameObject);
				currentTrail.name = "Trail";
				//currentTrail.transform.position = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y,gameObject.transform.position.z);

				//ListOfTrail.Add(currentTrail);
				//Debug.Break();


				changeDetected = true;
				lastChangeTime = Time.time;
				particles.SetActive (true);
				Vector3 max = rotArray.getMaxPosition ();

				particles.transform.position=max;
				particles.transform.rotation = Quaternion.LookRotation(1* rotArray.getPreviousVelocity().normalized);

//				gameObject.GetComponent<LSystemGenerator>().enabled = true;
				currentTrailRenderer = gameObject.GetComponents<TrailRenderer> () [0];
				currentTrailRenderer.time = 50;
				currentTrailRenderer.autodestruct = true;
				//max.y = 0;//-max.z;
				max = max + rotArray.getPlanProjection ();

				tree.GetComponent<LSystem>().enabled = true;
				tree.transform.position=max;
				//tree.transform.rotation = Quaternion.LookRotation(rotArray.getPreviousVelocity().normalized);
				//Debug.Break();
			}
		}
	}
}
