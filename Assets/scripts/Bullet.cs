using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		23/01/2018 19:53
-------------------------------------
Description:

===================================*/

public class Bullet : MonoBehaviour 
{
    #region Public Variables
    public float speed;
    public float trail;
    #endregion

    #region Private Variables
    #endregion

    #region Unity Methods
    private void Awake()
    {
        this.gameObject.GetComponent<TrailRenderer>().time = trail;
    }
    void Start () {
	}
	
	void Update () {
        transform.Translate(Vector3.forward*speed);
	}
	#endregion
	
	#region Custom Methods
	#endregion
}
