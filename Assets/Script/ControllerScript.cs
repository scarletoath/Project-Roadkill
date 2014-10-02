using UnityEngine;
using System.Collections;

public class ControllerScript : MonoBehaviour {
	public GameObject deer;
	// Use this for initialization
	void Start () {
		for (int i=0; i<30; i++) {
			GameObject deerobj = (GameObject)Instantiate(deer);
			Vector3 pos = deerobj.transform.position;

			pos.z = Random.Range(0, 1000);
			pos.x = 10 * Mathf.Sin(Mathf.Deg2Rad * pos.z);
			deerobj.transform.position = pos;
				}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
