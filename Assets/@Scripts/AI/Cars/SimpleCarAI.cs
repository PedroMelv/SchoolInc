using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCarAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float safeSpeed;


    private float targetSpeed;
    private float currentSpeed;
    [SerializeField] private float speedChangeMultiplier = 2f;

    [Space]

    private bool stressed;
    private float stoppedLimit = 8f;
    [SerializeField] private float stoppedLimitMax = 8;

    [SerializeField] private Vector3 carOffset = new Vector2(0,0);
    [SerializeField] private Vector2 carSize = new Vector2(1f,1f);
    
    [SerializeField] private float detectionRange = 2f;
    [SerializeField] private float safeSpeedDetectionRange = 7f;

    [SerializeField] private LayerMask stopLayer;

    private bool stopping;
    private bool isOnSafeSpeed;

    private void Start()
    {
        stoppedLimit = stoppedLimitMax;
    }

    private void Update()
    {
        CheckForStop();

        if(stressed)
        {
            stoppedLimit += Time.deltaTime;

            if(stoppedLimit >= stoppedLimitMax)
            {
                stressed = false;
            }
        }
        else
        {
            if (stoppedLimit <= 0)
                stressed = true;
            if (stopping)
            {
                stoppedLimit -= Time.deltaTime;
                targetSpeed = 0;
            }
            else
            {
                //stoppedLimit -= Time.deltaTime;
                if (isOnSafeSpeed)
                {
                    targetSpeed = safeSpeed;
                }
            }
        }

        if ((!stopping && !isOnSafeSpeed) || stressed) targetSpeed = moveSpeed;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedChangeMultiplier);

        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }

    private void CheckForStop()
    {
        stopping = Physics.BoxCast(transform.position + carOffset, carSize / 2, transform.forward, transform.rotation, detectionRange, stopLayer);
        isOnSafeSpeed = Physics.BoxCast(transform.position + carOffset, carSize / 2, transform.forward, transform.rotation, safeSpeedDetectionRange, stopLayer);
    }

    private void OnDrawGizmosSelected()
    {
        RotaryHeart.Lib.PhysicsExtension.DebugExtensions.DebugBoxCast(transform.position + carOffset, carSize / 2, transform.forward, detectionRange, Color.red, transform.rotation);
        RotaryHeart.Lib.PhysicsExtension.DebugExtensions.DebugBoxCast(transform.position + carOffset, carSize / 2, transform.forward, safeSpeedDetectionRange, Color.green, transform.rotation);
    }
}