using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		28/01/2018 06:33
-------------------------------------
Description:
TODO: Make this script object oriented so we can customize weapons EZ (including melee)
===================================*/

[RequireComponent(typeof(Interactable))]
public class Weapon : MonoBehaviour
{
    public Weapon(Rate Type, float rateOfFire, int dmg)
    {
        FiringMode = Type;
        fireRate = rateOfFire;
        damage = dmg;
    }

    public Weapon(Rate Type, float rateOfFire, int dmg, int numberOfProjectiles)
    {
        FiringMode = Type;
        fireRate = rateOfFire;
        damage = dmg;
        projectiles = numberOfProjectiles;
    }

    #region Public Variables
    [HideInInspector]
    public bool isHeld = false;
    public bool isGun = true;
    public int damage = 1;
    public float fireRate = 1;
    public int projectiles = 1;
    public GameObject bullet;
    public Transform bulletSpawn;
    #endregion

    #region Private Variables
    private Rigidbody rb;
    private Collider col;
    private bool colliding = true;
    [SerializeField]
    private Rate FiringMode = Rate.Semi;
    private bool fired = false;
    private bool triggerHeld = false;
    private float rTimer = 0.0f;
    #endregion

    #region Enumerations
    public enum Rate
    {
        Semi,
        Auto,
        Burst
    };
    #endregion

    #region Unity Methods
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        Physics.IgnoreCollision(col, GameManager.gm.player.modelObject.GetComponent<Collider>(), true);
        colliding = col.enabled;
    }
    private void Update()
    {
        if (isHeld && colliding == true)
        {
            GetComponent<AutoSpin>().enabled = false;
            col.enabled = false;
            colliding = false;
        }
        else if (!isHeld && colliding == false)
        {
            GetComponent<AutoSpin>().enabled = true;
            col.enabled = true;
            colliding = true;

        }
        if (rTimer > 0)
        {
            rTimer -= Time.deltaTime;
        }
        else if (fired == true)
        {
            Reload();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.timeScale != 0)
        {
            if (other.CompareTag("floor"))
            {
                rb.useGravity = false;

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Time.timeScale != 0)
        {
            if (other.CompareTag("floor"))
            {
                rb.useGravity = true;
            }
        }
    }
    #endregion

    #region Custom Methods
    public void Shoot()
    {
        if (fired == false)
        {
            if (FiringMode == Rate.Auto)
            {
                Instantiate(bullet, this.transform.position, this.transform.rotation);
                rTimer = fireRate;
                fired = true;
            }
            else if (triggerHeld == false)
            {
                if(FiringMode == Rate.Burst)
                {
                    for (int i = 0; i < projectiles; i++)
                    {
                        Instantiate(bullet, this.transform.position, Quaternion.Euler(0,bulletSpawn.transform.rotation.eulerAngles.y + Random.Range(-2.0f, 2.0f), 0));
                    }
                    rTimer = fireRate;
                    fired = true;
                }
                else
                {
                    Instantiate(bullet, this.transform.position, this.transform.rotation);
                    rTimer = fireRate;
                    fired = true;
                }
            }
        }
    }

    public void Reload()
    {
        if (rTimer <= 0)
        {
            fired = false;
        }
    }
    public void TriggerHeld(bool b)
    {
        triggerHeld = b;
    }
    #endregion
}
