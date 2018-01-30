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
    public bool isGun = true;
    public int damage = 1;
    public float reloadTime = 1;
    public int projectiles = 1;
    #endregion

    #region Private Variables
    private Rigidbody rb;
    #endregion

    #region Unity Methods
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.IgnoreCollision(GetComponent<Collider>(), GameManager.gm.playerObject.GetComponent<Player>().modelObject.GetComponent<Collider>(), true);
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
