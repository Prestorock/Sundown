using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public GameObject[] buildFloor;
    public GameObject[] noBuildFloor;
    public Vector2 floorGridSize;
    public GameObject pauseMenu;
    public GameObject buildingMenu;
    public Player player;
    public GameObject powerups;
    #endregion

    #region Private Variables
    private GameObject FloorParent;
    private GameObject PowerupParent;
    private bool paused = false;
    private GameObject[,] floorgrid;
    [SerializeField]
    private Mode GameMode = Mode.Scavenge;
    private float floorheight = 0; //comment out reference in GenerateFloor if we ever start changing this.
    private float modeTimer = 0f;
    #endregion

    #region Enumerators
    public enum Mode { Scavenge, Survive, Dev };

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
        if (GameMode != Mode.Dev) //development mode can be started by setting the mode from the gamemaster object
            //this will stop the normal progression of the game like removing the floor and spawning objects;
        {
            GameMode = Mode.Scavenge;
            GenerateFloor();
            GenerateNavMesh();
            //SpawnObjects();
            SpawnPowerups();
        }
        else
        {
            //development mode special code.
            GenerateFloor();

        }
    }
    private void Update()
    {
        if (Time.timeScale != 0)
        {
            modeTimer += Time.deltaTime;
        }

        if(GameMode == Mode.Dev)
        {
            ChangeGameMode(Mode.Survive); //NOTE: Right now the Dev game mode starts the game in survival mode.

        }

        //TODO: Find a good scavenge timer
        if (modeTimer >= 60 && GameMode == Mode.Scavenge)
        {
            ChangeGameMode(Mode.Survive);
        }
        //TODO: Add Survival Mode when enemies are done;
        /*
        if(GameMode == Mode.Survival && ENEMIESAREDEAD)
        {
            ChangeGameMode(Mode.Scavenge);
        }
        */
    }
    #endregion

    #region Custom Methods

    private void GenerateNavMesh()
    {
        floorgrid[0, 0].GetComponent<NavMeshSurface>().BuildNavMesh();
    }
    private void ChangeGameMode(Mode mode)
    {
        modeTimer = 0;
        GameMode = mode;
        DestroyFloor();
        GenerateFloor();
        //TODO: Cleanup on gamemode change
    }
    public Mode GetGameMode()
    {
        return GameMode;
    }
    public float GetFloorHeight()
    {
        return floorheight;
    }

    /// <summary>
    /// EZ floor destruction. Throw away the parent and just make a new one. :)
    /// </summary>
    private void DestroyFloor()
    {
        Destroy(FloorParent);
    }
    /// <summary>
    /// Generates a floor and tiles it, based on the localscale of the object and the size defined by the user.
    /// </summary>
    private void GenerateFloor()
    {
        FloorParent = new GameObject("Floors");
        FloorParent.transform.parent = this.gameObject.transform;

        GameObject floorPrefab;
        if (GameMode == Mode.Survive)
        {
            floorPrefab = buildFloor[0];

            floorgrid = new GameObject[(int)Mathf.Round(floorGridSize.x), (int)Mathf.Round(floorGridSize.y)];
            if (floorPrefab != null)
            {
                floorheight = floorPrefab.transform.position.y; //NOTE: comment this out if we ever start manually changing the floor height

                float floorsize = 10 * floorPrefab.transform.localScale.x;
                print("Floor size: " + floorsize);

                int imax = floorgrid.GetLength(0);
                int jmax = floorgrid.GetLength(1);

                for (int i = 0; i < imax; i++)
                {
                    for (int j = 0; j < jmax; j++)
                    {
                        //BORDERS
                        if ((i == 0) || (i == imax - 1) || (j == 0) || (j == jmax - 1))
                        {
                            //CORNERS
                            if (
                                (i == 0 && j == 0) ||
                                (i == imax - 1 && j == jmax - 1) ||
                                (i == 0 && j == jmax - 1) ||
                                (i == imax - 1 && j == 0)
                                )
                            {
                                floorPrefab = buildFloor[2];
                            }
                            //WALLS
                            else
                            {
                                floorPrefab = buildFloor[1];
                            }
                        }
                        //CENTER
                        else
                        {
                            floorPrefab = buildFloor[0];

                        }

                        GameObject temp = Instantiate(floorPrefab, floorPrefab.transform.position, floorPrefab.transform.rotation, FloorParent.transform);
                        temp.name = ("floor" + i.ToString() + j.ToString());
                        floorgrid[i, j] = temp;

                        //TRANSLATION AND ROTATION
                        temp.transform.Translate(new Vector3(i * floorsize, floorheight, j * floorsize));

                        //BORDERS
                        if ((i == 0) || (i == imax - 1) || (j == 0) || (j == jmax - 1))
                        {
                            //CORNERS
                            if (
                                (i == 0 && j == 0) ||
                                (i == imax - 1 && j == jmax - 1) ||
                                (i == 0 && j == jmax - 1) ||
                                (i == imax - 1 && j == 0)
                                )
                            {
                                if (i == 0 && j == 0)
                                {
                                }
                                else if ((i == imax - 1 && j == jmax - 1))
                                {
                                    temp.transform.Rotate(Vector3.up * 180);
                                }
                                else if (i == 0 && j == jmax - 1)
                                {
                                    temp.transform.Rotate(Vector3.up * 90);
                                }
                                else if (i == imax - 1 && j == 0)
                                {
                                    temp.transform.Rotate(Vector3.up * 270);

                                }
                            }
                            //WALLS
                            else
                            {
                                if (i == 0)
                                {
                                    temp.transform.Rotate(Vector3.up * 90);
                                }
                                else if (i == imax - 1)
                                {

                                    temp.transform.Rotate(Vector3.up * 270);
                                }
                                else if (j == 0)
                                {
                                }
                                else if (j == jmax - 1)
                                {
                                    temp.transform.Rotate(Vector3.up * 180);

                                }
                            }
                        }
                    }
                }
            }
        }
        else if (GameMode == Mode.Scavenge)
        {
            floorPrefab = noBuildFloor[0];

            floorgrid = new GameObject[(int)Mathf.Round(floorGridSize.x), (int)Mathf.Round(floorGridSize.y)];
            if (floorPrefab != null)
            {
                floorheight = floorPrefab.transform.position.y; //NOTE: comment this out if we ever start manually changing the floor height

                float floorsize = 10 * floorPrefab.transform.localScale.x;
                print("Floor size: " + floorsize);

                int imax = floorgrid.GetLength(0); 
                int jmax = floorgrid.GetLength(1);

                for (int i = 0; i < imax; i++)
                {
                    for (int j = 0; j < jmax; j++)
                    {
                        //BORDERS
                        if ((i == 0 )|| (i == imax-1) || (j == 0) || (j == jmax-1))
                        {
                            //CORNERS
                            if (
                                (i == 0 && j == 0)  ||
                                (i == imax-1 && j == jmax-1) ||
                                (i == 0 && j == jmax-1) ||
                                (i == imax-1 && j == 0)
                                )
                            {
                                floorPrefab = noBuildFloor[2];
                            }
                            //WALLS
                            else
                            {
                                floorPrefab = noBuildFloor[1];
                            }
                        }
                        //CENTER
                        else
                        {
                            floorPrefab = noBuildFloor[0];
                            
                        }

                        GameObject temp = Instantiate(floorPrefab, floorPrefab.transform.position, floorPrefab.transform.rotation, FloorParent.transform);
                        temp.name = ("floor" + i.ToString() + j.ToString());
                        floorgrid[i, j] = temp;
                        //TRANSLATION AND ROTATION
                        temp.transform.Translate(new Vector3(i * floorsize, floorheight, j * floorsize));

                        //BORDERS
                        if ((i == 0) || (i == imax - 1) || (j == 0) || (j == jmax - 1))
                        {
                            //CORNERS
                            if (
                                (i == 0 && j == 0) ||
                                (i == imax - 1 && j == jmax - 1) ||
                                (i == 0 && j == jmax - 1) ||
                                (i == imax - 1 && j == 0)
                                )
                            {
                                if (i == 0 && j == 0)
                                {
                                }
                                else if ((i == imax - 1 && j == jmax - 1))
                                {
                                    temp.transform.Rotate(Vector3.up * 180);
                                }
                                else if (i == 0 && j == jmax - 1)
                                {
                                    temp.transform.Rotate(Vector3.up * 90);
                                }
                                else if (i == imax - 1 && j == 0)
                                {
                                    temp.transform.Rotate(Vector3.up * 270);

                                }
                            }
                            //WALLS
                            else
                            {
                                if (i == 0)
                                {
                                    temp.transform.Rotate(Vector3.up * 90);
                                }
                                else if ( i == imax -1)
                                {

                                    temp.transform.Rotate(Vector3.up * 270);
                                }
                                else if ( j == 0)
                                {
                                }
                                else if (j == jmax-1)
                                {
                                    temp.transform.Rotate(Vector3.up * 180);

                                }
                            }
                            if(i == imax-1 && j == jmax-1)
                            {
                                temp.GetComponent<NavMeshSurface>().BuildNavMesh();
                            }
                        }
                    }
                }
            }
        }

    }
    private void SpawnPowerups()
    {
        PowerupParent = new GameObject("Powerups");
        PowerupParent.transform.parent = this.gameObject.transform;

        int r = Random.Range(1, 100);
        Debug.Log("Powerups Spawned: " + r);
        for (int i = 0; i < r; i++)
        {
            GameObject temp = Instantiate(powerups, PowerupParent.transform);
            temp.transform.position = new Vector3(Random.Range(0, floorGridSize.x * 10) - 5,
                                                    floorheight + .5f,
                                                    Random.Range(0, floorGridSize.y * 10) - 5
                                                    );
        }
    }
    private void CleanupObjects()
    {
        Destroy(PowerupParent);
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

        Cursor.visible = !buildingMenu.activeInHierarchy;
        buildingMenu.SetActive(!buildingMenu.activeInHierarchy);
    }
    #endregion
}
