using UnityEngine;
using System.Collections;

public class HeatSeeker : PlayerRocket {
	protected Unit seekTarget;
	protected bool locked;
// Use this for initialization
	protected override void Start () {
		base.Start();
		locked = true;
		shotDamage = 50;
	}
	
	protected void FixedUpdate(){
		rigidbody.velocity = Vector3.zero;
		rigidbody.AddRelativeForce (Vector3.forward*40);
		if(locked && seekTarget!=null){
			
			//Vector3 dist = seekTarget.transform.position - transform.position;
			//Quaternion nr = Quaternion.LookRotation(dist);
			//rigidbody.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x,nr.eulerAngles.y,transform.eulerAngles.z));
			transform.LookAt(seekTarget.transform.position);
			
			if(seekTarget.flareProtection > 0){
				locked = false;
				HUD.misslesInFlight -=1;
				rigidbody.rotation = Quaternion.Euler(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360));
			}
		}
	}
	
	protected override void OnCollisionEnter (Collision hit){
		//base.OnCollisionEnter (hit);
		trailSystem.GetComponent<ParticleSystem>().Stop();
		trailSystem.transform.parent = null;
		Transform tn = Logic.Playstate.getExplosion();
		if(seekTarget!=null&&locked&&seekTarget.name=="PlayerCopter")
			HUD.misslesInFlight -=1;
		tn.transform.position = hit.contacts[0].point;
		Unit nu = hit.gameObject.GetComponent<Unit>();
		if(nu != null){
			nu.Hurt(shotDamage);
		}
		
		Destroy(gameObject);
	}
	
	public void SetTarget(Unit Target){
		seekTarget = Target;
		if(seekTarget.name=="PlayerCopter"){
			HUD.misslesInFlight += 1;
		}
	}
	
}
