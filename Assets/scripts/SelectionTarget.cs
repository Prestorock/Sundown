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
    public GameObject target; 
	#endregion
	
	#region Private Variables
	#endregion

	#region Unity Methods
	void Start () {
		
	}
	
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Interactable>())
        {
            target = other.gameObject;
            target.GetComponent<Outline>().enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Interactable>())
        {
            target.GetComponent<Outline>().enabled = false;
            target = null;
        }
    }
    #endregion

    #region Custom Methods
    #endregion
}
