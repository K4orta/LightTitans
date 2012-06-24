using UnityEngine;
using System.Collections;

public class Player : Unit {

	//movement vars
	public float sensitivityX = -23F;
	public float sensitivityY = 23F;
	public float xTip = 0;
	public float zTip = 0;
	public float rotorRPM = 0; 
	public int invertX=1;
	public int invertY=1;
	
	// weapon payload
	protected float cannonHeat = 0;
	protected float cannonOverheat = 100;
	public float cannonVerticalSpread = 1;
	public float cannonHorizontalSpread = 1;
	
	protected float rocketRPM = 300; 
	protected float rocketCooldown=0;
	protected int rocketPod = 16;
	public int rocketsInPod = 16;
	protected int maxRockets = 120;
	protected float rocketReloadTime = 3;
	protected bool reloadingRockets;
	protected float rocketReloadTimer = 0;
	
	public Transform rocketPrefab;
	
	public Transform respawnLoc;
	
	//hp
	protected float hpRegen=4.6f; 
	protected float regenDelay=6; 
	protected float regenTimer=0; 
	
	public AudioClip shotSound;
	public AudioClip winddown;
	public Controls controlSetup; 

	public enum Controls{
		Simple,
		Advanced
	}
	

	// Use this for initialization
	protected override void Start () {
		base.Start();
		rigidbody.centerOfMass=new Vector3(0,0,2);
		cannonRPM = 1600;
		cannonSpread = 2;
		armor = 20;
		SetOwner(Unit.Side.Player);
		Logic.Playstate.playerUnits.Add (this);
		controlSetup = Controls.Advanced;
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
		if(Input.GetMouseButton(0)&&rotorRPM>0){
			if(cannonCooldown<=0){
				FireCannon(curCannon);
				curCannon = !curCannon;
				cannonCooldown = 60/cannonRPM; 
			}
		}else if(Input.GetMouseButton(1)){
			if(rocketCooldown <=0&&rocketsInPod>0){
				FireRockets(curCannon);
				--rocketsInPod;
				curCannon = !curCannon;
				rocketCooldown = 60/rocketRPM;
				if(rocketsInPod<=0){
					reloadingRockets =true;
					rocketReloadTimer = rocketReloadTime;
				}
			}
		}
		
		if(Input.GetKeyDown(KeyCode.X)){
			flareProtection = 1;
		}
		
		if(Input.GetKeyDown(KeyCode.R)){
			reloadingRockets =true;
			rocketReloadTimer = rocketReloadTime;
			rocketsInPod = 0;
		}
		
		
		if(Input.GetMouseButtonUp(0)&&rotorRPM>0){
			logic.AudioAt(winddown,transform.position,1,200,0);
		}
		
		if(reloadingRockets){
			if(rocketReloadTimer>0){
				rocketReloadTimer -= Time.deltaTime;
			}else{
				reloadingRockets = false;
				rocketsInPod = rocketPod;
			}
		}
		
		if(cannonCooldown>0){
			cannonCooldown -= Time.deltaTime;	
		}
		if(rocketCooldown>0){
			rocketCooldown -= Time.deltaTime;	
		}
		if(flareProtection>0){
			flareProtection -= Time.deltaTime;	
		}
		if(regenTimer>0){
			regenTimer -= Time.deltaTime;	
		}else if(hp<maxHp){
			hp+=Time.deltaTime*hpRegen;
			if(hp>maxHp){
				hp=maxHp;	
			}
		}
		
	}
	
	protected void FireRockets(bool GunNum){
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width*.5f, Screen.height*.5f, 0));
			RaycastHit hit;
			Vector3 nr;
			Transform cShot;
			Vector3 gunPos;
			if(GunNum){
				gunPos = new Vector3(transform.position.x, transform.position.y, transform.position.z)-transform.right*4-transform.up*2;
			}else{
				gunPos = new Vector3(transform.position.x, transform.position.y, transform.position.z)+transform.right*4-transform.up*2;
			}
			if(Physics.Raycast(ray, out hit, 10000)){
				nr = Vector3.Normalize(hit.point - gunPos);
				cShot =(Transform)Instantiate(rocketPrefab,gunPos, Quaternion.LookRotation(nr));
			}else{
				cShot =(Transform)Instantiate(rocketPrefab,gunPos, Quaternion.LookRotation(ray.direction));
			}
			cShot.Rotate(Random.value*1.2f*Random.Range(-1,1),Random.value*1.2f*Random.Range(-1,1),Random.value*1.2f*Random.Range(-1,1));
			cShot.rigidbody.AddRelativeForce (Vector3.forward*5500);
			cShot.rigidbody.centerOfMass = cShot.transform.forward*3;
			Physics.IgnoreCollision(cShot.collider,collider);
		}
	
	protected void FireCannon(bool GunNum){
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width*.5f, Screen.height*.5f, 0));
		RaycastHit hit;
		Vector3 nr;
		Transform cShot;
		Vector3 gunPos;
		if(GunNum){
			gunPos = new Vector3(transform.position.x, transform.position.y, transform.position.z)-transform.right*4-transform.up*2;
		}else{
			gunPos = new Vector3(transform.position.x, transform.position.y, transform.position.z)+transform.right*4-transform.up*2;
		}
		logic.AudioAt(shotSound,gunPos,.8f,100,0);
		//AudioSource.PlayClipAtPoint(shotSound,Camera.main.transform.position,2);
		if(Physics.Raycast(ray, out hit, 10000)){
			nr = Vector3.Normalize(hit.point - gunPos);
			cShot =(Transform)Instantiate(shotPrefab,gunPos, Quaternion.LookRotation(nr));
		}else{
			cShot =(Transform)Instantiate(shotPrefab,gunPos, Quaternion.LookRotation(ray.direction));
		}
		cShot.Rotate(Random.value*cannonSpread*Random.Range(-1,1),Random.value*cannonSpread*Random.Range(-1,1),Random.value*cannonSpread*Random.Range(-1,1));
		cShot.rigidbody.AddRelativeForce (Vector3.forward*25);
		cShot.rigidbody.centerOfMass = cShot.transform.forward*3;
		Physics.IgnoreCollision(cShot.collider,collider);
	}
	
	void FixedUpdate(){
			
		xTip = (transform.eulerAngles.x-180<0?transform.eulerAngles.x-180+180:transform.eulerAngles.x-180-180);
		zTip = (transform.eulerAngles.z-180<0?transform.eulerAngles.z-180+180:transform.eulerAngles.z-180-180);
			
		doControls(controlSetup);
	}
	
	public override void Hurt (float Damage){
		base.Hurt(Damage);
		regenTimer = regenDelay;
		HUD.hurtThisFrame = 1;
	} 
	
	public override void Kill (){
		hp = maxHp;
		if(respawnLoc!=null){
			transform.position = respawnLoc.position;
			transform.rotation = respawnLoc.rotation;
		}
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		flareProtection+=1;
		HUD.misslesInFlight=0;
		
		rotorRPM = 0;
	} 

	protected void doControls(Controls CurCon){
		switch(CurCon){
			case Controls.Simple:
					SimpleControls();
				break;
			case Controls.Advanced:
					AdvancedControls();
				break;
			default:
				break;
		}
	}

	protected void autoLevel(){
		if(Mathf.Abs(zTip)>2){
			rigidbody.AddRelativeTorque(new Vector3(0, 0, -zTip*.45f));
		}
	}

	protected void SimpleControls(){
		//rigidbody.AddRelativeTorque(new Vector3(0, 0, Input.GetAxis("Mouse X") * sensitivityX * rotorRPM * Time.deltaTime * invertX));
		//rigidbody.AddRelativeTorque(new Vector3(Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime * 58 * invertY,0, 0));
		rigidbody.angularDrag = 2;
		rigidbody.AddRelativeTorque(new Vector3(0, Input.GetAxis("Mouse X") * -sensitivityX * rotorRPM * Time.deltaTime * 60 * invertX), 0);
		rigidbody.AddRelativeTorque(new Vector3(Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime * 58 * invertY,0, 0));
		
		rigidbody.AddRelativeTorque(new Vector3(0, 0, Input.GetAxis("Horizontal") * sensitivityX * Time.deltaTime * 30));
		autoLevel();

		if(Input.GetKey(KeyCode.LeftShift)){
			rigidbody.AddRelativeForce(Vector3.forward*400* Time.deltaTime * 58);	
		}

		if(Input.GetKey(KeyCode.W)){
				rotorRPM = Mathf.Clamp(rotorRPM+.5f*Time.deltaTime,0,1);
				if(xTip>10){
					rigidbody.AddForce(new Vector3(transform.forward.x, 0 , transform.forward.z)*5.5f*xTip);
					if(xTip>30){
						rigidbody.AddRelativeForce(Vector3.up*-15* Time.deltaTime * 58);
					}
				}else{
					rigidbody.AddRelativeForce(Vector3.up*38 * Time.deltaTime * 58);
				}
			}

		if(Input.GetKey (KeyCode.S)){
			rigidbody.AddRelativeForce(Vector3.up*-38* Time.deltaTime * 58);	
		}else{
			rigidbody.AddRelativeForce(Vector3.up*28*rotorRPM* Time.deltaTime * 58);		
		}

		if(Mathf.Abs(Input.GetAxis("Horizontal"))>.4f){
			rigidbody.AddForce(new Vector3(transform.right.x, 0 , transform.right.z)*-4f*zTip);
		}
		Debug.Log("Ho!");
	}

	

	protected void AdvancedControls(){
		rigidbody.AddRelativeTorque(new Vector3(0, 0, Input.GetAxis("Mouse X") * sensitivityX * rotorRPM * Time.deltaTime * 58 * invertX));
		rigidbody.AddRelativeTorque(new Vector3(Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime * 58 * invertY,0, 0));

		rigidbody.AddRelativeTorque(new Vector3(0,Input.GetAxis("Horizontal") * sensitivityX*-1* Time.deltaTime * 58, 0));
		
		rigidbody.AddForce(new Vector3(transform.forward.x, 0 , transform.forward.z)*2f*xTip);
		rigidbody.AddForce(new Vector3(transform.right.x, 0 , transform.right.z)*-2f*zTip);
		
		if(Input.GetKey(KeyCode.W)){
			rotorRPM = Mathf.Clamp(rotorRPM+.5f*Time.deltaTime,0,1);
			if(xTip>10){
				rigidbody.AddForce(new Vector3(transform.forward.x, 0 , transform.forward.z)*5f*xTip);
				rigidbody.AddRelativeForce(Vector3.up*15* Time.deltaTime * 58);
			}else{
				rigidbody.AddRelativeForce(Vector3.up*38 * Time.deltaTime * 58);
			}
		}

		if(Input.GetKey(KeyCode.LeftShift)){
			rigidbody.AddRelativeForce(Vector3.forward*400* Time.deltaTime * 58);	
		}
		
		if(Input.GetKey (KeyCode.S)){
			rigidbody.AddRelativeForce(Vector3.up*-38* Time.deltaTime * 58);	
		}else{
			rigidbody.AddRelativeForce(Vector3.up*28*rotorRPM* Time.deltaTime * 58);		
		}
	}
}