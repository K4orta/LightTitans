using UnityEngine;
using System.Collections;

public class Sam : Unit {
	protected Transform turret;
	protected Transform body;
	protected Transform missles;
	
	protected float lockDuration = 4.5f;
	// Use this for initialization
	protected override void Start () {
		base.Start();
		
		body = transform.FindChild("Body");
		turret = transform.FindChild("Turret");
		missles = transform.FindChild("Rockets");
		missles.eulerAngles = new Vector3(-15,0,0);
		sightRange = 600;
		hp = 300;
		cannonRPM = 100;
		
		SetOwner(Side.Enemies);
		Logic.Playstate.enemyUnits.Add(this);
	}
	
	protected override void Update (){
		base.Update ();
		if(attTar!=null&&attTar){
			turret.LookAt(attTar.transform.position);
			turret.transform.eulerAngles = new Vector3(transform.eulerAngles.x,turret.transform.eulerAngles.y,transform.eulerAngles.z);
			missles.LookAt(attTar.transform.position);
			
			if(LOS(attTar)&&attTar.flareProtection<=0){
				lockTime+=Time.deltaTime;
				if(lockTime >= lockDuration){
					if(cannonCooldown<=0){
						FireCannon(curCannon);
						lockTime=0;
					//curCannon = !curCannon;
						
					}
				}
				//Debug.DrawLine(transform.position,attTar.transform.position, Color.red);	
			}else{
				attTar = null;
				lockTime=0;	
			}

			if(cannonCooldown>0){
				cannonCooldown -= Time.deltaTime;	
			}
		}else{
			attTar = Scan();
		}
	}
	
	public void FireCannon(bool Tube){
		Transform tn = (Transform)Instantiate(shotPrefab, missles.transform.position+missles.transform.forward*3, Quaternion.identity);
		tn.LookAt(attTar.transform);
		tn.GetComponent<HeatSeeker>().SetTarget(attTar);
		cannonCooldown = 60/cannonRPM;
	}

	public override void Kill(){
		Transform exp = logic.getExplosion();
		exp.position = transform.position;
		base.Kill ();	
	}

	
}
