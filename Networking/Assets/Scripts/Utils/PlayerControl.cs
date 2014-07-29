using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class PlayerControl : MonoBehaviour 
    {
        [SerializeField()]
        private string m_PlayerName;
        [SerializeField()]
        private float m_MoveSpeed = 6.0f;
        [SerializeField()]
        private float m_TurnSpeed = 45.0f;
        [SerializeField()]
        private float m_JumpSpeed = 8.0f;
        [SerializeField()]
        private float m_Gravity = 20.0f;

        private Vector3 m_Velocity = Vector3.zero;
        private CharacterController m_Controller;

        [SerializeField()]
        private NetworkPlayer m_Owner;

        [SerializeField()]
        private Camera m_Camera;

        public Camera cam
        {
            get { return m_Camera; }
            set { m_Camera = value; }
        }

		// Use this for initialization
		void Start () 
        {
            m_Controller = GetComponent<CharacterController>();
            m_Camera = GetComponentInChildren<Camera>();
		}
		
		// Update is called once per frame
		void Update () 
        {
            if (Network.player == m_Owner)
            {
                float forwardSpeed = Input.GetAxis("Vertical");
                float strafeSpeed = Input.GetAxis("Horizontal");
                float turnSpeed = Input.GetAxis("Mouse X") * m_TurnSpeed * Time.deltaTime;




                if (m_Controller != null)
                {

                    if (m_Controller.isGrounded)
                    {
                        m_Velocity = new Vector3(strafeSpeed, 0.0f, forwardSpeed);
                        m_Velocity = transform.TransformDirection(m_Velocity);
                        m_Velocity *= m_MoveSpeed;
                        if (Input.GetButton("Jump"))
                        {
                            m_Velocity.y = m_JumpSpeed;
                        }
                    }
                    m_Velocity.y -= m_Gravity * Time.deltaTime;
                    m_Controller.Move(m_Velocity * Time.deltaTime);
                    transform.Rotate(0.0f, turnSpeed, 0.0f);
                }
            }
		}

        void OnNetworkInstantiate(NetworkMessageInfo aInfo)
        {
            m_Owner = aInfo.sender;
        }
	}

}