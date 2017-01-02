/* NAME:            BattleManager.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     The script class manages the battle and its canvas when battles are triggered. 
 * 				    It derives from  singleton.
 * REQUIREMENTS:    Base class Singleton.cs must be present.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : Singleton<BattleManager> {

	public Stats PlayerStats;
	public Stats MonsterStats;

	private Canvas BattleCanvas;
	private Text[] texts;
	private Text canvas_text;
	private Text bot_text;
	private Text top_text;

	void Awake()
	{
		PlayerStats = Player.Instance.GetComponent<Stats>();
		MonsterStats = new Stats ("GenericMonster", 1, 50, 0, 5, 5);
		BattleCanvas = GetComponent<Canvas> ();
		gameObject.SetActive(false);
	}

	// Use this for initialization
	void Start () 
	{
		texts = GetComponentsInChildren<Text> ();

		foreach(Text child in texts)
		{
			if (child.name == "CanvasText")
				canvas_text = child;
			else if (child.name == "BottomText")
				bot_text = child;
			else if(child.name == "TopText")
				top_text = child;
		}

		canvas_text.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		bot_text.text = PlayerStats.Name + "\nHP: " + PlayerStats.HP + "\nMP: " + PlayerStats.MP;
		top_text.text = MonsterStats.Name + "\nHP: " + MonsterStats.HP + "\nMP: " + MonsterStats.MP;

		if (PlayerStats.HP <= 0)
		{
			StartCoroutine(BattleDefeat());
		}
		else if (MonsterStats.HP <= 0)
		{
			StartCoroutine(BattleVictory());
		}
	}

	/// <summary>
	/// Public function that Player can call when initiating a battle
	/// </summary>
	public void Encounter(Stats hit_monster)
	{
		Player.Instance.PlayersTurn = false;
		gameObject.SetActive(true);
		MonsterStats = hit_monster;
	}

	/// <summary>
	/// Have the player attack the Monster
	/// </summary>
	public void PlayerAttack()
	{
		PlayerStats.Attack (MonsterStats);
	}

	/// <summary>
	/// Exits the battle when the player loses
	/// </summary>
	IEnumerator BattleDefeat()
	{
		canvas_text.text = "DEFEAT!";
		canvas_text.gameObject.SetActive (true);
		yield return new WaitForSeconds (1);
		canvas_text.gameObject.SetActive (false);
		gameObject.SetActive(false);
		GameManager.Instance.GameOver();
	}

	/// <summary>
	/// Exits the battle when the player wins
	/// </summary>
	IEnumerator BattleVictory()
	{
		canvas_text.text = "VICTORY!";
		canvas_text.gameObject.SetActive (true);
		yield return new WaitForSeconds (1);
		canvas_text.gameObject.SetActive (false);
		gameObject.SetActive(false);
	}
}
