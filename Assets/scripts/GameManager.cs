using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    [Header("Scene Objects")]
    public GameObject surviveSpawn;
    public GameObject scavengeSpawn;
    public GameObject baseParent;
    public GameObject storeObjectGroup;
    

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject buildFloor;
    public GameObject pauseMenu;
    public GameObject buildingMenu;
    public GameObject powerups;
    public Text ammoText;
    public Text supplyText;
    public Text timerText;

    [Header("Variables")]
    public Vector2 floorGridSize;
    public float ScavengeSeconds = 60;

    [HideInInspector]
    public Player player;
    #endregion

    #region Private Variables
    private GameObject FloorParent = null;
    private GameObject PowerupParent;
    private bool paused = false;
    private GameObject[,] floorgrid;
    [SerializeField]
    private Mode GameMode = Mode.Scavenge;
    private float floorheight = 0; //comment out reference in GenerateFloor if we ever start changing this.
    private float modeTimer = 0f;
    private bool gameFullyInitialized = false;
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
        player = SpawnPlayer();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        if (GameMode != Mode.Dev) //development mode can be started by setting the mode from the gamemaster object
                                  //this will stop the normal progression of the game like removing the floor and spawning objects;
        {
            StartCoroutine(ChangeGameMode(Mode.Scavenge));
        }
        else
        {
            //development mode special code.
            ChangeGameMode(Mode.Dev);
        }
    }
    private void Update()
    {
        
        if (Time.timeScale != 0)
        {
            modeTimer += Time.deltaTime;
            timerText.text = Mathf.RoundToInt( modeTimer) / 60 + " : " + Mathf.RoundToInt(modeTimer) % 60;
        }

        if (GameMode == Mode.Dev)
        {
            //DEV MODE CODE
        }

        //TODO: Find a good scavenge timer
        if (modeTimer >= ScavengeSeconds && GameMode == Mode.Scavenge)
        {
            StartCoroutine(ChangeGameMode(Mode.Survive));
            print("times up. survive mode");
        }
        //TODO: Add Survival Mode when enemies are done;
        
        if(modeTimer >= ScavengeSeconds && GameMode == Mode.Survive)
        {
            StartCoroutine(ChangeGameMode(Mode.Scavenge));
            print("times up. scavenge mode");
        }

    }
    #endregion

    #region Custom Methods

    private void GenerateNavMesh()
    {
        if (GameMode != Mode.Scavenge)
        {
            floorgrid[0, 0].GetComponent<NavMeshSurface>().RemoveData();

            floorgrid[0, 0].GetComponent<NavMeshSurface>().BuildNavMesh();
        }
        else
        {
            storeObjectGroup.transform.parent.GetComponent<NavMeshSurface>().RemoveData();
            storeObjectGroup.transform.parent.GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }
    IEnumerator ChangeGameMode(Mode mode)
    {
        modeTimer = 0; // stop infinite loop.
        Camera.main.GetComponent<TDCamera>().fadeOut();

        GameMode = mode;
        player.canMove = false;
        player.canAttack = false;

        while (Camera.main.GetComponent<TDCamera>().stillFading)
        {
            //print("fading out");
            yield return null;
        }

        Cleanup();
        DestroyFloor();
        CollectPlayer();

        Camera.main.GetComponent<TDCamera>().fadeIn();

        GenerateFloor();
        GenerateNavMesh();
        SpawnThings();
        PlacePlayer();
        Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + Camera.main.GetComponent<TDCamera>().heightBuffer, player.transform.position.z);
        player.canMove = true;
        player.canAttack = true;

        while (Camera.main.GetComponent<TDCamera>().stillFading)
        {
            //print("fading in");
            yield return null;
        }
        
        modeTimer = 0;
        gameFullyInitialized = true;
    }
    public Mode GetGameMode()
    {
        return GameMode;
    }
    public float GetFloorHeight()
    {
        return floorheight;
    }

    private void CollectPlayer()
    {
        player.gameObject.SetActive(false);
    }
    private void PlacePlayer()
    {
        if (GameMode == Mode.Scavenge)
        {
            player.transform.position = scavengeSpawn.transform.position;
        }
        else if (GameMode == Mode.Survive)
        {
            player.transform.position = surviveSpawn.transform.position;

        }
        else
        {
            //dev mode placement
            player.transform.position = surviveSpawn.transform.position;

        }
        player.gameObject.SetActive(true);

    }

    private Player SpawnPlayer()
    {
        GameObject playerObj = Instantiate(playerPrefab);
        return playerObj.GetComponent<Player>();
    }

    /// <summary>
    /// EZ floor destruction. Throw away the parent and just make a new one. :)
    /// </summary>
    private void DestroyFloor()
    {
        if (gameFullyInitialized != true)
        {
        }
        else
        {
            baseParent.SetActive(false);
        }
    }
    /// <summary>
    /// Generates a floor and tiles it, based on the localscale of the object and the size defined by the user.
    /// </summary>
    private void GenerateFloor()
    {
        if (FloorParent == null)
        {
            FloorParent = new GameObject("Floors");
        }
        FloorParent.transform.parent = baseParent.transform;

        GameObject floorPrefab;
        if (GameMode == Mode.Survive || GameMode == Mode.Dev)
        {

            floorPrefab = buildFloor;

            floorgrid = new GameObject[(int)Mathf.Round(floorGridSize.x), (int)Mathf.Round(floorGridSize.y)];
            if (floorPrefab != null)
            {
                floorheight = baseParent.transform.position.y; //NOTE: comment this out if we ever start manually changing the floor height

                float floorsize = 10 * floorPrefab.transform.localScale.x;
                print("Floor size: " + floorsize);

                int imax = floorgrid.GetLength(0);
                int jmax = floorgrid.GetLength(1);

                for (int i = 0; i < imax; i++)
                {
                    for (int j = 0; j < jmax; j++)
                    {
                        //commented out the procedural floor code because we have a model for the store. If we had a lot more time this would be beefed up.
                        #region procedural floor selection
                        /*
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
                        */
                        #endregion
                        GameObject temp = Instantiate(floorPrefab, baseParent.transform.position, baseParent.transform.rotation, FloorParent.transform);
                        temp.name = ("floor" + i.ToString() + j.ToString());
                        floorgrid[i, j] = temp;

                        //TRANSLATION AND ROTATION
                        temp.transform.Translate(new Vector3(i * floorsize, floorheight, j * floorsize));
                        #region procedural border translation
                        /*
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
                        */
                        #endregion
                    }
                }
            }
        }
        #region procedural scavenge floor
        else if (GameMode == Mode.Scavenge)
        {
            /*
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
        */
        }
        #endregion
    }
    private void SpawnThings()
    {
        PowerupParent = new GameObject("Powerups");
        PowerupParent.transform.parent = this.gameObject.transform;
        if (GameMode == Mode.Scavenge)
        {
            storeObjectGroup.SetActive(true);
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
        else if (GameMode == Mode.Survive)
        {
            baseParent.SetActive(true);

        }
    }
    private void Cleanup()
    {
        Destroy(PowerupParent);
        if (GameMode == Mode.Survive)
        {
            storeObjectGroup.SetActive(false);
        }
        else if (GameMode == Mode.Scavenge)
        {
            baseParent.SetActive(false);
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

        Cursor.visible = !buildingMenu.activeInHierarchy;
        buildingMenu.SetActive(!buildingMenu.activeInHierarchy);
    }
    #endregion
}
