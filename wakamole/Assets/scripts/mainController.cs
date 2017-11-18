using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mainController : MonoBehaviour {

	public GameObject mainMenu;
	public GameObject creditsMenu;
	public GameObject gameMenu;
	public GameObject scoreMenu;

	public Text score;
	public Text final_score;
	public Image[] w_images = new Image [9];
	public Sprite[] c_sprites = new Sprite[10];

	//Array bidimensional que guarda o id da janela, o status dela e o timer em caso de ataque
	// [id_personagem,status]
	// status: 0 = vazio, 1 = com personagem, 2 = atacando/beijando
	int[,] w_status = new int[9,3];

	float[] entity_timer = new float[9];

	public bool in_game = false;
	public float timer  = 2.0f;
	public float timer_action  = 1.0f;
	public float timer_spawn  = 1.0f;
	public int level = 0;
	public int kills = 0;
	public int life = 3;
	public float spawn_rate = 1.0f;

	void Start () {
		setActiveWindow (0);	
		mainMenu.SetActive (true);
		for(int i=0;i<9;++i){
			w_status[i,0] = i;
			w_status[i,1] = 0;
			entity_timer [i] = timer;
		}
	}
	
	void Update () {		
		if (in_game){
			score.text = (kills.ToString());
			processAI ();
			processKills ();
			isDead ();
		}
	}

	// verifica os timers e aciona o ataque caso precise
	// procesa o timer de criação de inimigos
	void isDead(){
		if(life == 0){
			scoreMenu.SetActive(true);
			final_score.text = (kills.ToString());
			in_game = false;
		}
	}
	void processAI(){		

		//Tratando as entidades já instanciadas
		for(int i=0;i<9;++i){
			int s = w_status [i, 1];
			if (s != 0) {
				//Janela ocupada, tmando providências
				entity_timer[i]-=Time.deltaTime;
				if (entity_timer [i] < 0) {
					//fim do tempo do personagem, enviar para ação (ataque beijo :B  ou reset)
					if (s == 1) {
						entity_timer[i] = timer_action;
						w_status [i, 1] = 2; //  mudando o status para 2
						entityAction (i);
					} else if (s == 2) {						
						//resetando
						entity_timer[i] = timer - (level*0.1f);
						w_status [i, 1] = 0; //  mudando o status para 0, vazio
						entityAction(i);
					}

					if (w_status [i, 1] == 3) {
						entity_timer [i] = timer - (level * 0.1f);
						w_status [i, 1] = 0;
						w_status [i, 2] = 0;
						entityAction (i);
					}
				}
			}			
		}

		//instanciando entidades
		timer_spawn -= Time.deltaTime;
		if (timer_spawn < 0) {
			StartCoroutine (entityCreate());
			timer_spawn = spawn_rate;
		}
	}


	void processKills(){
		if (kills == 3 && level == 0)
			level++;
		else if (kills == 6 && level == 1)
			level++;
		else if (kills == 9 && level == 2)
			level++;
		else if (kills == 15 && level == 3)
			level++;
		else if (kills == 30 && level == 4)
			level++;
		else if (kills == 50 && level == 5)
			level++;

	}


	void entityAction(int id_window){
		if(w_status[id_window, 1] == 2){
			changeWindowSprite (id_window, (w_status [id_window, 0] + 3));
			if (w_status [id_window,0] != 3)
				life--;
		}
		if (w_status [id_window, 1] == 0) {
			changeWindowSprite (id_window, 0);
		}
		if (w_status [id_window, 1] == 3) {
			changeWindowSprite(id_window, w_status[id_window,0] + 6);
		}
	}

	//verifica o input to jogador
	void checkInput(int w){
		if (w_status [w, 1] == 1 && w_status[w,2] == 0) {
			if (w_status[w,0] > 0 && w_status[w,0] < 3)
				kills++;
			else
				life--;
			w_status[w,2] = 1;
			w_status [w, 1] = 3;
			entityAction (w);
			processKills ();

		}
	}

	//cria as variáveis de um entidade e uma janela que não esteja em uso
	IEnumerator entityCreate(){
		yield return new WaitForSeconds (Random.Range(0,2.0f));

		int window=0;
		while (true) {
			//janela pretendida
			int w = (int)Random.Range (0,8);
			if (w_status [w,1] == 0) {
				window = w;
				break;
			}			
		}

		//sorteando o char
		int character =Mathf.RoundToInt(Random.Range (1,4));
		w_status [window,0] = character;// quem esta la dentro	
		w_status [window,1] = 1; // o status com alguem dentro (ocupada)
		changeWindowSprite(window,character);
	}


	void changeWindowSprite(int id_window, int id_sprite){
		w_images[id_window].sprite = c_sprites [id_sprite];
	}



	void reset(){
		in_game = true;
		for (int i = 0; i < 9; ++i) {
			w_status [i, 0] = i;
			w_status [i, 1] = 0;
			w_status [i, 2] = 0;
			changeWindowSprite (i, 0);
			entity_timer [i] = timer;
		}
		life = 3;
		kills = 0;
		level = 0;
	}

	public void onEntityClick(int window){
		scoreMenu.SetActive (false);
		checkInput (window);
	}

	public void onPlayClick(){		
		setActiveWindow (3);
		reset ();
	}

	public void onCreditsClick(){
		setActiveWindow (2);
	}

	public void onExitClick(){
		Application.Quit ();
	}

	public void onMainMenuClick(){
		
		setActiveWindow (1);
		in_game = false;
	}

	public void onRetryClick(){
		scoreMenu.SetActive (false);
		reset ();
	}

	void setActiveWindow(int w){
		
		creditsMenu.SetActive (false);
		gameMenu.SetActive (false);
		mainMenu.SetActive (false);
		scoreMenu.SetActive (false);

		if(w==1)
			mainMenu.SetActive (true);
		else if(w==2)
			creditsMenu.SetActive (true);
		else if(w==3)
			gameMenu.SetActive (true);			
	}
}
