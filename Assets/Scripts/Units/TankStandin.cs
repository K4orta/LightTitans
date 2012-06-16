using UnityEngine;
using System.Collections;

public class TankStandin : MonoBehaviour {
	public Unit.Side owner;
	// Use this for initialization
	void Start () {
		Tank tn= Logic.Playstate.addTank(transform.position, owner, transform.rotation);
		tn.rigidbody.useGravity=true;
		tn.rigidbody.WakeUp();
		Destroy(this.gameObject);
	}
}
