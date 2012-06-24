using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : Entity {
	// Groups 

	public List<Unit> hostiles;

	public float maxHp=100;
	
	// AI / targeting data
	public Side owner;
	public Unit attTar;
	public Vector3 moveTar;
	public Logic logic;
	public enum Side{
		Player=9,
		Enemies
	}
	public List<Unit> HostileGroup;
	
	public float topSpeed;

	// weapons / attacks
	protected float sightRange=200;
	public Transform shotPrefab;
	protected float cannonRPM = 10;
	protected bool curCannon = false;
	protected float cannonCooldown = 0;
	protected float cannonSpread = 5;
	protected float cannonReload = 1.6f;
	protected float cannonMagazine = 8;
	
	public float lockTime=3;
	public float lockTimer=0; 
	public float flareProtection=0;
	
	public float attackRange;
	public Vector3 eyes;
	public float armor=0;
	public AudioClip dieSound = null;  
	
	// Use this for initialization
	protected override void Start () {
		logic = Logic.Playstate;
		base.Start();
		hp = maxHp;
		eyes = new Vector3(0,0,0);
		//transform.gameObject.layer =10;
		
		
	}
	
	public virtual void SetOwner(Side NewOwner){
		owner = NewOwner;
		gameObject.layer = (int)owner;
		foreach(Transform bf in transform){
			if(bf.name=="Body"){
				bf.gameObject.layer = (int)owner;
			}
			if(bf.renderer!=null){
				if(owner == Side.Enemies){
				
					bf.renderer.material.color = new Color(1f, .05f, .01f, 1f);
				//Debug.Log();
				}else{
					bf.renderer.material.color = new Color(.2f, .5f, 1f, 1f);
				}
			}
		}
		
		if(owner == Side.Enemies){
			hostiles = Logic.Playstate.playerUnits;
		}else if(owner == Side.Player){
			hostiles = Logic.Playstate.enemyUnits;
		}
		
	}
	
	// Update is called once per frame
	protected virtual void Update () {
	
	}
	
	protected virtual void EndMove(){
		
	}
	
	public virtual void MoveForward(){
		
		if(rigidbody.velocity.magnitude<topSpeed){
			if(attTar!=null){
				rigidbody.AddRelativeForce(Vector3.forward*(40-rigidbody.velocity.magnitude));
			}else{
				rigidbody.AddRelativeForce(Vector3.forward*(80-rigidbody.velocity.magnitude));
			}
			//rigidbody.AddRelativeForce(new Vector3(0,0,0)*(80-rigidbody.velocity.magnitude));
		}
		
	}
	
	public virtual Unit Scan(){
		Unit pSave=null;
		for(int i=0;i<hostiles.Count;++i){
			Unit cHost = hostiles[i];
			if(cHost!=null&&cHost.alive){
				if(cHost.name == "PlayerCopter"){
					pSave = cHost;
					continue;
				}else if(LOS(cHost)){
					return cHost;	
				}
			}
		}
		if(pSave!=null){
			if(LOS (pSave)){
				return pSave;	
			}
		}
		return null;
	}
	
	public virtual bool LOS(Unit Target){
		// check if target is in range;
		if(Mathf.Abs(Vector3.Distance(Target.transform.position, transform.position))<sightRange){
			// cast a ray to the taget
			
			RaycastHit hit;
			// ignore certain layers 10 = enemies, 11 & 8 are projectiles
			
			if(Physics.Linecast(transform.position+Vector3.up*1,Target.transform.position+Vector3.up*1, out hit, ~(1<<(int)owner|1<<11|1<<8))){
				if(hit.rigidbody == Target.rigidbody){
					return true;
				}
					
			}
			//if everything checks out, return true
		}
		return false;
	}
	
	public override void Kill(){
		if(owner == Side.Player){
			logic.playerUnits.Remove(this);
		}else if(owner == Side.Enemies){
			logic.enemyUnits.Remove(this);
		}
		if(dieSound!=null){
			logic.AudioAt(dieSound,transform.position,1,300);
			//AudioSource.PlayClipAtPoint(dieSound,transform.position,900f);
		}
		base.Kill ();	
	}
	
}
