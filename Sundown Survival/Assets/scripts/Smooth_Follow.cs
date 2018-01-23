using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		23/01/2018 05:44
-------------------------------------
Description:

===================================*/

public class Smooth_Follow : MonoBehaviour 
{
    #region Public Variables
    #endregion
    public Transform follow;
    public float smoothTime = 0.3f;
    public GameObject crosshair;

    

    #region Private Variables
    private Vector3 velocity = Vector3.zero;
    private Vector3 middlePosition;
    #endregion

    #region Unity Methods

    private void Update()
    {
        Vector3 wPos = Input.mousePosition;
        wPos.z = follow.position.z - Camera.main.transform.position.z;
        wPos = Camera.main.ScreenToWorldPoint(wPos);
        Vector3 direction = wPos - follow.position;
        float radius = 5;

        direction = Vector3.ClampMagnitude(direction, radius);

        middlePosition = follow.position+direction;
    }

    void LateUpdate()
    {
        // Define a target position above the target transform
        Vector3 targetPosition = new Vector3(middlePosition.x, follow.position.y + 15, middlePosition.z);

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        crosshair.transform.position = middlePosition;
    }
    #endregion

    #region Custom Methods
    #endregion
}
