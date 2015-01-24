using UnityEngine;
using System.Collections;

public class Antidote : MonoBehaviour {

    float inactivetime = 0f;
    bool active = true;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        inactivetime -= Time.deltaTime;
        if (inactivetime <= 0) {
            active = true;
            collider2D.enabled = true;
        }
	}
    public void SetInactiveTime(float t) {
        inactivetime = t;
        active = false;
        collider2D.enabled = false;

    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            PlayerInput p = (PlayerInput)other.gameObject.GetComponent(typeof(PlayerInput));
            p.PickUpAntidote(transform);
            
        }
    }

}
