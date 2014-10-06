using UnityEngine;
using System.Collections;

public class SplatterTImeoutScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    StartCoroutine(GoAway());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator GoAway()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(this.gameObject);
    }
}
