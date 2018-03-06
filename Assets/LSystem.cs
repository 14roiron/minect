using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TransformInfo {
	public Vector3 position;
	public Quaternion rotation;

	public TransformInfo () {}
	public TransformInfo (Vector3 pos, Quaternion rot) {
		position = pos;
		rotation = rot;
	}
}

public class LSystem : MonoBehaviour {
	private string axiom = "F";
	private float turnAngle = 25.0f;
	private float branchLength = 0.5f;
	//[Range(0,6)]
	private int totalIterations = 3;
	private float drawTime = 5;

	public string result;
	private Dictionary<char, string> rules = new Dictionary<char, string> ();
	private Stack<TransformInfo> transformStack = new Stack<TransformInfo> ();

	// Use this for initialization
	void Start () {
//		rules.Add ('A', "AB");
//		rules.Add ('B', "A");

		rules.Add('F', "FF+[+F-F-F]-[-F+F+F]");

//		rules.Add ('X', "F[-X][X]F[-X]+FX");
//		rules.Add ('F', "FF");

		result = axiom;
		GenerateString ();
		StartCoroutine(DrawTree ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void GenerateString () {
		for (int i = 0; i < totalIterations; i++) {
			string newString = "";
			foreach (char c in result) {
				if (rules.ContainsKey (c))
					newString += rules [c];
				else
					newString += c;
			}
			result = newString;
		}
	}

	IEnumerator DrawTree() {
		float pauseTime = drawTime / result.Length;
		foreach (char c in result) {
			if (c == 'F') {
				Vector3 initialPosition = transform.position;
				transform.Translate (Vector3.up * branchLength);
				Debug.DrawLine (initialPosition, transform.position, Color.white, 100000f, false);
				yield return new WaitForSeconds (pauseTime);
			} else if (c == '+')
				transform.Rotate (Vector3.right * turnAngle);
			else if (c == '-')
				transform.Rotate (Vector3.right * -turnAngle);
			else if (c == '[')
				transformStack.Push (new TransformInfo (transform.position, transform.rotation));
			else if (c == ']') {
				TransformInfo ti = transformStack.Pop ();
				transform.position = ti.position;
				transform.rotation = ti.rotation;
			}
		}
	}
}
