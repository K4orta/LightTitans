using UnityEngine;
using System.Collections;

public class Chaingun : BasicProjectile {
	
	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	protected override void Update () {
		lifeTime -= Time.deltaTime;
		if(lifeTime<=0){
			Destroy(gameObject);	
		}
	}
	
	protected override void OnCollisionEnter(Collision hit){
		Unit nu = hit.gameObject.GetComponent<Unit>();
		if(nu != null){
			if(tag=="PlayerShot" && nu.tag == "Titan"){
			shotDamage *= 0; 
			}
			nu.Hurt(shotDamage);
			HUD.hitThisFrame = true;
		}
		Transform tn = (Transform)Logic.Playstate.getImpact();
		tn.GetComponent<ParticleSystem>().startColor = new Color(.2f, .5f, 1f, 1f);
		tn.transform.position = hit.contacts[0].point;
		Destroy(gameObject);
	}
}
