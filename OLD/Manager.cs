using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

	private static Manager playerInstance;
	public Guy[] genomes;
	public int numguys;
	public int numgenes;
	public int numcommands = 11;
	private int iterations = 0;
	public int mutRate = 1;

	[Header("Year")]
	[Header("Score")]
	[Header("Time")]
	[Header("Creature")]

	private static Text[] signs = new Text[4];

	void Awake () {
		//singleton
		DontDestroyOnLoad (this);
		if (playerInstance == null) {
			playerInstance = this;
			StartCoroutine ("Algo");
		} else {
			DestroyImmediate (gameObject);
		}
	}

	IEnumerator Algo() {
		//start
		if (iterations == 0) {
			genomes = new Guy[numguys];
			yield return null;
			//randomize all of guys
			for (int i = 0; i < numguys; i++) {
				yield return null;
				genomes [i] = new Guy ();
				for (int d = 0; d < numgenes; d++) {
					genomes [i].Add (Random.Range(0,numcommands));
					yield return null;
				}
			}
			yield return null;
			iterations++;
			//start a new iteration
			SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
			StartCoroutine ("Algo");
		} else {
			signs [0] = GameObject.Find ("Year").GetComponent<Text> ();
			signs [1] = GameObject.Find ("Score").GetComponent<Text> ();
			signs [2] = GameObject.Find ("Time").GetComponent<Text> ();
			signs [3] = GameObject.Find ("Creature").GetComponent<Text> ();

			signs[0].text = "Year: " + iterations.ToString();
			int i = 0;
			yield return null;
			SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
			foreach (var item in genomes) {
				yield return new WaitForSeconds (0.5f);

				signs [0] = GameObject.Find ("Year").GetComponent<Text> ();
				signs [1] = GameObject.Find ("Score").GetComponent<Text> ();
				signs [2] = GameObject.Find ("Time").GetComponent<Text> ();
				signs [3] = GameObject.Find ("Creature").GetComponent<Text> ();

				signs[0].text = "Year: " + iterations.ToString();

				Bot bot = GameObject.FindObjectOfType<Bot> ().GetComponent<Bot> ();
				bot.s = item.self;
				yield return null;
				float starttime = Time.time;
				while (bot.isDead != true) {
					yield return new WaitForSeconds (0.1f);
					signs[1].text = "Score: " + bot.score.ToString();
					signs [2].text = "Time: " + (Time.time - starttime).ToString();
					signs [3].text = "Creature: " + i.ToString ();
					yield return null;
				}
				genomes [i].score = bot.score;
				i++;
				yield return null;
				SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
			}
			float max = 0f;
			int[] maxGenome = genomes [0].self;
			yield return null;
			foreach (var item in genomes) {
				yield return null;
				if (item.score > max) {
					max = item.score;
					maxGenome = item.self;
					yield return null;
				}
			}
			genomes = new Guy[numguys];
			yield return null;
			for (i = 0; i < genomes.Length; i++) {
				yield return null;
				genomes[i].self = maxGenome;
				genomes[i].Mutate (mutRate,numcommands);
			}
			iterations++;
			yield return null;
			SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
		}
	}
}

[System.Serializable]
public class Guy {

	public int[] self;
	public float score;

	public Guy () {
		self = new int[0];
		score = 0f;
	}

	public void Add (int elem) {
		List<int> selfp = new List<int>(self);
		selfp.Add (elem);
		self = (int[]) selfp.ToArray().Clone();
	}

	public void Mutate (int amount, int numcommands) {
		for(int i = 0; i < self.Length; i++) {
			if (Random.Range (0, 1) == 1) {
				self[i] -= Random.Range (0,amount);
				if (self[i] < 0) {
					self[i] = numcommands - 1;
				}
				if (self[i] >= numcommands) {
					self[i] = 0;
				}
			} else {
				self[i] += Random.Range (0,amount);
				if (self[i] < 0) {
					self[i] = numcommands - 1;
				}
				if (self[i] >= numcommands) {
					self[i] = 0;
				}
			}
		}
	}
}