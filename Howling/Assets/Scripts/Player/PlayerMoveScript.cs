using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Howling
{
    public class PlayerMoveScript : MonoBehaviour
    {
        public Transform respawnPos;
        private Rigidbody _rigidbody;

        private enum PlayerState { idle, walk, run, die };
        private PlayerState state = PlayerState.idle;

        public Transform playerCamera;
        private Animator animator;
        private StatusController statusController;
        private TutorialController tutorialController;
        
        // WASD move
        private float velocity = 0.0f;
        public static float walkAcc = 10f;
        public static float runAcc = 20f;
        private static float walkMaxVel = 10f;
        private static float runMaxVel = 20f;
        private Vector3 moveVector = new Vector3(0, 0, 0);
        private float verticalMove = 0.0f;
        private float horizontalMove = 0.0f;

        // jump
        //private float jumpPower = 5.0f;
        //private bool isJump = false;

        // mouse rotation
        private float minY = -45.0f;
        private float maxY = 45.0f;
        private float sensX = 300.0f;
        private float sensY = 150.0f;
        private float rotationX = 0.0f;
        private float rotationY = 0.0f;
        private float mouseRotationX = 0.0f;
        private float mouseRotationY = 0.0f;

        void Awake()
        {
            //Screen.SetResolution(Screen.width, (Screen.width * 16) / 9, true);
            
            //playerCamera = Camera.main.transform;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            animator = GetComponent<Animator>();
            statusController = FindObjectOfType<StatusController>();
            tutorialController = FindObjectOfType<TutorialController>();
            _rigidbody = GetComponent<Rigidbody>();
        }
        void Update()
        {
            mouseRotationX = Input.GetAxis("Mouse X");
            mouseRotationY = Input.GetAxis("Mouse Y");
            verticalMove = Input.GetAxis("Vertical");
            horizontalMove = Input.GetAxis("Horizontal");
            if (Input.GetButton("Run") && (statusController.GetCurrentMp() > 0))
            {
                if (tutorialController.currentShow > 2)
                    state = PlayerState.run;
            }
            else state = PlayerState.walk;

            //if (!Slot.isSlotDrag && tutorialController.currentShow > 0)
            {
                PlayerRotation();
            }
            if (tutorialController.currentShow > 1)
                PlayerMove();
        }

        private void PlayerRotation()
        {
            // mouse rotation
            if ((mouseRotationX > Mathf.Epsilon) || (mouseRotationX < -Mathf.Epsilon))
            {
                rotationX += mouseRotationX * sensX * Time.deltaTime;
                transform.localEulerAngles = new Vector3(0, rotationX, 0);
            }
            if ((mouseRotationY > Mathf.Epsilon) || (mouseRotationY < -Mathf.Epsilon))
            {
                rotationY += mouseRotationY * sensY * Time.deltaTime;
                rotationY = Mathf.Clamp(rotationY, minY, maxY);
                playerCamera.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
            }

            //tutorialController.isPlayerRotation = true;
        }

        private void PlayerMove()
        {
            // 방향키 안눌렀을 때 
            if (((horizontalMove < Mathf.Epsilon) && (horizontalMove > -Mathf.Epsilon))
            && ((verticalMove < Mathf.Epsilon) && (verticalMove > -Mathf.Epsilon)))
            {
                velocity = 0.0f;
                moveVector.Set(horizontalMove, -0.0f, verticalMove);
                moveVector = moveVector.normalized;
                transform.Translate(moveVector);

                animator.SetInteger("State", 0);
                return;
            }
            
            // WASD/방향키 move
            if (state == PlayerState.walk)
            {
                if (velocity < walkMaxVel)
                {
                    velocity = velocity + walkAcc * Time.deltaTime;
                    velocity = Mathf.Clamp(velocity, 0.0f, walkMaxVel);
                }
                else
                {
                    velocity = velocity - runAcc * Time.deltaTime;
                    velocity = Mathf.Clamp(velocity, 0.0f, runMaxVel);
                }

                animator.SetInteger("State", 1);
            }
            else if (state == PlayerState.run)
            {
                velocity = velocity + runAcc * Time.deltaTime;
                velocity = Mathf.Clamp(velocity, 0.0f, runMaxVel);
        
                statusController.DecreaseSp(1000 * Time.deltaTime);

                tutorialController.isPlayerRun = true;

                animator.SetInteger("State", 2);
            }

            moveVector.Set(horizontalMove, -0.0f, verticalMove);
            moveVector = moveVector.normalized * velocity * Time.deltaTime;
            transform.Translate(moveVector);
            //_rigidbody.MovePosition(transform.position + moveVector);
            //_rigidbody.AddForce(moveVector);

            tutorialController.isPlayerMove = true;
        }

        public void Respawn()
        {
            statusController.ResetForRespawn();
            transform.position = respawnPos.position;
            transform.rotation = respawnPos.rotation;
            playerCamera.rotation = Quaternion.Euler(0, 0, 0);
            rotationX = rotationY = 0;
        }

        //private void PlayerJump()
        //{
        //    if (!isJump) return;
        //    rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        //    isJump = false;
        //}
    }
}