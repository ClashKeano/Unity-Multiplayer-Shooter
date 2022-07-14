using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform viewPoint;
    private float vertRotation;
    private Vector2 mouseInput;
    public float mouseSens = 1f;

    public float moveSpeed = 1f;
    private Vector3 moveInput, moveDir;

    public CharacterController charControl;

    private Camera cam;

    public float jump = 7;
   
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Prevent cursor from clicking outside the game window

        cam = Camera.main; 
    }

    // Update is called once per frame
    void Update()
    {
        /* Camera Movement */
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSens; // Get mouse input

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z); // Move camera on X axis based on mouse input

        vertRotation -= mouseInput.y;
        vertRotation = Mathf.Clamp(vertRotation, -60, 60); // Limit vertical rotation between -60 and 60 degrees

        viewPoint.rotation = Quaternion.Euler( vertRotation, viewPoint.rotation.eulerAngles.y,  viewPoint.rotation.eulerAngles.z); // Move camera on Y axis based on mouse input
        /*******************/


        /* Player Movement */
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")); // Get keyboard input for player movement

        float velY = moveDir.y;
        moveDir = ((transform.forward * moveInput.z) + (transform.right * moveInput.x).normalized) * moveSpeed;  //Set the values to move in direction relative to current view point location
        moveDir.y = velY;

        if (Input.GetButtonDown("Jump") && charControl.isGrounded){
            moveDir.y = jump;
            // Add jump functionality
        }

        moveDir.y += Physics.gravity.y * Time.deltaTime; // Apply gravity to movement

        charControl.Move (moveDir * Time.deltaTime); // Move player
        /*******************/

        /* Cursor Lock*/
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None; // Free mouse if player presses escape
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked; // Re-lock cursor if player clicks within game window again
            }
            
        }
        /*******************/

    }

    // Called once per frame, after every update function is complete
    private void LateUpdate()
    {
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;

        // Tie main camera to view point location
    }
}
