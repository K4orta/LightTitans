using UnityEngine;
using System.Collections;

public class PlayerRocket : Chaingun {
	protected Transform trailSystem;
	// Use this for initialization
	protected override void Start () {
		base.Start();
		shotDamage = 100;
		trailSystem = Logic.Playstate.getRocketTrail();
		//trailSystem.transform.localPosition = Vector3.zero;
		trailSystem.transform.rotation = Quaternion.identity;
		trailSystem.transform.position = transform.position;
		trailSystem.GetComponent<ParticleSystem>().Clear();
		trailSystem.transform.parent = transform;
		trailSystem.GetComponent<ParticleSystem>().Play();
	}
	
	protected override void OnCollisionEnter (Collision hit){
		trailSystem.GetComponent<ParticleSystem>().Stop();
		trailSystem.transform.parent = null;
		Transform tn = Logic.Playstate.getExplosion();
				
		tn.transform.position = hit.contacts[0].point;

		Collider[] expHit = Logic.Playstate.AddExplosiveForce(tn.transform.position, 13);
		foreach(Collider blast in expHit){
			Unit uni;
			if(blast.transform.parent!=null){
				uni = blast.transform.parent.GetComponent<Unit>();
			}else{
				uni = blast.gameObject.GetComponent<Unit>();
			}
			if(uni != null&&uni.tag!="Titan"){
							
				uni.Hurt(25);
				HUD.hitThisFrame = true;
			}
		}
		
		base.OnCollisionEnter (hit);
	} 
	
	
	
}
