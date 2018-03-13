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
    [Range(0,100)]
    public float relativeAmmoChance;
    [Range(0,100)]
    public float relativeSuppliesChance;
    [Range(0,100)]
    public float relativeGunChance;
    public GameObject baseGunPrefab;
    #endregion

    #region Private Variables
    private Type Power;
    private float aChance;
    private float sChance;
    private float gChance;
    #endregion

    #region Enumerators
    public enum Type
    {
        Ammo,
        Supplies,
        Gun
    };
    #endregion

    #region Unity Methods
    private void Start()
    {
        //this is probably unnecessary but I'm tired
        //takes the public variables and turns them into a standard percentage value
        aChance = ((relativeAmmoChance / (relativeAmmoChance + relativeSuppliesChance + relativeGunChance ))*100);
        sChance = ((relativeSuppliesChance / (relativeAmmoChance + relativeSuppliesChance + relativeGunChance)) * 100);
        gChance = ((relativeGunChance / (relativeAmmoChance + relativeSuppliesChance + relativeGunChance)) * 100);

        int r = Random.Range(1, 100);
        if (r <= aChance)
        {
            Power = Type.Ammo;
        }
        else if (r <= sChance + aChance)
        {
            Power = Type.Supplies;

        }
        else
        {
            Power = Type.Gun;

        }
        transform.position = new Vector3(transform.position.x, GameManager.gm.GetFloorHeight()+.5f, transform.position.z);
        if(Power == Type.Gun)
        {
            GameObject temp = Instantiate(baseGunPrefab, this.gameObject.transform.position, baseGunPrefab.transform.rotation, this.gameObject.transform.parent);
            temp.name = "Weapon";
            Destroy(this.gameObject);
            //temp.transform.position = this.gameObject.transform.position;
        }
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
        if(Power == Type.Gun)
        {
            //Debug.Log("POWERUP!");
        }
        else if (Power == Type.Ammo)
        {
            p.AlterAmmo(Random.Range(1,3));
            //Debug.Log("ammo: "+ p.ammo);
        }
        else if (Power == Type.Supplies)
        {
            p.AlterSupplies(1);
            //Debug.Log("supplies" + p.supplies);
        }
    }
    #endregion
}
