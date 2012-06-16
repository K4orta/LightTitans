using UnityEngine;
using System.Collections;

public class TitanShot : BasicProjectile {
	
	
	// Use this for initialization
	protected override void Start () {
		shotDamage = 250;
		
	}
	
	
	
	// Update is called once per frame
	protected override void OnCollisionEnter(Collision hit){
		Unit nu = hit.gameObject.GetComponent<Unit>();
		if(nu != null){
			nu.Hurt(Mathf.Max(shotDamage-nu.armor,0));
		}
		
		Transform tn = Logic.Playstate.getExplosion();
		tn.transform.position = hit.contacts[0].point;
		
		/*Collider[] exps = Logic.Playstate.AddExplosiveForce(transform.position, 100);
		for(var i=0;i<exps.GetLength(0);++i){
			if(exps[i].rigidbody!=null)
				exps[i].rigidbody.AddExplosionForce(500,transform.position,100);
		}*/
		
		Destroy(gameObject);
	}
}
