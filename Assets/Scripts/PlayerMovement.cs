using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class PlayerMovement : MonoBehaviour{
    public float turnSpeed = 20f;
    public float moveSpeed = 1f;
    public float jumpHeight = 2000f;
    public int moveSpeedMultiplier = 1;
    public bool isGrounded = true;

    Animator m_Animator;
    Vector3 m_Movement;
    Rigidbody m_Rigidbody;
    Quaternion m_Rotation = Quaternion.identity;    //Default value is butts. all 0s for default rotation is butts. 
    Vector3 desiredForward;

    void Start(){
        m_Animator = GetComponent<Animator>();      //reference to our Animator so we can update it
        m_Rigidbody = GetComponent<Rigidbody>();    //reference to Rigid Body so we can update it for proper collision 
    }

    // Update is called once per frame. FixedUpdate is called once per physics update
    void FixedUpdate(){ 
        // ========================================================================== WALKING =====================================================
        if((Input.GetKey(KeyCode.LeftShift)) || (Input.GetKey(KeyCode.RightShift))){
            moveSpeedMultiplier = 2;
        } else {
            moveSpeedMultiplier = 1;
        }
        if (Input.GetKey(KeyCode.Space) && isGrounded) {
            m_Rigidbody.AddForce(Vector3.up * jumpHeight);
        }
        float horizontal = Input.GetAxisRaw("Horizontal");                 //gets Horizontal Axis to discern whether there is horizonal movement
        float vertical = Input.GetAxisRaw("Vertical");                     //gets Vertical Axis to discern whether there is vertical movement
        m_Movement.Set(horizontal, 0f, vertical);                       //updates movement based on previous axiss changes
        m_Movement.Normalize();                                         //normalizes movement due to approximations against an isoceles triangle (1,1,1.444)
        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f); //testing for animation change
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);     //same as above
        bool isWalking = hasHorizontalInput || hasVerticalInput;        //logic to discern if we are in motion
        m_Animator.SetBool("IsWalking", isWalking);                     //updates animator's condition
        if (!isWalking){
             m_Rigidbody.velocity = Vector3.zero;
        }
        // ======================================================================== ROTATION =======================================================
        desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f); //This is what we want our "forward" to be. We use delta Time so that FPS doesn't affect how fast we turn.                                                                    
        m_Rotation = Quaternion.LookRotation(desiredForward);                                                          //Reassign rotation after initializing it 


    }

    void OnAnimatorMove(){
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude * moveSpeed * moveSpeedMultiplier); //Move the rigidbody of the prefab the same way that the animated character model moves. deltaPosition is the change in position due to Root Motion?????
        m_Rigidbody.MoveRotation(m_Rotation);                                                             //Rotate the rigidbody of the prefab the same way that the animated character model rotates.
    }

    void OnCollisionEnter(Collision collision) {
        Debug.Log("Entered");
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;
        }
        Debug.Log("isGrounded is" + isGrounded);
    }

    void OnCollisionExit(Collision collision) {
        Debug.Log("Exited");
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = false;
        }
        Debug.Log("isGrounded is" + isGrounded);
    }

}
