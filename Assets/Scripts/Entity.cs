using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {
	public bool alive = true;
	public float hp;
	// Use this for initialization
	protected virtual void Start () {
	
	}
	
	public virtual void Hurt(float Damage){
		hp -= Damage;
		if(hp <= 0){
			Kill();
		}
	}
	
	public virtual void Kill(){
		alive=false;
		Destroy(gameObject);	
	}
}
