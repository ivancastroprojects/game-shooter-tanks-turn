using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    private Rigidbody rigidbody;
    private Vector3 finalTurretLookDir;

    //Movement variables of tank
    [Header("Turret Inputs")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    [Header("Turret Properties")]
    public Transform turretTransform;
    public float turrentLagSpeed;

    private Joystick LeftJoystick;
    private float horizontal;
    private float vertical;

    public AudioClip inIdle;
    public AudioClip inMovement;
    private bool controlAudio = false;

    //Variables to limit the distance of movement
    float radiusLimit; //radius of limit zone
    public Vector3 centerPosition; //center of limiter circle
    float distance; //tank <---> center of circle
    private GameObject containerCircle;

    private void Awake()
    {
        LeftJoystick = FindObjectOfType<Joystick>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Start()
    {
        //Radius of limit zone circle
        radiusLimit = 50;
        Destroy(containerCircle);
        //The center of circle will be the actual position at beginning of turn
        centerPosition = transform.localPosition;
        //We create a visualizable circle to see where is the limit of the zone
        containerCircle = new GameObject();
        containerCircle.transform.position = gameObject.transform.position + new Vector3(0f,1f,0f);
        DrawCircle(containerCircle, radiusLimit, 0.5f);
    }

    private void FixedUpdate()
    {
        TankMovement();
        TankRotate();
    }

    // Update is called once per frame
    void Update()
    {
        //Axis of tank. Vertical(Z) Horizontal(X)

        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");

        distance = Vector3.Distance(transform.position, centerPosition); //distance from tank to center of circle

        if (gameObject.GetComponent<TankShooting>().enabled == false)
            Destroy(containerCircle);
    }

    //If we are moving, we add the new position to the actual position vector of tank
    private void TankMovement()
    {
        float verticalJoystick = LeftJoystick.Vertical;

        if ((verticalJoystick > 0.2f || verticalJoystick < -0.2f || vertical != 0))
        {
            //If the distance is more than the radius, we limit his movement
            if (distance > radiusLimit)
            {
                Vector3 fromOriginToObject = transform.position - centerPosition; //Vector of distance from tank to center of circle
                fromOriginToObject *= radiusLimit / distance; //Multiply by radius //Divide by Distance
                transform.position = centerPosition + fromOriginToObject; //Position of tank + Math calcules
            }
            //If the distance is less than the radius, we can move free into
            else
            {
                Vector3 moveTank = (transform.forward * LeftJoystick.Vertical * moveSpeed + transform.forward * vertical * moveSpeed) * Time.deltaTime;
                rigidbody.MovePosition(rigidbody.position + moveTank);

                if (!controlAudio)
                {
                    controlAudio = true;
                    gameObject.GetComponent<AudioSource>().clip = inMovement;
                    gameObject.GetComponent<AudioSource>().Play();
                }
            }
        }
        else
        {
            rigidbody.MovePosition(rigidbody.position);

            if (controlAudio)
            {
                controlAudio = false;
                gameObject.GetComponent<AudioSource>().clip = inIdle;
                gameObject.GetComponent<AudioSource>().Play();
                gameObject.GetComponent<AudioSource>().loop = true;
            }
        }
    }

    //If we are rotating, we add the new position to the actual rotation vector of tank

    private void TankRotate()
    {
        float Joystickhorizontal = LeftJoystick.Horizontal;

        if (Joystickhorizontal > 0.2f || Joystickhorizontal < -0.2f || horizontal != 0)
        {
            float rotate = (horizontal * rotateSpeed + Joystickhorizontal * rotateSpeed) * Time.deltaTime;
            Quaternion rotateTank = Quaternion.Euler(0f, rotate, 0f);
            rigidbody.MoveRotation(rigidbody.rotation * rotateTank);
        }
        else
        {
            rigidbody.MoveRotation(rigidbody.rotation);
        }
    }

    public void DrawCircle(GameObject container, float radius, float lineWidth)
    {
        var segments = 360;
        var line = container.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;

        var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
        }

        line.SetPositions(points);
    }

    public void DestroyNewCircle()
    {
        Destroy(containerCircle);
    }
}