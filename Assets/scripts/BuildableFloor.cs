using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		27/01/2018 17:48
-------------------------------------
Description: A script that allows snapping and placing of "buildings" on a flat plane.

===================================*/

public class BuildableFloor : MonoBehaviour
{
    #region Public Variables
    public Transform[,] attachPoints = new Transform[4, 4];
    #endregion

    #region Private Variables
    private GameObject[,] buildings = new GameObject[4,4];
    #endregion

    #region Unity Methods
    private void Start()
    {
        for(int i = 0; i < buildings.GetLength(0); i++)
        {
            for (int j = 0; j < buildings.GetLength(1); j++)
            {
                attachPoints[i,j] = new GameObject(i.ToString() + j.ToString()).transform;
                attachPoints[i,j].transform.parent = this.gameObject.transform;
                attachPoints[i,j].transform.localPosition = new Vector3(((i * 2.5f) - 3.75f), 0, ((j * 2.5f) - 3.75f));
            }
        }
    }
    #endregion

    #region Custom Methods
    public void ParentBuilding(GameObject building, Transform attachPoint)
    {
        building.transform.parent = attachPoint;
        building.transform.position = attachPoint.position;
        building.transform.rotation = attachPoint.rotation;

    }
    #endregion
}
