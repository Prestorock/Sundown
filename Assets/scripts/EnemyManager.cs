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
    public int upkeep = 0;
    #endregion

    #region Private Variables
    private GameObject[] Enemies = new GameObject[1];
    #endregion

    #region Enumerations

    #endregion

    #region Unity Methods
    #endregion

    #region Custom Methods
    public void SpawnEnemies(int number)
    {
        Enemies = new GameObject[number];
        System.Array.Resize(ref Enemies, upkeep + number);

        for (int i = 0; i < number; i++)
        {
            Vector3 spawnpoint = SpawnArea.RandomPoint();
            GameObject temp = Instantiate(EnemyToSpawn, spawnpoint, EnemyToSpawn.transform.rotation, this.gameObject.transform);
            Enemies[i] = temp;
            upkeep++;
        }

        GameManager.gm.spawningEnemies = false;
    }

    public void CleanupEnemies()
    {
        foreach (GameObject enemy in Enemies)
        {
            Destroy(enemy);
        }
        upkeep = 0;
        System.Array.Resize(ref Enemies, 0);
    }
    #endregion
}
