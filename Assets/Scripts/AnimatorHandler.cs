using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Souls
{
    public class AnimatorHandler : MonoBehaviour
    {
        PlayerManager playerManager;
        public Animator anim;
        InputHandler inputHandler;
        PlayerLocomotion playerLocomotion;
        // Collider playerCollider;
        int vertical;
        int horizontal;
        public bool canRotate;

        public void Initialize()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            // playerCollider = GetComponentInParent<Collider>();
            anim = GetComponent<Animator>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            vertical = Animator.StringToHash("vertical");
            horizontal = Animator.StringToHash("horizontal");
        }

        private void Update()
        {
            // tie animator bool to playermanger bool
            // anim.SetBool("isInAir", playerManager.isInAir);
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting) //
        {
            #region Vertical
            float v = 0;

            //clamp Vertical values
            if(verticalMovement > 0 && verticalMovement < 0.55f)
            {
                v = 0.5f;
            }
            else if(verticalMovement > 0.55f)
            {
                v = 1;
            }    
            else if (verticalMovement < 0 && verticalMovement > -0.55f)
            {
                v = -0.5f;
            }
            else if (verticalMovement < -0.55f)
            {
                v = -1;
            }
            else
            {
                v = 0;
            }
            #endregion

            #region Horizontal
            float h = 0;

            //clamp Horizontal values
            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                h = 0.5f;
            }
            else if (horizontalMovement > 0.55f)
            {
                h = 1;
            }
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if (horizontalMovement < -0.55f)
            {
                h = -1;
            }
            else
            {
                h = 0;
            }
            #endregion

            if (isSprinting && verticalMovement > 0) // check for bool AND movement
            {
                v = 2; // up value to 2, cueing the sprint animation
                h = horizontalMovement;
            }

            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        public void PlayTargetAnimation(string targetAnim, bool isInteracting)
        {
            anim.applyRootMotion = isInteracting; // animation will only play if bool set to true
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.2f); // crossfade for smoother transitions
        }

        public void CanRotate()
        {
            canRotate = true;
        }

        public void StopRotation()
        {
            canRotate = false;
        }

        public void EnableCombo()
        {
            anim.SetBool("canDoCombo", true);
        }

        public void DisableCombo()
        {
            anim.SetBool("canDoCombo", false);
        }

        // public void EnablePlayerBoxCollider()
        // {
        //     playerCollider.enabled = true;
        // }

        // public void DisablePlayerBoxCollider()
        // {
        //     playerCollider.enabled = false;
        // }

        public void OnAnimatorMove()
        {
            //dont run codew below if its not interacting
            if (playerManager.isInteracting == false)
                return;

            // readjust our player model to the centre of its game object after roll animation
            float delta = Time.deltaTime;
            playerLocomotion.rigidbody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            playerLocomotion.rigidbody.velocity = velocity;
        }

    }
}