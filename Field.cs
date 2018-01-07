using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Field : MonoBehaviour {
//
//	public GameObject controlledObject;
//	public float score = 0f;
//	public float timer;
//	private float starttime;
//	public bool isDead;
//
//	void Start () {
//		List<float> genes = new List<float> {1f,0.5f,0f,0f,0f,0f};
//		isDead = false;
//		starttime = Time.time;
//		GameObject bot = Instantiate (controlledObject, transform.position, transform.rotation);
//		//TODO: construct based on parameters
//		//nope.
//		//actually yep!
//		var xsize = genes [0];
//		var ysize = genes [1];
//		var fxpos = genes [2];
//		var bxpos = genes [3];
//		var fypos = genes [4];
//		var bypos = genes [5];
//		Vector3 n = GameObject.Find ("Racer").transform.localScale;
//		GameObject.Find ("Racer").transform.localScale = new Vector3 (n.x * xsize, n.y * ysize, n.z);
//		Transform b = GameObject.Find ("backwheel").transform;
//		Transform f = GameObject.Find ("frontwheel").transform;
//		b.position = b.position + new Vector3 (bxpos,bypos,0f);
//		f.position = f.position + new Vector3 (-fxpos,fypos,0f);
//		//END_TODO
//	}
//
//	public void Run (List<float> genes) {
//		
//	}
//}
//
//
//

//API wrapper
public class Learn {
	public static bool NN = false;
	public static bool Tree = false;
	private List<Property> i;
	private List<Property> o;
	public NeuralNetwork self;
	public DecisionTree selft;
	public bool t;

	public Learn (int numInputs, int numOutputs, bool type) {
		/*i = new List<Property> (new Property[numInputs]);
		o = new List<Property> (new Property[numOutputs]);
		//NN
		//i wont do that 'cause im lazy...

		//Tree
		//ill do that!
		self = new DecisionTree (i, o);*/
		/*
		//(no)
		self = new NeuralNetwork(numInputs,3,6,numOutputs);*/
		//(no)

		t = type;
		if (type) {
			i = new List<Property> (new Property[numInputs]);
			o = new List<Property> (new Property[numOutputs]);
			selft = new DecisionTree (i, o);
		} else {
			self = new NeuralNetwork(numInputs,3,6,numOutputs);
		}
	}

	public List<float> Run (List<float> inputs) {
		/*
		i = Property.From (inputs);
		self.Run ();
		return Property.To (o);*/

		//(no)
		if (t) {
			i = Property.From (inputs);
			selft.Run ();
			return Property.To (o);
		} else {
			return self.Run (inputs);
		}
	}

	public void Mutate (float rate) {
		if (t) {
			selft = selft.Mutate (rate);
		} else {
			self = self.Mutate (rate);
		}
	}
}




//that's it! real talk!! think about it dude!!! NOPE.
public class DecisionTree {
	//the root of all of the problems here on earth. FRODO! you must destroy it.
	public TreeNode root;
	public List<Property> outp;

	//the beginning of the mutants' birth.
	public List<bool> toMutate;

	//something epic is coming... OOOH BOOOY!
	public DecisionTree (List<Property> inputs, List<Property> outputs) {
		//TADADADA!
		outp = outputs;
		int numInputs = inputs.Count;
		int numOutputs = inputs.Count;
		root = new TreeNode (inputs[0]);
		int iter = 1;
		//thats hard
		for (int i = 1; i < numInputs - 1; i++) {
			List<TreeNode> current = Node.Children(root).ConvertAll(x => (TreeNode)x);
			foreach (var item in current) {
				item.Add (new TreeNode (inputs [iter]));
				iter++;
				item.Add (new TreeNode (inputs [iter]));
				iter++;
			}
			var curren = new List<TreeNode> ();
			foreach (var item in current) {
				foreach (var m in item.next) {
					curren.Add ((TreeNode)m);
				}
			}
			current = curren;
		}
		//sorry dave, im afraid i cant do that.
		toMutate = new List<bool> ();
		int numFire = (Node.Children(root).ConvertAll(x => (TreeNode)x)).Count - numOutputs;
		for (int i = 0; i < numFire; i++) {
			toMutate.Add (true);
		}
		for (int i = 0; i < numOutputs; i++) {
			toMutate.Add (false);
		}
		//shuffle up your mouth!
		for (int i = 0; i < toMutate.Count; i++) {
			bool temp = toMutate [i];
			int randomIndex = Random.Range (i, toMutate.Count);
			toMutate [i] = toMutate [randomIndex];
			toMutate [randomIndex] = temp;
		}
		//return YEEEEEEEEE;
	}

	//not done yet...
	public void Run() {
		//preparations. a simple tree parser
		List<Node> current = Node.Children(root);
		//add fire nodes
		int iter = 0;
		int n = 0;
		foreach (var item in current) {
			if (toMutate [iter]) {
				current.Add (new FireNode(outp[n], 1f));
				n++;
			}
			iter++;
		}
		//eezy
		root.Eval();
		//some cleanup
		foreach (var item in current) {
			item.next = new List<Node> ();
		}
		//finally.
	}

	//what? i didnt expect that!!!
	public DecisionTree Mutate (float rate) {
		List<bool> newToMutate = new List<bool> (toMutate);
		List<int> pos = new List<int> ();
		int m = Mathf.FloorToInt (((float)(toMutate.Count)) * rate);
		for (int i = 0; i < m; i++) {
			pos.Add (Random.Range (0, toMutate.Count));
		}
		foreach (var item in pos) {
			if (Random.Range (0, 1) > 0) {
				var r = toMutate [item + 1];
				var l = toMutate [item];
				newToMutate [item + 1] = l;
				newToMutate [item] = r;
			} else {
				var r = toMutate [item - 1];
				var l = toMutate [item];
				newToMutate [item - 1] = l;
				newToMutate [item] = r;
			}
		}
		DecisionTree d = (DecisionTree)this.MemberwiseClone ();
		d.toMutate = newToMutate;
		return d;
	}
	//that was truly easy.

	//there will be some visualization code after making the field.
}

public class Node {			
	public List<Node> next;
	public Node previous;

	public virtual void Eval () {
		if (next.Count > 0) {
			foreach (var item in next) {
				item.Eval ();
			}
		}
	}

	public Node Add (Node n) {
		next.Add (n);
		next [next.Count - 1].previous = this;
		return next [next.Count - 1];
	}

	public static List<Node> Children (Node start) {
		List<Node> current = new List<Node>();
		current.Add (start);
		while(true) {
			var curren = new List<Node> ();
			foreach (var item in current) {
				foreach (var i in item.next) {
					curren.Add (i);
				}
			}
			if (curren.Count < 1) {
				break;
			}
			current = curren;
		}
		return current;
	}
}

public class TreeNode : Node {
	public float condition = 0.5f;
	public Property prop;

	public TreeNode (Property p) {
		next = new List<Node> ();
		previous = null;
		condition = 0.5f;
		prop = p;
	}

	public TreeNode (Property p, float cond) {
		next = new List<Node> ();
		previous = null;
		condition = cond;
		prop = p;
	}

	public TreeNode Add (Property p) {
		next.Add (new TreeNode(p));
		next [next.Count - 1].previous = this;
		return (TreeNode) (next [next.Count - 1]);
	}

	public TreeNode Add (Property p, float cond) {
		next.Add (new TreeNode(p, cond));
		next [next.Count - 1].previous = this;
		return (TreeNode) (next [next.Count - 1]);
	}

	public override void Eval () {
		int div2 = Mathf.FloorToInt (next.Count / 2);
		int div1 = next.Count - div2;
		if (prop.self >= condition) {
			List<Node> m = next.GetRange (0, div1);
			foreach (var item in m) {
				item.Eval ();
			}
		} else {
			List<Node> m = next.GetRange (div1, div2);
			foreach (var item in m) {
				item.Eval ();
			}
		}
	}
}

public class FireNode : Node {
	public Property toChange;
	public float to;

	public FireNode (Property what, float fireTo) {
		toChange = what;
		to = fireTo;
	}

	public override void Eval () {
		if (toChange.self != to) {
			toChange.self = to;
		}
		base.Eval ();
	}
}

public class Property {
	public float self;

	public Property (float val) {self = val;}

	public static List<float> To (List<Property> p) {
		List<float> m = new List<float> ();
		foreach (var item in p) {
			m.Add (item.self);
		}
		return m;
	}

	public static List<Property> From (List<float> p) {
		List<Property> m = new List<Property> ();
		foreach (var item in p) {
			m.Add (new Property(item));
		}
		return m;
	}
}

//I actually have a class for that!
public class NeuralNetwork {

	//IT.
	public List<Matrix> weights = new List<Matrix>();

	//constructor
	public NeuralNetwork (int inputs, int hiddenlayers, int hiddenneurons, int outputs) {
		Matrix curlayer = new Matrix (inputs, hiddenneurons, true);
		weights.Add (curlayer);
		for (int i = 0; i < hiddenlayers; i++) {
			curlayer = new Matrix (hiddenneurons, hiddenneurons, true);
			weights.Add (curlayer);
		}
		curlayer = new Matrix (hiddenneurons, outputs, true);
		weights.Add (curlayer);
	}

	//run
	public List<float> Run (List<float> input) {
		Matrix current = new Matrix(input);
		for (int i = 0; i < weights.Count; i++) {
			current = Matrix.Dot (current, weights [i]);
			for (int x = 0; x < current.self[0].Count; x++) {
				if (current.self [0] [x] > 1f) {
					current.self [0] [x] = current.self [0] [x] - Mathf.Floor (current.self [0] [x]);
				}
			}
		}
		for (int x = 0; x < current.self[0].Count; x++) {
			if (current.self [0] [x] > 1f) {
				current.self [0] [x] = current.self [0] [x] - Mathf.Floor (current.self [0] [x]);
			}
		}
		return current.self[0];
	}

	//ruun foorreesstt ruun!!!
	public List<float> Run (List<float> input, bool dosigmoid) {
		Matrix current = new Matrix(input);
		for (int i = 0; i < weights.Count; i++) {
			current = Matrix.Dot (current, weights [i]);
			if (dosigmoid) {
				current.Sigmoid ();
			} else {
				for (int x = 0; x < current.self[0].Count; x++) {
					if (current.self [0] [x] > 1f) {
						current.self [0] [x] = current.self [0] [x] - Mathf.Floor (current.self [0] [x]);
					}
				}
			}
		}
		if (!dosigmoid) {
			for (int x = 0; x < current.self[0].Count; x++) {
				if (current.self [0] [x] > 1f) {
					current.self [0] [x] = current.self [0] [x] - Mathf.Floor (current.self [0] [x]);
				}
			}
		}
		return current.self[0];
	}

	//yeah mutants!
	public NeuralNetwork Mutate (float rate) {
		NeuralNetwork n = (NeuralNetwork)this.MemberwiseClone ();
		foreach (Matrix item in n.weights) {
			item.Mutate (rate);
		}
		return n;
	}
}

public class Matrix {

	//sigmoid
	public static float Sig(float x) {
		return ((2 / (1 + Mathf.Exp(-2 * x))) - 1);
	}

	//tangent
	public static float Tanh (float x) {
		return (2 * Matrix.Sig (x * 2)) - 1;
	}

	//core
	public List<List<float>> self = new List<List<float>>();

	//constructor
	public Matrix(int x, int y) {
		for (int xo = 0; xo < x; xo++) {
			List<float> curlayer = new List<float> ();
			for (int yo = 0; yo < y; yo++) {
				curlayer.Add (0f);
			}
			self.Add (curlayer);
		}
	}

	//constructor
	public Matrix(int x, int y, bool isRandom) {
		for (int xo = 0; xo < x; xo++) {
			List<float> curlayer = new List<float> ();
			for (int yo = 0; yo < y; yo++) {
				if (isRandom) {
					curlayer.Add (Random.value);
				} else {
					curlayer.Add (0f);
				}
			}
			self.Add (curlayer);
		}
	}

	//yet another constructor
	public Matrix(List<float> vector) {
		List<float> result = new List<float> ();
		for (int i = 0; i < vector.Count; i++) {
			result.Add(vector [i]);
		}
		self.Add (result);
	}

	//get row
	public List<float> GetRow(int row) {
		return self [row];
	}

	//get column
	public List<float> GetColumn(int column) {
		List<float> answer = new List<float> ();
		for (int i = 0; i < self.Count; i++) {
			answer.Add (self [i] [column]);
		}
		return answer;
	}

	//get size
	public List<int> GetDimensions() {
		List<int> result = new List<int> ();
		result.Add (self.Count);
		result.Add (self [0].Count);
		return result;
	}

	//multiply
	public static Matrix Dot(Matrix a, Matrix b) {
		if (a.GetDimensions() [1] != b.GetDimensions() [0]) {
			Debug.LogError("Error!!! Matrices not alligned!!!!");
			return new Matrix (0, 0, false);
		}

		Matrix answer = new Matrix (a.GetDimensions() [0], b.GetDimensions() [1], false);
		//cycle through rows
		for (int r = 0; r < a.GetDimensions()[0]; r++) {
			//get current row
			List<float> row = a.GetRow (r);
			//cycle through columns
			for (int col = 0; col < b.GetDimensions()[1]; col++) {
				//get current column
				List<float> column = b.GetColumn (col);
				//initialize result
				float result = 0f;
				//multiply current row by current column
				for (int i = 0; i < column.Count; i++) {
					result += row [i] * column [i];
				}
				//set answer
				answer.self [r] [col] = result;
			}
		}
		return answer;
	}

	//sigmoid that matrix!!!
	public void Sigmoid() {
		for (int x = 0; x < self.Count; x++) {
			for (int y = 0; y < self[x].Count; y++) {
				self [x] [y] = Matrix.Tanh (self [x] [y]);
			}
		}
	}

	//mutants rule!
	public void Mutate(float rate) {
		for (int x = 0; x < self.Count; x++) {
			for (int y = 0; y < self [x].Count; y++) {
				self [x] [y] += ((Random.value - 0.5f) * 2f) * rate;
			}
		}
	}
}