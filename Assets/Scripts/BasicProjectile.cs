using UnityEngine;
using System.Collections;

public class BasicProjectile : MonoBehaviour {
	public float lifeTime=5;
	protected float shotDamage=25;
	
	// Use this for initialization
	protected virtual void Start () {
		
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		lifeTime -= Time.deltaTime;
		if(lifeTime<=0){
			Destroy(gameObject);	
		}
	}
	
	protected virtual void OnCollisionEnter(Collision hit){
		Unit nu = hit.gameObject.GetComponent<Unit>();
		
		if(nu != null){
			nu.Hurt(Mathf.Max(shotDamage-nu.armor,0));
		}
		Destroy(gameObject);
	}
}
