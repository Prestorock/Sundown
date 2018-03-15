using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		13/03/2018 05:46
-------------------------------------
Description:

===================================*/

public class SpawnArea : MonoBehaviour
{
    #region Public Variables
    public GameObject[] quadPoints = new GameObject[4];
    #endregion

    #region Private Variables
    Color testcolor;
    #endregion

    #region Enumerations

    #endregion

    #region Unity Methods
    private void OnDrawGizmosSelected()
    {
        Vector3 temp1, temp3;
        temp1 = new Vector3(
            quadPoints[1].transform.position.x,
            (quadPoints[2].transform.position.y + quadPoints[0].transform.position.y) /2,
            quadPoints[1].transform.position.z
            );
        temp3 = new Vector3(
            quadPoints[3].transform.position.x,
            (quadPoints[2].transform.position.y + quadPoints[0].transform.position.y) /2,
            quadPoints[3].transform.position.z
            );

        Gizmos.color = Color.cyan;

        Gizmos.DrawLine(quadPoints[0].transform.position, temp1);
        Gizmos.DrawLine(temp1, quadPoints[2].transform.position);
        Gizmos.DrawLine(quadPoints[2].transform.position, temp3);
        Gizmos.DrawLine(temp3, quadPoints[0].transform.position);

        Gizmos.color = Color.white;
         
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawWireSphere(quadPoints[i].transform.position, 1.0f);
        }
    }
    #endregion

    #region Custom Methods
    public Vector3 RandomPoint()
    {
        return new Vector3
            (
            Random.Range(quadPoints[0].transform.position.x, quadPoints[2].transform.position.x),          //x
            Random.Range(quadPoints[0].transform.position.y, quadPoints[2].transform.position.y),          //y
            Random.Range(quadPoints[0].transform.position.z, quadPoints[2].transform.position.z)           //z
            );
    }
    #endregion
}
