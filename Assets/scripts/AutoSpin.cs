using UnityEngine;
using System.Collections;

public class AutoSpin : MonoBehaviour {

    public float spinSpeed;
    public enum Type { NONE, Left, Right, Tumble};
    public Type spinDir;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (spinDir == Type.Left)
        {
            transform.Rotate(0, spinSpeed, 0, Space.World);
        }
        else if (spinDir == Type.Right)
        {
            transform.Rotate(0, -spinSpeed, 0, Space.World);
        }
        else if (spinDir == Type.Tumble)
        {
            transform.Rotate(-spinSpeed, 0, 0, Space.World);
        }
        else if (spinDir == Type.NONE)
        {

        }
	}
}
