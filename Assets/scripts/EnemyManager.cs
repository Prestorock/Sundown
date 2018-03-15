using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		14/03/2018 05:33
-------------------------------------
Description:

===================================*/

public class EnemyManager : MonoBehaviour
{
    #region Public Variables
    public SpawnArea SpawnArea;
    public GameObject EnemyToSpawn;
    [HideInInspector]
    public int upkeep = 0;
    #endregion

    #region Private Variables
    private GameObject[] Enemies = new GameObject[40];
    #endregion

    #region Enumerations

    #endregion

    #region Unity Methods
    private void Update()
    {
    }
    #endregion

    #region Custom Methods
    public void SpawnEnemy()
    {
        Vector3 spawnpoint = SpawnArea.RandomPoint();
        GameObject temp = Instantiate(EnemyToSpawn, spawnpoint, EnemyToSpawn.transform.rotation, this.gameObject.transform);
        Enemies[Enemies.Length] = temp;
    }

    public void CleanupEnemies()
    {
        foreach (GameObject enemy in Enemies)
        {
            Destroy(enemy);
        }
    }
    #endregion
}
