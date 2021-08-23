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
        Debug.Log(player.velocity);
        gameCam.fieldOfView = Mathf.Clamp(player.velocity*13, 70, 120); // TODO: fov 따라 카메라 distance도 동적으로 변경할지 결정하기.
        transform.position = target.position - transform.forward * distance;
        transform.parent.eulerAngles = rotater.eulerAngles;
    }
}
