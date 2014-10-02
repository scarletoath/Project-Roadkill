using UnityEngine;

public class SimulationInput : MonoBehaviour {

	public float MouseSensitivity = 120.0f;
	public float MoveSpeed = 3.0f;

	public bool UseDrift = false;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public bool GetLatestPose ( GameInput.PoseData Pose ) {
		float RotX , RotY;
		Quaternion Rot = Pose.Rotation;
		RotX = Pose.Rotation.eulerAngles.x - GetAxisValue ( "Mouse Y" ) * Time.deltaTime * MouseSensitivity;
		RotY = Pose.Rotation.eulerAngles.y + GetAxisValue ( "Mouse X" ) * Time.deltaTime * MouseSensitivity;
		Rot.eulerAngles = new Vector3 ( RotX , RotY , 0 );
		Pose.Rotation = Rot;

		Vector3 DirForward , DirRight , DirUp , Pos = Pose.Position;
		Rot.eulerAngles = new Vector3 ( 0 , RotY , 0 );
		DirForward = Rot * Vector3.forward;
		DirRight = Rot * Vector3.right;
		DirUp = Vector3.up;
		Pos += GetAxisValue ( "Vertical" ) * DirForward * Time.deltaTime * MoveSpeed;
		Pos += GetAxisValue ( "Horizontal" ) * DirRight * Time.deltaTime * MoveSpeed;
		Pose.Position = Pos;

		return true;
	}

	/// <summary>
	/// Gets the value of an axis, including drift if UseDrift is <code>true</code>.
	/// </summary>
	/// <param name="AxisName">The name of the axis whose value to retrieve.</param>
	/// <returns></returns>
	private float GetAxisValue ( string AxisName ) {
		if ( UseDrift ) {
			return Input.GetAxis ( AxisName );
		}
		else {
			return Input.GetAxisRaw ( AxisName );
		}
	}

}