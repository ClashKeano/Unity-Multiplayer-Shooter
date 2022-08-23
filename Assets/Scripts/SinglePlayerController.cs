using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerController : MonoBehaviour
{
    public static SinglePlayerController instance;

    private void Awake()
    {
        instance = this;
    }

    public Transform viewPoint;
    private float vertRotation;
    private Vector2 mouseInput;
    public float mouseSens = 1f;

    public float moveSpeed = 1f;
    private Vector3 moveInput, moveDir;

    public CharacterController charControl;

    private Camera cam;
    public float jump = 7;

    public Weapons[] allWeapons;
    public int currentWeapon;
    public float aimSpeed = 5f;

    public int maxHealth = 100;
    int currentHealth;


    public AudioSource fsFast, fsSlow;


    // Start is called before the first frame update
    void Start()
    {


        Cursor.lockState = CursorLockMode.Locked; //Prevent cursor from clicking outside the game windowzzz

        cam = Camera.main;

        //UIController.instance.weaponHeatSlider.maxValue = maxWeaponHeat;

        Transform newSpawnTransform = SpawnManager.instance.spawn(); // Generate random spawn tranform values
        currentHealth = maxHealth;
        // transform.position = newSpawnTransform.position;
        // transform.rotation = newSpawnTransform.rotation;  // Apply above transform values to plaher spawn location

    }

    // Update is called once per frame
    void Update()
    {
            /* Camera Movement */
            mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSens; // Get mouse input

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z); // Move camera on X axis based on mouse input

            vertRotation -= mouseInput.y;
            vertRotation = Mathf.Clamp(vertRotation, -60, 60); // Limit vertical rotation between -60 and 60 degrees

            viewPoint.rotation = Quaternion.Euler(vertRotation, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z); // Move camera on Y axis based on mouse input
            /*******************/


            /* Player Movement */
            moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")); // Get keyboard input for player movement

            float velY = moveDir.y;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveDir = ((transform.forward * moveInput.z) + (transform.right * moveInput.x).normalized) * (moveSpeed + 4);  //the player will run if press left shift

                if (!fsFast.isPlaying && moveDir != Vector3.zero) // If footsteps aren't already playing and player is not stationary
                {
                    fsFast.Play();
                    fsSlow.Stop();
                }
            }
            else
            {

                moveDir = ((transform.forward * moveInput.z) + (transform.right * moveInput.x).normalized) * moveSpeed;  //the player walk normally

                if (!fsSlow.isPlaying && moveDir != Vector3.zero) // If footsteps aren't already playing and player is not stationary
                {
                    fsSlow.Play();
                    fsFast.Stop();
                }
            }

            if (moveDir == Vector3.zero || !charControl.isGrounded) // If player is not moving or not grounded, stop footsteps 
            {
                fsSlow.Stop();
                fsFast.Stop();
            }

            moveDir.y = velY;


            if (Input.GetButtonDown("Jump") && charControl.isGrounded)
            {
                moveDir.y = jump;
                // Add jump functionality
            }

            moveDir.y += Physics.gravity.y * Time.deltaTime; // Apply gravity to movement

            charControl.Move(moveDir * Time.deltaTime); // Move player

            /*******************/


            /* Gun Shooting and Overheating */

            //allWeapons[currentWeapon].muzzleFlash.gameObject.SetActive(false);

            //if (!overheated)
            //{
            //    if (Input.GetMouseButtonDown(0))
            //    {
            //        Shoot();
            //    }

            //    if (Input.GetMouseButton(0) && (allWeapons[currentWeapon].isAutomatic == true))
            //    {
            //        shotCounter -= Time.deltaTime;

            //        if (shotCounter <= 0)
            //        {
            //            Shoot();
            //        }

            //    }

            //    heatCounter -= coolRate * Time.deltaTime;
            //}
            //else
            //{
            //    heatCounter -= overheatCoolRate * Time.deltaTime;
            //    if (heatCounter <= 0)
            //    {
            //        overheated = false;

            //        UIController.instance.overheatedMessage.gameObject.SetActive(false);

            //    }
            //}

            //if (heatCounter < 0)
            //{
            //    heatCounter = 0;
            //}

            //UIController.instance.weaponHeatSlider.value = heatCounter;
            /*******************/


            /* Weapon Controls */

            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            {
                currentWeapon++; // Scroll up through weapons with mouse wheel

                if (currentWeapon >= allWeapons.Length)
                {
                    currentWeapon = 0; // Allow for looping when scrolling
                }

                switchWeapon();

        }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
            {
                currentWeapon--;  // Scroll down through weapons with mouse wheel

                if (currentWeapon < 0)
                {
                    currentWeapon = allWeapons.Length - 1;// Allow for looping when scrolling
                }

                switchWeapon();


        }

            for (int i = 1; i <= allWeapons.Length; i++)
            {
                if (Input.GetKeyDown(i.ToString()))
                {
                    currentWeapon = i - 1;
                    switchWeapon();
                          // Change weapon based on number key input
                }
            }

            



            if (Input.GetMouseButton(1))
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, allWeapons[currentWeapon].aimZoom, aimSpeed * Time.deltaTime); // Zoom in from current view to zoom view at a rate determined by aim speed
            }
            else
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f, aimSpeed * Time.deltaTime); // Reset camera back to default field of view
            }


            /* Cursor Lock*/
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None; // Free mouse if player presses escape
            }
            else if (Cursor.lockState == CursorLockMode.None)
            {
                if (Input.GetMouseButtonDown(0) && !UIController.instance.optionsScreen.activeInHierarchy)
                {
                    Cursor.lockState = CursorLockMode.Locked; // Re-lock cursor if player clicks within game window again
                }

            }
            /*******************/

        }
    
    //private void Shoot()
    //{
    //    Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f)); // Create direction for raycast from centre of player view

    //    ray.origin = cam.transform.position;

    //    if(Physics.Raycast(ray, out RaycastHit hit))
    //    {
    //        Debug.Log("We Hit " + hit.collider.gameObject.name); // Print to debug log the name of the object that was hit by raycast

    //        GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * .001f), Quaternion.LookRotation(hit.normal, Vector3.up)) ; // Place bullet impact in correct location

    //        Destroy(bulletImpactObject, 5f); // Remove bullet impacts after 5 seconds
    //    }

    //    allWeapons[currentWeapon].muzzleFlash.gameObject.SetActive(true); //Activate muzzle flush when firing


    //    shotCounter = allWeapons[currentWeapon].fireRate;

    //    heatCounter += allWeapons[currentWeapon].heatPerShot;

    //    if(heatCounter >= maxWeaponHeat)
    //    {
    //        heatCounter = maxWeaponHeat;

    //        overheated = true;

    //        UIController.instance.overheatedMessage.gameObject.SetActive(true);

    //    }
    //}

    void switchWeapon()
    {
        foreach (Weapons weapons in allWeapons)
        {
            weapons.gameObject.SetActive(false); // De-activate all weapons
        }

        allWeapons[currentWeapon].gameObject.SetActive(true); // Activate only currently selected weapon


    }



    // Called once per frame, after every update function is complete
    private void LateUpdate()
    {


        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;




        // Tie main camera to view point location
    }
    
}