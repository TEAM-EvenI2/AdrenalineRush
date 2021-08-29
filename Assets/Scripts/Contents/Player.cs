using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

	public MapSystem mapSystem;

	public float velocity;
	public float rotationVelocity;


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

	public int earnedScore;

	public float health = 100;


	private void Start()
	{
		world = mapSystem.transform.parent;
		rotater = transform.GetChild(0);
		currentPipe = mapSystem.SetupFirstPipe();
		SetupCurrentPipe();
	}

	private void Update()
	{
		float delta = velocity * Time.deltaTime;
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
			health += velocity * Time.deltaTime;
			if (health > 100)
				health = 100;
        }
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
		avatarRotation +=
			rotationVelocity * Time.deltaTime * Input.GetAxis("Horizontal");
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

	public void Hit()
	{

		health -= 34;
		if (health <= 0)
			Die();
	}

	private void Die()
	{
		gameObject.SetActive(false);
		Managers.Instance.GetUIManager<GameUIManager>().ActiveRe();
	}
}
