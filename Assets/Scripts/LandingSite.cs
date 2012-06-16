using UnityEngine;
using System.Collections;

public class LandingSite : MonoBehaviour {
	
	public Transform oppositeSpawn;
	public Unit.Side owner;
	// Use this for initialization
	void Start () {
		renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
