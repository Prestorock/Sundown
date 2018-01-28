using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		23/01/2018 05:18
-------------------------------------
Description: A top down player controller. Should have capability to pick up items like ammo and supplies and to shoot a gun.
            Base building may also become a possiblity so universal interaction should be the core idea.

===================================*/

public class Player : MonoBehaviour
{
    #region Public Variables
    public int maxHealth = 100;
    public float speed = 0.1f;
    public GameObject modelObject;
    public GameObject bullet;
    public GameObject gunAttach;
    public SelectionTarget SelectCollider;
    #endregion

    #region Private Variables
    private int healthPoints;
    private GameObject targetObj = null;
    //TODO: make it so we can only ref the interactable script 
    private GameObject heldObj = null;
    #endregion

    #region Unity Methods
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.gm.TogglePause();
        }

        if (Time.timeScale != 0)
        {
            /*********************************************************
            ANYTHING NOT IN THIS STATEMENT WILL NOT ADHERE TO PAUSING
            **********************************************************/



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

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!heldObj)
                {
                    if (SelectCollider.target != null)
                    {
                        heldObj = SelectCollider.target;
                        heldObj.transform.parent = gunAttach.transform;
                        heldObj.transform.localPosition = Vector3.zero;
                        heldObj.transform.rotation = gunAttach.transform.rotation;
                        heldObj.GetComponent<Rigidbody>().useGravity = false;
                        heldObj.GetComponent<Rigidbody>().isKinematic = true;
                    }
                }
                else
                {
                    heldObj.transform.parent = null;
                    heldObj.GetComponent<Rigidbody>().useGravity = true;
                    heldObj.GetComponent<Rigidbody>().isKinematic = false;
                    heldObj = null;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (heldObj) //if holding an object
                {
                    if (heldObj.CompareTag("gun") == true || heldObj.CompareTag("melee") == true) //if the object is a weapon
                    //dont like tags 
                    //TODO: implement an enum on interactable script once interactable script is made
                    {
                        Attack(heldObj);
                    }
                    else //if the object is a placable object
                    {
                        //TODO: implement building/interaction

                        //if(heldObj == building)
                        //Build(heldobj);

                        //if holding an item and clicking on a target
                        //if(targetting interactable)
                        //InteractWithObject(targetObj, heldObj);
                    }
                }
                else //if not holding an item
                {
                    if (targetObj == null) //no item and nothing being targetted
                    {
                        Attack(null);
                    }
                    else // no item but something is being targetted
                    {
                        //TODO: Create working interaction script
                        //TODO: implement picking items up.
                    }
                }
            }
        }
    }
    #endregion

    #region Custom Methods
    private void PickUp(GameObject target)
    {

    }

    /// <summary>
    /// If the held object is a gun, it shoots. If it is melee, it swings.
    /// </summary>
    /// <param name="weapon">The object the player is holding.</param>
    ///
    private void Attack(GameObject weapon)
    {
        if (weapon)
        {
            if (weapon.CompareTag("gun") == true)
            {
                //TODO: implement guns
                Instantiate(bullet, heldObj.transform.position, heldObj.transform.rotation);
            }

            else if (weapon.CompareTag("melee") == true)
            {
                //TODO implement melee
            }
        }
        else 
        {

        }
    }
    #endregion
}
