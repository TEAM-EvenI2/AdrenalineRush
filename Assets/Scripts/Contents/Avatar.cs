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

	private void Awake()
	{
		player = transform.root.GetComponent<Player>();
	}


	private void OnTriggerEnter(Collider collider)
	{
        if (collider.tag.Equals("Obstacle"))
            player.Die();
        else if (collider.tag.Equals("Item"))
        {

            player.earnedScore += collider.GetComponentInParent<ScoreItem>().point;
            Destroy(collider.gameObject);
        }
    }

	private void Update()
	{
		smoothRoatation = Mathf.Clamp(Input.GetAxis("Horizontal") * accelRotationSpeed + smoothRoatation, -maxRotateX, maxRotateX);
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
