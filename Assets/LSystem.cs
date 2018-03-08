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
	public static int totalNode(List<List<Vector3>> Mainlist)
	{
		int total=0;
		foreach (List<Vector3> list in Mainlist)
			total += list.Count;
		return total;
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
		Version2,
		Version3
	}

	public TreeType treeType;
	public float turnAngle = 25.0f;
	[Range(1, 4)]
	public int totalIterations = 2;
	public float branchLength = 0.5f;
	[Range(0, 1)]
	public float lenghtMultiplier = 1;

//	[Tooltip("The minimum distance that must be in between line segments (0 for infinite). Attempts to make lines with distances smaller than this will fail.")]
//	public float MinimumDistance = 0.0f;
//	[Tooltip("Seconds that each new line segment should animate with")]
//	public float SecondsPerLine = 0.1f;
//	[Tooltip("Start color for the line renderer since Unity does not provider a getter for this")]
//	public Color StartColor = Color.white;
//	[Tooltip("End color for the line renderer since Unity does not provide a getter for this")]
//	public Color EndColor = Color.white;
//	[Tooltip("Start line width")]
//	public float StartWidth = 2.0f;
//	[Tooltip("End line width")]
//	public float EndWidth = 2.0f;

	private string encodedTree;
	private Dictionary<char, string> rules = new Dictionary<char, string> ();

	public GameObject AnimatedLine;
	private Stack<TransformInfo> transformStack = new Stack<TransformInfo> ();
	private Stack<Node> nodeStack = new Stack<Node> ();
	private Node NodeToDraw;
	List<List<Vector3>> MainPointsList = new List<List<Vector3>>();

	// Use this for initialization
	void Start () {
//		AnimatedLine = new GameObject ();
//		AnimatedLine.transform.parent = gameObject.transform;
//		LineRenderer lr = AnimatedLine.AddComponent<LineRenderer>() as LineRenderer;
//		//lr.material = new Material (Shader.Find ("Particles/Additive"));
//		AnimatedLineRenderer alr = AnimatedLine.AddComponent<AnimatedLineRenderer>() as AnimatedLineRenderer;
//		alr.MinimumDistance = MinimumDistance;
//		alr.SecondsPerLine = SecondsPerLine;
//		alr.StartColor = StartColor;
//		alr.EndColor = EndColor;
//		alr.StartWidth = StartWidth;
//		alr.EndWidth = EndWidth;
//		alr.SortLayerName = "Default";
//		alr.OrderInSortLayer = 1;
		//AnimatedLine.SetActive (false);

		if (treeType == TreeType.Version1) {
			rules.Add ('F', "FF+[+F-F-F]-[-F+F+F]");
			encodedTree = "F";
		} else if (treeType == TreeType.Version2) {
			rules.Add ('X', "F[-X][X]F[-X]+FX");
			rules.Add ('F', "FF");
			encodedTree = "X";
		} else if (treeType == TreeType.Version3) {
			rules.Add ('F', "FF[+FF+F][-FF-F]");
			encodedTree = "F";
		}

		NodeToDraw = new Node (new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), null);
		GenerateString ();
		Debug.Log (encodedTree);

		StartCoroutine (DrawTree ());
		// DrawTree ();
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

	//void DrawTree () {
	IEnumerator DrawTree () {
		Node currentNode = NodeToDraw;
		float currentBranchLength = branchLength;
		TransformInfo originalTransform = new TransformInfo (gameObject.transform.position, gameObject.transform.rotation);
		foreach (char c in encodedTree) {
			if (c == 'F') {
				Vector3 initialPosition = transform.position;
				transform.Translate (Vector3.up * currentBranchLength);
				// Debug.DrawLine (initialPosition, transform.position, Color.white, 100000f, false);
				currentNode.EndNode = transform.position;
				currentNode = new Node (transform.position, currentNode);
			}
			else if (c == '+')
				transform.Rotate (Vector3.right * turnAngle);
			else if (c == '-')
				transform.Rotate (Vector3.right * -turnAngle);
			else if (c == '[') {
				currentBranchLength *= lenghtMultiplier;
				transformStack.Push (new TransformInfo (transform.position, transform.rotation));
				nodeStack.Push (new Node (transform.position, currentNode));
			}
			else if (c == ']') {
				currentBranchLength /= lenghtMultiplier;
				TransformInfo ti = transformStack.Pop ();
				transform.position = ti.position;
				transform.rotation = ti.rotation;
				currentNode = nodeStack.Pop ();
			}
		}
		Debug.Log ("Depth: " + NodeToDraw.Depth());
		Debug.Log ("Nodes: " + NodeToDraw.NumberOfNodes());

		gameObject.transform.position = originalTransform.position;
		gameObject.transform.rotation = originalTransform.rotation;

		//lineraze the node tree
		initDrawTreeLines ();
		//Draw it
		DrawTreeLines ();
		Debug.Log ("total points: " + Node.totalNode(MainPointsList));
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

		int debug1 = 0;
		//clean loop
		foreach (List<Vector3> nodes in MainPointsList) {
			for(int i=2; i<nodes.Count;i++)
			{
				if  (nodes [i] == nodes [i - 2]){ //&& Vector3.Equals (nodes [i - 1].SourceNode, nodes [i].EndNode)) {
					nodes.RemoveAt(i-1); //test the linearization
					nodes.RemoveAt(i-2); //test the linearization
					debug1++;
					i = 2;
				}
			}
		}
		Debug.Log ("points remove:" + debug1);
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
		Vector3 previousPoint;
		Vector3 ppreviousPoint;
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

			previousPoint = currentPointsList[currentPointsList.Count - 1];
			ppreviousPoint = currentPointsList[currentPointsList.Count - 2];
			if (Vector3.Equals (node.EndNode, ppreviousPoint)) { //loop detection
				currentPointsList.RemoveAt(currentPointsList.Count - 1);

			}
			else if(!Vector3.Equals(node.EndNode,previousPoint)){
					//only if it's different from the last point, to prevent two equals points in the list.
				if(!Vector3.Equals(node.SourceNode,previousPoint)){ 
						currentPointsList.Add(node.SourceNode);
					}
				currentPointsList.Add(node.EndNode);
			}

			CreateTreeLines (node.Children, currentPointsList);
		}

	}
	/**
	 *  
	 * Draw the graph
	 */
	void DrawTreeLines()
	{
		int count = 0;
		foreach (List<Vector3> pointList in this.MainPointsList) {
			GameObject ALR;
			if (count != this.MainPointsList.Count) {
				ALR = Instantiate (AnimatedLine, gameObject.transform);
			} else {
				ALR = AnimatedLine;
			}
			ALR.SetActive (true);

			int c = 0;
			for(c = 0;c < pointList.Count; c++) {
				Vector3 v;
				Vector3 vm1;
				int interpole = 1;
				if (c != pointList.Count-1 && c != 0) {
					v = pointList [c];
					vm1 = pointList [c-1];
					if (Vector3.Equals (v, vm1)) { //if the values are equals, not going to interpolate
						continue;
					}
					for (int i = 0; i < interpole; i++) {
						ALR.GetComponent<AnimatedLineRenderer> ().Enqueue ((vm1 + (v-vm1) * (((float)i)/((float)interpole))));//interpolate points
						//Debug.Log((vm1 + (v-vm1) * (((float)i)/((float)interpole))));
					}
				}
				else if(c==1)
					ALR.GetComponent<AnimatedLineRenderer> ().Enqueue (pointList[0]);
			}
			count++;
		}
		return;
	}
}
