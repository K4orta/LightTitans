using UnityEngine;
using System.Collections;

public class Title : MonoBehaviour {
	public Camera titleCam;
	public Camera winCam;
	public Camera lossCam;
	public Camera mainCam;
	public GUISkin titleSkin;
	public static bool visible = true;
	protected string continueText = "Start Game";
	protected string victoryText = ""; 
	public float hSliderValue = 1;
	public Player player;
	public AudioSource menuMusic;
	public AudioClip startJingle;
	public float GammaCorrection=0;
	protected Color baseAmb;
	public bool victoryScreen;
	public bool invertX=false;
	public bool invertY=false;
	// Use this for initialization
	void Start () {
		
		Time.timeScale = 0;
		HUD.visible = false;
		winCam.enabled=false;
		lossCam.enabled=false;
		baseAmb = new Color(RenderSettings.ambientLight.r,RenderSettings.ambientLight.g,RenderSettings.ambientLight.b,1);
		
		//iTween.MoveTo(titleCam.gameObject, titleCam.transform.position+titleCam.transform.right*200,600);
	}
	
	void OnGUI(){
		
		
		if(visible){
			GUI.skin = titleSkin;
			GUI.skin.label.fontSize = 68;
			GUI.Label(new Rect(20, Screen.height*.25f, 900, 80), "Light Titans");
			GUI.skin.label.fontSize = 16;
			GUI.Label(new Rect(25, Screen.height*.25f+30, 400, 100), "A game made in 7 days by: K4Orta (@K4Orta on Twitter)");
			GUI.Label(new Rect(25, Screen.height*.25f+50, 400, 100), "Title music by: hisboyelroy");
			
			if(GUI.Button(new Rect(20, (Screen.height*.25f)+110, 300, 80), continueText)){
				HUD.visible = true;
				Time.timeScale = 1;
				titleCam.enabled=false;
				Screen.lockCursor = true;
				visible = false;
				
				if(continueText=="Start Game"){
					AudioSource.PlayClipAtPoint(startJingle, titleCam.transform.position);
				}
				continueText = "Continue Game";
				Logic.Playstate.NewGame();
				
				//Destroy(this.gameObject);
			}
			GUI.skin.label.fontSize = 16;
			GUI.Label(new Rect(20, (Screen.height*.25f)+185, 200, 20), "Mouse X Sensitivity");
			GUI.Label(new Rect(20, (Screen.height*.25f)+215, 200,20), "Mouse Y Sensitivity");
			player.sensitivityX = -GUI.HorizontalSlider(new Rect(200, (Screen.height*.25f)+190, 200, 30), -player.sensitivityX, 0.0F, 46.0F);
			player.sensitivityY = GUI.HorizontalSlider(new Rect(200, (Screen.height*.25f)+220, 200, 30), player.sensitivityY, 0.0F, 46.0F);
			
			invertY = GUI.Toggle(new Rect(25, (Screen.height*.25f)+280, 200, 30), invertY, "Invert Mouse Y");
			invertX = GUI.Toggle(new Rect(25, (Screen.height*.25f)+300, 200, 30), invertX, "Invert Mouse X");
			player.invertX = invertX?-1:1;
			player.invertY = invertY?-1:1;
			
			GUI.Label(new Rect(20, (Screen.height*.25f)+255, 200,20), "Brightness");
			GammaCorrection = GUI.HorizontalSlider(new Rect(200, (Screen.height*.25f)+260, 200, 30), GammaCorrection, -4f, 4f);
			RenderSettings.ambientLight = new Color(baseAmb.r+GammaCorrection, baseAmb.g+GammaCorrection, baseAmb.b+GammaCorrection, 1.0f);
			
			GUI.Label(new Rect(12, Screen.height-145, 400, 100), "A game made in 7 days by: K4Orta (@K4Orta on Twitter)");
			GUI.Label(new Rect(12, Screen.height-125, 400, 100), "Title music by: hisboyelroy");
			
		}
		
		if(!visible&&victoryScreen){
			GUI.skin = titleSkin;
			GUI.skin.label.fontSize = 52;
			GUI.Label(new Rect(Screen.width*.5f-250, Screen.height*.5f-40, 500, 80), victoryText);
			GUI.skin.label.fontSize = 16;
			GUI.Label(new Rect(Screen.width*.5f-250, Screen.height*.5f, 500, 80), "Press [Escape] to play again");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.Escape)){
			if(!visible){
				Screen.lockCursor = !Screen.lockCursor;
				visible = true;
				audio.Play();
				Time.timeScale = 0;
				if(Logic.Playstate.GameOver){
					Application.LoadLevel("Rift");	
				}
			}else{
				HUD.visible = true;
				Time.timeScale = 1;
				titleCam.enabled=false;
				Screen.lockCursor = true;
				visible = false;
			}
			
		}
		if(!visible){
			if(audio.volume>0){
				audio.volume -= Time.deltaTime;
				if(audio.volume<=0){
					audio.Pause();
				}
			}
		}else{
			if(audio.volume<.5f){
				audio.volume += Time.deltaTime*.5f;	
			}
		}
	}
	
	
	
	public void gameEndScreen(Unit.Side loser){
		victoryScreen = true;
		if(loser == Unit.Side.Player){
		 	victoryText = "Defeat";
		}else{
			victoryText= "Victory!";
		}
		HUD.visible=false;
	}
}
