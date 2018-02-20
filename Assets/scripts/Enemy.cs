using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		14/02/2018 04:20
-------------------------------------
Description:

===================================*/

public class Enemy : MonoBehaviour 
{
    #region Public Variables

    #endregion

    #region Private Variables
    private NavMeshAgent agent;
    #endregion

    #region Enumerations

    #endregion

    #region Unity Methods
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

    }
    private void Update()
    {
        if (Vector3.Distance(GameManager.gm.player.transform.position, transform.position) > 2)
        {
            agent.SetDestination(GameManager.gm.player.transform.position);
            agent.isStopped = false;
        }
        else
        {
            agent.isStopped = true;
            //attack?
        }
    }
    #endregion

    #region Custom Methods

    #endregion
}
