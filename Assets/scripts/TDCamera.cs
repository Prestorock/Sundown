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

    public Texture2D fadeOutTexture;
    public float fadeSpeed = 0.3f;

    public int fadeDrawDepth = -1000;
    [HideInInspector]
    public bool stillFading = false;
    #endregion

    #region Private Variables

    public Sens sensitivity;

    private Vector3 velocity = Vector3.zero;
    private float alpha = 1.0f;
    private int fadeDir = -1;
    #endregion

    #region Enumerations
    public enum Sens
    {
        low = 300,
        medium = 150,
        high = 50
    }
    #endregion

    #region Unity Methods

    private void Start()
    {
        //target = GameManager.gm.player;
        alpha = 1;
        FadeIn();
        if (target)
        {
            target.SetCamera(this);
        }
    }

    private void OnGUI()
    {
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        GUI.color = new Color(0,0,0,alpha);

        GUI.depth = fadeDrawDepth;

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);

        if ((alpha == 1 || alpha == 0) && stillFading == true)
        {
            stillFading = false;
        }
    }

    private void Update()
    {
        if(target == null)
        {
            target = GameManager.gm.player;
        }
        if (Time.timeScale != 0 && GameManager.gm.playing)
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


            // Define a target position relative to the target transform
            Vector3 targetPos = new Vector3(middlePosition.x, target.transform.position.y + heightBuffer, middlePosition.z);
            Vector3 smoothPos = new Vector3(target.transform.position.x, target.transform.position.y + heightBuffer, target.transform.position.z);
            float offset = (crosshairRadius / 2.0f) - 0.75f;
            if (targetPos.x > target.transform.position.x + offset)
            {
                smoothPos.x += crosshairRadius;
            }
            else if (targetPos.x < target.transform.position.x - offset)
            {
                smoothPos.x -= crosshairRadius;
            }
            if (targetPos.z > target.transform.position.z + offset)
            {
                smoothPos.z += crosshairRadius;
            }
            else if (targetPos.z < target.transform.position.z - offset)
            {
                smoothPos.z -= crosshairRadius;
            }
            // Smoothly move the camera towards that target position
            transform.position = Vector3.SmoothDamp(transform.position, smoothPos, ref velocity, smoothTime);

            //move the gui crosshair over the middle area
            crosshair.GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToScreenPoint(middlePosition);
            middlePosition.y = target.transform.position.y;
            if (target.canMove)
            {
                target.modelObject.transform.LookAt(middlePosition);
            }
        }
    }

   /* void LateUpdate()
    {

        if (Time.timeScale != 0 && GameManager.gm.playing)
        {
            // Define a target position relative to the target transform
            Vector3 targetPos = new Vector3(middlePosition.x, target.transform.position.y + heightBuffer, middlePosition.z);
            Vector3 smoothPos = new Vector3(target.transform.position.x, target.transform.position.y + heightBuffer, target.transform.position.z);
            float offset = (crosshairRadius / 2.0f) - 0.75f;
            if (targetPos.x > target.transform.position.x + offset)
            {
                smoothPos.x += crosshairRadius;
            }
            else if (targetPos.x < target.transform.position.x - offset)
            {
                smoothPos.x -= crosshairRadius;
            }
            if (targetPos.z > target.transform.position.z + offset)
            {
                smoothPos.z += crosshairRadius;
            }
            else if (targetPos.z < target.transform.position.z - offset)
            {
                smoothPos.z -= crosshairRadius;
            }
            // Smoothly move the camera towards that target position
            transform.position = Vector3.SmoothDamp(transform.position, smoothPos, ref velocity, smoothTime);

            //move the gui crosshair over the middle area
            crosshair.GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToScreenPoint(middlePosition);
            middlePosition.y = target.transform.position.y;
            if (target.canMove)
            {
                target.modelObject.transform.LookAt(middlePosition);
            }
        }
    }*/
    #endregion

    #region Custom Methods
    public void FadeOut()
    {
        stillFading = true;
        fadeDir = 1;
    }
    public void FadeIn()
    {
        stillFading = true;
        fadeDir = -1;
    }
    #endregion
}
