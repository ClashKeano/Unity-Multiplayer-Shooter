using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Weapons : MonoBehaviourPunCallbacks
{
    public bool isAutomatic;
    public float fireRate = .1f, heatPerShot = 1f;
    public GameObject muzzleFlash;


    public TrailRenderer BulletTrail;
    public GameObject bulletImpact;
    public GameObject playerHitImpact;
    private float shotCounter;
    private Camera cam;
    public float maxWeaponHeat = 10f, coolRate = 4f, overheatCoolRate = 5f;
    private float heatCounter;
    private bool overheated;
    LayerMask mask;
    public Vector3 normalPosition;
    public float aimSmoothing = 100f;

    // Start is called before the first frame update
    void Start()
    {
        UIController.instance.weaponHeatSlider.maxValue = maxWeaponHeat;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            aim();
            muzzleFlash.gameObject.SetActive(false);

            if (!overheated)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Shoot();
                }

                if (Input.GetMouseButton(0) && isAutomatic == true)
                {
                    shotCounter -= Time.deltaTime;

                    if (shotCounter <= 0)
                    {
                        Shoot();
                    }

                }

                heatCounter -= coolRate * Time.deltaTime;
            }
            else
            {
                heatCounter -= overheatCoolRate * Time.deltaTime;
                if (heatCounter <= 0)
                {
                    overheated = false;

                    UIController.instance.overheatedMessage.gameObject.SetActive(false);

                }
            }

            if (heatCounter < 0)
            {
                heatCounter = 0;
            }

            UIController.instance.weaponHeatSlider.value = heatCounter;
        }
    }

    void aim()
    {
        Vector3 target = normalPosition;
        Vector3 desiredPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * aimSmoothing);
        transform.localPosition = desiredPosition;
    }

    private void Shoot()
    {
        Ray ray;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        {
            ray = cam.ViewportPointToRay(new Vector3(Random.Range(.45f, .55f), Random.Range(.45f, .55f), 0f)); // will shoot at random direction when player moving
        }
        else
        {
            ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f)); //shoot straight when player stand
        }

        ray.origin = cam.transform.position;

       if (Physics.Raycast(ray, out RaycastHit hit))
        {
            TrailRenderer trail = Instantiate(BulletTrail, muzzleFlash.transform.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit));
        }


        muzzleFlash.gameObject.SetActive(true); //Activate muzzle flush when firing


        shotCounter = fireRate;

        heatCounter += heatPerShot;

        if (heatCounter >= maxWeaponHeat)
        {
            heatCounter = maxWeaponHeat;

            overheated = true;

            UIController.instance.overheatedMessage.gameObject.SetActive(true);

        }
    }

    IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;
        while (time < 1)
        {
            transform.localPosition -= Vector3.forward * 0.02f;
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = hit.point;

        if(hit.collider.gameObject.tag == "Player")
        {
            PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);

            hit.collider.gameObject.GetPhotonView().RPC("PlayerDamage", RpcTarget.All, photonView.Owner.NickName);
        }
        else
        {
            GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));

            Destroy(trail.gameObject, trail.time);
            Destroy(bulletImpactObject, 5f);
        }
        
    }

    
    [PunRPC]
    public void PlayerDamage(string damager)
    {
        TakeDamage(damager);
    }

    public void TakeDamage(string damager)
    {
        Debug.Log(photonView.Owner.NickName + " has been hit by " + damager);

        gameObject.SetActive(false);
       
    }
  
   

}
