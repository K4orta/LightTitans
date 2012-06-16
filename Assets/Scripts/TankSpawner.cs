using UnityEngine;
using System.Collections;

public class TankSpawner : Unit {
	
	public Transform oppositeSpawn;
	public Vector3 attackTarget;
	public LandingSite targetLanding;
	public Titan mothership;
	
	protected bool deployed=false;
	protected bool settingUp=false;
	
	protected int perWave = 4;
	protected int thisWave = 0;
	protected float waveDelay = 2f;
	protected float gapDelay = 16f;
	protected float delay = 0;
	
	
	// Use this for initialization
	protected override void Start () {
		base.Start();
		hp=120;
		topSpeed = 100;
	}
	
	// Update is called once per frame
	protected override void Update () {
		if(deployed&&!logic.GameOver){
			if(delay<=0){
				Tank tk = Logic.Playstate.addTank(transform.position-Vector3.up*2.5f, owner, Quaternion.Euler(-transform.right));
				tk.PathTo(attackTarget);
				if(++thisWave<perWave){
					delay = waveDelay;
				}else{
					delay = gapDelay;
					thisWave = 0;
				}
			}else{
				delay -= Time.deltaTime;
			}
		}
	}
	
	protected void FixedUpdate(){
		if(!deployed&&targetLanding!=null){
			Vector3 dist = targetLanding.transform.position - transform.position;
			Quaternion nr = Quaternion.LookRotation(dist);
			rigidbody.rotation = Quaternion.Euler(new Vector3(nr.eulerAngles.x,nr.eulerAngles.y,nr.eulerAngles.z));
			rigidbody.AddRelativeForce(Vector3.forward*500);
			if(Mathf.Abs(dist.magnitude)<30){
				Quaternion ni = targetLanding.transform.rotation;
				
				rigidbody.velocity *=.5f;
				rigidbody.angularVelocity=Vector3.zero;
				iTween.RotateTo(gameObject, iTween.Hash("rotation", new Vector3(0,ni.eulerAngles.y,0), "time", 1.5f));
				iTween.MoveTo(gameObject, iTween.Hash("position", targetLanding.transform.position, "time", 1.5f, "oncompletetarget", gameObject, "oncomplete", "DropToGround"));
				targetLanding=null;
			}
			//MoveForward();
		}
	}
	
	public override void SetOwner(Unit.Side NewOwner){
		base.SetOwner(NewOwner);
		gameObject.layer = 15;
		if(owner == Side.Enemies){
			renderer.material.color = new Color(1f, .05f, .01f, 1f);
			hp=480;
			
		}else{
			renderer.material.color = new Color(.2f, .5f, 1f, 1f);
		}
	}
	
	public void FinishSetup(){
		hp=600;
		deployed=true;
	}
	
	public void DropToGround(){
		rigidbody.velocity = Vector3.zero;
		RaycastHit ray;
		rigidbody.useGravity = true;
		rigidbody.isKinematic = true;
		Physics.Raycast(transform.position,-Vector3.up,out ray,1000,1 << 12);
		iTween.MoveTo(gameObject, iTween.Hash("position", ray.point+Vector3.up*3.5f, "easetype", "easeOutQuart", "time", 1, "oncompletetarget", gameObject, "oncomplete", "FinishSetup"));
	}
	
	public override void Kill(){
		// inform our titan that I've been killed, send opposite spawn and landing site.
		Transform tn = Logic.Playstate.getExplosion();
		tn.transform.position = transform.position;
		base.Kill();	
	}
	
	public void sendLZ(LandingSite LZ){
		targetLanding = LZ;
		oppositeSpawn = LZ.oppositeSpawn;
		attackTarget = new Vector3(oppositeSpawn.position.x, 2,oppositeSpawn.position.z);
		
	}
}
