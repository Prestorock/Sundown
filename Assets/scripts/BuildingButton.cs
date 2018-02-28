using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		28/02/2018 06:23
-------------------------------------
Description:

===================================*/

public class BuildingButton : MonoBehaviour
{
    #region Public Variables

    #endregion

    #region Private Variables
    private Player player;
    #endregion

    #region Enumerations

    #endregion

    #region Unity Methods
    private void Update()
    {
        if (player == null)
        {
            player = GameManager.gm.player;
        }
    }
    #endregion

    #region Custom Methods
    public void SendBuildCommand(GameObject toBeBuilt)
    {
        player.StartBuildPlacement(toBeBuilt);
    }
    #endregion
}
