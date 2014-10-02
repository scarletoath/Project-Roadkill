
public class PlayerController : Singleton<PlayerController> {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		transform.position = GameInput.Pose.Position;
		transform.rotation = GameInput.Pose.Rotation;
	}

}