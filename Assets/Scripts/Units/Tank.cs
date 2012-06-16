using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Navigator))]
public class Tank : Unit {
	protected Transform turret;
	protected Transform body;
	protected Transform missles;
	
	//Pathing stuff
	public Transform m_Target;
	private Path m_CurrentPath;
	protected bool onPath;
	protected int curWaypoint;
	protected bool rotating;
	
	
	// Use this for initialization
	protected override void Start () {
		base.Start();
		//attTar = GameObject.Find("PlayerCopter").GetComponent<Unit>();
		body = transform.FindChild("Body");
		turret = transform.FindChild("Turret");
		missles = transform.FindChild("Rockets");
		missles.eulerAngles = new Vector3(-15,0,0);
		sightRange = 300;
		onPath = false;
		Navigator nav = GetComponent<Navigator> ();
		nav.RegisterWeightHandler ("Water", OnHandleWaterWeight);
		topSpeed = 80;
		cannonRPM = 230;
		cannonSpread =2;
		eyes = Vector3.up*2;
		//rigidbody.centerOfMass = new Vector3(0,-3,0);
		//moveTar = GameObject.Find("Waypoint1").transform.position;
		//iTween.MoveTo(gameObject, iTween.Hash("position", wp1, "speed", 10f));
	}
	
	// Update is called once per frame
	protected void FixedUpdate () {
		if(m_CurrentPath!=null&&m_CurrentPath.Segments.Count>0){
			if(curWaypoint<m_CurrentPath.Segments.Count && Physics.Raycast(transform.position+transform.up*.1f,-transform.up,.5f,1 << 12)){
				if(onPath){
					if(Mathf.Abs(Vector3.Distance(transform.position,m_CurrentPath.Segments[curWaypoint].To.Position))<m_CurrentPath.Segments[curWaypoint].To.Radius*.5f){
						++curWaypoint;
						if(curWaypoint<m_CurrentPath.Segments.Count){
							Quaternion nr = Quaternion.LookRotation(m_CurrentPath.Segments[curWaypoint].To.Position - body.transform.position);
							rotating = true;
							iTween.RotateTo(gameObject, iTween.Hash("rotation", new Vector3(transform.eulerAngles.x,nr.eulerAngles.y,transform.eulerAngles.z), "time", 1, "oncompletetarget", gameObject, "oncomplete", "autoRotate"));
						}
						
					}else{
						if(!rotating){
							Vector3 dist = m_CurrentPath.Segments[curWaypoint].To.Position - body.transform.position;
							Quaternion nr = Quaternion.LookRotation(dist);
							rigidbody.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x,nr.eulerAngles.y,transform.eulerAngles.z));
						}
						MoveForward();
					}
				}else{
					//Quaternion qu = new Quaternion(0,0,0,0);
					if(Mathf.Abs(Vector3.Distance(transform.position,m_CurrentPath.Segments[0].From.Position))<m_CurrentPath.Segments[0].From.Radius*.5f){
						
						Quaternion nr = Quaternion.LookRotation(m_CurrentPath.Segments[curWaypoint].To.Position - body.transform.position);
						rotating = true;
						iTween.RotateTo(gameObject, iTween.Hash("rotation", new Vector3(transform.eulerAngles.x,nr.eulerAngles.y,transform.eulerAngles.z), "time", 1, "oncompletetarget", gameObject, "oncomplete", "autoRotate"));
						onPath=true;
					}else{
						Vector3 dist = m_CurrentPath.Segments[0].From.Position - body.transform.position;
						Quaternion nr = Quaternion.LookRotation(dist);
						rigidbody.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x,nr.eulerAngles.y,transform.eulerAngles.z));
						MoveForward();
					}
				}
			}else{
				rigidbody.AddForce(Vector3.down*9.8f*rigidbody.mass);
			}
		}
	}
	
	public void PathTo(Vector3 ToGo){
		Navigator nav = GetComponent<Navigator> ();
		nav.RequestPath(transform.position, ToGo);
	}
	
	float OnHandleWaterWeight (object obj){
		return 3.0f;
	}
	
	public void autoRotate(){
		rotating=false;
	}


	protected override void Update (){
		base.Update ();
		if(attTar!=null&&attTar.alive&&Mathf.Abs(Vector3.Distance(attTar.transform.position, transform.position))<=sightRange){
			turret.LookAt(attTar.transform.position);
			turret.transform.eulerAngles = new Vector3(transform.eulerAngles.x,turret.transform.eulerAngles.y,transform.eulerAngles.z);
			missles.LookAt(attTar.transform.position);
			
			if(cannonCooldown<=0){
				if(LOS(attTar)){
					FireCannon(curCannon);
					curCannon = !curCannon;
					cannonCooldown = 60/cannonRPM; 
				}else{
					attTar=null;
				}
				//Debug.DrawLine(transform.position,attTar.transform.position, Color.red);	
			}

			if(cannonCooldown>0){
				cannonCooldown -= Time.deltaTime;	
			}
		}else{
			attTar = Scan();
		}
	}
	
public void FireCannon(bool GunNum){
	Vector3 gunPos;
		if(GunNum){
			gunPos = missles.position-missles.right;
		}else{
			gunPos = missles.position+missles.right;
		}	
		
	Transform cShot = (Transform)Instantiate(shotPrefab,gunPos+missles.forward*4,Quaternion.LookRotation(attTar.transform.position));
	cShot.LookAt(attTar.transform.position+(attTar.rigidbody.velocity*.53f)+attTar.eyes);
	
	
		//cannonSpread=1;
	if(attTar is Player){
		cannonSpread=6;
	}else{
		cannonSpread=2;
	}
	cShot.Rotate(Random.value*cannonSpread*(Random.value>.5?-1:1),Random.value*cannonSpread*(Random.value>.5?-1:1),Random.value*cannonSpread*(Random.value>.5?-1:1));
	cShot.rigidbody.AddForce(cShot.forward*8);
	if(owner==Side.Enemies){
		cShot.renderer.material.SetColor("_Emission", new Color(1,.3f,.2f,1));
	}else{
		cShot.renderer.material.SetColor("_Emission", new Color(.2f,.3f,1f,1));	
		cShot.gameObject.layer = 8;
	}
		//ShootAt(attTar, sh.rigidbody);
}

	public override void Kill(){
		Transform exp = logic.getExplosion();
		exp.position = transform.position;
		base.Kill ();	
	}


	// pathing 

	void OnNewPath (Path path)
	// When pathfinding via Navigator.targetPosition
	{
		Debug.Log ("Received new Path from " + path.StartNode + " to " + path.EndNode + ". Took " + path.SeekTime + " seconds.");
		m_CurrentPath = path;
	}
	
	
	void OnTargetUnreachable ()
	// When pathfinding via Navigator.targetPosition
	{
		Debug.Log ("Could not pathfind to target position");
		m_CurrentPath = null;
	}
	
	
	void OnPathAvailable (Path path){
	// When pathfinding via Navigator.RequestPath (startPositio, endPosition)
	
		m_CurrentPath = path;
		curWaypoint = 0;
		//Debug.Log ("Requested Path from " + path.StartNode + " to " + path.EndNode + " is now available. Took " + path.SeekTime + " seconds.");
		//Debug.Log (path.Segments[1]);
	}
	
	
	void OnPathUnavailable (){
	// When pathfinding via Navigator.RequestPath (startPositio, endPosition)
	
		Debug.Log ("The requested path could not be established.");
	}	
	
	
	void OnDrawGizmos ()
	{
		if (m_CurrentPath == null)
		{
			return;
		}
		
		m_CurrentPath.OnDrawGizmos ();
	}
}
