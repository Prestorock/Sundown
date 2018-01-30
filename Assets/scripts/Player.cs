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
    public GameObject stashAttach;
    public SelectionTarget SelectCollider;
    #endregion

    #region Private Variables
    private int healthPoints;
    private GameObject targetObj = null;
    //TODO: make it so we can only have to ref the interactable script for object info
    private GameObject heldObj = null;
    private GameObject stashedWeapon = null;
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
            targetObj = SelectCollider.Target;


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

            if (Input.GetKeyDown(KeyCode.Tab)) // swapping weapons
            {
                SwapWeapons();
            }
            if (Input.GetKeyDown(KeyCode.F)) //picking up and dropping weapons
            {
                if (!heldObj)
                {
                    if (targetObj != null) //No held obj with a target
                    {
                        PickUpItem(targetObj);
                    }
                }
                else
                {
                    if (targetObj != null)
                    {
                        if (stashedWeapon)//Held obj, a target, and a stashed weapon
                        {
                            DropItem(heldObj);
                        }
                        else//Held obj, a target, and no stashed weapon
                        {
                            PickUpItem(targetObj);
                        }
                    }
                    else//Held obj, no target
                    {
                        DropItem(heldObj);
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (heldObj) //if holding an object
                {
                    if (heldObj.CompareTag("gun") == true || heldObj.CompareTag("melee") == true) //if the object is a weapon
                    //TODO: implement an enum on weapon script once interactable script is made to remove tags
                    {
                        Attack(heldObj);
                    }
                }
                else //if not holding an item
                {
                    if (targetObj == null) //no item and nothing being targetted
                    {
                        Attack(null);
                    }
                }
            }
        }
    }
    #endregion

    #region Custom Methods
    /// <summary>
    /// Picks up target item. If an item is held, it puts the weapon in the holster and picks up the item.
    /// </summary>
    /// <param name="target"></param>
    private void PickUpItem(GameObject target)
    {
        if (heldObj)
        {
            if (heldObj.GetComponent<Weapon>() && stashedWeapon == null)
            {
                stashedWeapon = heldObj;
                heldObj.transform.parent = stashAttach.transform;
                heldObj.transform.position = stashAttach.transform.position;
                heldObj.transform.rotation = stashAttach.transform.rotation;
            }
            else
            {
                return;
            }
        }
            heldObj = target;
            SelectCollider.SetHeldObject(heldObj);
            heldObj.transform.parent = gunAttach.transform;
            heldObj.transform.localPosition = Vector3.zero;
            heldObj.transform.rotation = gunAttach.transform.rotation;
            heldObj.GetComponent<Rigidbody>().useGravity = false;
            heldObj.GetComponent<Rigidbody>().isKinematic = true;
        
    }

    private void DropItem(GameObject item)
    {
        if (item == heldObj)
        {
            Rigidbody rb = heldObj.GetComponent<Rigidbody>();

            heldObj.transform.parent = null;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.AddRelativeForce(Vector3.forward*100);
            SelectCollider.SetHeldObject(null);
            heldObj = null;
        }
        else
        {
            //idk what else you'd drop yet but just in case.
        }

    }

    /// <summary>
    /// Swaps the heldObj with the stashedWeapon, if the stash if null then it picks the item up and puts the held in your stash.
    /// </summary>
    private void SwapWeapons()
    {
        if (stashedWeapon != null)
        {
            if (heldObj)
            {
                GameObject temp = stashedWeapon;
                stashedWeapon = heldObj;
                heldObj.transform.parent = stashAttach.transform;
                heldObj.transform.position = stashAttach.transform.position;
                heldObj.transform.rotation = stashAttach.transform.rotation;

                heldObj = temp;
                SelectCollider.SetHeldObject(heldObj);
                heldObj.transform.parent = gunAttach.transform;
                heldObj.transform.localPosition = Vector3.zero;
                heldObj.transform.rotation = gunAttach.transform.rotation;
                heldObj.GetComponent<Rigidbody>().useGravity = false;
                heldObj.GetComponent<Rigidbody>().isKinematic = true;
            }
            else
            {
                heldObj = stashedWeapon;
                stashedWeapon = null;
                SelectCollider.SetHeldObject(heldObj);
                heldObj.transform.parent = gunAttach.transform;
                heldObj.transform.localPosition = Vector3.zero;
                heldObj.transform.rotation = gunAttach.transform.rotation;
                heldObj.GetComponent<Rigidbody>().useGravity = false;
                heldObj.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
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
                Instantiate(bullet, heldObj.transform.position, heldObj.transform.rotation);
            }

            else if (weapon.CompareTag("melee") == true)
            {
                //TODO: implement melee
            }
        }
        else
        {
            //TODO: implement punching
        }
    }
    #endregion
}
