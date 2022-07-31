using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public bool isAutomatic;
    public float fireRate = .1f, heatPerShot = 1f;
    public GameObject muzzleFlash;


    public TrailRenderer BulletTrail;
    public GameObject bulletImpact;
    private float shotCounter;
    private Camera cam;
    public float maxWeaponHeat = 10f, coolRate = 4f, overheatCoolRate = 5f;
    private float heatCounter;
    private bool overheated;
    LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        UIController.instance.weaponHeatSlider.maxValue = maxWeaponHeat;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
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

    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f)); // Create direction for raycast from centre of player view

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
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = hit.point;
        Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time);
    }
}
