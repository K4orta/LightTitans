using UnityEngine;
using System.Collections;

public class TankShot : BasicProjectile {
	
	public AudioClip ts1;
	public AudioClip ts2;
	// Use this for initialization
	protected override void Start () {
		base.Start();
		shotDamage = 25;
		
	}
	
	void Awake(){
		audio.clip = Random.value<.5f?ts1:ts2;
		//audio.pitch = Random.Range (-1f,1f);
		audio.Play();
	}
	
	protected override void OnCollisionEnter (Collision hit){
		Transform tn = (Transform)Logic.Playstate.getImpact();
		
		tn.transform.position = hit.contacts[0].point;
		if(gameObject.layer == 8){
			tn.GetComponent<ParticleSystem>().startColor = new Color(.2f, .5f, 1f, 1f);
		}else{
			tn.GetComponent<ParticleSystem>().startColor = new Color(1f, .4f, .2f, 1f);
		}
		base.OnCollisionEnter (hit);
	} 
}
