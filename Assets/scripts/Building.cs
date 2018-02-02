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
    public bool built;
    private Vector3 indicatedLocation;
    private Outline oline;
    private Material mat;
    #endregion

    #region Unity Methods
    void Start ()
    {
        buildable = false;
        built = false;
        oline = GetComponent<Outline>();
        mat = GetComponent<Renderer>().material;
        GetComponent<Renderer>().material = BuildingCheckMaterial;
        GetComponent<Collider>().isTrigger = true;

    }

    void Update () {
        if (!built)
        {
            transform.parent.position = indicatedLocation;
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

    public void GiveBuildLocation(Vector3 pos)
    {
        if (pos != null)
        {
            indicatedLocation = pos;
        }
        else
        {
            indicatedLocation = Vector3.zero;
            Debug.Log("Failed to get building position");
        }
    }

    public bool Build()
    {
        if (buildable)
        {
            built = true;
            if (gameObject.GetComponent<Collider>())
            {
                gameObject.GetComponent<Collider>().isTrigger = false;
            }
            GetComponent<Renderer>().material = mat;

            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
}
