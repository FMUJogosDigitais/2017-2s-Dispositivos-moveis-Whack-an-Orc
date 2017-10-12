using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mainControl : MonoBehaviour {

	public Sprite[] characters;
	public Image[] windows;

	

	void Start () {
		
	}
	
	
	void Update () {
		
	}


	public void playerHit(int w){
		float x = Random.Range(0,9f);
		int xx = (int)Mathf.Ceil(x);
		Debug.Log(w+" : "+xx);
		for(int i = 0;i<9;++i)
			windows[w].sprite = characters[xx];
	}
}
