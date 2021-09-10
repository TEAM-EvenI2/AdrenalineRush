using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
public class Player : MonoBehaviour
{

	public MapSystem mapSystem;

	public float curVelocity;
	public float acceleration = 1f;
	public float maxVelocity;
	public float decreaseVelWhenCollided = 4f;
	private float rotationVelocity;

	public Camera gameCam;
	private MapMeshWrapper currentPipe;
	private float distanceTraveled;
	public float DistanceTraveled
    {
        get
        {
			return distanceTraveled;
        }
    }
	private float deltaToRotation;
	private float systemRotation;


	private Transform world;
	private Transform rotater;
	private float worldRotation;
	private float avatarRotation;

	public int[] earnedItems = new int[2];
	public int earnedScore;

	public float health = 100;

	public SmoothDampStruct<float> inputSmooth;
	private float targetInput = 0;
	private float curInput = 0;

	public float invincibleTime = 0.4f;
	private float _invincibleTime = 0;
	public bool invincible = false;

	public int TotalScore()
	{
		/**
		총점을 반환합니다.
		*/
		return 70; // 일단 무조건 70을 반환하게 만들었습니다 TODO FIXME
	}

	private void Start()
	{
		world = mapSystem.transform.parent;
		rotater = transform.GetChild(0);
		currentPipe = mapSystem.SetupFirstPipe();
		SetupCurrentPipe();

		maxVelocity = Managers.Instance.Config.playerInfo.velocity;
		rotationVelocity = Managers.Instance.Config.playerInfo.rotateVelocity;

	}

	private void Update()
	{
		if (Managers.Instance.GetScene<GameScene>().isPause)
			return;

		if(_invincibleTime > 0)
        {
			_invincibleTime -= Time.deltaTime;
        }

		if (curVelocity < maxVelocity)
		{
			curVelocity += acceleration * Time.deltaTime;
		}

		float delta = curVelocity * Time.deltaTime;
		distanceTraveled += delta;
		systemRotation += delta * deltaToRotation;

		if (systemRotation >= currentPipe.curveAngle)
		{
			delta = (systemRotation - currentPipe.curveAngle) / deltaToRotation;
			currentPipe = mapSystem.SetupNextPipe();
			SetupCurrentPipe();
			systemRotation = delta * deltaToRotation;
		}

		mapSystem.transform.localRotation =
			Quaternion.Euler(0f, 0f, systemRotation);

		UpdateAvatarRotation();

		if(health < 100)
        {
			health += curVelocity * Time.deltaTime;
			if (health > 100)
				health = 100;
        }
	}

	public void SetupNetStage()
	{
		currentPipe = mapSystem.SetupNextPipe();
		SetupCurrentPipe();
		systemRotation =0;
	}


	private void SetupCurrentPipe()
	{
		deltaToRotation = Mathf.Rad2Deg * (1 / currentPipe.curveRadius); 
		worldRotation += currentPipe.relativeRotation;
		if (worldRotation < 0f)
		{
			worldRotation += 360f;
		}
		else if (worldRotation >= 360f)
		{
			worldRotation -= 360f;
		}
		world.localRotation = Quaternion.Euler(worldRotation, 0f, 0f);
	}

	private void UpdateAvatarRotation()
	{
		SetInput();
		curInput = Mathf.SmoothDamp(curInput, targetInput, ref inputSmooth.smoothVelocity, inputSmooth.smoothTime);

		avatarRotation +=
			rotationVelocity * Time.deltaTime * curInput;
		if (avatarRotation < 0f)
		{
			avatarRotation += 360f;
		}
		else if (avatarRotation >= 360f)
		{
			avatarRotation -= 360f;
		}
		rotater.localRotation = Quaternion.Euler(avatarRotation, 0f, 0f);
	}

	private void SetInput()
    {
#if UNITY_EDITOR
		targetInput = Input.GetAxisRaw("Horizontal");
#else
		if(Input.touchCount > 0)
        {
			Vector3 pos = Input.GetTouch(0).position;

			if (pos.x > Screen.width / 2)
				targetInput = 1;
			else
				targetInput = -1;
        }
		else
			targetInput= 0;
#endif

	}

	public bool Hit()
	{
		if (_invincibleTime <= 0 && !invincible)
		{

			health -= 34;
			curVelocity  = 0;
			if (health <= 0) {
				FindObjectOfType<AudioManager>().Play("PlayerDie");
				Die();
			} else {
				gameObject.GetComponentInChildren<GraphicManager>().Damaged();
				FindObjectOfType<AudioManager>().Play("PlayerHit");
			}
			_invincibleTime = invincibleTime;

			return true;
		}
		return false;
	}

	private void Die()
	{
		FindObjectOfType<PlayGames>().playerScore = TotalScore(); // 총점
		int score = FindObjectOfType<PlayGames>().playerScore;
		Debug.Log(score);
		DataManager dataManager = FindObjectOfType<DataManager>();
		if (dataManager)
		{
			Debug.Log("소프트커런시 획득량: " + score);
			dataManager.gameData.SoftCurr += score; // 점수를 얼마를 줄지는 PM분들 결정되면 수정. 일단은 총점만큼 획득.
			dataManager.SaveGameData();
		}
		else
		{
			Debug.LogWarning("DataManager 인스턴스를 찾을 수 없습니다");
		}
		FindObjectOfType<PlayGames>().AddScoreToLeaderboard();

		gameObject.GetComponentInChildren<GraphicManager>().Die();
		gameObject.SetActive(false);
		Managers.Instance.GetUIManager<GameUIManager>().ActiveRe();
	}

	public void CollideItem(GameObject item)
	{
		gameObject.GetComponentInChildren<GraphicManager>().CollideItem(item);
	}
}
