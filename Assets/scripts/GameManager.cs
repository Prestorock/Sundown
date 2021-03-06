using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		23/01/2018 05:58
-------------------------------------
Description: A self referencial singleton that can help objects communicate and manage hud/gameplay

    Sorry this code could definitely be cleaner
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

    [Header("Children")]
    public EnemyManager EM;
    public GameObject mainCamera;
    public GameObject menuCamera;
    public GameObject mainMenu;
    public GameObject HUDGroup;
    public GameObject pauseMenu;
    public GameObject buildingMenu;
    public Text ammoText;
    public Text supplyText;
    public Text timerText;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject buildFloor;
    public GameObject powerups;

    [Header("Variables")]
    public Vector2 floorGridSize;
    public float ScavengeSeconds = 60;
    public SpawnArea SpawnArea;
    [HideInInspector]
    public Player player;
    [HideInInspector]
    public bool playing = false;
    [HideInInspector]
    public bool spawningEnemies = false;
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
    private bool hasBeggedToTheGameDevGodsMightiestofMighty = false;
    #endregion

    #region Enumerators
    public enum Mode { Scavenge, Survive, Dev, MainMenu };

    #endregion

    #region Unity Methods
    private void Awake()
    {
        gm = this;
        hasBeggedToTheGameDevGodsMightiestofMighty = BegToTheElderGods("Oh Orryx, brigtest of lights, please deliver my code from evil and may my arrays always start at 0 and never stray. Amen.");
        if (hasBeggedToTheGameDevGodsMightiestofMighty)
        {
            Debug.Log("We have said our prayers, but the gods are fickle and may have abandoned us yet.");
        }
    }
    private bool BegToTheElderGods(string prayer)
    {
        bool accepted;
        if (Random.value > 1 || Random.value < 0)
        {
            accepted = false;
        }
        else
        {
            accepted = true;
        }

        if (accepted == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Start()
    {
        GenerateNavMesh();
        player = SpawnPlayer();

        Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.Confined;
        if (GameMode == Mode.Survive) //development mode can be started by setting the mode from the gamemaster object
                                      //this will stop the normal progression of the game like removing the floor and spawning objects;
        {
            StartCoroutine(ChangeGameMode(Mode.Survive));
        }
        else if (GameMode == Mode.Scavenge)
        {
            StartCoroutine(ChangeGameMode(Mode.Scavenge));

        }
        else if (GameMode == Mode.MainMenu)
        {
            StartCoroutine(ChangeGameMode(Mode.MainMenu));
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
            timerText.text = Mathf.RoundToInt(modeTimer) / 60 + " : " + Mathf.RoundToInt(modeTimer) % 60;

        }

        if (GameMode == Mode.Dev)
        {
            //DEV MODE CODE
        }

        if (modeTimer >= ScavengeSeconds && GameMode == Mode.Scavenge)
        {
            StartCoroutine(ChangeGameMode(Mode.Survive));
            print("times up. survive mode");
        }
        //TODO: Add Scavenge Mode when enemies are done;

        if (modeTimer >= ScavengeSeconds && GameMode == Mode.Survive && EM.upkeep <= 0)
        {
            StartCoroutine(ChangeGameMode(Mode.Scavenge));
            print("times up. scavenge mode");
        }



    }
    #endregion

    #region Custom Methods
    public void PlayTheGame()
    {
        StartCoroutine(ChangeGameMode(Mode.Scavenge));
    }
    public void QuitToMenu()
    {
        StartCoroutine(ChangeGameMode(Mode.MainMenu));
        Time.timeScale = 1;
    }

    public void QuitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }
    private void GenerateNavMesh()
    {
        storeObjectGroup.transform.parent.GetComponent<NavMeshSurface>().RemoveData();
        storeObjectGroup.transform.parent.GetComponent<NavMeshSurface>().BuildNavMesh();

    }
    public void PlayerDeath()
    {
        QuitToMenu();
    }
    private IEnumerator ChangeGameMode(Mode mode)
    {
        GameMode = mode;

        if (GameMode == Mode.MainMenu)
        {

            if (playing)
            {
                Cleanup();
                DestroyFloor();
                CollectPlayer();
            }
            mainCamera.GetComponent<TDCamera>().FadeOut();

            playing = false;
            player.canMove = false;
            player.canAttack = false;

            while (mainCamera.GetComponent<TDCamera>().stillFading)
            {
                //print("fading out");
                yield return null;
            }
            if (gameFullyInitialized == true)
            {
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }

            mainCamera.GetComponent<TDCamera>().FadeIn();
            menuCamera.SetActive(true);

            while (mainCamera.GetComponent<TDCamera>().stillFading)
            {
                //print("fading in");
                yield return null;
            }
            mainMenu.SetActive(true);
            mainCamera.SetActive(false);

        }
        else
        {
            if (playing == false)
            {
                mainCamera.SetActive(true);

                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
                playing = true;
            }
            modeTimer = 0; // stop infinite loop.
            mainCamera.GetComponent<TDCamera>().FadeOut();

            player.canMove = true;
            player.canAttack = true;

            while (mainCamera.GetComponent<TDCamera>().stillFading)
            {
                //print("fading out");
                yield return null;
            }

            menuCamera.SetActive(false);
            mainMenu.SetActive(false);
            Cleanup();
            if (gameFullyInitialized == false)
            {
                Destroy(GameObject.FindGameObjectWithTag("enemy"));
            }
            DestroyFloor();
            CollectPlayer();

            mainCamera.GetComponent<TDCamera>().FadeIn();

            GenerateFloor();
            GenerateNavMesh();
            SpawnThings();
            PlacePlayer();
            mainCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + mainCamera.GetComponent<TDCamera>().heightBuffer, player.transform.position.z);
            player.canMove = true;
            player.canAttack = true;

            while (mainCamera.GetComponent<TDCamera>().stillFading)
            {
                //print("fading in");
                yield return null;
            }

            modeTimer = 0; //actually set timer
            gameFullyInitialized = true;
            if (GameMode == Mode.Survive)
            {
                EM.SpawnEnemies(40);
            }
        }
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
        else if (GameMode == Mode.Dev)
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
            int r = Random.Range(60, 100);
            Debug.Log("Powerups Spawned: " + r);
            for (int i = 0; i < r; i++)
            {
                GameObject temp = Instantiate(powerups, PowerupParent.transform);
                temp.transform.position = SpawnArea.RandomPoint();
            }
        }
        else if (GameMode == Mode.Survive)
        {
            baseParent.SetActive(true);

        }
    }
    private void Cleanup()
    {
        EM.CleanupEnemies();
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
