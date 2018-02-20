using UnityEngine;
using System.Collections;

public class NodeControl : MonoBehaviour {

	public bool active{
		get{return isActive; }
		set{isActive = !isActive; }
	}public bool isActive;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
