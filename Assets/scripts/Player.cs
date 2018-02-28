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
    public GameObject bullet; //weapon should change bullets
    public GameObject gunAttach;
    public GameObject stashAttach;
    public SelectionTarget SelectCollider;
    [HideInInspector]
    public bool canMove = true;
    [HideInInspector]
    public bool canAttack = true;
    #endregion

    #region Private Variables
    private TDCamera maincam;
    private int healthPoints;
    private GameObject targetObj = null;
    //TODO: make it so we can only have to ref the interactable script for object info
    private GameObject heldObj = null;
    private GameObject stashedWeapon = null;
    private bool buildingMode = false;
    private Building building;
    private bool isAlive = true;

    public int ammo { get; private set; }
    public int supplies { get; private set; }
    #endregion

    #region Unity Methods
    private void Start()
    {
        healthPoints = maxHealth;
    }

    private void Update()
    {
        if(healthPoints <= 0)
        {
            Death();
        }

        AntiPauseActions();

        if (Time.timeScale != 0)
        {
            /*********************************************************
            ANYTHING IN THIS STATEMENT WILL ADHERE TO PAUSING
            **********************************************************/
            targetObj = SelectCollider.Target;

            KeyboardInput();
            MouseInput();

            if (buildingMode)
            {
                BuildingMode();
            }
            /****************************************************************
             * NOTHING AFTER THIS ADHERES TO PAUSING
             * **************************************************************/
        }
    }
    #endregion

    #region Custom Methods
    private void Death()
    {
        Debug.Log("Player Death");
        canMove = false;
        canAttack = false;
        isAlive = false;
    }
    public int GetHealth()
    {
        return healthPoints;
    }
    public void AlterAmmo(int amount)
    {
        ammo += amount;
    }
    public void AlterSupplies(int amount)
    {
        supplies += amount;
    }
    private void AntiPauseActions()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !buildingMode)
        {
            GameManager.gm.TogglePause();
        }
    }

    private void BuildingMode()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.B))
        {
            buildingMode = false;
            building = null;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(maincam.middlePosition));
        if (Physics.Raycast(ray, out hit)) //NOTE: Building ray needs to ignore all layers but the floor and the player 
                                            //(the player just stops from building on yourself. Not a bug, a feature. :D)
        {
            if (hit.transform.GetComponent<BuildableFloor>())
            {
                BuildableFloor floor = hit.transform.GetComponent<BuildableFloor>();

                float closestdistance = Mathf.Infinity;
                GameObject closestObject = null;
                for (int i = 0; i < floor.attachPoints.GetLength(0); i++)
                {
                    for (int j = 0; j < floor.attachPoints.GetLength(1); j++)
                    {
                        float distance = Vector3.Distance(hit.point, floor.attachPoints[i, j].transform.position);
                        if (distance <= closestdistance) // or <=
                        {
                            closestdistance = distance;
                            closestObject = floor.attachPoints[i, j].gameObject;
                        }
                        Debug.Log(closestObject.name);
                    }
                }
                if (closestObject)
                {
                    building.GiveBuildLocation(closestObject.transform.position);
                }
            }
            else
            {
                //if the ray doesnt hit a floor
            }

        }
    }

    private void KeyboardInput()
    {
        if(Input.GetKeyDown(KeyCode.LeftBracket))
        {
            Mathf.Clamp(healthPoints -= 5, 0, 100);
        }
        else if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            Mathf.Clamp(healthPoints += 5, 0, 100);
        }
        if (canMove)
        {
            if (Input.GetKey(KeyCode.W))
            {
                this.transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                this.transform.Translate(Vector3.back * speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A))
            {
                this.transform.Translate(Vector3.left * speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                this.transform.Translate(Vector3.right * speed * Time.deltaTime);
            }
        }

        if (Input.GetKeyDown(KeyCode.B) && GameManager.gm.GetGameMode() == GameManager.Mode.Survive)
        {
            GameManager.gm.ToggleBuildMenu();
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

    }

    private void MouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!buildingMode) //NOT BUILDING
            {
                if (heldObj) //if holding an object
                {
                    if (heldObj.CompareTag("gun") == true || heldObj.CompareTag("melee") == true) //if the object is a weapon
                                                                                                  //TODO: implement an enum on weapon script once interactable script is made to remove tags
                    {
                        if (canAttack)
                        {
                            Attack(heldObj);
                        }
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
            else //BUILDING
            {
                PlaceBuilding(building);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (buildingMode)
            {
                buildingMode = false;
                Destroy(building.gameObject);
                building = null;
            }
        }
    }

    public void SetCamera(TDCamera cam)
    {
        maincam = cam;
    }

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
        heldObj.GetComponentInParent<Rigidbody>().useGravity = false;
        heldObj.GetComponentInParent<Rigidbody>().isKinematic = true;
        heldObj.GetComponent<Weapon>().isHeld = true;

    }

    private void DropItem(GameObject item)
    {
        if (item == heldObj)
        {
            Rigidbody rb = heldObj.GetComponentInParent<Rigidbody>();
            heldObj.GetComponent<Weapon>().isHeld = false;

            heldObj.transform.parent = null;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.AddRelativeForce(Vector3.forward * 100);
            SelectCollider.SetHeldObject(null);
            heldObj = null;

        }
        else
        {
            //idk what else you'd drop yet but just in case.
        }

    }

    /// <summary>
    /// Swaps the heldObj with the stashedWeapon, if the stash if null then it picks the target item up and puts the held in your stash.
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
                heldObj.GetComponentInParent<Rigidbody>().useGravity = false;
                heldObj.GetComponentInParent<Rigidbody>().isKinematic = true;
            }
            else
            {
                heldObj = stashedWeapon;
                stashedWeapon = null;
                SelectCollider.SetHeldObject(heldObj);
                heldObj.transform.parent = gunAttach.transform;
                heldObj.transform.localPosition = Vector3.zero;
                heldObj.transform.rotation = gunAttach.transform.rotation;
                heldObj.GetComponentInParent<Rigidbody>().useGravity = false;
                heldObj.GetComponentInParent<Rigidbody>().isKinematic = true;
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

    private void PlaceBuilding(Building toBeBuilt)
    {
        Vector3 buildLocation = new Vector3(maincam.middlePosition.x, GameManager.gm.GetFloorHeight(), maincam.middlePosition.z);
        if (building.Build() == true)
        {
            buildingMode = false;
            building = null;
        }
        else
        {
            //TODO: Add failed build feedback?
        }

    }

    public void StartBuildPlacement(GameObject toBeBuilt)
    {
        buildingMode = true;
        Vector3 buildLocation = new Vector3(maincam.middlePosition.x, GameManager.gm.GetFloorHeight(), maincam.middlePosition.z);
        GameObject temp = Instantiate(toBeBuilt, buildLocation, toBeBuilt.transform.rotation);
        building = temp.GetComponentInChildren<Building>();

        GameManager.gm.ToggleBuildMenu();
    }

    #endregion

}
