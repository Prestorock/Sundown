using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		23/01/2018 05:58
-------------------------------------
Description:

===================================*/

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager gm;

    #region Public Variables
    public GameObject floor;
    public Vector2 floorGridSize;
    public GameObject pauseMenu;

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
        Destroy(floor);
        floorgrid = new GameObject[(int)Mathf.Round(floorGridSize.x), (int)Mathf.Round(floorGridSize.y)];
        if (floor != null)
        {
            float floorsize = floor.GetComponent<Collider>().bounds.size.x;
            print("Floor size: " + floorsize);
            for (int i = 0; i < floorgrid.GetLength(0); i++)
            {
                for (int j = 0; j < floorgrid.GetLength(1); j++)
                {
                    GameObject temp = Instantiate(floor, floor.transform.position, floor.transform.rotation);
                    temp.transform.Translate(new Vector3(i * floorsize, floor.transform.position.y, j * floorsize));
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

        if(paused)
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }
    }
    #endregion
}
