using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
	
	public GUISkin tutorialSkin;
	public int currentStep = 0; 
	protected float wStep = 1f;
	protected float rollStep = .75f;
	protected float gap = 0;
	protected float pressedA=0;
	protected float pressedD=0;
	public Player player;
	protected bool tutorialActive = true;
	// Use this for initialization
	void Start () {
	
	}
	
	
	void OnGUI(){
		if(tutorialActive&&Time.timeScale==1){
			GUI.skin = tutorialSkin;
			if(gap<=0){
				switch(currentStep){
					case 0:
						GUI.Label(new Rect(Screen.width*.5f-200, Screen.height*.5f+80, 400, 80), "Hold W to gain altitude. When not holding W you will hover");
						break;
					case 1:
						GUI.Label(new Rect(Screen.width*.5f-200, Screen.height*.5f+80, 400, 80), "Slide the mouse forward while holding W to move forward");
						break;
					case 2:
						GUI.Label(new Rect(Screen.width*.5f-200, Screen.height*.5f+80, 400, 80), "Move the mouse left and right to turn");
						break;
					case 3:
						GUI.Label(new Rect(Screen.width*.5f-200, Screen.height*.5f+80, 400, 80), "Press A or D to strafe left or right");
						break;
					case 4:
						GUI.Label(new Rect(Screen.width*.5f-200, Screen.height*.5f+80, 400, 80), "Press Mouse 1 to fire");
						break;
					case 5:
						GUI.Label(new Rect(Screen.width*.5f-200, Screen.height*.5f+80, 400, 80), "Press Mouse 2 for rockets");
						break;
					case 6:
						GUI.Label(new Rect(Screen.width*.5f-200, Screen.height*.5f+80, 400, 80), "Hold S to descend");
						break;
					case 7:
						GUI.Label(new Rect(Screen.width*.5f-200, Screen.height*.5f+80, 400, 80), "Tip: Press Shift for a boost of forward speed");
						break;
					case 8:
						GUI.Label(new Rect(Screen.width*.5f-200, Screen.height*.5f+80, 400, 80), "To win: Destroy as many enemy tanks and SAM sites as you can so your forces can bombard the enemy Titan");
						break;
					default:
						break;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(tutorialActive&&currentStep==0){
			if(Input.GetKey(KeyCode.W)){
				wStep-=Time.deltaTime;
				
			}
			if(wStep<=0){
				wStep = .75f;
				currentStep++;
				gap = .25f;
			}
		}else if(currentStep==1){
			
			if(player.xTip>25 && wStep <=0 || player.xTip>50){
				currentStep++;
				gap = .5f;
				wStep = 2.5f;
			}
			wStep-=Time.deltaTime;
		}else if(currentStep==2){
			
			if(wStep<=0){
				currentStep++;
				gap = .2f;
				wStep=2f;
				
			}
			wStep-=Time.deltaTime;
		}else if(currentStep==3){
			if(wStep<=0){
				currentStep++;
				wStep = 2.5f;
				gap = .25f;
			}
			wStep-=Time.deltaTime;
		}else if(currentStep==4){
			if(Input.GetMouseButtonDown(0)){
				currentStep++;
				gap = .25f;
			}
			wStep-=Time.deltaTime;
		}else if(currentStep==5){
			if(Input.GetMouseButtonDown(1)){
				currentStep++;
				wStep = .5f;
				gap = .25f;
			}
			wStep-=Time.deltaTime;
		}else if(currentStep==6){
			if(Input.GetKey(KeyCode.S)){
				wStep-=Time.deltaTime;
				
				if(wStep<=0){
					wStep = .25f;
					currentStep++;
					gap = .25f;
				}
			}
		}else if(currentStep==7){
			if(Input.GetKey(KeyCode.LeftShift)){
				wStep-=Time.deltaTime;
				
				if(wStep<=0){
					wStep = 15f;
					currentStep++;
					gap = .5f;
				}
			}
		}else if(currentStep==8){
			
				wStep-=Time.deltaTime;
				if(wStep<=0){
					wStep = .75f;
					currentStep++;
				tutorialActive=false;
					gap = .5f;
				}
			
		}
		if(gap>0){
			gap-=Time.deltaTime;
		}
	}
}
