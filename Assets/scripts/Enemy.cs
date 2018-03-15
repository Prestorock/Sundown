using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		14/02/2018 04:20
-------------------------------------
Description:

===================================*/

public class Enemy : MonoBehaviour
{
    #region Public Variables
    public int maxHealth = 100;
    public int attackDamage = 5;
    public float attackSpeed = 2.0f;
    #endregion

    #region Private Variables
    private NavMeshAgent agent;
    private int healthPoints;
    private bool canAttack = true;
    private bool canMove = true;
    private GameObject target = null;
    private GameObject secondaryTarget = null;
    private float attackCD = 0.0f;
    private EnemyManager EM;
    #endregion

    #region Enumerations

    #endregion

    #region Unity Methods
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //target = GameManager.gm.player.gameObject;
        healthPoints = maxHealth;
        attackCD = attackSpeed;
    }
    private void Update()
        
    {
        if (Time.timeScale != 0)
        {
            Intelligence();

            if (healthPoints <= 0)
            {
                Death();
            }
        }
    }
    #endregion

    #region Custom Methods
    private void Death()
    {
        canMove = false;
        canAttack = false;

        GameManager.gm.EM.upkeep -= 1;
        Destroy(this.gameObject);
    }

    private void Intelligence()
    { //TODO: State based AI
        if (target)
        {
            if (Vector3.Distance(target.transform.position, transform.position) > 3)
            {
                if (agent.isOnNavMesh)
                {

                    NavMeshPath path = new NavMeshPath();
                    agent.CalculatePath(target.transform.position, path);

                    if (path.status != NavMeshPathStatus.PathComplete)
                    {
                        target = null;
                    }
                    else
                    {
                        agent.SetDestination(target.transform.position);
                        agent.isStopped = false;
                    }
                        
                }
            }
            else
            {
                agent.isStopped = true;
                if (attackCD >= attackSpeed && canAttack)
                {
                    if (target.GetComponent<Player>())
                    {
                        Attack(attackDamage, target.GetComponent<Player>());
                    }
                    else if (target.GetComponent<Building>())
                    {
                        Attack(attackDamage, target.GetComponent<Building>());
                    }
                }
                else
                {
                    attackCD += Time.deltaTime;
                }
            }
            if(canMove && agent.isOnNavMesh)
            {
                agent.isStopped = false;
            }
            else
            {
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                }
            }
        }
        else
        {
            if (GameManager.gm.player.gameObject)
            {
                target = GameManager.gm.player.gameObject;
                if (agent.isOnNavMesh)
                {

                    NavMeshPath path = new NavMeshPath();
                    agent.CalculatePath(target.transform.position, path);

                    if (path.status != NavMeshPathStatus.PathComplete)
                    {
                        target = GameObject.FindGameObjectWithTag("building");
                        if (target)
                        {
                            agent.SetDestination(target.transform.position);
                            agent.isStopped = false;
                        }
                    }
                    else
                    {
                        agent.SetDestination(target.transform.position);
                        agent.isStopped = false;
                    }

                }
            }
        }
    }

    private void Attack(int dmg, Player target)
    {
        target.AlterHealth(-dmg);
        //Vector3 dir = target.transform.position - transform.position;
        //dir = -dir.normalized;
        //target.GetComponent<Rigidbody>().AddForce(dir*dmg);
        attackCD = 0.0f;
    }

    private void Attack(int dmg, Building target)
    {
        target.AlterHealth(-dmg);
        attackCD = 0.0f;
    }

    public void SetEnemyManager(EnemyManager em)
    {
        EM = em;
    }

    public void AlterHealth(int amount)
    {
        healthPoints += amount;
    }
    #endregion
}
