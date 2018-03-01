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
	#endregion
	
	#region Private Variables
    private bool buildable;
    private bool built;
    private Transform attachTo;
    private BuildableFloor floor;
    private Outline oline;
    private Material mat;
    private Color matColor;
    #endregion

    #region Unity Methods
    void Start ()
    {
        buildable = false;
        built = false;
        oline = GetComponent<Outline>();
        mat = GetComponent<Renderer>().material;
        matColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material = BuildingCheckMaterial;
        GetComponent<Collider>().isTrigger = true;

    }

    void Update () {
        if (!built)
        {
            if (attachTo)
            {
                transform.parent.position = attachTo.transform.position;
            }
            else
            {
                //?
            }
            if(buildable)
            {
                oline.color = 1;
                mat.color = new Color(0,1,0,.2f);
            }
            else
            {
                oline.color = 2;
                mat.color = new Color(1, 0, 0, .2f);
            }
        }
	}
    #endregion
    private void OnTriggerExit(Collider other)
    {
        
        buildable = true;

    }

    private void OnTriggerStay(Collider other)
    {
        if(buildable && !other.CompareTag("floor") && !other.GetComponent<SelectionTarget>())
        {
            buildable = false;
        }
    }
    #region Custom Methods

    public void GiveBuildLocation(GameObject attach, BuildableFloor flo)
    {
        if (attach != null)
        {
            attachTo = attach.transform;
            floor = flo;
        }
        else
        {
            attachTo = null;
            Debug.Log("Failed to get building position");
        }
    }

    public bool Build()
    {
        if (buildable)
        {
            floor.ParentBuilding(gameObject, attachTo.transform);
            built = true;
            if (gameObject.GetComponent<Collider>())
            {
                gameObject.GetComponent<Collider>().isTrigger = false;
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
