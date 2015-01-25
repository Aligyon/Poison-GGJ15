using UnityEngine;
using System.Collections;

public class Antidote : MonoBehaviour {

    float inactivetime = 0f;
    bool aactive = true;
    public Collider2D coll2;
    public Transform hand;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (hand != null) transform.position = hand.position;
        if (!aactive) return;
        inactivetime -= Time.deltaTime;
        if (inactivetime <= 0) {
            
            collider2D.enabled = true;
        }
	}
    public void SetInactiveTime(float t) {
        inactivetime = t;
        
        collider2D.enabled = false;

    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            PlayerInput p = (PlayerInput)other.gameObject.GetComponent(typeof(PlayerInput));
            p.PickUpAntidote(transform);
            
        }
    }
    public void ToggleActive(bool onoff) {
        collider2D.enabled = onoff;
        coll2.enabled = onoff;

        if (onoff)
            rigidbody2D.gravityScale = 1;
        else
            rigidbody2D.gravityScale = 0;
    }

}
