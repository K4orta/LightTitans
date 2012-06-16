using UnityEngine;
using System.Collections;

public class FPCamera : MonoBehaviour {
	public Transform target;
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(target.position.x, target.position.y, target.position.z) + Vector3.forward * 2;
		transform.rotation = target.rotation;
	}
	
	void OnGUI(){
		
	}
}
