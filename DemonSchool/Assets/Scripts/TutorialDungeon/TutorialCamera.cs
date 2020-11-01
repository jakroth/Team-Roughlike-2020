using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCamera : MonoBehaviour
{
    public float shakeTimer;
    public float shakeAmount;

    private bool shaking = false;
    private Vector3 originPos;

    private void Start()
    {
        originPos = transform.position;
    }

    void Update()
    {
        if (shakeTimer >= 0)
        {
            shaking = true;
            Vector2 shakePos = Random.insideUnitCircle * shakeAmount;
            transform.position = new Vector3(transform.position.x + shakePos.x, transform.position.y + shakePos.y, transform.position.z);
            StartCoroutine("EndShaking", shakeTimer);
            shakeTimer -= Time.deltaTime;
        }
    }   

    public void ShakeCamera(float shakePwr, float shakeDur)
    {
        shakeAmount = shakePwr;
        shakeTimer = shakeDur;
    }

    IEnumerator EndShaking(float shakeTimer)
    {
        yield return new WaitForSeconds(shakeTimer);
        shaking = false;
    }
}
