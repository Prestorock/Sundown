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

    public GameObject modelObject;
    //public GameObject bullet; //weapon should change bullets
    public GameObject gunAttach;
    public GameObject stashAttach;
    public SelectionTarget SelectCollider;
    public AudioClip hurtSound;
    [HideInInspector]
    public bool canMove = true;
    [HideInInspector]
    public bool canAttack = true;

    #endregion

    #region Private Variables
    [SerializeField]
    [Range(0, 1)]
    private float attackSpeed = 1.0f;
    private float attackTimer = 0.0f;
    [SerializeField]
    private int punchingDamage = 5;
    private int maxHealth = 100;
    [SerializeField]
    private float speed = 0.1f;
    private TDCamera maincam;
    private int healthPoints;
    private GameObject targetObj = null;
    //TODO: make it so we can only have to ref the interactable script for object info
    private GameObject heldObj = null;
    private Weapon weaponscript = null;
    private GameObject stashedWeapon = null;
    private bool buildingMode = false;
    private Building building;
    private bool isAlive = true;

    public int Ammo { get; private set; }
    public int Supplies { get; private set; }
    #endregion

    #region Unity Methods
    private void Start()
    {
        healthPoints = maxHealth;
        maincam = Camera.main.GetComponent<TDCamera>();
    }

    private void Update()
    {
        if (maincam == null)
        {
            maincam = GameManager.gm.mainCamera.GetComponent<TDCamera>();
        }
        if (healthPoints <= 0 && isAlive)
        {
            Death();
        }

        AntiPauseActions();

        if (Time.timeScale != 0)
        {
            if (attackTimer < attackSpeed)
            {
                attackTimer += Time.deltaTime;
            }
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
        //Debug.Log("Player Death");
        canMove = false;
        canAttack = false;
        isAlive = false;
        GameManager.gm.PlayerDeath();
    }
    public int GetHealth()
    {
        return healthPoints;
    }

    public void AlterHealth(int amount)
    {
        healthPoints = Mathf.Clamp(healthPoints += amount, 0, maxHealth);
        if (amount < 0)
        {
            AudioSource.PlayClipAtPoint(hurtSound, transform.position);
        }

    }
    public void AlterAmmo(int amount)
    {
        Ammo += amount;
        GameManager.gm.ammoText.text = Ammo.ToString();
    }

    public void AlterSupplies(int amount)
    {
        Supplies += amount;
        GameManager.gm.supplyText.text = Supplies.ToString();
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
        canAttack = false;
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.B))
        {
            canAttack = true;
            buildingMode = false;
            building.held = false;
            building = null;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(maincam.middlePosition));
        if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Floor"))) //NOTE: Building ray needs to ignore all layers but the floor and the player 
                                                                       //(the player just stops from building on yourself. Not a bug, a feature. :D)
        {
            BuildableFloor floor = hit.transform.GetComponent<BuildableFloor>();
            if (floor)
            {

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
                        //Debug.Log(closestObject.name);
                    }
                }
                if (closestObject)
                {
                    building.GiveBuildLocation(closestObject);
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
            canAttack = false;
            if (buildingMode)
            {
                building.held = false;
                canAttack = true;
            }
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
                        PickUpItem(targetObj);
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
        if (Input.GetMouseButton(0))
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
                            weaponscript.TriggerHeld(true);

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
        if (Input.GetMouseButtonUp(0))
        {
            if (heldObj)
                weaponscript.TriggerHeld(false);
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (buildingMode)
            {
                buildingMode = false;
                Destroy(building.gameObject);
                building = null;
                canAttack = true;
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
                weaponscript = heldObj.GetComponent<Weapon>();

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
        weaponscript = heldObj.GetComponent<Weapon>();



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
            weaponscript = null;
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
                weaponscript = heldObj.GetComponent<Weapon>();

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
                weaponscript = heldObj.GetComponent<Weapon>();

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
                if (!buildingMode && Ammo > 0)
                {
                    weaponscript.Shoot();
                }
            }

            else if (weapon.CompareTag("melee") == true)
            {
                //TODO: implement melee
            }
        }
        else
        {
            if (attackTimer > attackSpeed)
            {
                attackTimer = 0;
                RaycastHit hit;
                Ray ray = new Ray(gunAttach.transform.position, gunAttach.transform.forward);
                if (Physics.Raycast(ray, out hit, 3.0f))
                {
                    //Debug.DrawLine(gunAttach.transform.position, hit.point, Color.green, 5.0f);
                    if (hit.transform.gameObject.GetComponent<Enemy>())
                    {
                        hit.transform.gameObject.GetComponent<Enemy>().AlterHealth(-punchingDamage);
                        //Vector3 dir = hit.transform.position - transform.position;
                        //dir = -dir.normalized;
                        //hit.transform.gameObject.GetComponent<Rigidbody>().AddForce(dir * 100);
                        Debug.Log("punched " + hit.transform.gameObject.name);
                    }
                }
                else
                {
                    //Debug.DrawRay(gunAttach.transform.position, gunAttach.transform.forward*3.0f, Color.red, 5.0f);

                }
            }
        }
    }

    private void PlaceBuilding(Building toBeBuilt)
    {
        building = toBeBuilt;
        if (toBeBuilt.Build() == true)
        {
            AlterSupplies(-building.cost);
            building.held = false;
            buildingMode = false;
            building = null;
            Invoke("DelayedAttackBool", 0.2f);
        }
        else
        {
            building.held = true;
            buildingMode = true;
            building = toBeBuilt;
            canAttack = false;
        }

    }

    private void DelayedAttackBool()
    {
        canAttack = true;
    }
    public void StartBuildPlacement(GameObject toBeBuilt)
    {
        if (Supplies >= toBeBuilt.GetComponentInChildren<Building>().cost)
        {
            buildingMode = true;
            Vector3 buildLocation = new Vector3(maincam.middlePosition.x, GameManager.gm.GetFloorHeight(), maincam.middlePosition.z);
            GameObject temp = Instantiate(toBeBuilt, buildLocation, toBeBuilt.transform.rotation);
            building = temp.GetComponentInChildren<Building>();
            building.held = true;

            GameManager.gm.ToggleBuildMenu();

        }
        else
        {
            return;
        }
    }

    #endregion

}
