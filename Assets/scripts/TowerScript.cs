using UnityEngine;
using System.Collections;

public class TowerScript : MonoBehaviour
{

    private Collider[] nearEnemies;
    public float range = 40.0f;
    private GameObject currentTar = null;
    public float attackRate = 1;
    public GameObject bullet;
    public GameObject mountedGroup;
    public GameObject mountedGun;
    private GameObject lastBullet;
    private Building buildingscript;
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("Attack", 0, attackRate);
        buildingscript = GetComponent<Building>();
    }

    private void Update()
    {
        if (currentTar != null && buildingscript.built == true)
        {
            mountedGroup.transform.rotation = Quaternion.LookRotation(currentTar.transform.position - mountedGroup.transform.position, Vector3.up);
            mountedGun.transform.rotation = Quaternion.LookRotation(currentTar.transform.position - mountedGun.transform.position, Vector3.Cross(currentTar.transform.position, mountedGun.transform.position));
        }
    }
    private void Attack()
    {
        if (currentTar != null && buildingscript.built == true)
        {
            if (Vector3.Distance(currentTar.transform.position, transform.position) <= range)
            {

                lastBullet = Instantiate(bullet, transform.position, Quaternion.LookRotation(currentTar.transform.position - transform.position));
                lastBullet.GetComponent<Bullet>().SetDamage(Random.Range(1, 3));
            }
            else
            {
                currentTar = null;
            }
        }
        else
        {
            nearEnemies = Physics.OverlapSphere(transform.position, range);
            for (int i = 0; i < nearEnemies.Length; i++)
            {
                if (nearEnemies[i].gameObject.tag == "enemy")
                {
                    currentTar = nearEnemies[i].gameObject;
                    break;
                }
            }
        }
    }
}
