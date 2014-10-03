using UnityEngine;
using System.Collections;

public class DeerAIScript : MonoBehaviour {
	public GameObject caveman;
	bool dead;
	// Use this for initialization
	void Start () {
		dead = false;
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.collider.gameObject.name != "Terrain" && !dead) {
						Destroy (this.gameObject,1.0f);
			dead = true;
			//caveman.transform.Find ("blood").gameObject.GetComponent<ParticleSystem> ().Stop();
			caveman.transform.Find ("blood").gameObject.GetComponent<ParticleSystem> ().Emit(100);
				}
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 diff = this.transform.position - caveman.transform.position;
		diff.y = 0;
		if (diff.magnitude < 20.0f) 
		{
			Vector3 localvec = Quaternion.Inverse(this.transform.rotation) * diff;
			float doty = Vector3.Dot(localvec,Vector3.forward);
			if(doty < 0)
			{
				localvec.y=0;
				Quaternion rot = Quaternion.FromToRotation(Vector3.forward,localvec);
				StopCoroutine("run");
				StartCoroutine(run (diff,rot));
			}
		}
	}

	IEnumerator run(Vector3 direction,Quaternion lookAway)
	{
		this.transform.rotation = lookAway * this.transform.rotation;

		for (float i=0; i<10; i+=Time.deltaTime * 8) 
		{
			if(dead)yield break;
			this.transform.position += direction.normalized * 8 * Time.deltaTime;
			yield return null;
		}

		this.transform.Rotate (new Vector3 (0, 1, 0), 180f);
	}
}
