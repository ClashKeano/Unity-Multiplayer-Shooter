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

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Prevent cursor from clicking outside the game window
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

        moveDir = (transform.forward * moveInput.z) + (transform.right * moveInput.x).normalized;  //Set the values to move in direction relative to current view point location

        transform.position += moveDir * moveSpeed * Time.deltaTime; // Move player
        /*******************/

    }
}
