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
		//copyVect (SourceNode, this.SourceNode);
		this.SourceNode = SourceNode;
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
			return max+1;
		}
		//return -1;
	}
	public int NumberOfNodes(){
		if (Children.Count == 0)
			return 1;
		else {
			int max = 0;
			//int localDepth;
			foreach ( Node element in Children) {
				max = max + element.NumberOfNodes ();
			}
			return max+1;
		}
		//return -1;
	}
	public void addParent(Node Parent)
	{
		this.Parent = Parent;
		if (Parent == null)
			return;
		this.Parent.Children.Add(this);
	}
//	public static void copyVect(Vector3 source, Vector3 dest)
//	{
//		dest = new Vector3(source.x,source.y,source.z);
//	}
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
	private int totalIterations = 2;
	private float drawTime = 1;

	public string result;
	private Dictionary<char, string> rules = new Dictionary<char, string> ();
	private Stack<TransformInfo> transformStack = new Stack<TransformInfo> ();
	private Stack<Node> nodeStack = new Stack<Node> ();
	private Node NodeToDraw;
	public GameObject AnimatedLine;
	List<List<Vector3>> MainPointsList = new List<List<Vector3>>();




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
		DrawTree ();



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
//		//remove the last ]
//		if (result [result.Length - 1] == ']') {
//			result = result.Remove (result.Length-1);
//		}
	}

	void DrawTree() {
		float pauseTime = drawTime / result.Length;
		Node currentNode = NodeToDraw;
		foreach (char c in result) {
			if (c == 'F') {
				Vector3 initialPosition = transform.position;
				transform.Translate (Vector3.up * branchLength);
				//Debug.DrawLine (initialPosition, transform.position, Color.white, 100000f, false);
				//Node.copyVect (initialPosition, currentNode.SourceNode);
				//Node.copyVect (transform.position, currentNode.EndNode);
				currentNode.EndNode=transform.position;
				currentNode = new Node (transform.position, currentNode);
		
				//yield return new WaitForSeconds (pauseTime/10000);
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
		Debug.Log (NodeToDraw.Depth());
		Debug.Log (NodeToDraw.NumberOfNodes());

		initDrawTreeLines ();
		DrawTreeLines ();

	}
	void initDrawTreeLines() {
		AnimatedLineRenderer lineRenderer = 
			AnimatedLine.GetComponent<AnimatedLineRenderer>();

		lineRenderer.Enqueue(NodeToDraw.SourceNode);
		lineRenderer.Enqueue(NodeToDraw.EndNode);
		MainPointsList.Add(new List<Vector3> ());
		MainPointsList [0].Add (NodeToDraw.SourceNode);
		MainPointsList [0].Add (NodeToDraw.EndNode);

		CreateTreeLines (NodeToDraw.Children, MainPointsList[0]);
	}
	void CreateTreeLines(List<Node> NodesToDraw,List<Vector3> pointList)
	{
		int count = 0;
		foreach (Node node in NodesToDraw) {
			count++;

			List<Vector3> currentPointsList;
			if (count != NodesToDraw.Count) {
				//deep copy of the liste
				currentPointsList = new List<Vector3>(pointList.Count);
				pointList.ForEach((item)=>
					{
						currentPointsList.Add(new Vector3(item.x,item.y,item.z));
					});
				MainPointsList.Add (currentPointsList);
				
			} else {
				currentPointsList = pointList;
			}

			currentPointsList.Add(node.SourceNode);
			currentPointsList.Add(node.EndNode);
			CreateTreeLines (node.Children, currentPointsList);
		}
	}
	void DrawTreeLines()
	{
		int count = 0;
		foreach (List<Vector3> pointsListe in this.MainPointsList) {
			

			GameObject ALR;
			if (count != this.MainPointsList.Count) {
				ALR = Instantiate (AnimatedLine,gameObject.transform);
			} else {
				ALR = AnimatedLine;
			}
			int c=0;
			for(c=0;c<pointsListe.Count;c++) {
				Vector3 v;
				Vector3 vm1;
				int interpole = 10;
				if (c != pointsListe.Count-1 && c != 0) {
					v = pointsListe [c];
					vm1 = pointsListe [c-1];
					for (int i = 0; i < interpole; i++) {
						ALR.GetComponent<AnimatedLineRenderer> ().Enqueue ((vm1 + (v-vm1) * (((float)i)/((float)interpole))));//interpolate points
					}
				}
				else if(c==1)
					ALR.GetComponent<AnimatedLineRenderer> ().Enqueue (pointsListe[0]);
				
			}
			count++;

		}
		return;
	}

}
