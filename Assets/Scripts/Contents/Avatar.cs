using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour
{
	private Player player;
	public float deathCountdown = -1f;
	public float maxRotateX = 60f;
	private float xRotate = 0;
	private float smoothRoatation = 0f;
	public float accelRotationSpeed = 1.2f;
	public float balanceRotationSpeed = 0.3f;
	private CameraController cameraController;

	private void Awake()
	{
		player = transform.root.GetComponent<Player>();
	}

	private void Start()
	{
		cameraController = player.gameCam.GetComponent<CameraController>();
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.tag.Equals("Obstacle"))
		{
			player.Hit();
			cameraController.ShakeCam(cameraController.playerCollisionShakeDuration, cameraController.playerCollisionShakePower);


			if (collider.GetComponentInParent<ScoreItem>() != null)
			{
				Destroy(collider.GetComponentInParent<ScoreItem>().gameObject);
			}
			else if (player.invincible)
			{
				Transform effect = Managers.Instance.GetScene<GameScene>().ec.CreateEffect("Explosion", true);
				effect.position = transform.position;
				Destroy(collider.gameObject);
			}

		}
		else if (collider.tag.Equals("Item"))
		{
			player.CollideItem(collider.gameObject);
			ScoreItem item = collider.GetComponentInParent<ScoreItem>();
			if (item.id >= 0 && item.id < player.earnedItems.Length)
				player.earnedItems[item.id]++;

			player.earnedScore += item.point;
		}
		//player.GetComponentInChildren<PlayerGraphicManager>().Damaged();
	}

	private void Update()
	{
		smoothRoatation = Mathf.Clamp(player.curInput* accelRotationSpeed + smoothRoatation, -maxRotateX, maxRotateX);
		xRotate = Mathf.Clamp(smoothRoatation * 1 + transform.localRotation.x, -maxRotateX, maxRotateX);
		if (smoothRoatation > 0)
		{
			smoothRoatation -= balanceRotationSpeed;
		}
		else if (smoothRoatation < 0)
		{
			smoothRoatation += balanceRotationSpeed;
		}
		transform.localRotation = Quaternion.Euler(xRotate, transform.localRotation.y, transform.localRotation.z);
	}
}
