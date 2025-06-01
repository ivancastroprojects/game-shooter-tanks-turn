using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//let camera follow target
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float lerpSpeed;

    private Vector3 offset;

    private Vector3 targetPos;

    private Vector3 actualPosition;

    private void Start()
    {
        if (target == null) return;

        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        //targetPos = target.position + offset;
        //transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
        float desiredXAngle = target.eulerAngles.x;
        float desiredYAngle = target.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(desiredXAngle, desiredYAngle, 0);
        transform.position = target.position + (rotation * offset);

        if (GameObject.FindGameObjectWithTag("TouchField").GetComponent<TouchField>().Pressed)
        {
            Camera.main.transform.position = target.position;
        }
        else
        {
            transform.LookAt(target);
        }
       
    }
}