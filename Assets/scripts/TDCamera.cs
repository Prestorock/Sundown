using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		23/01/2018 05:44
-------------------------------------
Description: A top down camera script. Follows the target Player object at a height of "heightBuffer".
            Does so smoothly (affected by "smoothTime"). Also draws the crosshair at a clamped point.

===================================*/

public class TDCamera : MonoBehaviour
{
    #region Public Variables
    public Player target;
    [Tooltip("How far behind the camera follows the target.\n0 means no smoothing.")]
    [Range(0.0f, 0.5f)]
    public float smoothTime = 0.1f;
    public float heightBuffer = 15;
    public GameObject crosshair;
    [Tooltip("The max distance the crosshair gets from the player, this also affects the camera.")]
    [Range(1, 7)]
    public float crosshairRadius = 5;
    [HideInInspector]
    public Vector3 middlePosition;

    #endregion

    #region Private Variables

    public Sens sensitivity;

    private Vector3 velocity = Vector3.zero;
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

    private void Start()
    {
        if(target)
        {
            target.SetCamera(this);
        }
    }
    private void Update()
    {
        if (Time.timeScale != 0)
        {
            //get the mouse cursor location
            Vector3 wPos = Input.mousePosition;
            //set a height buffer for perspective camera. The sensitivity isnt necessary but its too fast without it.
            //also Input.mousePosition is a Vector2, so we set the z.
            wPos.z = (target.transform.position.z - Camera.main.transform.position.z) + heightBuffer * (float)sensitivity;
            //get the mouse cursor location from before, but set at the world position of the camera.
            wPos = Camera.main.ScreenToWorldPoint(wPos);
            Vector3 direction = wPos - target.transform.position;

            direction = Vector3.ClampMagnitude(direction, crosshairRadius);

            middlePosition = target.transform.position + direction;
        }
    }

    void LateUpdate()
    {

        if (Time.timeScale != 0)
        {
            // Define a target position above the target transform
            Vector3 targetPosition = new Vector3(middlePosition.x, target.transform.position.y + heightBuffer, middlePosition.z);
            // Smoothly move the camera towards that target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            //move the gui crosshair over the middle area
            crosshair.GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToScreenPoint(middlePosition);
            middlePosition.y = target.transform.position.y;
            target.modelObject.transform.LookAt(middlePosition);
        }
    }
    #endregion

    #region Custom Methods
    #endregion
}
