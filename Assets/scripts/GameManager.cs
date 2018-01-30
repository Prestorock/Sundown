using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		23/01/2018 05:58
-------------------------------------
Description: A self referencial singleton that can help objects communicate and manage hud/gameplay

===================================*/

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager gm;

    #region Public Variables
    public GameObject floorPrefab;
    public Vector2 floorGridSize;
    public GameObject pauseMenu;
    public GameObject buildingMenu; //TODO: Implement building
    public GameObject playerObject;

    #endregion

    #region Private Variables
    private bool paused = false;
    private GameObject[,] floorgrid;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        gm = this;
    }

    private void Start()
    {
        Cursor.visible = false;

        Cursor.lockState = CursorLockMode.Confined;

        floorgrid = new GameObject[(int)Mathf.Round(floorGridSize.x), (int)Mathf.Round(floorGridSize.y)];
        if (floorPrefab != null)
        {
            float floorsize = 10 * floorPrefab.transform.localScale.x;
            print("Floor size: " + floorsize);
            for (int i = 0; i < floorgrid.GetLength(0); i++)
            {
                for (int j = 0; j < floorgrid.GetLength(1); j++)
                {
                    GameObject temp = Instantiate(floorPrefab, floorPrefab.transform.position, floorPrefab.transform.rotation);
                    temp.name = ("floor" + i.ToString() + j.ToString());
                    temp.transform.Translate(new Vector3(i * floorsize, this.transform.position.y, j * floorsize));
                }
            }
        }
    }
    #endregion

    #region Custom Methods
    /// <summary>
    /// TogglePause inverts bool paused and opens/closes menu while stopping/continuing time;
    /// </summary>
    public void TogglePause()
    {
        paused = !paused;

        if (paused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }
    }
    #endregion
}
