using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shower : MonoBehaviour {

	EdgeCollider2D col;
	LineRenderer l;

	void Awake () {
		l = GetComponent<LineRenderer>();
		col = GetComponent<EdgeCollider2D> ();

		foreach (var m in col.points) {
			l.positionCount = l.positionCount + 1;
			l.SetPosition (l.positionCount - 1, (Vector3)m);

		}
	}
}
