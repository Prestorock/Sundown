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

public class TDCamera : MonoBehaviour 
{
    #region Public Variables
    public Transform follow;
    [Tooltip("How far behind the camera follows the target.\n0 means no smoothing.")]
    [Range(0.0f,0.5f)]
    public float smoothTime = 0.1f;
    public float heightBuffer = 15;
    public GameObject crosshair;

    
    #endregion

    #region Private Variables

    public Sens sensitivity;

    private Vector3 velocity = Vector3.zero;
    private Vector3 middlePosition;
    #endregion

    #region Enumerations
    public enum Sens
    {
        low = 100,
        medium = 50,
        high = 1
    }
    #endregion

    #region Unity Methods

    private void Update()
    {
        Vector3 wPos = Input.mousePosition;
        wPos.z =(follow.position.z - Camera.main.transform.position.z) + heightBuffer * (float)sensitivity;
        wPos = Camera.main.ScreenToWorldPoint(wPos);
        Vector3 direction = wPos - follow.position;

        float radius = 5;
        direction = Vector3.ClampMagnitude(direction, radius);

        middlePosition = follow.position+direction;
    }

    void LateUpdate()
    {
        // Define a target position above the target transform
        Vector3 targetPosition = new Vector3(middlePosition.x, follow.position.y + heightBuffer, middlePosition.z);
        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        crosshair.GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToScreenPoint(middlePosition);
    }
    #endregion

    #region Custom Methods
    #endregion
}
