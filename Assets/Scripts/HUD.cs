using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {
	public Logic logic;

	public Texture crosshair;
	public GUISkin hudSkin;
	public Texture hitMarker;
	public float hitFade;
	protected Color WHITE = new Color(1,1,1,1);
	protected Color RED = new Color(1,0,0,1);
	public static bool hitThisFrame=false;
	public static float hurtThisFrame=0;
	public static int misslesInFlight=0;
	public static int radarLocks=0;
	
	public Font boldFont; 
	public Font normalFont; 
	public static bool visible=true;
	public bool lockTone=false;
	public AudioClip locksound;
	public Transform lockLoc;
	
	protected Titan playerTitan;
	protected Titan enemyTitan;
	// Use this for initialization
	void Start () {
		logic = GameObject.Find ("Logic").GetComponent<Logic> ();
		hitFade = 0;
		boldFont = hudSkin.label.font; 
		normalFont = hudSkin.font; 
		GameObject[] titans = GameObject.FindGameObjectsWithTag("Titan");
		foreach (GameObject tit in titans){

			if(tit.GetComponent<Titan>().owner==Unit.Side.Player){
				playerTitan = tit.GetComponent<Titan>();
			}else{
				enemyTitan = tit.GetComponent<Titan>();	
			}
		}
	}
	
	void OnGUI(){
		// Draw player health
		if(visible){
		GUI.skin = hudSkin;
		GUI.skin.label.fontSize = 32;
		if(hitThisFrame){
			hitFade = 1;
			GUI.DrawTexture(new Rect(Screen.width*.5f-hitMarker.width*.5f,Screen.height*.5f-hitMarker.height*.5f, hitMarker.width, hitMarker.height), hitMarker);
			hitThisFrame=false;
		}else if(hitFade>0){
			GUI.color = new Color(1,1,1,hitFade);
			GUI.DrawTexture(new Rect(Screen.width*.5f-hitMarker.width*.5f,Screen.height*.5f-hitMarker.height*.5f, hitMarker.width, hitMarker.height), hitMarker);
			
			hitFade-=Time.deltaTime*2;
		}
		
		
		if(misslesInFlight>0){
			if(!lockTone){
				lockTone=true;
				lockLoc=Logic.Playstate.AudioAt(locksound,Logic.Playstate.curSel.transform.position,1,500,0);
			}
			lockLoc.transform.position = Logic.Playstate.curSel.transform.position;
			GUI.skin.label.fontSize = 20;
			GUI.Label(new Rect(Screen.width*.5f-160,Screen.height*.5f+230, 320, 50), "PRESS [X] TO DEPLOY FLARES");
			GUI.skin.label.fontSize = 32;
			GUI.color = RED;
			GUI.Label(new Rect(Screen.width*.5f-130,Screen.height*.5f+200, 260, 50), "MISSLE LOCK");
		}else{
			GUI.color = WHITE;
				if(lockTone&&lockLoc!=null){
					lockLoc.audio.Stop();
					lockTone=false;
				}
		}
		
		// crosshair
		GUI.DrawTexture(new Rect(Screen.width*.5f-crosshair.width*.5f,Screen.height*.5f-crosshair.height*.5f, crosshair.width, crosshair.height), crosshair);
		
		
		
		// Draw flares / ammo / hp
		
		
		GUI.Box(new Rect(Screen.width-160,Screen.height-60, 150, 50), ""+Mathf.Round(logic.GetPlayerHP())+"%");
		
		// Draw Titan health
		GUI.skin.label.font=GUI.skin.font;
		
		GUI.skin.label.fontSize = 16;
		GUI.Label(new Rect(Screen.width*.5f-200,10, 400, 50), "YOUR TITAN  -  ENEMY TITAN");
		GUI.skin.label.fontSize = 24;
		GUI.Label(new Rect(Screen.width*.5f-100,30, 100, 50), Mathf.Round(logic.getTitanHealth(0)*100)+"%");
		GUI.Label(new Rect(Screen.width*.5f,30, 100, 50), Mathf.Round(logic.getTitanHealth(1)*100)+"%");
		
		GUI.skin.label.fontSize = 13;
		//GUI.color = new Color(0.96f, 0.625f, 0.125f, 1);
		GUI.Label(new Rect(Screen.width*.5f-60, Screen.height*.5f+30, 32, 32),logic.GetPlayerRockets()+"");
		GUI.Label(new Rect(Screen.width*.5f+30, Screen.height*.5f+30, 32, 32), Mathf.Round(logic.GetPlayerHP())+"%");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
