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
    public float playerCollisionShakeDuration;
    public float playerCollisionShakePower;
    private float currentShakeTime = 0f;
    private float currentShakePower = 0f;
    private Vector3 camInitLocalPos;


    public ParticleSystem particle;
    private ParticleSystem speedLine;
    public float particleDistance = 5f;
    public float drawSpeedlineVelocityMin = 6f;

    void Start()
    {
        gameCam = GetComponent<Camera>();
        camInitLocalPos = transform.localPosition;

        speedLine = Instantiate(particle);
        speedLine.transform.localPosition = new Vector3(particleDistance, 0, 0);
        speedLine.gameObject.SetActive(false);
    }

    void Update()
    {
        if (currentShakeTime > 0)
        {
            transform.localPosition = Random.insideUnitSphere * currentShakePower + camInitLocalPos;
            currentShakeTime -= Time.deltaTime;
        }
        else
        {
            transform.position = camInitLocalPos;
            currentShakeTime = 0f;

            gameCam.fieldOfView = Mathf.Clamp(player.velocity*10, 70, 120);
            distance = Mathf.Clamp(player.velocity/5, 1.1f, 2f);
            transform.position = target.position - transform.forward * distance + transform.up * (distance * 0.2f);
        }
        transform.parent.eulerAngles = rotater.eulerAngles;       

        if (player.velocity >= drawSpeedlineVelocityMin)
            speedLine.gameObject.SetActive(true);
        else
            speedLine.gameObject.SetActive(false);
    }

    public void ShakeCam(float duration, float power)
    {
        currentShakeTime = duration;
        currentShakePower = power;
    }
}
