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
===================================*/

[RequireComponent(typeof(Interactable))]
public class Weapon : MonoBehaviour
{
    #region Public Variables
    [HideInInspector]
    public bool isHeld = false;
    public bool isGun = true;
    public int damage = 1;
    public float fireRate = 1;
    public int projectiles = 1;
    public GameObject bullet;
    public Transform bulletSpawn;
    public Mesh[] gunMeshes;
    #endregion

    #region Private Variables
    private Rigidbody rb;
    private Collider col;
    private MeshFilter mesh;
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
        mesh = GetComponent<MeshFilter>();
        colliding = col.enabled;

        FiringMode = (Rate)Mathf.Clamp(
            Mathf.RoundToInt(Random.Range(-0.5f, 2.5f)),
            0,
            2
            );

        if (FiringMode == Rate.Semi)
        {
            this.gameObject.name = "Pistol";
            mesh.mesh = gunMeshes[0];
            damage = Random.Range(3, 6);
            fireRate = Random.Range(0.2f, 0.8f);

        }
        else if (FiringMode == Rate.Auto)
        {
            this.gameObject.name = "Uzi";
            mesh.mesh = gunMeshes[1];
            damage = Random.Range(1, 3);
            fireRate = Random.Range(0.1f, 0.3f);

        }
        else if (FiringMode == Rate.Burst)
        {
            this.gameObject.name = "Shotgun";
            mesh.mesh = gunMeshes[2];
            damage = Random.Range(1, 3);
            fireRate = Random.Range(2, 4);
            projectiles = Random.Range(4, 8);

        }

        Invoke("StopTheErrors", 1.0f);
    }

    private void StopTheErrors()
    {
        Physics.IgnoreCollision(col, GameManager.gm.player.modelObject.GetComponent<Collider>(), true);
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
            GameObject temp;
            if (FiringMode == Rate.Auto)
            {
                temp = Instantiate(bullet, this.transform.position, this.transform.rotation);
                temp.GetComponent<Bullet>().SetDamage(damage);
                rTimer = fireRate;
                fired = true;
            }
            else if (triggerHeld == false)
            {
                if (FiringMode == Rate.Burst)
                {
                    for (int i = 0; i < projectiles; i++)
                    {
                        temp = Instantiate(bullet, this.transform.position, Quaternion.Euler(0, bulletSpawn.transform.rotation.eulerAngles.y + Random.Range(-2.0f, 2.0f), 0));
                        temp.GetComponent<Bullet>().SetDamage(damage);
                    }
                    rTimer = fireRate;
                    fired = true;
                }
                else
                {
                    temp = Instantiate(bullet, this.transform.position, this.transform.rotation);
                    temp.GetComponent<Bullet>().SetDamage(damage);
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
