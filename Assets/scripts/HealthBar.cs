using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===================================
Project:	Sundown Survival	
Developer:	Preston Rockholt Prockho0@email.cpcc.edu
Company:	Sundown Studios
Date:		27/02/2018 22:12
-------------------------------------
Description:

===================================*/

public class HealthBar : MonoBehaviour
{
    #region Public Variables
    public AnimationCurve healthRamp;
    public Sprite[] Segments = new Sprite[6];
    #endregion

    #region Private Variables
    private int healthchunks = 5;
    private SpriteRenderer sr;
    #endregion

    #region Enumerations

    #endregion

    #region Unity Methods
    private void Start()
    {
        sr = this.gameObject.GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        healthchunks = Mathf.CeilToInt(healthRamp.Evaluate(GameManager.gm.player.GetHealth()));
        sr.sprite = Segments[healthchunks];
    }
    #endregion

    #region Custom Methods

    #endregion
}
