using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Souls
{

    public class PlayerLocomotion : MonoBehaviour
    {
        PlayerManager playerManager;
        Transform cameraObject;
        InputHandler inputHandler;
        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;

        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Stats")]
        [SerializeField]
        float movementSpeed= 5.0f;
        [SerializeField]
        float sprintSpeed = 7.0f;
        [SerializeField]
        float rotationSpeed = 10.0f;
        [SerializeField]
        float fallingSpeed = 45f;
        [SerializeField] // this varible abd below doesn't really need to be shown in editor..
        float fallVelocity;
        [SerializeField]
        float gravityIntesity = 9.8f;
        [SerializeField]
        float jumpHeight = 10f;


        // Start is called before the first frame update
        void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();
            normalVector = Vector3.up;  // Initialize normalVector 
        }


        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        public void HandleMovement(float delta){


            // cannot interrupt rolls with added movement
            if (inputHandler.rollFlag)
                return;

            // cant move if falling
            // if (playerManager.isInteracting)
            //     return;


            moveDirection = cameraObject.forward * inputHandler.vertical;    
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;

            if(inputHandler.sprintFlag)
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
                moveDirection *= speed;
            }
            else
            {
                moveDirection *= speed;
                playerManager.isSprinting = false;
            }

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            // Vector3 projectVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);

            if(animatorHandler.canRotate){
                HandleRotation(delta);
            }
        }

        public void HandleRotation(float delta) {
            {
                Vector3 targetDir = Vector3.zero;
                float moveOverride = inputHandler.moveAmount;

                targetDir = cameraObject.forward * inputHandler.vertical;
                targetDir += cameraObject.right * inputHandler.horizontal;

                targetDir.Normalize();
                targetDir.y = 0;

                if(targetDir == Vector3.zero)
                {
                    targetDir = myTransform.forward;
                }
                float rs = rotationSpeed;

                Quaternion tr = Quaternion.LookRotation(targetDir);
                Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

                myTransform.rotation = targetRotation;
            }
        }

         public void HandleRollingAndSprinting(float delta)
        {
            //do this so we cannot roll whenever, when other actions are happening
            if (animatorHandler.anim.GetBool("isInteracting"))
                return;

            //if we Can roll, calulate roll direction
            if (inputHandler.rollFlag)
            {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;

                //if you have any movement at all when roll button is pressed
                if(inputHandler.moveAmount > 0)
                {

                    // play target animation Roll in dirtection of movemtm
                    animatorHandler.PlayTargetAnimation("Rolling", true);
                    moveDirection.y = 0; // not flying up or down

                    // bounce player upwards a little 
                    //rigidbody.AddForce((Vector3.up * walkingSpeed) + (moveDirection.normalized * 4f), ForceMode.Impulse);

                    // rotate into direction we are rolling
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                }
                else // if no movement on button press
                {
                    //play back dodge animation
                    animatorHandler.PlayTargetAnimation("Backstep", true);
                }
            }
        }
        #endregion
    }
}
