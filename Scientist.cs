using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class Scientist : MonoBehaviour {

	private List<Car> bots = new List<Car> ();
	[Header("General")]
	public int iteration = 1;
	public int creature = 1;
	private Dictionary<Car,float> scores = new Dictionary<Car,float> ();
	public int numberOfBots;
	public int numberOfGenes;
	private float started;
	public float life;
	public int numParents;
	public float lieTime;
	public List<char> vowels;
	public float mutationRate;
	[Header("Car")]
	public GameObject inner;
	public float minSpeed;
	public float maxWheelSize;
	public float minBodySize;
	public float maxClearence;
	public float maxOffset;
	public float speedMult;
	public float minMass;
	public float maxMass;
	public float bodyRatioXY;
	[Header("Menu")]
	public Text[] desc;
	public Slider sl;
	public Text nam;

	//private
	float max = 0f;
	bool isDead;
	float timeOut;
	bool isLying;

	void Start () {
		Population ();
		started = Time.time;
		foreach (var item in bots) {
			item.name = Name (item.self);
		}
		Launch (0);
	}

	void Update () {
		if (bots.Count > 0) {
			foreach (var item in bots) {
				if (item.self.Count > 0) {
					item.name = Name (item.self);
				}
			}
		}
		if (max < Find(GameObject.FindObjectOfType<Inner>().gameObject, "Racer").transform.position.x - transform.position.x) {
			max = Mathf.Round (Find (GameObject.FindObjectOfType<Inner> ().gameObject, "Racer").transform.position.x - transform.position.x);
		}
		Time.timeScale = sl.value;
		desc [0].text = "Iteration: " + iteration.ToString ();
		desc [1].text = "Creature: " + (creature + 1).ToString ();
		desc [2].text = "Score: " + Mathf.RoundToInt(Find(GameObject.FindObjectOfType<Inner>().gameObject, "Racer").transform.position.x - transform.position.x).ToString();
		desc [3].text = "Remaining time: " + Mathf.RoundToInt(life - (Time.time - started)).ToString ();
		desc [4].text = "Name: " + bots [creature].name;
		desc [5].text = "Max Score: " + max.ToString ();
		if (creature >= numberOfBots) {
			Generate ();
		} else {
			if (Time.time - life >= started) {
				print ("timeout");
				isDead = true;
			}
			if (!isLying && Find (GameObject.FindObjectOfType<Inner> ().gameObject, "Racer").GetComponent<Rigidbody2D> ().velocity.magnitude <= minSpeed && Time.time - 1 >= started) {
				isLying = true;
				timeOut = Time.time;
				print (Name(bots [creature].self) + " lying (" + started.ToString() + ", " + Time.time.ToString() + ")");
			}
			if (isLying && Time.time - lieTime >= timeOut) {
				isLying = false;
				isDead = true;
			}
			if (isDead) {
				print ("dead");
				isLying = false;
				isDead = false;
				Next ();
				if (creature >= numberOfBots) {
					Generate ();
				}
				Launch (1);
			}
		}
	}

	List<float> Mutate (List<float> m, float rate) {
		List<float> result = new List<float> ();
		for (int i = 0; i < m.Count; i++) {
			result.Add (Mathf.Clamp01 (m [i] + (Random.value - 0.5f) * 2f * rate));
		}
		return result;
	}

	void Population () {
		for (int i = 0; i < numberOfBots; i++) {
			Car current = new Car ();
			for (int x = 0; x < numberOfGenes; x++) {
				current.self.Add (Random.value);
			}
			bots.Add (current);
		}
	}

	void Launch (int hide) {
		if (hide == 1) {
			Destroy (GameObject.FindObjectOfType<Inner>().gameObject);
		}
		GameObject parent = Instantiate (inner, transform.position, transform.rotation) as GameObject;
		string name = parent.name;
		print (Name(bots [creature].self));
		List<float> genes = bots [creature].self;
		var xsize = genes [0] * (1 / bodyRatioXY);
		var ysize = genes [1] * bodyRatioXY;
		var fxpos = genes [2] * maxOffset;
		var bxpos = genes [3] * maxOffset;
		var fypos = genes [4] * maxClearence;
		var bypos = genes [5] * maxClearence;
		var bsize = genes [6];
		var fsize = genes [7];
		var benergy = genes [8] * speedMult;
		var fenergy = genes [9] * speedMult;
		//var mass = (genes [10] * (maxMass - minMass)) + minMass;
		GameObject car = Find (parent, "Racer");
		GameObject ff = Find (parent, "front");
		GameObject bb = Find (parent, "back");
		Transform b = Find (car, "backwheel").transform;
		Transform f = Find (car, "frontwheel").transform;
		//car.GetComponent<Rigidbody2D> ().mass *= mass;
		Vector3 n = car.transform.localScale;
		Vector3 original = b.transform.lossyScale;
		car.transform.localScale = new Vector3 (n.x * (minBodySize + (1-minBodySize) * xsize), n.y * (minBodySize + (1-minBodySize) * ysize), n.z);
		SetGlobalScale (b, original);
		SetGlobalScale (f, original);
		float dl = Vector3.Distance (b.localPosition, f.localPosition);
		bxpos = bxpos * dl;
		bypos = bypos * dl;
		fxpos = fxpos * dl;
		fypos = fypos * dl;
		float scalarback = 0.5f + bsize * (maxWheelSize - 0.5f);
		float scalarfront = 0.5f + fsize * (maxWheelSize - 0.5f);
		b.localScale *= scalarback;
		f.localScale *= scalarfront;
		bb.transform.localScale *= scalarback;
		ff.transform.localScale *= scalarfront;
		b.position = b.position + new Vector3 (bxpos, bypos, 0f);
		f.position = f.position + new Vector3 (-fxpos, fypos, 0f);
		WheelJoint2D[] j = car.GetComponents<WheelJoint2D> ();
		WheelJoint2D back = new WheelJoint2D();
		WheelJoint2D front = new WheelJoint2D();
		foreach (var item in j) {
			if (item.connectedBody == b.GetComponent<Rigidbody2D>()) {
				back = item;
			} else if (item.connectedBody == f.GetComponent<Rigidbody2D>()) {
				front = item;
			}
		}
		JointMotor2D mb = back.motor;
		JointMotor2D mf = front.motor;
		mb.motorSpeed *= scalarback;
		mf.motorSpeed *= scalarfront;
		mb.motorSpeed *= speedMult;
		mf.motorSpeed *= speedMult;
		mb.motorSpeed *= benergy;
		mf.motorSpeed *= fenergy;
		back.motor = mb;
		front.motor = mf;
		back.anchor = back.anchor + new Vector2 (bxpos, bypos);
		front.anchor = front.anchor + new Vector2 (-fxpos, fypos);
	}

	void Generate () {
		iteration++;
		creature = 0;
		started = Time.time;
		var list = scores.ToList ();
		var topp = list.OrderByDescending (x => x.Value).Take(numParents);
		var top = new List<Car>();
		foreach (var item in topp) {
			top.Add (item.Key);
		}
		print ("Parent: " + Name(top[0].self));
		bots = new List<Car> ();
		bots.AddRange (top);
		for (int i = 0; i < numberOfBots - numParents; i++) {
			bots.Add (Breed (top, mutationRate));
		}
		scores = new Dictionary<Car, float> ();
	}

	Car Breed (List<Car> parents, float mutRate) {
		List<int> mix = new List<int> ();
		var numGenes = parents [0].self.Count;
		var numParents = parents.Count;
		var result = new Car ();
		for (int i = 0; i < numGenes; i++) {
			mix.Add (Random.Range(0, numParents));
		}
		for (int i = 0; i < numGenes; i++) {
			result.self.Add(parents[mix[i]].self[i]);
		}
		result.self = Mutate (result.self, mutRate);
		return result;
	}

	void Next () {
		scores.Add (bots[creature], Find(GameObject.FindObjectOfType<Inner>().gameObject, "Racer").transform.position.x - transform.position.x);
		started = Time.time;
		creature++;
	}

	public GameObject Find (GameObject parent, string name) {
		foreach (var item in parent.GetComponentsInChildren<Transform>()) {
			if (item.gameObject.name == name) {
				return item.gameObject;
			}
		}
		return null;
	}

	public void SetGlobalScale (Transform t, Vector3 globalScale)
	{
		t.localScale = Vector3.one;
		t.localScale = Divide(globalScale, t.lossyScale);
	}

	public Vector3 Divide (Vector3 a, Vector3 b) {
		return new Vector3 (a.x / b.x, a.y / b.y, a.z / b.z);
	}

	public string Name (List<float> param) {
		if (param.Count > 0) {
			List<int> intParam = new List<int> ();
			foreach (var item in param) {
				intParam.Add (Mathf.FloorToInt (Mathf.Clamp (item, 0f, 1f) * (vowels.Count - 1)));
			}
			string s = "";
			foreach (var item in intParam) {
				s += vowels [item];
			}
			return s;
		} else {
			return "";
		}
	}

	public Car FromName (string param) {
		print (param);
		List<float> o = new List<float> ();
		foreach (var item in param) {
			o.Add (((float) vowels.IndexOf(item) + 1f) / (float) vowels.Count);
		}
		return new Car (o);
	}

	public Car Best () {
		float min = -10000f;
		Car best = new Car ();
		foreach (var item in scores.Keys) {
			float val;
			if (scores.TryGetValue(item, out val)) {
				if (val > min) {
					best = item;
				}
			}
		}
		return best;
	}

	public void FromBest (Car best) {
		iteration = 0;
		print (Name (best.self));
		scores = new Dictionary<Car, float> ();
		scores.Add (best, 100f);
		for (int i = 0; i < numberOfBots - 1; i++) {
			scores.Add (new Car (Mutate (best.self, mutationRate)), -100f);
		}
		var np = numParents;
		numParents = 1;
		Generate ();
		numParents = np;
		creature = 0;
		Launch (1);
		isLying = false;
		isDead = false;
		started = Time.time;
		timeOut = Time.time + life;
	}

	public void Save () {
		print("saving " + nam.text);
		string n = Name (Best ().self);
		File.WriteAllText (nam.text, n);
	}

	public void Load () {
		print("loading " + nam.text);
		StreamReader i = new StreamReader (nam.text);
		string n = i.ReadToEnd ();
		print (Name (FromName (n).self));
		FromBest (FromName (n));
	}
}

[System.Serializable]
public class Car {
	public List<float> self;
	public string name;

	public Car () {self = new List<float> ();}
	public Car (List<float> f) {self = f;}
}