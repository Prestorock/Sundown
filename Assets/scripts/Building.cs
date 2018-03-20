using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		02/02/2018 05:04
-------------------------------------
Description:

===================================*/

public class Building : MonoBehaviour
{
    #region Public Variables
    //public GameObject modelObject;
    public Material BuildingCheckMaterial;
    public int maxHealth = 100;
    public int cost;
    [HideInInspector]
    public bool built;
    [HideInInspector]
    public bool held = false;
    #endregion

    #region Private Variables
    private bool buildable;
    private Transform attachTo;
    private Outline oline;
    private Material mat;
    private Color matColor;
    private int healthPoints;
    #endregion

    #region Unity Methods
    void Start()
    {
        buildable = false;
        built = false;
        oline = GetComponent<Outline>();
        mat = GetComponent<Renderer>().material;
        matColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material = BuildingCheckMaterial;
        GetComponent<Collider>().isTrigger = true;
        healthPoints = maxHealth;

    }

    void Update()
    {
        if (!built)
        {
            if (!held)
            {
                Destroy(this.gameObject);
            }

            if (attachTo)
            {
                transform.parent.position = attachTo.transform.position;
                buildable = true;
            }
            else
            {
                //?
            }
            if (buildable)
            {
                oline.color = 1;
                mat.color = new Color(0, 1, 0, .2f);
            }
            else
            {
                oline.color = 2;
                mat.color = new Color(1, 0, 0, .2f);
            }
        }
        else
        {
            if (healthPoints <= 0)
            {
                Death();
            }
        }
    }
    #endregion
    private void OnTriggerExit(Collider other)
    {
        if (attachTo != null)
        {
            buildable = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (buildable && !other.CompareTag("floor") && !other.GetComponent<SelectionTarget>())
        {
            buildable = false;
        }
    }
    #region Custom Methods
    public void Death()
    {
        Destroy(this.gameObject);
    }
    public void AlterHealth(int amount)
    {
        healthPoints = Mathf.Clamp(healthPoints += amount, 0, maxHealth);

    }

    public void GiveBuildLocation(GameObject attach)
    {
        if (attach != null)
        {
            buildable = true;
            attachTo = attach.transform;
        }
        else
        {
            attachTo = null;
            buildable = false;
            Debug.Log("Failed to get building position");
        }
    }

    public bool Build()
    {
        if (buildable)
        {
            //floor.ParentBuilding(gameObject, attachTo.transform);
            built = true;
            if (gameObject.GetComponent<Collider>())
            {
                gameObject.GetComponent<Collider>().isTrigger = false;
            }
            if(gameObject.GetComponent<UnityEngine.AI.NavMeshObstacle>())
            {
                gameObject.GetComponent<UnityEngine.AI.NavMeshObstacle>().carving = true;
            }
            GetComponent<Renderer>().material = mat;
            GetComponent<Renderer>().material.color = matColor;
            oline.eraseRenderer = true;

            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
}
