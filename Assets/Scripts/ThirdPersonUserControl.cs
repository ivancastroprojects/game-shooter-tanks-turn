using System;
using UnityEngine;

    [RequireComponent(typeof (TankController))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private TankController m_tank;             // A reference to the TankController on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        ///////////private Vector3 m_Move;
        [HideInInspector]
        public bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        [HideInInspector]
        public float Hinput;
        [HideInInspector]
        public float Vinput;
        
        private void Start()
        {
            if (Camera.main != null) 
                m_Cam = Camera.main.transform;

            m_tank = GetComponent<TankController>();
        }

        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // We want to use our own axis
            //float h = CrossPlatformInputManager.GetAxis("Horizontal");
            //float v = CrossPlatformInputManager.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                /////////m_Move = Vinput*m_CamForward + Hinput*m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                //////////m_Move = Vinput*Vector3.forward + Hinput*Vector3.right;
            }
#if !MOBILE_INPUT
			// walk speed multiplier
	        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            //////////m_tank.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
        }
    }

