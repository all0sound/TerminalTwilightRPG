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
using System.Linq;

public class BattleManager : Singleton<BattleManager> {

    protected BattleManager() { } //null constructor

	//public Stats PlayerStats;
	//public Stats MonsterStats;
    public Monster EncounteredMonster; //the monster currently encountered
	//public List<GameObject> Party;
	public GameObject[] PlayerParty;
	private int PlayerPartySize;
	private int PlayerPartyCount = 0;
	public Stats[] MonParty;// = new Stats[4];
	private int MonPartySize;
	private int MonstersCount = 0;
	public Stats target;
	public int targetInt = -1;
	public int Damage;	//precalculated damage from Skill or Attack
	//private List<Skill> tempSkillList;
	//public Text DamageText;
	private Canvas BattleCanvas;


	public Text SkillMPCost;
	public Text SkillDescText;
	public Text SkillNameText;
	public Text SkillNumber;
	private Text[] texts;		//for getting an array of texts of which are children to the BattleCanvas
	private Text canvas_text;	//text in center of canvas used for DEFEAT or VICTORY announcement
	private Text bot_text1;		//text on bottom showing player stat
	private Text bot_text2;
	private Text bot_text3;
	private Text bot_text4;
	private Text top_text1;		//text on top showing monster stat
	private Text top_text2;	
	private Text top_text3;	
	private Text top_text4;	
	private Text desc_text; 	//text that describes the battle, etc.
	private Text dmg1;
	private Text dmg2;
	private Text dmg3;
	private Text dmg4;
	private Text dmg5;
	private Text dmg6;
	private Text dmg7;
	private Text dmg8;

	private GameObject[] PlayerPositions;
	private GameObject p_pos1;
	private GameObject p_pos2;
	private GameObject p_pos3;
	private GameObject p_pos4;

	private GameObject[] EnemyPositions;
	private GameObject e_pos1;
	private GameObject e_pos2;
	private GameObject e_pos3;
	private GameObject e_pos4;

    private Button[] buttons;
	private Button ATTACK_Button;
	private Button DEFEND_Button;
	private Button SKILLS_Button;
	private Button ITEMS_Button;
	private Button RUN_Button;
	public GameObject OptionsMenu;
	public GameObject SkillsMenu;
    private List<AudioSource> allAudioSources;

	private List<Stats> turn_order = new List<Stats>();	//list that will be ordered to keep track of turn order
	private Stats current_stats;						//the current stat taking its turn in combat
	private bool waiting = false;
	private bool running = false;
	private bool escaped = false;
	private bool exiting = false;
	//private bool attacking = false;

	//ListIndex property: keeps track of whose turn it is in battle
	private int _ListIndex = 0;
	public int ListIndex
	{
		get {return _ListIndex; }
		set 
		{
			_ListIndex = value;
			if (_ListIndex >= turn_order.Count)
				_ListIndex = 0;
			else if (_ListIndex < 0)
				_ListIndex = 0;
		}
	}

	//SkillIndex property: index for navigating skills
	private int _SkillIndex = 0;
	public int SkillIndex
	{
		get {return _SkillIndex; }
		set {
			_SkillIndex = value;
			//if (current_stats.Skills.Count > 0) {
				//_SkillIndex = 0;
				if (_SkillIndex >= current_stats.Skills.Count)
					_SkillIndex = (current_stats.Skills.Count - 1);
				else if (_SkillIndex < 0)
					_SkillIndex = 0;
			//}
		}
	}

	//option states
	public enum states {NULL = 0, ATTACK = 1, DEFEND = 2, SKILLS = 3, ITEMS = 4, RUN = 5, SKILLSMENU = 6, TARGETALLY = 8, TARGETENEMY = 9, WAITING = 10, EXITING = 11};
	private states curr_state;	//memorizes what is the current state the battle_canvas is in

	protected override void Awake()
	{
		//MonsterStats = new Stats ("GenericMonster", false, 1, 50, 0, 5, 5, 5, 5, 5, 5);
		//turn_order.Add(MonsterStats);
		//PlayerStats = new Stats ("GenericPlayer", true, 1, 100, 100, 10, 10, 10, 10, 10, 10);
		//turn_order.Add(PlayerStats);
		BattleCanvas = GetComponent<Canvas>();
		//current_stats = turn_order.ElementAt (0);
        base.Awake();
        //gameObject.SetActive(false);    //hide BattleCanvas from game view 
    }

	// Use this for initialization
	void Start () 
	{
		//set up battle's turn order
		ListIndex = 0;											//set ListIndex to beginning of the list for tunr_order
        //PlayerStats = Player.Instance.GetComponent<Stats>();	//grab Stats from Player and put into PlayerStats
	    //turn_order.Add (PlayerStats);							//add PlayerStats
	    //turn_order.Add (MonsterStats);							//add MonsterStats
		//turn_order = turn_order.OrderByDescending(x => x.Agility).ToList();	//order tunr_order list by Agility(descending)
		//current_stats = turn_order.ElementAt(0);				//set current_stats to first in turn_order
		//curr_state = states.ATTACK;								//curr_state is ATTACK by default
		curr_state = states.NULL;

		//assign Text children to Text instances in file
		texts = GetComponentsInChildren<Text> ();				
		foreach(Text child in texts)
		{
			switch(child.name) {
			case "CanvasText":
				canvas_text = child;
				break;
			case "BottomText1":
				bot_text1 = child;
				break;
			case "BottomText2":
				bot_text2 = child;
				break;
			case "BottomText3":
				bot_text3 = child;
				break;
			case "BottomText4":
				bot_text4 = child;
				break;
			case "TopText1":
				top_text1 = child;
				break;
			case "TopText2":
				top_text2 = child;
				break;
			case "TopText3":
				top_text3 = child;
				break;
			case "TopText4":
				top_text4 = child;
				break;
			case "DescriptionText":
				desc_text = child;
				break;
			case "damage1":
				dmg1 = child;
                dmg1.text = "";
				break;
			case "damage2":
				dmg2 = child;
                dmg2.text = "";
                break;
			case "damage3":
				dmg3 = child;
                dmg3.text = "";
                break;
			case "damage4":
				dmg4 = child;
                dmg4.text = "";
                break;
			case "damage5":
				dmg5 = child;
                dmg5.text = "";
                break;
			case "damage6":
				dmg6 = child;
                dmg6.text = "";
                break;
			case "damage7":
				dmg7 = child;
                dmg7.text = "";
                break;
			case "damage8":
				dmg8 = child;
                dmg8.text = "";
                break;
			}
		}
		canvas_text.gameObject.SetActive (false);	//hides canvas_text
		desc_text.text = "";						//clears desc_text

		//assign Button children to Button instances in file
		buttons = GetComponentsInChildren<Button> ();
		foreach (Button child in buttons) 
		{
			if (child.name == "ATTACK_Button")
				ATTACK_Button = child;	
			else if(child.name == "DEFEND_Button")
				DEFEND_Button = child;
			else if (child.name == "SKILLS_Button")
				SKILLS_Button = child;
			else if(child.name == "ITEMS_Button")
				ITEMS_Button = child;
			else if(child.name == "RUN_Button")
				RUN_Button = child;
		}
        OptionsMenu.SetActive(false);
		SkillsMenu.SetActive (false);

		PlayerPositions = GameObject.FindGameObjectsWithTag ("PlayerPosition");
		foreach (GameObject child in PlayerPositions) 
		{
			if (child.name == "Position1")
				p_pos1 = child;
			else if (child.name == "Position2")
				p_pos2 = child;
			else if (child.name == "Position3")
				p_pos3 = child;
			else if (child.name == "Position4")
				p_pos4 = child;
		}

		EnemyPositions = GameObject.FindGameObjectsWithTag ("EnemyPosition");
		foreach (GameObject child in EnemyPositions) 
		{
			if (child.name == "MonsterPosition1")
				e_pos1 = child;
			else if (child.name == "MonsterPosition2")
				e_pos2 = child;
			else if (child.name == "MonsterPosition3")
				e_pos3 = child;
			else if (child.name == "MonsterPosition4")
				e_pos4 = child;
		}
        gameObject.SetActive(false);
    }

	/// <summary>
	/// //////////////////////////////////////////////Update is called once per frame/////////////////////////////////////////////////////////////////////////
	/// </summary>
	void Update () 
	{
		//Debug.Log("turn_order size: " + turn_order.Count());
		Debug.Log ("Current State: " + curr_state + "\nCurrent MonPartySize: " + MonPartySize);// + "\nCurrent Stats: " + current_stats.Name);
		DisplayTexts();
		if (curr_state != states.SKILLSMENU)
			SkillsMenu.SetActive (false);
		//switch between turns
		if (curr_state == states.WAITING) 
		{
			if (!waiting) {
				waiting = true;
				StartCoroutine (WaitASec ());
			}
		} 
		else if (curr_state == states.TARGETENEMY) ////////TARGETING ENEMY STATE////////////////////////////////////////////////////////
		{
			Debug.Log ("MonParty.Length: " + MonParty.Length);
			//IF there is only one enemy, skip targting and go straight to attack.
			if (MonParty.Length == 1) {
				//target = MonParty.ElementAt (0);
				target = MonParty[0];
				target.TakeDamage (Damage);	//Target takes damage
				DisplayDamage (5, Damage); //Displays damage
				//DisplayDamage (5, current_stats.Attack (target));
				desc_text.text = current_stats.Name + " attacks " + target.Name + "!";
				current_stats = turn_order.ElementAt (++ListIndex);
				curr_state = states.WAITING;
				Damage = 0;	//resets Damage
			} 
			//If there is more than one enemy, go to targeting
			else if (MonParty.Length > 1)
			{
				desc_text.text = "Press 1, 2, 3, or 4 to select target.";
				if (Input.GetButtonDown ("1") && MonParty[0].HP > 0) {
					targetInt = 0;
					if (MonParty [targetInt].HP <= 0) {
						desc_text.text = "Invalid target!";
					} else {
						//DisplayDamage (targetInt + 5, MonParty [0].TakeDamage (current_stats.Attack()));
						DisplayDamage (targetInt + 5, MonParty [0].TakeDamage (Damage));
						current_stats = turn_order.ElementAt (++ListIndex);
						Damage = 0; //resets Damage
						curr_state = states.WAITING;
						targetInt = -1;
					}
				} else if (Input.GetButtonDown ("2") && MonParty[1].HP > 0) {
					targetInt = 1;
					if (MonParty [targetInt].HP <= 0) {
						desc_text.text = "Invalid target!";
					} else {
						//DisplayDamage (targetInt + 5, MonParty [1].TakeDamage (current_stats.Attack()));
						DisplayDamage (targetInt + 5, MonParty [1].TakeDamage (Damage));
						current_stats = turn_order.ElementAt (++ListIndex);
						curr_state = states.WAITING;
						targetInt = -1;
					}
				} else if (Input.GetButtonDown ("3") && MonParty[2].HP > 0) {
					targetInt = 2;
					if (MonParty [targetInt].HP <= 0) {
						desc_text.text = "Invalid target!";
					} else {
						//DisplayDamage (targetInt + 5, MonParty [2].TakeDamage (current_stats.Attack()));
						DisplayDamage (targetInt + 5, MonParty [2].TakeDamage (Damage));
						current_stats = turn_order.ElementAt (++ListIndex);
						curr_state = states.WAITING;
						targetInt = -1;
					}
				} else if (Input.GetButtonDown ("4") && MonParty[3].HP > 0) {
					targetInt = 3;
					if (MonParty [targetInt].HP <= 0) {
						desc_text.text = "Invalid target!";
					} else {
						//DisplayDamage (targetInt + 5, MonParty [3].TakeDamage (current_stats.Attack()));
						DisplayDamage (targetInt + 5, MonParty [3].TakeDamage (Damage));
						current_stats = turn_order.ElementAt (++ListIndex);
						curr_state = states.WAITING;
						targetInt = -1;
					}
				} else {
					curr_state = states.TARGETENEMY;
				}
			} 
			else {
				Debug.Log ("Targeting state error: Targeting MonParty of count 0.");	
			}
		}
		else if(curr_state == states.TARGETALLY)
		{
			Debug.Log ("PlayerParty.Length: " + PlayerParty.Length + ". targetInt = " + targetInt);
			if (PlayerParty.Length == 1) {
				target = PlayerParty [0].GetComponent<Stats> ();
				target.TakeDamage (Damage);
				DisplayDamage (1, Damage); //Display damage on ally?
				current_stats = turn_order.ElementAt (++ListIndex);
				curr_state = states.WAITING;
				Damage = 0;	
				targetInt = -1;
			} else if (PlayerParty.Length > 1) {
				desc_text.text = "Press 1, 2, 3, or 4 to select target.";
				if (Input.GetButtonDown ("1") && PlayerParty [0].GetComponent<Stats> ().HP > 0) {
					targetInt = 0;
				} else if (Input.GetButtonDown ("2") && PlayerParty [1].GetComponent<Stats> ().HP > 0) {
					targetInt = 1;
				} else if (Input.GetButtonDown ("3") && PlayerParty [2].GetComponent<Stats> ().HP > 0) {
					targetInt = 2;
				} else if (Input.GetButtonDown ("4") && PlayerParty [3].GetComponent<Stats> ().HP > 0) {
					targetInt = 3;
				} 
				if (targetInt != -1) {
					DisplayDamage (targetInt + 1, PlayerParty[targetInt].GetComponent<Stats>().Heal (Damage));
					current_stats = turn_order.ElementAt (++ListIndex);
					curr_state = states.WAITING;
					Damage = 0;
					targetInt = -1;
				}
			} else {
				curr_state = states.TARGETALLY;
			}
		}
		else if (curr_state == states.EXITING)	//EXITING STATE//////////////////////////////////////////////////////////////////////////////////////////////
		{
			ClearDamageTexts ();
			Debug.Log ("Exiting Battle");
		}
		else if(curr_state == states.SKILLSMENU)	//SKILLSMENU STATE//////////////////////////////////////////////////////////////////////////////////////
		{
			SkillsMenu.SetActive (true);
			int SkillIntTemp = 0;
			int SkillCount = current_stats.Skills.Count;
			desc_text.text = "Use LEFT and RIGHT keys to navigate. ENTER to choose skill. ESC to cancel.";
			//Debug.Log ("Skills Menu current stats: " + current_stats.Name);
			if (SkillCount != 0) {
				SkillNameText.text = current_stats.Skills.ElementAt (SkillIndex).SkillName;
				SkillMPCost.text = "MP: " + current_stats.Skills.ElementAt (SkillIndex).MPCost;
				SkillDescText.text = current_stats.Skills.ElementAt (SkillIndex).SkillDescText;
				SkillIntTemp = SkillIndex + 1;	//Skill's number in list
				//SkillCount = current_stats.Skills.Count;
				SkillNumber.text = SkillIntTemp.ToString() + "/" + SkillCount.ToString();
			}
			if (Input.GetButtonDown ("Horizontal") && Input.GetAxisRaw ("Horizontal") > 0) { //right is pressed
				++SkillIndex;
				//Debug.Log ("Right button pressed!");
			} else if (Input.GetButtonDown ("Horizontal") && Input.GetAxisRaw ("Horizontal") < 0 && SkillIndex > 0) { 		//left is pressed
				--SkillIndex;
			} else if (Input.GetButtonDown ("Cancel")) {
				SkillsMenu.SetActive (false);
				curr_state = states.ATTACK;
				SkillIndex = 0;
			} else if (Input.GetButtonDown ("Submit") && SkillCount > 0) {
				SkillBase CurrentSkill = current_stats.Skills.ElementAt (SkillIndex);

				//Should check if Stats have enough MP to cast

				Damage = current_stats.UseSkill(CurrentSkill);
				if (CurrentSkill.targettype == TargetType.EnemySingle) {
					curr_state = states.TARGETENEMY;
				} else if (CurrentSkill.targettype == TargetType.EnemyAll) {
					curr_state = states.TARGETENEMY;	//WIP
				} else if (CurrentSkill.targettype == TargetType.PartySingle) {
					curr_state = states.TARGETALLY;
				} else if (CurrentSkill.targettype == TargetType.PartyAll) {
					curr_state = states.TARGETALLY;		//WIP
				}
			}
			//Debug.Log ("Current Skill Index: " + SkillIndex +" and SkillCount: " + SkillCount);
		}
		//Player's Turn
		else if(current_stats.Playable && curr_state != states.WAITING) //Selecting from main options menu////////////////////////////////////////////////////
		{
			desc_text.text = current_stats.Name + "'s Turn";
			current_stats.defending = false;
			OptionsMenu.SetActive(true);
			if (curr_state == states.NULL) 
			{
				//Debug.Log ("Switched from NULL to ATTACK");
				curr_state = states.ATTACK;
				OptionsMenu.SetActive(true);
				ATTACK_Button.interactable = true;
			}
			ChangeSelectionColor(curr_state);
			if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") > 0) 		//up is pressed
			{
				if (curr_state != states.ATTACK)
					--curr_state;
			}
			else if(Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") < 0)	//down is pressed
			{
				if(curr_state != states.RUN)
					++curr_state;
			}
			//choose highlighted option
			else if(Input.GetButtonDown("Submit"))
			{
				switch (curr_state) 
				{
				case states.ATTACK:
					{
						ATTACK_Button.interactable = false;
						if (MonParty.Length <= 0) 
						{
							Debug.Log ("Error: Attacking when MonParty has no elements.");
						}
						Damage = Random.Range(current_stats.Strength / 2, current_stats.Strength);
						curr_state = states.TARGETENEMY;
						//desc_text.text = current_stats.Name + " attacks " + target.Name + "!";
						break;
					}
				case states.DEFEND:
					{
						desc_text.text = current_stats.Name + " defends!";
						current_stats.defending = true;
						current_stats = turn_order.ElementAt (++ListIndex);
						curr_state = states.WAITING;
						break;
					}
				case states.SKILLS:
					{
						curr_state = states.SKILLSMENU;
						SkillIndex = 0;
						//desc_text = "Use LEFT and RIGHT keys to navigate. ENTER to choose skill. ESC to back.\n";
						break;
					}
				case states.ITEMS:
					{
						///WIP...
						current_stats = turn_order.ElementAt (++ListIndex);
						curr_state = states.WAITING;
						break;
					}
				case states.RUN:
					{
						desc_text.text = "Attempting to run...";
						running = true;
						//StartCoroutine(BattleEscape());
						BattleEscape();
						current_stats = turn_order.ElementAt (++ListIndex);
						curr_state = states.WAITING;
						break;
					}
				}	//end: switch
				//				if (curr_state == states.ATTACK)
				//					ATTACK_Button.interactable = false;
				//curr_state = states.NULL;
//				current_stats = turn_order.ElementAt (++ListIndex);
//				curr_state = states.WAITING;
			}	//end: else if(Input.GetButtonDown("Submit"))
		}	//end: if (current_stats.Playable) 
		else if(!current_stats.Playable && (curr_state != states.WAITING || curr_state != states.EXITING))	//MONSTER's TURN///////////////////
		{
			//Monster's turn
			desc_text.text = current_stats.Name + "'s Turn";
			current_stats.defending = false;

			//set attack target as random party member
			targetInt = Random.Range(0, PlayerPartySize - 1);
			//target = Party.ElementAt(targetInt).GetComponent<Stats>();	
			target = PlayerParty[targetInt].GetComponent<Stats>();

			DisplayDamage (targetInt + 1, target.TakeDamage(current_stats.Attack ()));	//attacks random person
			desc_text.text = current_stats.Name + " attacks " + target.Name + "!";
			++ListIndex;	//because properties sucks
			current_stats = turn_order.ElementAt (ListIndex);
			curr_state = states.WAITING;
			targetInt = -1;
		}

		//removes any monsters who died from battle
		for(int i = 0; i < MonPartySize; ++i)
		{
			if(MonParty[i].HP <= 0)
			{
				turn_order.Remove(MonParty[i]);
			}
		}

		//counts how many party members are alive
		PlayerPartyCount = 0;
		foreach (Stats stat in turn_order) {
			if (stat.Playable)
				PlayerPartyCount++;
		}

		//checks for DEFEAT and VICTORY
		if (PlayerPartyCount <= 0) 
		{
			curr_state = states.WAITING;
			exiting = true;
			OptionsMenu.SetActive(false);
			StartCoroutine(BattleDefeat());
		} 

		//counts how many monsters are alive
		MonstersCount = 0;
		foreach (Stats stat in turn_order) {
			if (!stat.Playable)
				MonstersCount++;
		}
		//if no monsters lieft alive, trigger battle victory
		if (MonstersCount <= 0) {
			//Debug.Log ("Victory called. MonPartySize: " + MonPartySize);
			curr_state = states.WAITING;
			exiting = true;
			OptionsMenu.SetActive(false);
			StartCoroutine(BattleVictory());
		}
	}

	/// <summary>
	/// A function that changes the color of texts in options menu
	/// </summary>
	private void ChangeSelectionColor(states current) {
		Debug.Log ("Color change has been called");
		switch (current) 
		{
			case states.ATTACK:
			{	
				ATTACK_Button.Select();
				break;
			}
			case states.DEFEND:
			{	
				DEFEND_Button.Select();
				break;
			}
			case states.SKILLS:
			{
				SKILLS_Button.Select();
				break;
			}
			case states.ITEMS:
			{	
				ITEMS_Button.Select();
				break;
			}
			case states.RUN:
			{	
				RUN_Button.Select();
				break;
			}
			default:
			{
				break;
			}
		}
	}

	/// <summary>
	/// Displays damage taken for characters
	/// </summary>
	public void DisplayDamage(int pos, int damage)
	{
		switch (pos) 
		{
		case 1:
			{
				dmg1.text = damage.ToString ();
				break;
			}
		case 2:
			{
				dmg2.text = damage.ToString ();
				break;
			}
		case 3:
			{
				dmg3.text = damage.ToString ();
				break;
			}
		case 4:
			{
				dmg4.text = damage.ToString ();
				break;
			}
		case 5:
			{
				dmg5.text = damage.ToString ();
				break;
			}
		case 6:
			{
				dmg6.text = damage.ToString ();
				break;
			}
		case 7:
			{
				dmg7.text = damage.ToString ();
				break;
			}
		case 8:
			{
				dmg8.text = damage.ToString ();
				break;
			}
		default:
			{
				Debug.Log ("DisplayDamage: Invalid pos input");
				break;
			}
		}
		//Text DmgTextClone = Instantiate(DamageText, loc, Quaternion.identity) as Text;
		//DmgTextClone.text = damage.ToString();
		//DmgTextClone.transform.SetParent (BattleCanvas.transform, false);
	}

	private void ClearDamageTexts()
	{
		dmg1.text = "";
		dmg2.text = "";
		dmg3.text = "";
		dmg4.text = "";
		dmg5.text = "";
		dmg6.text = "";
		dmg7.text = "";
		dmg8.text = "";
	}

    /// <summary>
    /// Public function that Player can call when initiating a battle
    /// </summary>
    public void Encounter(Monster hit_monster)
    {
		EncounteredMonster = hit_monster;
        //stop all monster sounds when entering battle
        foreach (Monster mon in GameManager.Instance.all_monsters) {
            mon.GetComponent<AudioSource>().Stop();
        }
        if (GameManager.Instance.IsState(GameStates.IdleState)) {
            GameManager.Instance.SetState(GameStates.BattleState); //disable player movement
            SoundManager.Instance.SetMusic(ResourceManager.Instance.GetSound("BattleMusic"));
			Debug.Log ("Changing from Idle to Battle State");
			gameObject.SetActive (true);
            //gameObject.SetActive(true);             //activate BattleCanvas so it can be seen
			curr_state = states.NULL;

			turn_order.Clear ();
			MonstersCount = 0;
			MonPartySize = hit_monster.EnemyParty.Count;
			MonParty = new Stats[MonPartySize];
			for (int i = 0; i < MonPartySize; i++) {
				//Debug.Log ("Enemy Stat added: " + hit_monster.EnemyParty.ElementAt(i).Name);
				MonParty[i] = hit_monster.EnemyParty.ElementAt(i);
			}
			if (MonParty.Length <= 0)
				Debug.Log ("MonParty has no monsters.");
			foreach (Stats stat in hit_monster.EnemyParty) //add encountered monsters into turn_order
			{
				turn_order.Add (stat);
			}

			//Player's party as an array
			PlayerPartyCount = 0;
			PlayerPartySize = Player.Instance.GetComponent<Player> ().Party.Count;
			PlayerParty = new GameObject[PlayerPartySize];
			for (int i = 0; i < PlayerPartySize; i++) {
				PlayerParty [i] = Player.Instance.GetComponent<Player> ().Party.ElementAt (i);
			}
			if (PlayerParty.Length <= 0)
				Debug.Log ("ERROR: PlayerParty has no characters; Encounter()");
			foreach (GameObject obj in Player.Instance.GetComponent<Player> ().Party) {
				turn_order.Add (obj.GetComponent<Stats> ());
			}

			//Player's party as a list
//			Party = Player.Instance.GetComponent<Player> ().Party;
//			if (Party.Count == 0)
//				Debug.Log ("Party has no members.");
//			foreach (GameObject obj in Party) 	//add player's party members into turn_order
//			{
//				turn_order.Add (obj.GetComponent<Stats>());
//			}

			Debug.Log ("Turn order list size: " + turn_order.Count);
            //sets turn order and resets the ListIndex
            turn_order = turn_order.OrderByDescending(x => x.Agility).ToList();
			current_stats = turn_order.ElementAt(0);
            ListIndex = 0;
        }
	}

	/// <summary>
	/// For displaying HP and MP of active characters in the battle
	/// </summary>
	private void DisplayTexts()
	{
		/*
		if (Party.ElementAt (0) != null) {
            bot_text1.text = Party.ElementAt(0).GetComponent<Stats>().Name + "\nHP: " + Party.ElementAt(0).GetComponent<Stats>().HP + "\nMP: " + Party.ElementAt(0).GetComponent<Stats>().MP;
        } else {
			//bot_text1.gameObject.SetActive (false);
		}
		if (Party.ElementAt (1) != null) {
			bot_text2.text = Party.ElementAt (1).GetComponent<Stats> ().Name + "\nHP: " + Party.ElementAt (1).GetComponent<Stats> ().HP + "\nMP: " + Party.ElementAt (1).GetComponent<Stats> ().MP;
		} else {
			//bot_text2.gameObject.SetActive (false);
		}
		if (Party.ElementAt (2) != null) {
			bot_text3.text = Party.ElementAt (2).GetComponent<Stats> ().Name + "\nHP: " + Party.ElementAt (2).GetComponent<Stats> ().HP + "\nMP: " + Party.ElementAt (2).GetComponent<Stats> ().MP;
		} else {
			//bot_text3.gameObject.SetActive (false);
		}
		if (Party.ElementAt (3) != null) {
			bot_text4.text = Party.ElementAt (3).GetComponent<Stats> ().Name + "\nHP: " + Party.ElementAt (3).GetComponent<Stats> ().HP + "\nMP: " + Party.ElementAt (3).GetComponent<Stats> ().MP;
		} else {
			//bot_text4.gameObject.SetActive (false);
		}
		*/
		if (PlayerPartySize > 0) {
			if (PlayerParty[0].GetComponent<Stats>().HP > 0)
				bot_text1.text = PlayerParty[0].GetComponent<Stats>().Name + "\nHP: " + PlayerParty[0].GetComponent<Stats>().HP + "\nMP: " + PlayerParty[0].GetComponent<Stats>().MP;
			else
				bot_text1.text = "";
		}
		if (PlayerPartySize > 1) {
			if (PlayerParty[1].GetComponent<Stats>().HP > 0)
				bot_text2.text = PlayerParty[1].GetComponent<Stats>().Name + "\nHP: " + PlayerParty[1].GetComponent<Stats>().HP + "\nMP: " + PlayerParty[1].GetComponent<Stats>().MP;
			else
				bot_text2.text = "";
		}
		else
			bot_text2.text = "";

		if (PlayerPartySize > 2) {
			if (PlayerParty[2].GetComponent<Stats>().HP > 0)
				bot_text3.text = PlayerParty[2].GetComponent<Stats>().Name + "\nHP: " + PlayerParty[2].GetComponent<Stats>().HP + "\nMP: " + PlayerParty[2].GetComponent<Stats>().MP;
			else
				bot_text3.text = "";
		}
		else
			bot_text3.text = "";

		if (PlayerPartySize > 3) {
			if (PlayerParty[3].GetComponent<Stats>().HP > 0)
				bot_text4.text = PlayerParty[3].GetComponent<Stats>().Name + "\nHP: " + PlayerParty[3].GetComponent<Stats>().HP + "\nMP: " + PlayerParty[3].GetComponent<Stats>().MP;
			else
				bot_text4.text = "";
		}
		else
			bot_text4.text = "";
		//Display Monsters' stuff
		if (MonPartySize > 0) {
			if (MonParty[0].HP > 0)
				top_text1.text = MonParty [0].Name + "\nHP: " + MonParty [0].HP + "\nMP: " + MonParty [0].MP;
			else
				top_text1.text = "";
		}
		if (MonPartySize > 1) {
			if (MonParty[1].HP > 0)
				top_text2.text = MonParty [1].Name + "\nHP: " + MonParty [1].HP + "\nMP: " + MonParty [1].MP;
			else
				top_text2.text = "";
		}
		else
			top_text2.text = "";
		
		if (MonPartySize > 2) {
			if (MonParty[2].HP > 0)
				top_text3.text = MonParty [2].Name + "\nHP: " + MonParty [2].HP + "\nMP: " + MonParty [2].MP;
			else
				top_text3.text = "";
		}
		else
			top_text3.text = "";
		
		if (MonPartySize > 3) {
			if (MonParty[3].HP > 0)
				top_text4.text = MonParty [3].Name + "\nHP: " + MonParty [3].HP + "\nMP: " + MonParty [3].MP;
			else
				top_text4.text = "";
		}
		else
			top_text4.text = "";
	}

	/// <summary>
	/// Clearing the stat-texts
	/// </summary>
	private void ClearTexts()
	{
		Debug.Log ("Clearing Texts!");
		bot_text1.text = "";
		bot_text2.text = "";
		bot_text3.text = "";
		bot_text4.text = "";
		top_text1.text = "";
		top_text2.text = "";
		top_text3.text = "";
		top_text4.text = "";
	}

	/// <summary>
	/// Used for testing. Prints the order of battle in the console.
	/// </summary>
	public void PrintOrder()
	{
		foreach (Stats t in turn_order)
			Debug.Log (t.Name);
	}

    /// <summary>
    /// Deactivates the battle canvas and changes game state to idle.
    /// </summary>
    private void EndBattle()
    {
        SoundManager.Instance.SetMusic(ResourceManager.Instance.GetSound("CastleMusic"));
		ATTACK_Button.interactable = false;
		waiting = false;
		exiting = false;
		curr_state = states.NULL;
		//gameObject.SetActive(false);
        turn_order.Clear(); //clear the turn order list for the next encournter
        if (GameManager.Instance.IsState(GameStates.BattleState))
            GameManager.Instance.SetState(GameStates.IdleState);
        else
            Debug.LogError("Game should be in battle state, but you are not.");
		gameObject.SetActive(false);
    }

	/// <summary>
	/// Wait for 1 sec
	/// </summary>
	/// <summary>
	/// Waits for a few secs to show text to player, then "turn off" text
	/// TL;DR It's used for smooth transitioning between states after a move have been made
	/// </summary>
	IEnumerator WaitASec()
	{
		OptionsMenu.SetActive (false);
		yield return new WaitForSeconds (1);
		foreach (Text child in texts) 
		{
			if (child.name.Substring(0, 3) == "dam") 
			{
				child.text = "";
			}
		}
		waiting = false;			//no longer waiting
		curr_state = states.NULL;	//resets the state to NULL
		if (running && escaped) {
			curr_state = states.EXITING;
			canvas_text.text = "ESCAPED!";
			canvas_text.gameObject.SetActive (true);
			yield return new WaitForSeconds (2);
			canvas_text.gameObject.SetActive (false);
			EncounteredMonster.Pause (2f);
			EndBattle ();
		} else if (running && !escaped) {
			canvas_text.text = "CANNOT ESCAPE!";
			canvas_text.gameObject.SetActive (true);
			yield return new WaitForSeconds (2);
			canvas_text.gameObject.SetActive (false);
		} else if (exiting)
			curr_state = states.EXITING;
		running = false;			//no longer running/trying to escape
	}
		
	/// Exits the battle when the player loses
	/// </summary>
	//IEnumerator BattleEscape()
	void BattleEscape()
	{
		Debug.Log ("Calculating escape");
		if (current_stats.Agility >= 2 * 5) 	//5 is a placeholder value
		{
			escaped = true;
			//canvas_text.text = "ESCAPED!";
			//			canvas_text.gameObject.SetActive (true);
			//			yield return new WaitForSeconds (2);
			//			canvas_text.gameObject.SetActive (false);
			//MonsterStats.gameObject.GetComponent<Monster>().Pause(2f);
			//Monster.Pause(2f);
			//EndBattle();
		} 
		else 
		{
			escaped = false;
			//			canvas_text.text = "CANNOT ESCAPE!";
			//			yield return new WaitForSeconds (2);
		}
	}

	/// <summary>
	/// Exits the battle when the player loses
	/// </summary>
	IEnumerator BattleDefeat()
	{
		//EncounteredMonster.gameObject.SetActive (false);
		Debug.Log ("Defeat triggered");
		desc_text.text = "";
		canvas_text.text = "DEFEAT!";
		canvas_text.gameObject.SetActive (true);
		yield return new WaitForSeconds (2);
		canvas_text.gameObject.SetActive (false);
        EndBattle();
		GameManager.Instance.GameOver();
	}

	/// <summary>
	/// Exits the battle when the player wins
	/// </summary>
	IEnumerator BattleVictory()
	{
		EncounteredMonster.killed = true;
		EncounteredMonster.gameObject.SetActive (false);
		Debug.Log ("Victory triggered");
		desc_text.text = "";
		canvas_text.text = "VICTORY!";
		canvas_text.gameObject.SetActive (true);
		yield return new WaitForSeconds (2);
		canvas_text.gameObject.SetActive (false);
        EndBattle();
	}
}
