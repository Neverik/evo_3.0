using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bot : MonoBehaviour
{
	private static readonly int size = 65535;
	private int[] memory = new int[size];
	private int ptr { get; set; }
	private int i = 0;
	public int[] s;
	public float lifetime;
	//public float delay;
	public GameObject caster;
	private int numcontacts;
	public float score;
	public float jumpforce;
	public float jumpenergy;
	public float rollspeed;
	public float starttime;
	[HideInInspector]
	public bool isDead = false;

	void Awake ()
	{
		starttime = Time.time;
		ptr = 0;
		Array.Clear(memory, 0, memory.Length);
		StartCoroutine ("Interpret");
	}

	void OnCollisionEnter2D(Collision2D col) {
		numcontacts++;
	}

	void OnCollisionExit2D(Collision2D col) {
		numcontacts--;
	}

	void Update () {
		if (Time.time - starttime > 0f) {
			isDead = true;
		}
	}

	public IEnumerator Interpret()
	{
		int right = s.Length;

		yield return null;

		Dictionary<int,int> Dict = new Dictionary<int, int>();
		foreach(int item in s)
		{
			yield return null;
			if (!Dict.ContainsKey(item))
			{
				yield return null;
				Dict.Add(item,1);
			}
			else
			{
				yield return null;
				int count = 0;
				Dict.TryGetValue(item, out count);
				Dict.Remove(item);
				Dict.Add(item, count+1);
			}
			yield return null;
		}

		yield return null;

		int fourcount = 0;
		Dict.TryGetValue (4, out fourcount);

		int fivecount = 0;
		Dict.TryGetValue (5, out fivecount);

		if (fourcount != fivecount) {
			score = -10f;
			isDead = true;
			yield break;
		}

		while (i < right)
		{
			yield return null;
			switch (s[i])
			{
			case 0:
				{
					yield return null;
					// <
					ptr++;
					if (ptr >= size)
					{
						yield return null;
						ptr = 0;
					}
					break;
				}
			case 1:
				{
					yield return null;
					// >
					ptr--;
					if (ptr < 0)
					{
						yield return null;
						ptr = size - 1;
					}
					break;
				}
			case 2:
				{
					yield return null;
					// +
					memory[ptr]++;
					break;
				}
			case 3:
				{
					yield return null;
					// -
					memory[ptr]--;
					break;
				}
			case 4:
				{
					yield return null;
					//start loop
					if (memory[ptr] == 0)
					{
						yield return null;
						int loop = 1;
						while (loop > 0)
						{
							yield return null;
							i ++;
							//yield return new WaitForSeconds (delay);
							int c = s[i];
							if (c == '[')
							{
								yield return null;
								loop ++;
							}
							else
								if (c == ']')
								{
									yield return null;
									loop --;
								}
							yield return null;
						}
					}
					break;
				}
			case 5:
				{
					yield return null;
					//end loop
					int loop = 1;
					while (loop > 0)
					{
						yield return null;
						i --;
						//yield return new WaitForSeconds (delay);
						int c = s[i];
						if (c == '[')
						{
							yield return null;
							loop --;
						}
						else
							if (c == ']')
							{
								yield return null;
								loop ++;
							}
					}
					yield return null;
					i --;
					//yield return new WaitForSeconds (delay);
					break;
				}
				case 6:
					{
						yield return null;
						//jump
						if (numcontacts > 0) {
							GetComponent<Rigidbody2D> ().AddForce ((Vector2) transform.right * jumpforce * memory[ptr]);
							score -= jumpenergy;
							yield return null;
						}
						break;
					}
				case 7:
					{
						//rotate self
						yield return null;
						GetComponent<Rigidbody2D> ().AddTorque(memory[ptr] * rollspeed);
						break;
					}
				case 8:
					{
						//rotate eye
						yield return null;
						caster.transform.Rotate (new Vector3 (0f, 0f, (float)memory [ptr] * rollspeed));
						break;
					}
				case 9:
					{
						yield return null;
						//get distance from eye to terrain
						RaycastHit2D hit = Physics2D.Raycast (caster.transform.position, caster.transform.right);
						memory [ptr] = (int)Mathf.Round (hit.distance);
						break;
					}
				case 10:
					{
						yield return null;
						//get rotation
						memory [ptr] = (int)Mathf.Round (transform.rotation.eulerAngles.z/45f - 4f);
						break;
					}
			}
			i++;
			//yield return new WaitForSeconds (delay);
			yield return null;
		}
		yield return null;
		if (!isDead) {
			yield return null;
			StartCoroutine ("Interpret");
		}
	}
}