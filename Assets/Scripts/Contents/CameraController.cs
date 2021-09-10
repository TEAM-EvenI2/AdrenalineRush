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


    public ParticleSystem particle;
    private ParticleSystem speedLine;
    public float particleDistance = 5f;
    public float drawSpeedlineVelocityMin = 6f;

    void Start()
    {
        gameCam = GetComponent<Camera>();

        speedLine = Instantiate(particle);
        speedLine.transform.localPosition = new Vector3(particleDistance, particleDistance * 0.5f, 0);
        speedLine.gameObject.SetActive(false);
    }

    void Update()
    {
        Vector3 camShakePos = Vector3.zero;
        if (currentShakeTime > 0)
        {
            camShakePos = Random.insideUnitSphere * currentShakePower;
            currentShakeTime -= Time.deltaTime;
        }

            distance = Mathf.Clamp(player.curVelocity/5, 1.1f, 2f);
            transform.position = target.position - transform.forward * distance + transform.up * (distance * 0.2f) + camShakePos;
        
        gameCam.fieldOfView = Mathf.Clamp(player.curVelocity * 12, 70, 110);

        transform.parent.eulerAngles = rotater.eulerAngles;

        if (player.curVelocity >= drawSpeedlineVelocityMin)
            speedLine.gameObject.SetActive(true);
        else
            speedLine.gameObject.SetActive(false);

        if (speedLine.gameObject.activeSelf)
        {
            speedLine.transform.localPosition = new Vector3(particleDistance, 0, 0);
        }
    }

    public void ShakeCam(float duration, float power)
    {
        currentShakeTime = duration;
        currentShakePower = power;
    }
}
