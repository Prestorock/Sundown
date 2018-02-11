using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		07/02/2018 05:21
-------------------------------------
Description:

===================================*/

public class Powerup : MonoBehaviour 
{
    #region Public Variables
    public Type Power;
    #endregion

    #region Private Variables
    #endregion

    #region Enumerators
    public enum Type
    {
        Ammo,
        Supplies,
        Powerup
    };
    #endregion

    #region Unity Methods
    private void Start()
    {
        int r = Random.Range(1, 100);
        if (r <= 43)
        {
            Power = Type.Ammo;
        }
        else if (r <= 86)
        {
            Power = Type.Supplies;

        }
        else
        {
            Power = Type.Powerup;

        }
        transform.position = new Vector3(transform.position.x, GameManager.gm.GetFloorHeight()+.5f, transform.position.z);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<Player>() && !other.GetComponent<SelectionTarget>())
        {
            AwardPowerup(other.GetComponentInParent<Player>().gameObject);
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Custom Methods
    private void AwardPowerup(GameObject player)
    {
        Player p = player.GetComponent<Player>();
        if(Power == Type.Powerup)
        {
            Debug.Log("POWERUP!");
        }
        else if (Power == Type.Ammo)
        {
            p.AlterAmmo(1);
            Debug.Log("ammo: "+ p.ammo);
        }
        else if (Power == Type.Supplies)
        {
            p.AlterSupplies(1);
            Debug.Log("supplies" + p.supplies);
        }
    }
    #endregion
}
