using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		28/01/2018 06:39
-------------------------------------
Description:

===================================*/

[RequireComponent(typeof(Outline))]
public class SelectionTarget : MonoBehaviour
{
    #region Public Variables
    public GameObject HeldObj { get; private set; }
    public GameObject Target { get; private set; }
    #endregion

    #region Private Variables

    #endregion

    #region Unity Methods

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Interactable>() && other.gameObject != HeldObj)
        {
            if (Target != null && other.gameObject != Target) //if theres a target, stop outlining OLD object
            {
                Target.GetComponent<Outline>().eraseRenderer = true;

            }
            Target = other.gameObject; // set new object as target
            Target.GetComponent<Outline>().eraseRenderer = false; //outline new object
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Interactable>() && other.gameObject != HeldObj)
        {
            if (Target == null)
            {
                Target = other.gameObject;
                Target.GetComponent<Outline>().eraseRenderer = false;

            }
            else if (Target == other.gameObject)
            {
                Target.GetComponent<Outline>().eraseRenderer = false; //this might be unneccesary but it stops small cases of items not outlining
            }
        }
        if (other.gameObject != Target && other.gameObject.GetComponent<Interactable>())
        {
            other.GetComponent<Outline>().eraseRenderer = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Interactable>())
        {
            other.GetComponent<Outline>().eraseRenderer = true;
            Target = null;
        }
    }
    #endregion

    #region Custom Methods
    public void SetHeldObject(GameObject h)
    {
        if (h == null)
        {
            HeldObj = null;
        }
        else
        {
            HeldObj = h;
            Target = null;
        }

        if (HeldObj != null)
        {
            HeldObj.GetComponent<Outline>().eraseRenderer = true;
        }
    }
    #endregion
}
