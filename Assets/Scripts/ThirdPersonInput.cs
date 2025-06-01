using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonInput : MonoBehaviour
{

    public Joystick LeftJoystick;
    public TouchField TouchField;
    protected ThirdPersonUserControl Control;
    public Camera m_camera;
    protected float CameraAngleY;
    protected float CameraPosY;
    protected float CameraAngleSpeed = 0.2f;
    protected float CameraPosSpeed = 0.2f;
    protected ParticleSystem ShootParticle;
    public Transform tower;
    public float xAxisClamp;
    public float touchSensibiliy;
    Vector3 towerRotation;
    
    private void Start()
    {
        LeftJoystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<Joystick>();
        TouchField = GameObject.FindGameObjectWithTag("TouchField").GetComponent<TouchField>();
    }

    // Update is called once per frame
    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hitInfo;

        if (TouchField.Pressed)
        {
            float distX = TouchField.TouchDist.x;
            float distY = TouchField.TouchDist.y;

            float rotAmountX = distX * touchSensibiliy * Time.deltaTime;
            float rotAmountY = distY * touchSensibiliy * Time.deltaTime;
            xAxisClamp += rotAmountX;
            Vector3 canyon2 = tower.transform.rotation.eulerAngles;

            canyon2.x -= rotAmountY;
            canyon2.z = 0;
            canyon2.y += rotAmountX;
            if (xAxisClamp > 90)
            {
                xAxisClamp = 90;
                canyon2.x = 90;
            }
            else if (xAxisClamp < -90)
            {
                xAxisClamp = -90;
                canyon2.x = 270;
            }
            tower.rotation = Quaternion.Euler(canyon2.x, canyon2.y, 0);
        }
        else
        {
            tower.rotation = gameObject.transform.rotation;
        }
    }
}