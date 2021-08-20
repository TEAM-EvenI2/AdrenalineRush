using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour
{
	private Player player;
	public float deathCountdown = -1f;

	private void Awake()
	{
		player = transform.root.GetComponent<Player>();
	}


	private void OnTriggerEnter(Collider collider)
	{
		if(collider.tag.Equals("Obstacle"))
		player.Die();
		else if (collider.tag.Equals("Item"))
        {

			player.earnedScore += collider.GetComponentInParent<ScoreItem>().point;
			Destroy(collider.gameObject);
        }
	}
}
