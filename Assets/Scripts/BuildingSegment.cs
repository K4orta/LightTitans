using UnityEngine;
using System.Collections;

public class BuildingSegment : Entity {
	
	public AudioClip breakSound;

	// Use this for initialization
	protected override void Start () {

	}
	
	void OnJointBreak(){
		Transform tn = Logic.Playstate.getExplosion();
		tn.transform.position = transform.position-transform.up*20;
		if(breakSound!=null)
			Logic.Playstate.AudioAt(breakSound,transform.position-transform.up*20,.8f);
	}
	
	void OnCollisionEnter(Collision hit){
		if(hit.gameObject.tag == "Damageable"){
			if(hit.relativeVelocity.magnitude > 17){
				hit.gameObject.GetComponent<Entity>().Hurt(hit.relativeVelocity.magnitude);
			}
		}else if(hit.gameObject.layer == 12){
			
			if(hit.relativeVelocity.magnitude > 12){
				//Kill();
				//rigidbody.detectCollisions= false;
				Transform dc = Logic.Playstate.getDustCloud();
				dc.transform.position = hit.contacts[0].point;
				StartCoroutine(KillSeg(2.8F));
				gameObject.layer = 14;
			}

		}
	}
	
	 IEnumerator KillSeg(float Wait){
		
		yield return new WaitForSeconds(Wait);
		
		Kill();
	}
	
}
