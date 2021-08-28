using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CameraController : MonoBehaviour
{

    public Transform target;
    public Transform rotater;
    public float distance;
    public Player player;
    private Camera gameCam;

    void Start()
    {
        gameCam = GetComponent<Camera>();
    }

    void Update()
    {
        gameCam.fieldOfView = Mathf.Clamp(player.velocity*10, 70, 120);
        distance = Mathf.Clamp(player.velocity/6, 1.1f, 2f);
        transform.position = target.position - transform.forward * distance + transform.up * (distance * 0.2f);
        transform.parent.eulerAngles = rotater.eulerAngles;
    }
}
