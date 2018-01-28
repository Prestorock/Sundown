using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		28/01/2018 06:33
-------------------------------------
Description:

===================================*/

[RequireComponent(typeof(Interactable))]
public class Weapon : MonoBehaviour
{
    #region Public Variables
    public bool isGun = true;
    public int damage = 1;
    #endregion

    #region Private Variables

    #endregion

    #region Unity Methods
    private void Start()
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), GameManager.gm.playerObject.GetComponent<Player>().modelObject.GetComponent<Collider>(), true);
    }
    #endregion

    #region Custom Methods
    #endregion
}
