using UnityEngine;
using System.Collections;

public class Lifetime : MonoBehaviour {

    public float lifetime;
    float t;

	// Use this for initialization
	void Start () {
        t = lifetime;
	}
	
	// Update is called once per frame
	void Update () {
        t -= Time.deltaTime;
        if (t <= 0f) Destroy(gameObject);
	}
}
