using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
	public MapMeshWrapper currentPipe;
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
	public float curInput { get; private set; } = 0;
	private AudioManager audioManager;
	public float invincibleTime = 0.4f;
	private float _invincibleTime = 0;
	public bool invincible = false;

	private void Start()
	{
		world = mapSystem.transform.parent;
		rotater = transform.GetChild(0);
		currentPipe = mapSystem.SetupFirstPipe();
		SetupCurrentPipe();

		maxVelocity = Managers.Instance.Config.playerInfo.velocity;
		rotationVelocity = Managers.Instance.Config.playerInfo.rotateVelocity;

		audioManager = FindObjectOfType<AudioManager>();


		GetComponentInChildren<GraphicManager>().Init();
	}

	private void Update()
	{
		if (Managers.Instance.GetScene<GameScene>().isPause)
			return;

		if(_invincibleTime > 0)
        {
			_invincibleTime -= Time.unscaledDeltaTime;
        }

		if (curVelocity < maxVelocity)
		{
			curVelocity += acceleration * Time.deltaTime;
		}

		CalculatePlayerPosition();

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
		RecoverHealth();
	}

	private void RecoverHealth()
    {

		if (health < 100)
		{
			float multiflier = 1;
            switch (DataManager.instance.gameData.equippedCharaIndex)
            {
				case 1:
					multiflier = 1.2f;
					break;
				case 2:
					multiflier = 1.5f;
					break;
				case 3:
					multiflier = 2f;
					break;
			}


			health += curVelocity * multiflier * Time.deltaTime;
			if (health > 100)
				health = 100;
		}
	}

	private void CalculatePlayerPosition()
    {
		Transform avatar = rotater.GetChild(0);

		//float percent = systemRotation / currentPipe.curveAngle;
		//float angle = (rotater.localEulerAngles.x + 180) % 360;

		//float distance = currentPipe.mapMesh.GetDistance(currentPipe, percent, angle / 360);

		avatar.localPosition = Vector3.down *( currentPipe.mapMesh.mapSize  );

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
			if (!EventSystem.current.IsPointerOverGameObject(0))
			{
				Vector3 pos = Input.GetTouch(0).position;

				if (pos.x > Screen.width / 2)
					targetInput = 1;
				else
					targetInput = -1;
			}
		}
		else
			targetInput= 0;
#endif

	}

	public bool Hit()
	{
		if (_invincibleTime <= 0 && !invincible)
		{
			Managers.Instance.GetUIManager<GameUIManager>().HitScreen();
			audioManager.Vibrate(); // ?????? ???????????? ??????

			health -= 34;
			curVelocity  = 0;
			audioManager.Play("PlayerHit");
			if (health <= 0) {
				audioManager.Play("PlayerDie");
				Die();
			} else {
				GetComponentInChildren<GraphicManager>().Damaged();
			}
			_invincibleTime = invincibleTime;

			return true;
		}
		return false;
	}

	private void Die()
	{
		int score = Managers.Instance.GetScene<GameScene>().GetScore();
		print("Score: " +score);
		//FindObjectOfType<PlayGames>().playerScore = score; // ?????? GooglePlay??? ???????????? //?????? ?????? (????????????)
		Debug.Log(score);
		DataManager dataManager = DataManager.instance;
		if (dataManager)
		{
			Debug.Log("?????????????????? ?????????: " + score);
			dataManager.gameData.SoftCurr += score; // ?????? ????????? ???????????? ??????.
			dataManager.SaveGameData();
		}
		else
		{
			Debug.LogWarning("DataManager ??????????????? ?????? ??? ????????????");
		}
		//FindObjectOfType<PlayGames>().AddScoreToLeaderboard(); // ?????? //?????? ?????? (????????????)

		gameObject.GetComponentInChildren<GraphicManager>().Die();
		gameObject.SetActive(false);
		Managers.Instance.GetUIManager<GameUIManager>().OpenFinishWindow();
	}

	public void CollideItem(GameObject item)
	{
		audioManager.Play("ItemCollide");
		gameObject.GetComponentInChildren<GraphicManager>().CollideItem(item);
	}

	public int GetTotalItemCount()
    {
		int r = 0;
		for(int i = 0; i < earnedItems.Length; i++)
        {
			r += earnedItems[i];
        }
		return r;
    }
}
