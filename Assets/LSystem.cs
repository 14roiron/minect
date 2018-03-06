using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.AnimatedLineRenderer;

public class Node{
	public Vector3 SourceNode;
	public Vector3 EndNode;
	public Node Parent;
	public List<Node> Children;

	public Node(Vector3 SourceNode, Node Parent)
	{
		copyVect (SourceNode, this.SourceNode);
		this.addParent(Parent);
		Children = new List<Node> ();
	}

	public int Depth(){
		if (Children.Count == 0)
			return 1;
		else {
			int max = 0;
			int localDepth;
			foreach ( Node element in Children) {
				max = ((localDepth = element.Depth ()) > max) ? localDepth : max;
			}
			return max;
		}
		return -1;
	}
	public void addParent(Node Parent)
	{
		this.Parent = Parent;
		if (Parent == null)
			return;
		this.Parent.Children.Add(this);
	}
	public static void copyVect(Vector3 source, Vector3 dest)
	{
		dest = new Vector3(source.x,source.y,source.z);
	}
}


public class TransformInfo {
	public Vector3 position;
	public Quaternion rotation;

	public TransformInfo () {}
	public TransformInfo (Vector3 pos, Quaternion rot) {
		position = pos;
		rotation = rot;
	}
}

//[RequireComponent(typeof(AnimatedLineRenderer))]
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
	private Stack<Node> nodeStack = new Stack<Node> ();
	private Node NodeToDraw;
	public GameObject AnimatedLine;


	// Use this for initialization
	void Start () {
//		rules.Add ('A', "AB");
//		rules.Add ('B', "A");

		rules.Add('F', "FF+[+F-F-F]-[-F+F+F]");

//		rules.Add ('X', "F[-X][X]F[-X]+FX");
//		rules.Add ('F', "FF");

		result = axiom;
		GenerateString ();
		NodeToDraw = new Node (new Vector3 (0.0f, 0.0f, 0.0f), null);
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
		Node currentNode = NodeToDraw;
		foreach (char c in result) {
			if (c == 'F') {
				Vector3 initialPosition = transform.position;
				transform.Translate (Vector3.up * branchLength);
				//Debug.DrawLine (initialPosition, transform.position, Color.white, 100000f, false);
				//Node.copyVect (initialPosition, currentNode.SourceNode);
				Node.copyVect (transform.position, currentNode.EndNode);
		
				//yield return new WaitForSeconds (pauseTime);
			} else if (c == '+')
				transform.Rotate (Vector3.right * turnAngle);
			else if (c == '-')
				transform.Rotate (Vector3.right * -turnAngle);
			else if (c == '[') {
				transformStack.Push (new TransformInfo (transform.position, transform.rotation));
				nodeStack.Push (new Node (transform.position, currentNode));
			}
			else if (c == ']') {
				TransformInfo ti = transformStack.Pop ();
				transform.position = ti.position;
				transform.rotation = ti.rotation;
				currentNode = nodeStack.Pop ();
			}
		}
		initDrawTreeLines ();
	}
	void initDrawTreeLines() {
		AnimatedLineRenderer lineRenderer = 
			AnimatedLine.GetComponent<AnimatedLineRenderer>();

		lineRenderer.Enqueue(NodeToDraw.SourceNode);
		lineRenderer.Enqueue(NodeToDraw.EndNode);
		DrawTreeLines (NodeToDraw.Children, lineRenderer);

	}
	void DrawTreeLines(List<Node> NodesToDraw,GameObject line)
	{
		int count = 0;
		foreach (Node node in NodesToDraw) {
			count++;

			GameObject ALR;
			if (count != NodesToDraw.Count) {
				ALR = Instantiate (line);
			} else {
				ALR = line;
			}

			ALR.GetComponent<AnimatedLineRenderer>().Enqueue(node.SourceNode);
			ALR.GetComponent<AnimatedLineRenderer>().Enqueue(node.EndNode);
			DrawTreeLines (node.Children, ALR);
		}
	}

}
