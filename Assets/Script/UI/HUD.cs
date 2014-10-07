using System.Collections;
using UnityEngine;

public class HUD : Singleton<HUD> {

	private const string DESC_SPEAR_LEVEL = "Spear\nLevel\n";
	private const string DESC_MOVE_SPEED = "Speed\nx ";
	private const string DESC_QUIET_FEET = "Sneak\n";

	public UISlider ExperienceBar;
	public UISprite ExperienceLevelSprite;
	public string [] ExperienceLevelSpriteNames;
	public UISprite QuietFeetIcon;
	public UISprite MoveSpeedIcon;

	private float DisplayedExpRatio;
	private float TargetExpRatio;
	private float ExpBarAnimVelocity;

	private bool IsNeedUpdateExpBar = true;

	// Use this for initialization
	void Start () {
		GameManager.GetBonuses ().OnSpearLevelUp += UpdateTargetExpRatio;
		GameManager.GetBonuses ().OnIncreaseExp += UpdateTargetExpRatio;

		GameManager.GetBonuses ().OnMoveSpeedChange += UpdateMoveSpeedIcon;
		GameManager.GetBonuses ().OnQuietFeetChange += UpdateQuietFeetIcon;

		UpdateQuietFeetIcon ( false );
		UpdateMoveSpeedIcon ( false );
	}

	// Update is called once per frame
	void Update () {
		if ( IsNeedUpdateExpBar ) {
			// Animate experience bar value
			if ( ExpBarAnimVelocity > 0.01f ) {
				DisplayedExpRatio = Mathf.SmoothDamp ( DisplayedExpRatio , TargetExpRatio , ref ExpBarAnimVelocity , 0.5f );
			}
			else {
				if ( TargetExpRatio == 1 ) {
					TargetExpRatio = 0;
				}

				if ( GameManager.GetBonuses ().SpearLevel == Bonuses.MAX_SPEAR_LEVEL ) {
					TargetExpRatio = 1;
					IsNeedUpdateExpBar = false;
					ExperienceLevelSprite.aspectRatio = 4.25f; // hard-coded as max level sprite is different size
				}

				DisplayedExpRatio = TargetExpRatio;
				ExperienceLevelSprite.spriteName = ExperienceLevelSpriteNames [ GameManager.GetBonuses ().SpearLevel - 1 ];
			}

			// Actual experience bar update
			ExperienceBar.value = DisplayedExpRatio;
		}
	}

	public void OnMenuClick () {
		StartCoroutine ( LoadLevelAfterSound () );
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

	private void UpdateMoveSpeedIcon ( bool IsBonusActive ) {
		MoveSpeedIcon.cachedGameObject.SetActive ( IsBonusActive );
	}

	private void UpdateQuietFeetIcon ( bool IsBonusActive ) {
		QuietFeetIcon.cachedGameObject.SetActive ( IsBonusActive );
	}

	private IEnumerator LoadLevelAfterSound () {
		//Time.timeScale = 1;

		yield return new WaitForSeconds ( 0.288f );

		Application.LoadLevel ( "MainMenuScene" );
	}

}