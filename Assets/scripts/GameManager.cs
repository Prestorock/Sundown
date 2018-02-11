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
    public GameObject buildFloor;
    public GameObject noBuildFloor;
    public Vector2 floorGridSize;
    public GameObject pauseMenu;
    public GameObject buildingMenu;
    public Player player;
    public GameObject powerups;
    #endregion

    #region Private Variables
    private bool paused = false;
    private GameObject[,] floorgrid;
    private Mode GameMode = Mode.Scavenge;
    private float floorheight = 0; //comment out reference in GenerateFloor if we ever start changing this.
    #endregion

    #region Enumerators
    public enum Mode { Scavenge, Survive };

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

        GameMode = Mode.Scavenge;

        GenerateFloor();
        //SpawnObjects();
        SpawnPowerups();
    }
    #endregion

    #region Custom Methods
    public float GetFloorHeight()
    {
        return floorheight;
    }
    public Mode GetGameMode()
    {
        return GameMode;
    }
    /// <summary>
    /// Generates a floor and tiles it, based on the localscale of the object and the size defined by the user.
    /// </summary>
    private void GenerateFloor()
    {
        GameObject floorPrefab;
        if (GameMode == Mode.Survive)
        {
            floorPrefab = buildFloor;

            floorgrid = new GameObject[(int)Mathf.Round(floorGridSize.x), (int)Mathf.Round(floorGridSize.y)];
            if (floorPrefab != null)
            {
                floorheight = floorPrefab.transform.position.y; //comment this out if we ever start manually changing the floor height

                float floorsize = 10 * floorPrefab.transform.localScale.x;
                print("Floor size: " + floorsize);
                for (int i = 0; i < floorgrid.GetLength(0); i++)
                {
                    for (int j = 0; j < floorgrid.GetLength(1); j++)
                    {
                        GameObject temp = Instantiate(floorPrefab, floorPrefab.transform.position, floorPrefab.transform.rotation);
                        temp.name = ("floor" + i.ToString() + j.ToString());
                        temp.transform.Translate(new Vector3(i * floorsize, floorheight, j * floorsize));
                    }
                }
            }
        }
        else if (GameMode == Mode.Scavenge)
        {
            floorPrefab = noBuildFloor;

            floorgrid = new GameObject[(int)Mathf.Round(floorGridSize.x), (int)Mathf.Round(floorGridSize.y)];
            if (floorPrefab != null)
            {
                floorheight = floorPrefab.transform.position.y; //comment this out if we ever start manually changing the floor height

                float floorsize = 10 * floorPrefab.transform.localScale.x;
                print("Floor size: " + floorsize);
                for (int i = 0; i < floorgrid.GetLength(0); i++)
                {
                    for (int j = 0; j < floorgrid.GetLength(1); j++)
                    {
                        GameObject temp = Instantiate(floorPrefab, floorPrefab.transform.position, floorPrefab.transform.rotation);
                        temp.name = ("floor" + i.ToString() + j.ToString());
                        temp.transform.Translate(new Vector3(i * floorsize, floorheight, j * floorsize));
                    }
                }
            }
        }
        
    }
    private void SpawnPowerups()
    {
        int r = Random.Range(1, 100);
        Debug.Log("Powerups Spawned: " + r);
        for(int i = 0; i < r; i++)
        {
            GameObject temp = Instantiate(powerups);
            temp.transform.position = new Vector3(Random.Range(0, floorGridSize.x*10)-5, 
                                                    floorheight + .5f, 
                                                    Random.Range(0, floorGridSize.y*10)-5
                                                    );
        }
    }
    /// <summary>
    /// TogglePause inverts bool paused and opens/closes menu while stopping/continuing time;
    /// </summary>
    public void TogglePause()
    {
        paused = !paused;
        buildingMenu.SetActive(false);

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
    

    public void ToggleBuildMenu()
    {
        //player.canMove = (buildingMenu.activeInHierarchy);

        Cursor.visible = !Cursor.visible;
        buildingMenu.SetActive(!buildingMenu.activeInHierarchy);
    }
    #endregion
}
