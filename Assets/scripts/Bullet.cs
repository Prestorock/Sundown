using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		23/01/2018 19:53
-------------------------------------
Description: A bullet script. This should accept the damage from the weapon that fired it as a variable.
            When spawned it flies straight on its z axis at "speed"

===================================*/

public class Bullet : MonoBehaviour 
{
    #region Public Variables
    public float speed;
    public float trail;
    public float lifetime = 10.0f;
    #endregion

    #region Private Variables
    private float timer;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        this.gameObject.GetComponent<TrailRenderer>().time = trail;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Weapon>() && !other.GetComponent<SelectionTarget>())
        {
            Destroy(this.gameObject);
        }
    }

    void Update () {
        timer += Time.deltaTime;

        if(timer >= lifetime)
        {
            Destroy(this.gameObject);
        }
        transform.Translate(Vector3.forward*speed);
	}
	#endregion
	
	#region Custom Methods
	#endregion
}
