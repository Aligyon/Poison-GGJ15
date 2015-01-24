using UnityEngine;
using System.Collections;

public class Cranemove : MonoBehaviour {

    public Transform crane;

    public Transform pos1;
    public Transform pos2;

    public float speed;
    bool leftright = true; //true=toward pos2 (right);

    float t = 0;

    public bool active = true;


	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if(!active) return;

        t += (Time.deltaTime*speed);

        if(leftright) {
            crane.position = Vector2.Lerp(pos1.position, pos2.position, t);
            if (Vector2.Distance(crane.position, pos2.position) < 0.3f) {
                leftright = false;
                t = 0;
            }      
        }
        else {
            crane.position = Vector2.Lerp(pos2.position, pos1.position, t);
            if (Vector2.Distance(crane.position, pos1.position) < 0.3f) {
                leftright = true;
                t = 0;
            }
        }

	    
	}
}
