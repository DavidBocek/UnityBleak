using UnityEngine;
using System.Collections;

public class BleakUIManager : MonoBehaviour {

	private uint state {get; set;}
	public const uint STATE_NONE = 0;
	public const uint STATE_INVENTORY = 1;
	public const uint STATE_QUESTS = 2;
	public const uint STATE_MAP = 3;
	public const uint STATE_KEY = 4;
	public const uint STATE_CONVERSATION = 5;

	public BleakUIElement inventory;
	public BleakUIElement quests;
	public BleakUIElement key;
	public BleakUIElement map;
	public BleakUIElement conversation;

	public bool working {get; set;}

	private KeyCode currentInput;
	private BleakController controller;
	private BleakControllerTopDown controllerTD;

	// Use this for initialization
	void Start () {
		state = STATE_NONE;
		working = false;
		controller = GetComponent<BleakController>();
		controllerTD = GetComponent<BleakControllerTopDown>();
	}
	
	// Update is called once per frame
	void Update () {
		if (controller != null){
			if (!controller.canControl){
				return;
			}
		} else if (controllerTD != null){
			if (!controllerTD.canControl){
				return;
			}
		}
		if (working) return;
		currentInput = KeyCode.None;
		if (Input.GetKeyDown(KeyCode.RightArrow)){
			currentInput = KeyCode.RightArrow;
		} else if (Input.GetKeyDown(KeyCode.LeftArrow)){
			currentInput = KeyCode.LeftArrow;
		} else if (Input.GetKeyDown(KeyCode.UpArrow)){
			currentInput = KeyCode.UpArrow;
		} else if (Input.GetKeyDown(KeyCode.DownArrow)){
			currentInput = KeyCode.DownArrow;
		} else if (Input.GetKeyDown(KeyCode.I)){
			currentInput = KeyCode.I;
		} else if (Input.GetKeyDown(KeyCode.M)){
			currentInput = KeyCode.M;
		} else if (Input.GetKeyDown(KeyCode.K)){
			currentInput = KeyCode.K;
		} else if (Input.GetKeyDown(KeyCode.U)){
			currentInput = KeyCode.U;
		} else if (Input.GetKeyDown(KeyCode.O)){
			currentInput = KeyCode.O;
		} else if (Input.GetKeyDown (KeyCode.Escape)){
			currentInput = KeyCode.Escape;
		} else if (Input.GetKeyDown (KeyCode.Return)){
			currentInput = KeyCode.Return;
		}

		switch(currentInput){
		case KeyCode.None:
			return;
		case KeyCode.RightArrow:
			ActionRight();
			break;
		case KeyCode.LeftArrow:
			ActionLeft();
			break;
		case KeyCode.UpArrow:
			ActionUp();
			break;
		case KeyCode.DownArrow:
			ActionDown();
			break;
		case KeyCode.U:
			ActionQuests();
			break;
		case KeyCode.I:
			ActionInventory();
			break;
		case KeyCode.O:
			ActionConversation();
			break;
		case KeyCode.M:
			ActionMap();
			break;
		case KeyCode.K:
			ActionKey();
			break;
		case KeyCode.Escape:
			ActionEscape();
			break;
		case KeyCode.Return:
			ActionReturn();
			break;
		default:
			throw new UnityException("UI current input invald.");
		}
		//Debug.Log (state);
	}


	public void SetState(uint newState){
		state = newState;
	}


	void ActionRight(){
		switch (state){
		case STATE_NONE:
			inventory.OpenTree();
			SetState(STATE_INVENTORY);
			break;
		case STATE_INVENTORY:
			inventory.IncrementSelection();
			break;
		case STATE_QUESTS:
			quests.CloseTree();
			SetState(STATE_NONE);
			break;
		case STATE_MAP:
			map.CloseTree();
			SetState(STATE_NONE);
			break;
		case STATE_KEY:
			/*key.CloseTree();
			map.OpenTree();*/
			key.KeyToMap();
			SetState(STATE_MAP);
			break;
		case STATE_CONVERSATION:
			conversation.CloseTree();
			SetState(STATE_NONE);
			break;
		}
	}

	void ActionLeft(){
		switch (state){
		case STATE_NONE:
			map.OpenTree();
			SetState(STATE_MAP);
			break;
		case STATE_INVENTORY:
			inventory.DecrementSelection();
			break;
		case STATE_QUESTS:
			quests.CloseTree();
			SetState(STATE_NONE);
			break;
		case STATE_MAP:
			/*map.CloseTree();
			key.OpenTree();*/
			map.MapToKey();
			SetState(STATE_KEY);
			break;
		case STATE_KEY:
			break;
		case STATE_CONVERSATION:
			conversation.CloseTree();
			SetState(STATE_NONE);
			break;
		}
	}

	void ActionUp(){
		switch (state){
		case STATE_NONE:
			conversation.OpenTree();
			SetState(STATE_CONVERSATION);
			break;
		case STATE_INVENTORY:
			inventory.CloseTree();
			SetState(STATE_NONE);
			break;
		case STATE_QUESTS:
			quests.DecrementSelection();
			break;
		case STATE_MAP:
			map.CloseTree();
			SetState(STATE_NONE);
			break;
		case STATE_KEY:
			key.CloseTree();
			SetState(STATE_NONE);
			break;
		case STATE_CONVERSATION:
			conversation.DecrementSelection();
			break;
		}
	}

	void ActionDown(){
		switch (state){
		case STATE_NONE:
			quests.OpenTree();
			SetState(STATE_QUESTS);
			break;
		case STATE_INVENTORY:
			inventory.CloseTree();
			SetState(STATE_NONE);
			break;
		case STATE_QUESTS:
			quests.IncrementSelection();
			break;
		case STATE_MAP:
			map.CloseTree();
			SetState(STATE_NONE);
			break;
		case STATE_KEY:
			key.CloseTree();
			SetState(STATE_NONE);
			break;
		case STATE_CONVERSATION:
			conversation.IncrementSelection();
			break;
		}
	}

	void ActionInventory(){
		switch (state){
		case STATE_NONE:
			inventory.OpenTree();
			SetState(STATE_INVENTORY);
			break;
		case STATE_INVENTORY:
			break;
		case STATE_QUESTS:
			quests.CloseTree();
			inventory.OpenTree();
			SetState(STATE_INVENTORY);
			break;
		case STATE_MAP:
			map.CloseTree();
			inventory.OpenTree();
			SetState(STATE_INVENTORY);
			break;
		case STATE_KEY:
			key.CloseTree();
			inventory.OpenTree();
			SetState(STATE_INVENTORY);
			break;
		case STATE_CONVERSATION:
			conversation.CloseTree();
			inventory.OpenTree();
			SetState(STATE_INVENTORY);
			break;
		}
	}

	void ActionQuests(){
		switch (state){
		case STATE_NONE:
			quests.OpenTree();
			SetState(STATE_QUESTS);
			break;
		case STATE_INVENTORY:
			inventory.CloseTree();
			quests.OpenTree();
			SetState(STATE_QUESTS);
			break;
		case STATE_QUESTS:
			break;
		case STATE_MAP:
			map.CloseTree();
			quests.OpenTree();
			SetState(STATE_QUESTS);
			break;
		case STATE_KEY:
			key.CloseTree();
			quests.OpenTree();
			SetState(STATE_QUESTS);
			break;
		case STATE_CONVERSATION:
			conversation.CloseTree();
			quests.OpenTree();
			SetState(STATE_QUESTS);
			break;
		}
	}

	void ActionConversation(){
		switch (state){
		case STATE_NONE:
			conversation.OpenTree();
			SetState(STATE_CONVERSATION);
			break;
		case STATE_INVENTORY:
			inventory.CloseTree();
			conversation.OpenTree();
			SetState(STATE_CONVERSATION);
			break;
		case STATE_QUESTS:
			quests.CloseTree();
			conversation.OpenTree();
			SetState(STATE_CONVERSATION);
			break;
		case STATE_MAP:
			map.CloseTree();
			conversation.OpenTree();
			SetState(STATE_CONVERSATION);
			break;
		case STATE_KEY:
			key.CloseTree();
			conversation.OpenTree();
			SetState(STATE_CONVERSATION);
			break;
		case STATE_CONVERSATION:
			break;
		}
	}

	void ActionKey(){
		switch (state){
		case STATE_NONE:
			key.OpenTree();
			SetState(STATE_KEY);
			break;
		case STATE_INVENTORY:
			inventory.CloseTree();
			key.OpenTree();
			SetState(STATE_KEY);
			break;
		case STATE_QUESTS:
			quests.CloseTree();
			key.OpenTree();
			SetState(STATE_KEY);
			break;
		case STATE_MAP:
			/*map.CloseTree();
			key.OpenTree();*/
			map.MapToKey();
			SetState(STATE_KEY);
			break;
		case STATE_KEY:
			break;
		case STATE_CONVERSATION:
			conversation.CloseTree();
			key.OpenTree();
			SetState(STATE_KEY);
			break;
		}
	}

	void ActionMap(){
		switch (state){
		case STATE_NONE:
			map.OpenTree();
			SetState(STATE_MAP);
			break;
		case STATE_INVENTORY:
			inventory.CloseTree();
			map.OpenTree();
			SetState(STATE_MAP);
			break;
		case STATE_QUESTS:
			quests.CloseTree();
			map.OpenTree();
			SetState(STATE_MAP);
			break;
		case STATE_MAP:
			break;
		case STATE_KEY:
			/*key.CloseTree();
			map.OpenTree();*/
			key.KeyToMap();
			SetState(STATE_MAP);
			break;
		case STATE_CONVERSATION:
			conversation.CloseTree();
			map.OpenTree();
			SetState(STATE_MAP);
			break;
		}
	}

	void ActionEscape(){
		switch (state){
		case STATE_NONE:
			break;
		case STATE_INVENTORY:
			inventory.CloseTree();
			SetState(STATE_NONE);
			break;
		case STATE_QUESTS:
			quests.CloseTree();
			SetState(STATE_NONE);
			break;
		case STATE_MAP:
			map.CloseTree();
			SetState(STATE_NONE);
			break;
		case STATE_KEY:
			key.CloseTree();
			SetState(STATE_NONE);
			break;
		case STATE_CONVERSATION:
			conversation.CloseTree();
			SetState(STATE_NONE);
			break;
		}
	}

	void ActionReturn(){
		switch (state){
		case STATE_NONE:
			Debug.Break ();
			break;
		case STATE_INVENTORY:
			inventory.UISelect("inventory");
			inventory.CloseTree ();
			SetState(STATE_NONE);
			break;
		case STATE_QUESTS:
			quests.UISelect("quests");
			quests.CloseTree();
			SetState(STATE_NONE);
			break;
		case STATE_MAP:
			break;
		case STATE_KEY:
			break;
		case STATE_CONVERSATION:
			conversation.UISelect("conversation");
			conversation.CloseTree();
			SetState(STATE_NONE);
			break;
		}
	}

	public void StartWorkingManager(){
		working = true;
	}
	public void FinishWorkingManager(){
		working = false;
	}
}
