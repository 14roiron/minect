using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.AnimatedLineRenderer;

/**
 * Main node of the state tree.
 */
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
	//get some debug infos about depth and number of children of the tree, only use for debug
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
	}
	public void addParent(Node Parent)
	{
		this.Parent = Parent;
		if (Parent == null)
			return;
		this.Parent.Children.Add(this);
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
	public enum TreeType {
		Version1,
		Version2
	}

	public TreeType treeType;
	public float turnAngle = 25.0f;
	private float branchLength = 0.5f;
	[Range(0, 4)]
	public int totalIterations = 2;

	private string encodedTree;
	private Dictionary<char, string> rules = new Dictionary<char, string> ();

	private Stack<TransformInfo> transformStack = new Stack<TransformInfo> ();
	private Stack<Node> nodeStack = new Stack<Node> ();
	private Node NodeToDraw;
	public GameObject AnimatedLine;
	List<List<Vector3>> MainPointsList = new List<List<Vector3>>();


	// Use this for initialization
	void Start () {
		if (treeType == TreeType.Version1) {
			rules.Add ('F', "FF+[+F-F-F]-[-F+F+F]");
			encodedTree = "F";
		}
		else if (treeType == TreeType.Version2) {
			rules.Add ('F', "FF+[+F-F-F]-[-F+F++F]");
			encodedTree = "F";
//			rules.Add ('X', "F[-X][X]F[-X]+FX");
//			rules.Add ('F', "FF");
		}

		GenerateString ();
		NodeToDraw = new Node (new Vector3 (0.0f, 0.0f, 0.0f), null);
		StartCoroutine (DrawTree ());
	}

	// Update is called once per frame
	void Update () {

	}

	void GenerateString () {
		for (int i = 0; i < totalIterations; i++) {
			string newString = "";
			foreach (char c in encodedTree) {
				if (rules.ContainsKey (c))
					newString += rules [c];
				else
					newString += c;
			}
			encodedTree = newString;
		}
	}

	IEnumerator DrawTree() {
		Debug.Log ("Drawing in coroutine");
		Node currentNode = NodeToDraw;
		foreach (char c in encodedTree) {
			if (c == 'F') {
				Vector3 initialPosition = transform.position;
				transform.Translate (Vector3.up * branchLength);
				//Debug.DrawLine (initialPosition, transform.position, Color.white, 100000f, false);
				currentNode.EndNode = transform.position;
				currentNode = new Node (transform.position, currentNode);
				//yield return new WaitForSeconds (pauseTime/10000);
			}
			else if (c == '+')
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
		//lineraze the node tree
		initDrawTreeLines ();
		//Draw it
		DrawTreeLines ();
		yield return 0;

	}

	/*
	 * Init the reccursive function to create a 
	 * linearization of the tree
	 * the aim is a create n list of vect3 points,
	 * each list end by a vertice
	 */ 
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

	/**
	 * 
	 * Reccursive function
	 *  to linearize, via deep copy
	 * the idea is that we have a line from 
	 * the bottom to each vertice, 
	 * so multiple lines can be superposed
	 */ 
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

		//clean loop
		foreach (List<Vector3> nodes in MainPointsList) {
			for(int i=2; i<nodes.Count;i++)
			{
				if (Vector3.Equals (nodes [i], nodes [i - 2])){ ///&& Vector3.Equals (nodes [i - 1].SourceNode, nodes [i].EndNode)) {
					nodes.RemoveAt(i-1); //test the linearization
					nodes.RemoveAt(i-2); //test the linearization
				}
			}
		}
	}
	/**
	 *  
	 * Draw the graph
	 */
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
					if (Vector3.Equals (v, vm1)) { //if the values are equals, not going to interpolate
						continue;
					}
					for (int i = 0; i < interpole; i++) {
						ALR.GetComponent<AnimatedLineRenderer> ().Enqueue ((vm1 + (v-vm1) * (((float)i)/((float)interpole))));//interpolate points
						Debug.Log((vm1 + (v-vm1) * (((float)i)/((float)interpole))));
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
