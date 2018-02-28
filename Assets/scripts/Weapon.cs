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
    #region Public Variables
    public bool isHeld = false;
    public bool isGun = true;
    public int damage = 1;
    public float reloadTime = 1;
    public int projectiles = 1;
    #endregion

    #region Private Variables
    private Rigidbody rb;
    private Collider col;
    private bool colliding = true;
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
    #endregion
}
