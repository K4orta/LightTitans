using UnityEngine;
using System.Collections;

public class Titan : Unit {
	
	public Unit opponent;
	public Unit.Side titanSide;
	protected TankSpawner[] transports;
	protected LandingSite[] landingSpots;
	protected bool[] marked;
	protected float spawnDelay=2f;
	
	protected Transform[] gunPorts = new Transform[6];
	protected Transform[] cannonPorts = new Transform[2];
	
	protected float gunRPM = 10;
	protected float gunCooldown = 0;
	protected float gunSpread = 5;
	public AudioClip shotSound;
	
	// Use this for initialization
	void Awake(){
		transports = new TankSpawner[3];
		landingSpots = new LandingSite[3];
		marked = new bool[3];
		cannonRPM = 250;
		
		int foundCount=0;
		GameObject[] sites = GameObject.FindGameObjectsWithTag("DropshipLanding");
		attTar = opponent;
		eyes = new Vector3(0,0,0);
		foreach (GameObject site in sites){
			if(site.GetComponent<LandingSite>().owner == titanSide){
				marked[foundCount] = false;
				landingSpots[foundCount++] = site.GetComponent<LandingSite>();
			}
		}
		int gp = 0;
		int cp = 0;
		foreach (Transform child in transform){
		
			if(child.name == "GunPort"){
				gunPorts[gp++] = child;
			}else if(child.name == "CannonPort"){
				cannonPorts[cp++] = child;
			}
		}
		
	}
	
	protected override void Start () {
		base.Start();
		
		maxHp = 45000;
		hp = maxHp;
		GameObject[] titans = GameObject.FindGameObjectsWithTag("Titan");
		foreach (GameObject tit in titans){
			if(tit.GetComponent<Titan>() != this){
				opponent = tit.GetComponent<Titan>();
				break;
			}
		}
		SetOwner(titanSide);
		if(titanSide == Unit.Side.Player){
			Logic.Playstate.playerUnits.Add (this);
		}else{
			Logic.Playstate.enemyUnits.Add (this);
			spawnDelay = 30f;
			
		}
		Light lght = transform.FindChild("CoreLight").light;
		lght.color = owner==Side.Enemies?new Color(1f, .05f, .01f, 1f):new Color(.2f, .5f, 1f, 1f);
		
		if(titanSide!=Side.Enemies){
			for(int i =0;i<landingSpots.GetLength(0);++i){
				transports[i] = logic.addDropship(transform.position+(Vector3.right*(i-1)*16)-Vector3.up*50,titanSide, transform.rotation);
				transports[i].sendLZ(landingSpots[i]);
			}
		}
	}
	

	
	// Update is called once per frame
	
	void FixedUpdate(){
		if(opponent!=null&&Mathf.Abs(Vector3.Distance(opponent.transform.position, transform.position))>600)
			rigidbody.MovePosition(transform.position+transform.forward*.2f);
		
	}
	
	protected override void Update (){
		for(var i=0;i<transports.GetLength(0);++i){
			if(transports[i]==null|| !transports[i].alive){
				if(!marked[i]){
					marked[i]=true;
					StartCoroutine(DeployTransport(spawnDelay,i));
				}
			}
		}
		
		if(opponent!=null&&Mathf.Abs(Vector3.Distance(opponent.transform.position, transform.position))<850){
			
			if(cannonCooldown<=0){
			curCannon = !curCannon;
			cannonCooldown = 60/cannonRPM; 
			FireCannon(curCannon);
			
		}
			
			if(cannonCooldown>0){
				cannonCooldown -= Time.deltaTime;
			}
		}

	} 
	
	public void FireCannon(bool GunNum){
		Transform gunPos;
		if(GunNum){
			gunPos = cannonPorts[0];
		}else{
			gunPos = cannonPorts[1];
		}	
		Transform cShot = (Transform)Instantiate(shotPrefab,gunPos.position+gunPos.forward*20,Quaternion.LookRotation(gunPos.forward));
		cShot.Rotate(Random.value*cannonSpread*(Random.value>.5?-1:1),Random.value*cannonSpread*(Random.value>.5?-1:1),Random.value*cannonSpread*(Random.value>.5?-1:1));
		cShot.rigidbody.AddForce(cShot.forward*20000);
		//cShot.audio.pitch = Random.value;
		logic.AudioAt(shotSound,gunPos.position,1,600,1).audio.pitch=Random.Range(-4f,4f);
		if(owner==Side.Enemies){
			//cShot.renderer.material.SetColor("_Emission", new Color(1,.3f,.2f,1));
		}else{
			//cShot.renderer.material.SetColor("_Emission", new Color(.2f,.3f,1f,1));	
			cShot.gameObject.layer = 8;
		}
	}
	
	public override void Hurt(float Damage){
		if(!logic.GameOver&&hp-Damage<800){
			logic.EndGame(owner);
		}
		base.Hurt (Damage);	
	}
	
	public override void Kill (){
		Transform exp = logic.getExplosion();
		exp.position = transform.position;
		ParticleSystem pt;
				foreach(Transform child in exp){
					pt = child.GetComponent<ParticleSystem>();
					pt.startSize *= 30;
				}
		Collider[] exps = logic.AddExplosiveForce(transform.position, 1000);
		for(var i=0;i<exps.GetLength(0);++i){
			if(exps[i].rigidbody!=null)
				exps[i].rigidbody.AddExplosionForce(5000,transform.position,1400);
		}
		base.Kill ();
		
	} 
	
	IEnumerator DeployTransport(float Wait, int Id){
		
		yield return new WaitForSeconds(Wait);
		marked[Id] = false;
		transports[Id] = logic.addDropship(transform.position+(Vector3.right*(Id-1)*16)-Vector3.up*50,titanSide, transform.rotation);
		transports[Id].sendLZ(landingSpots[Id]);
		//Kill();
	}
}
