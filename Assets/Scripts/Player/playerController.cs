using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
 
 
    public float lookSpeed = 2f;
    public float lookXLimit = 90f;

    private bool cansee = false;
    public bool canSee {
        get
        {
            return cansee;
        }
        set
        {
            cansee = value;

            if (cansee) RenderSettings.ambientIntensity = 1.0f;
            else RenderSettings.ambientIntensity = 0.0f;
        }
    
    
    }
 
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
 
    public bool canMove = true;

    public float maxTriggerDistance;

    CharacterController characterController;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
 
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.RightShift)) canSee = !canSee;
        
 
        #region Handles Movment
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
 
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
 
        #endregion
 
        #region Handles Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
 
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
 
        #endregion
 
        #region Handles Rotation
        characterController.Move(moveDirection * Time.deltaTime);
 
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            float mouseXAxisRotation = Input.GetAxis("Mouse X");

            transform.rotation *= Quaternion.Euler(0, mouseXAxisRotation * lookSpeed, 0);

        }
 
        #endregion

        # region Handles Doors
        
        if (Input.GetKeyDown(KeyCode.E)) {
            CheckForDoor();
        }

        #endregion

    }

    public void CheckForDoor() {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if (!hit.collider.CompareTag("Door")) {
                return;
            }
            
            else if (hit.distance > maxTriggerDistance) {
                return;
            }

            GameObject door = hit.collider.gameObject;

            doorAnimator doorScript = door.GetComponentInChildren<doorAnimator>();

            if (doorScript.isTriggered) {
                doorScript.isTriggered = false;
            
                doorScript.TriggerClose();
            }

            else {
                doorScript.isTriggered = true;

                doorScript.TriggerOpen();
            }
            
        }
    }
}