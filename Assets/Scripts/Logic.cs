using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Logic : MonoBehaviour {
	
	public Transform tankPrefab;
	public Transform dropshipPrefab;
	public Transform explosionPrefab;
	public Transform trailPrefab;
	public Transform dustPrefab;
	public Transform impactPrefab;
	public Player curSel;
	
	public List<Unit> playerUnits = new List<Unit>();
	public List<Unit> enemyUnits= new List<Unit>();
	
	protected List<Transform> unusedRocketTrails;
	protected List<Transform> unusedDust;
	protected List<Transform> unusedExplosions;
	protected List<Transform> unusedImpacts;
	public static Logic Playstate;
	protected Titan playerTitan;
	protected Titan enemyTitan;
	
	public Transform audioExp;
	public Camera winCam;
	public Camera mainCam;
	public Camera lossCam;
	public bool GameOver;
	
	
	
	// Use this for initialization
	void Awake(){
		Playstate=this;
		playerUnits = new List<Unit>();
		enemyUnits= new List<Unit>();
		
		GameObject[] titans = GameObject.FindGameObjectsWithTag("Titan");
		foreach (GameObject tit in titans){

			if(tit.GetComponent<Titan>().owner==Unit.Side.Player){
				playerTitan = tit.GetComponent<Titan>();
			}else{
				enemyTitan = tit.GetComponent<Titan>();	
			}
		}
		winCam.enabled=false;
		lossCam.enabled=false;
	}
	
	void Start () {
		Time.timeScale=0;
		Playstate=this;
		curSel = GameObject.Find("PlayerCopter").GetComponent<Player>();
		
		/*while(i < 5){
			addTank(new Vector3(tn.x+12*++i, 0, tn.z+30));	
		}*/
		
		unusedRocketTrails = new List<Transform>();
		unusedExplosions = new List<Transform>();
		unusedDust = new List<Transform>();
		unusedImpacts = new List<Transform>();

		Physics.IgnoreLayerCollision(10,11);
		Physics.IgnoreLayerCollision(8,11);
		
		Physics.IgnoreLayerCollision(8,9);
		//Collapsing, Ground
		Physics.IgnoreLayerCollision(14,12);
		//Tanks and dropships
		Physics.IgnoreLayerCollision(10,15);
		Physics.IgnoreLayerCollision(9,15);
		
	}
	
	public void NewGame(){
		playerTitan.audio.Play();
		enemyTitan.audio.Play();
		curSel.audio.Play();
	}
	
	public Transform AudioAt(AudioClip Clip, Vector3 Position, float Volume=1f, float Range=500, float Doppler=0){
		Transform aud = (Transform)Instantiate(audioExp,Position,Quaternion.identity);
		aud.audio.clip = Clip;
		aud.audio.Play();
		aud.audio.maxDistance=Range;
		aud.audio.volume = Volume;
		aud.audio.dopplerLevel = Doppler;
		Destroy(aud.gameObject,Clip.length);
		return aud;
	}
	
	public Transform getRocketTrail(){
		Transform rt;
		for(int i=0;i<unusedRocketTrails.Count;++i){
			if(unusedRocketTrails[i]!=null &&!unusedRocketTrails[i].GetComponent<ParticleSystem>().IsAlive()){
				rt = unusedRocketTrails[i];
				return rt;
			}
		}
		rt= (Transform)Instantiate(trailPrefab,Vector3.zero,Quaternion.identity);
		unusedRocketTrails.Add(rt);
		return rt;
	}
	
	public void EndGame(Unit.Side Loser){
		GameOver=true;
		curSel.flareProtection=100000f;
		curSel.rotorRPM=0;
		curSel.audio.Stop ();
		curSel.rigidbody.velocity=Vector3.zero;
		curSel.rigidbody.angularVelocity=Vector3.zero;
		if(Loser == Unit.Side.Player){
			enemyTitan.hp=100000;
			lossCam.enabled=true;
			mainCam.transform.position = lossCam.transform.position;
			curSel.transform.position =  lossCam.transform.position;
			
		}else{
			playerTitan.hp=100000;
			winCam.enabled=true;
			mainCam.transform.position = winCam.transform.position;
			curSel.transform.position =  winCam.transform.position;
		}
		GameObject.Find("TitleScreen").GetComponent<Title>().gameEndScreen(Loser);
	}
	
	public float getTitanHealth(int Owner){
		
		if(Owner==0){
			if(playerTitan==null) return 0;
			return playerTitan.hp/playerTitan.maxHp;
		}else{
			if(enemyTitan==null) return 0;
			return enemyTitan.hp/enemyTitan.maxHp;
		}
		
	}
	
	public Transform getExplosion(){
		Transform rt;
		for(int i=0;i<unusedExplosions.Count;++i){
			
			if(!unusedExplosions[i].GetComponentInChildren<ParticleSystem>().IsAlive()){
				rt = unusedExplosions[i];
				rt.audio.pitch = Random.Range (-3f,3f);
				rt.audio.Play();
				ParticleSystem pt;
				foreach(Transform child in rt){
					pt = child.GetComponent<ParticleSystem>();
					pt.Clear();
					pt.Play();
				}
				
				return rt;
			}
		}
		rt = (Transform)Instantiate(explosionPrefab,Vector3.zero,Quaternion.identity);
		unusedExplosions.Add(rt);
		return rt;
	}
	
	public Transform getDustCloud(){
		Transform rt;
		for(int i=0;i<unusedDust.Count;++i){
			
			if(!unusedDust[i].GetComponentInChildren<ParticleSystem>().IsAlive()){
				rt = unusedDust[i];
				rt.audio.pitch = Random.Range (-3f,3f);
				rt.audio.Play();
				ParticleSystem pt;
				foreach(Transform child in rt){
					pt = child.GetComponent<ParticleSystem>();
					pt.Clear();
					pt.Play();
				}
				
				return rt;
			}
		}
		rt = (Transform)Instantiate(dustPrefab,Vector3.zero,Quaternion.identity);
		unusedDust.Add(rt);
		return rt;
	}
	
	public Transform getImpact(){
		Transform rt;
		ParticleSystem pt;
		for(int i=0;i<unusedImpacts.Count;++i){
			pt = unusedImpacts[i].GetComponent<ParticleSystem>();
			if(!pt.IsAlive()){
				rt = unusedImpacts[i];

				pt.Clear();
				pt.Play();
				
				
				return rt;
			}
		}
		rt = (Transform)Instantiate(impactPrefab,Vector3.zero,Quaternion.identity);
		unusedImpacts.Add(rt);
		return rt;
	}
	/*
	public void addUnusedExplosion(Transform Trail){
		unusedExplosions.Add(Trail);
	}
	
	public void addUnusedRocketTrail(Transform Trail){
		unusedRocketTrails.Add(Trail);
	}*/
	
	public Collider[] AddExplosiveForce(Vector3 Location, float Rad){
		Collider[] affected = Physics.OverlapSphere(Location,Rad);
		return affected;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public float GetPlayerHP(){
		return curSel.hp;
	}
	
	public float GetPlayerRockets(){
		return curSel.rocketsInPod;
	}
	
	public TankSpawner addDropship(Vector3 Location, Unit.Side NewOwner, Quaternion Rot=new Quaternion()){
		TankSpawner ret;
		Transform nu = (Transform)Instantiate(dropshipPrefab, Location, Rot);
		ret = nu.GetComponent<TankSpawner>();
		ret.SetOwner(NewOwner);
		if(NewOwner == Unit.Side.Player){
			playerUnits.Add(ret);
		}else{
			enemyUnits.Add (ret);	
		}
		return ret;
	}
	
	public Tank addTank(Vector3 Location, Unit.Side NewOwner, Quaternion Rot=new Quaternion()){
		Transform nu = (Transform)Instantiate(tankPrefab,Location, Rot);
		nu.GetComponent<Unit>().SetOwner(NewOwner);
		Tank nt = nu.gameObject.GetComponent<Tank>();
		if(NewOwner == Unit.Side.Player){
			playerUnits.Add(nt);
		}else{
			enemyUnits.Add (nt);	
		}
		return nt;
	}
}
