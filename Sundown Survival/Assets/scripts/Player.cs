using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		23/01/2018 05:18
-------------------------------------
Description:
            A top down player controller. Should have capability to pick up items like ammo and supplies and to shoot a gun.
            Base building may also become a possiblity so universal interaction should be the core idea.

===================================*/

public class Player : MonoBehaviour
{
    #region Public Variables
    public int maxHealth = 100;
    public float speed = 0.1f;
    #endregion

    #region Private Variables
    private int healthPoints;
    private GameObject targetObj = null;
    private GameObject heldObj = null;
    #endregion

    #region Unity Methods
    private void Update()
    {
        //Anything not in fixed update will not adhere to the rules of "pausing". Just a warning.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.gm.TogglePause();
        }
    }
    void FixedUpdate()
    {
        
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(Vector3.forward * speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(Vector3.back * speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(Vector3.left * speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Translate(Vector3.right * speed);
        }

        if (Input.GetKey(KeyCode.F))
        {
            //Interactions? Open doors? Take items? Switch guns?
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (heldObj)
            {
                if (heldObj.CompareTag("gun") == true || heldObj.CompareTag("melee") == true) //dont like tags, need something better
                {
                    Attack(heldObj);
                }
                else
                {
                    //TODO: implement building/interaction

                    //if(heldObj == building)
                    //Build(heldobj);
                    //if(targetting interactable)
                    //InteractWithObject(targetObj, heldObj);
                }
            }
            else
            {
                Attack();
            }
        }
    }
    #endregion

    /// <summary>
    /// If the held object is a gun, it shoots. If it is melee, it swings.
    /// </summary>
    /// <param name="weapon">The object the player is holding.</param>
    /// 
    private void Attack(GameObject weapon)
    {
        if (weapon.CompareTag("gun") == true)
        {
            //TODO: implement guns
        }

        if (weapon.CompareTag("melee") == true)
        {
            //TODO implement melee
        }
    }
    /// <summary>
    /// Attacks with bare hands.
    /// </summary>
    private void Attack()
    {
        //TODO: implement punching
    }
}
