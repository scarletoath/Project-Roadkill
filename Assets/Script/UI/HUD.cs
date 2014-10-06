using UnityEngine;

public class HUD : Singleton<HUD> {

	private const string DESC_SPEAR_LEVEL = "Spear\nLevel\n";
	private const string DESC_MOVE_SPEED = "Speed\nx ";
	private const string DESC_QUIET_FEET = "Sneak\n";
	public Texture2D expbartex;
	public Texture2D stealthtex;
	public Texture2D sprinttex;

	private float DisplayedExpRatio;
	private float TargetExpRatio;
	private float ExpBarAnimVelocity;

	// Use this for initialization
	void Start () {
		GameManager.GetBonuses ().OnSpearLevelUp += UpdateTargetExpRatio;
		GameManager.GetBonuses ().OnIncreaseExp += UpdateTargetExpRatio;
	}

	// Update is called once per frame
	void Update () {
		if ( ExpBarAnimVelocity > 0.01f ) {
			DisplayedExpRatio = Mathf.SmoothDamp ( DisplayedExpRatio , TargetExpRatio , ref ExpBarAnimVelocity , 0.5f );
		}
		else {
			if ( TargetExpRatio == 1 ) {
				TargetExpRatio = 0;
			}
			DisplayedExpRatio = TargetExpRatio;
		}
	}

	void OnGUI () {
		if ( GameManager.GetBonuses ().MoveSpeedMultiplier > 1 ) {
			GUI.DrawTexture ( new Rect ( 20 , 20 , 60 , 60 ) , sprinttex );
			GUI.Label ( new Rect ( 60 , 60 , 20 , 20 ) , GameManager.GetBonuses ().GetMoveSpeedTimeRemaining ().ToString () );
		}
		if ( GameManager.GetBonuses ().HasQuietFeet ) {
			GUI.DrawTexture ( new Rect ( 80 , 20 , 60 , 60 ) , stealthtex );
			GUI.Label ( new Rect ( 120 , 60 , 20 , 20 ) , GameManager.GetBonuses ().GetQuietFeetTimeRemaining ().ToString () );
		}

		GUI.Label ( new Rect ( 150 , 20 , 40 , 60 ) , DESC_SPEAR_LEVEL + GameManager.GetBonuses ().SpearLevel );

		GUI.Label ( new Rect ( Screen.width - 120 , 20 , 100 , Screen.height ) , GetKillCountAchievementText () );
		GUI.DrawTexture ( new Rect ( 40 , Screen.height - 40 , ( Screen.width - 80 ) * DisplayedExpRatio , 20 ) , expbartex );

	}

	private string GetKillCountAchievementText () {
		if ( Achievements.IsKillCountAchievement ( GameManager.NumDeadCreatures ) ) {
			return ( ( Achievements.KillCount ) GameManager.NumDeadCreatures ).ToString ().Replace ( '_' , ' ' );
		}
		else if ( GameManager.NumDeadCreatures >= 20 ) {
			return "Stop being cruel\nto animals!\n\nOr maybe not.";
		}
		else {
			return string.Empty;
		}
	}

	private void UpdateTargetExpRatio () {
		TargetExpRatio = GameManager.GetBonuses ().ExpRatio;

		if ( TargetExpRatio == 0 ) {
			TargetExpRatio = 1;
		}

		DisplayedExpRatio = Mathf.SmoothDamp ( DisplayedExpRatio , TargetExpRatio , ref ExpBarAnimVelocity , 0.5f );
	}

}