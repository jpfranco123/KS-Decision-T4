using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


// This Script (a component of Game Manager) Initializes the Borad (i.e. screen).
public class BoardManager : MonoBehaviour {

	//Resoultion width and Height
	//CAUTION! Modifying this does not modify the Screen resolution. This is related to the unit grid on Unity.
	public static int resolutionWidth = 800;
	public static int resolutionHeight = 600;

	//Number of Columns and rows of the grid (the possible positions of the items).
	public static int columns = 16;
	public static int rows = 12;

	//The item radius. This is used to avoid superposition of items.
	//public static float KSItemRadius = 1.5f;

	//Timer width
	//public static float timerWidth =400;

	//The method to be used to place items randomly on the grid.
	//1. Choose random positions from full grid. It might happen that no placement is found and the trial will be skipped.
	//2. Choose randomly out of 10 positions. A placement is guaranteed
	public static int randomPlacementType =1;

	//Prefab of the item interface configuration
	public static GameObject KSItemPrefab;

	//A canvas where all the board is going to be placed
	private GameObject canvas;

	//The possible positions of the items;
	private List <Vector3> gridPositions = new List<Vector3> ();

	//Weights and value vectors for this trial. CURRENTLY SET UP TO ALLOW ONLY INTEGERS.
	//ws and vs must be of the same length
	private int[] ws;
	private int[] vs;

	//If randomization of buttons:
	//1: No/Yes 0: Yes/No
	public static int randomYes;//=Random.Range(0,2);

	//The answer Input by the player
	//0:No / 1:Yes / 2:None
	public static int answer;

	private String question;

	//Should the key be working?
	public static bool keysON = false;

	//These variables shouldn't be modified. They just state that the area of the value part of the item and the weight part are assumed to be 1.
	private static float minAreaBill = 1f;
	private static float minAreaWeight = 1f;

	//The total area of all the items. Separated by the value part and the weighy part. A good initialization for this variables is the number of items plus 1.
	public static int totalAreaBill = 8;
	public static int totalAreaWeight = 8;


	//Structure with the relevant parameters of an item.
	//gameItem: is the game object
	//coorValue1: The coordinates of one of the corners of the encompassing rectangle of the Value Part of the Item. The coordinates are taken relative to the center of the item.
	//coorValue2: The coordinates of the diagonally opposite corner of the Value Part of the Item.
	//coordWeight1 and coordWeight2: Same as before but for the weight part of the item.
	private struct Item
	{
		public GameObject gameItem;
		public Vector2 coordValue1;
		public Vector2 coordValue2;
		public Vector2 coordWeight1;
		public Vector2 coordWeight2;
		public Vector2 center;
	}

	//The items for the scene are stored here.
	private static Item[] items;



	//This Initializes the GridPositions which are the possible places where the items will be placed.
	void InitialiseList()
	{
		gridPositions.Clear ();

		if (randomPlacementType == 1) {
			//"Completely-Random" Grid
			for (int x = -1; x < columns + 2; x++) {
				for (int y = -1; y < rows + 2; y++) {	
					float xUnit = (float)(resolutionWidth / 100) / columns;
					float yUnit = (float)(resolutionHeight / 100) / rows;
					gridPositions.Add (new Vector3 (x * xUnit, y * yUnit, 0f));
				}
			}
		} else if (randomPlacementType == 2){
			//Simple 10 positions grid. 
			for ( int y =1; y<rows+1;y=y+5)
			{
				if (y == 6) {
					for(int x=2;x<columns+1;x=x+12)
					{	
						float xUnit =(float) (resolutionWidth / 100)/columns;
						float yUnit =(float) (resolutionHeight / 100)/rows;
						gridPositions.Add(new Vector3(x*xUnit,y*yUnit,0f));
						//Debug.Log ("x" + x + " y" + y);
					}
				} else {
					for(int x=1;x<columns+1;x=x+5)
					{	
						float xUnit =(float) (resolutionWidth / 100)/columns;
						float yUnit =(float) (resolutionHeight / 100)/rows;
						gridPositions.Add(new Vector3(x*xUnit,y*yUnit,0f));
						//Debug.Log ("x" + x + " y" + y);
					}
				}
			}
		}


	}

	//Call only for visualizing grid in the Canvas.
	void seeGrid(){
		GameObject hangerpref = (GameObject)Resources.Load ("Hanger");
		for (int ss=0;ss<gridPositions.Count;ss++){
			GameObject hanger = Instantiate (hangerpref, gridPositions[ss], Quaternion.identity) as GameObject;
			canvas=GameObject.Find("Canvas");
			hanger.transform.SetParent (canvas.GetComponent<Transform> (),false);
			hanger.transform.position = gridPositions[ss];
		}
	}

	//Randomizes YES/NO button positions (left or right) and allocates corresponding script to save the correspondent answer.
	void RandomizeButtons(){
		Button btnLeft = GameObject.Find("LEFTbutton").GetComponent<Button>();
		Button btnRight = GameObject.Find("RIGHTbutton").GetComponent<Button>();

		randomYes=GameManager.buttonRandomization[GameManager.trial-1];

		if (randomYes == 1) {
			btnLeft.GetComponentInChildren<Text>().text = "No";
			btnRight.GetComponentInChildren<Text>().text = "Yes";
			//btnLeft.onClick.AddListener(()=>GameManager.changeToNextScene(0));
		} else {
			btnLeft.GetComponentInChildren<Text>().text = "Yes";
			btnRight.GetComponentInChildren<Text>().text = "No";
		}
	}

	//Initializes the instance for this trial:
	//1. Sets the question string using the instance (from the .txt files)
	//2. The weight and value vectors are uploaded
	//3. The instance prefab is uploaded
	void setKSInstance(){
		int randInstance = GameManager.instanceRandomization[GameManager.TotalTrials-1];

//		Text Quest = GameObject.Find("Question").GetComponent<Text>();
//		String question = "Can you obtain at least $" + GameManager.ksinstances[randInstance].profit + " with at most " + GameManager.ksinstances[randInstance].capacity +"kg?";
//		Quest.text = question;

		//question = "Can you pack $" + GameManager.ksinstances[randInstance].profit + " if your capacity is " + GameManager.ksinstances[randInstance].capacity +"kg?";
		question = "$" + GameManager.ksinstances[randInstance].profit + System.Environment.NewLine + GameManager.ksinstances[randInstance].capacity +"kg?";

		ws = GameManager.ksinstances [randInstance].weights;
		vs = GameManager.ksinstances [randInstance].values;

		KSItemPrefab = (GameObject)Resources.Load ("KSItem3");

	}

	//Shows the question on the screen
	public void setQuestion(){
		Text Quest = GameObject.Find("Question").GetComponent<Text>();
		Quest.text = question;
	}


	/// <summary>
	/// Instantiates an Item and places it on the position from the input
	/// </summary>
	/// <returns>The item structure</returns>
	/// The item placing here is temporary; The real placing is done by the placeItem() method.
	Item generateItem(int itemNumber ,Vector3 randomPosition){

		//Instantiates the item and places it.
		GameObject instance = Instantiate (KSItemPrefab, randomPosition, Quaternion.identity) as GameObject;

		canvas=GameObject.Find("Canvas");
		instance.transform.SetParent (canvas.GetComponent<Transform> (),false);

		//Setting the position in a separate line is importatant in order to set it according to global coordinates.
		instance.transform.position = randomPosition;

		//instance.GetComponentInChildren<Text>().text = ws[itemNumber]+ "Kg \n $" + vs[itemNumber];

		//Gets the subcomponents of the item 
		GameObject bill = instance.transform.Find("Bill").gameObject;
		GameObject weight = instance.transform.Find("Weight").gameObject;

		//Sets the Text of the items
		bill.GetComponentInChildren<Text>().text = "$" + vs[itemNumber];
		weight.GetComponentInChildren<Text>().text = "" + ws[itemNumber]+ "kg";

		// This calculates area accrding to approach 1
//		float areaItem1 = minAreaBill + (totalAreaBill - vs.Length * minAreaBill) * vs [itemNumber] / vs.Sum ();
//		float scale1 = Convert.ToSingle (Math.Sqrt (areaItem1) - 1);
//		bill.transform.localScale += new Vector3 (scale1, scale1, 0);
//		float areaItem2 = minAreaWeight + (totalAreaWeight - ws.Length * minAreaWeight) * ws [itemNumber] / ws.Sum ();
//		float scale2 = Convert.ToSingle (Math.Sqrt (areaItem2) - 1);
//		weight.transform.localScale += new Vector3 (scale2, scale2, 0);

		// Calculates the area of the Value and Weight sections of the item accrding to approach 2 and then Scales the sections so they match the corresponding area.
		//Area Approach 2 calculation general idea:
		//The total area is constant. The area is divided among the items propotional to the ratio between the value (weight) and the sum of all the values (weights) of the items. 
		//Afterwards a constant area is substracted (or added) from all items in order to make the area of the minimum item equal to the minimum area defined, mantianing the total area constant.
		// Equations: 1. area_i = c + (totalArea-numberOfItems*c)*(value_i/sum(value_i)) 2. min(area_i)=minimumAreaDefined
		float adjustmentBill = (minAreaBill - totalAreaBill * vs.Min () / vs.Sum ()) / (1 - vs.Length * vs.Min () / vs.Sum ());
		float areaItem1 = adjustmentBill + (totalAreaBill - vs.Length * adjustmentBill) * vs [itemNumber] / vs.Sum ();
		float scale1 = Convert.ToSingle (Math.Sqrt (areaItem1) - 1);
		bill.transform.localScale += new Vector3 (scale1, scale1, 0);

		float adjustmentWeight = (minAreaWeight - totalAreaWeight * ws.Min () / ws.Sum ()) / (1 - ws.Length * ws.Min () / ws.Sum ());
		float areaItem2 = adjustmentWeight + (totalAreaWeight - ws.Length * adjustmentWeight) * ws [itemNumber] / ws.Sum ();
		float scale2 = Convert.ToSingle (Math.Sqrt (areaItem2) - 1);
		weight.transform.localScale += new Vector3 (scale2, scale2, 0);
			
		//Using the scaling results it calculates the coordinates (with respect to the center of the item) of the item.
//		float weightH = weight.GetComponent<BoxCollider2D> ().size.y;
//		float weightW = weight.GetComponent<BoxCollider2D> ().size.x;
//		float valueH = bill.GetComponent<BoxCollider2D> ().size.y;
//		float valueW = bill.GetComponent<BoxCollider2D> ().size.x;
		float weightH = weight.GetComponent<BoxCollider2D> ().size.y*weight.transform.localScale.y;
		float weightW = weight.GetComponent<BoxCollider2D> ().size.x*weight.transform.localScale.x;
		float valueH = bill.GetComponent<BoxCollider2D> ().size.y*bill.transform.localScale.y;
		float valueW = bill.GetComponent<BoxCollider2D> ().size.x*bill.transform.localScale.x;

		Item itemInstance = new Item();
		itemInstance.gameItem=instance;
//		itemInstance.coordValue1=new Vector2(-valueW*(1+scale1)/2,0);
//		itemInstance.coordValue2=new Vector2(valueW*(1+scale1)/2,valueH*(1+scale1));
//		itemInstance.coordWeight1=new Vector2(-weightW*(1+scale2)/2,0);
//		itemInstance.coordWeight2=new Vector2(weightW*(1+scale2)/2,-weightH*(1+scale2));

		itemInstance.coordValue1=new Vector2(-valueW/2,0);
		itemInstance.coordValue2=new Vector2(valueW/2,valueH);
		itemInstance.coordWeight1=new Vector2(-weightW/2,0);
		itemInstance.coordWeight2=new Vector2(weightW/2,-weightH);



		return(itemInstance);

	}

	/// <summary>
	/// Places the item on the input position
	/// </summary>
	void placeItem(Item itemToLocate, Vector3 position){
		//Setting the position in a separate line is importatant in order to set it according to global coordinates.
		itemToLocate.gameItem.transform.position = position;
	}


	//Returns a random position from the grid and removes the item from the list.
	Vector3 RandomPosition()
	{
		int randomIndex=Random.Range(0,gridPositions.Count);
		Vector3 randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt(randomIndex);
		return randomPosition;
	}

	// Places all the objects from the instance (ws,vs) on the canvas. 
	// Returns TRUE if all items where positioned, FALSE otherwise.
	private bool LayoutObjectAtRandom()
	{
		int objectCount =ws.Length;
		items= new Item[objectCount];
		for(int i=0; i < objectCount;i++)
		{
			int objectPositioned = 0;
			Item itemToLocate = generateItem (i, new Vector3 (-1000,-1000,-1000));//66: Change to different Layer?
			while (objectPositioned == 0) 
			{
				if (gridPositions.Count > 0) {
					Vector3 randomPosition = RandomPosition ();

					if (!objectOverlapsQ (randomPosition,itemToLocate)) {
						placeItem (itemToLocate, randomPosition);
						itemToLocate.center = new Vector2(randomPosition.x,randomPosition.y);
						items [i] = itemToLocate;
						objectPositioned = 1;
					} 
				}
				else{
					//Debug.Log ("Not enough space to place all items");
					return false;
				}
			}

		}
		return true;
	}

	/// Macro function that initializes the Board
	/// 1: Trial / 2: trial game answer
	public void SetupScene(string sceneToSetup)
	{

		if (sceneToSetup == "Trial") {
			//InitialiseList();
			setKSInstance ();
			setQuestion ();
			keysON = true;

			//If the bool returned by LayoutObjectAtRandom() is false, then retry again:
			//Destroy all items. Initialize list again and try to place them once more.
			int nt=200;
			bool itemsPlaced = false;
			while (nt >= 1 && !itemsPlaced) {

				GameObject[] items1 = GameObject.FindGameObjectsWithTag("Item");
				foreach (GameObject item in items1)
				{
					Destroy(item);
				}

//				foreach (Item item in items)
//				{
//					Destroy(item.gameItem);
//				}

				InitialiseList ();
				//seeGrid();
				itemsPlaced = LayoutObjectAtRandom ();
				nt--;
				Debug.Log (nt);
			}
			if (itemsPlaced == false) {
				GameManager.errorInScene ("Not enough space to place all items");
			}

		} else if(sceneToSetup == "TrialAnswer"){
			
			answer = 2;
			setKSInstance ();
			RandomizeButtons ();
			keysON = true;

			//1234
//			InitialiseList ();
//			seeGrid();
		}

	}

//	//Checks if positioning an item in the new position generates an overlap. Assuming the new item has a radius of KSITemRadius.
//	//Returns: TRUE if there is an overlap. FALSE Otherwise.
//	bool objectOverlapsQ(Vector3 pos)
//	{
//		//If physics could be started before update we could use the following easier function:
//		//bool overlap = Physics2D.IsTouchingLayers(newObject.GetComponent<Collider2D>());
//
//		bool overlap = Physics2D.OverlapCircle(pos,KSItemRadius);
//		return overlap;
//
//	}

	//Checks if positioning an item in the new position generates an overlap.
	//Returns: TRUE if there is an overlap. FALSE Otherwise.
	bool objectOverlapsQ(Vector3 pos, Item item)
	{
		Vector2 posxy = new Vector3 (pos.x, pos.y);
		bool overlapValue = Physics2D.OverlapArea (item.coordValue1+posxy, item.coordValue2+posxy);
		bool overlapWeight = Physics2D.OverlapArea (item.coordWeight1+posxy, item.coordWeight2+posxy);

		//Debug.Log ("Item");
		//Debug.Log(item.coordValue1 + posxy);
		//Debug.Log(item.coordValue2+posxy);
		//1234
		return overlapValue || overlapWeight;
        //return false;
	}

	//Updates the timer rectangle size accoriding to the remaining time.
	public void updateTimer(){
		// timer = GameObject.Find ("Timer").GetComponent<RectTransform> ();
		// timer.sizeDelta = new Vector2 (timerWidth * (GameManager.tiempo / GameManager.totalTime), timer.rect.height);
		Image timer = GameObject.Find ("Timer").GetComponent<Image> ();
		timer.fillAmount = GameManager.tiempo / GameManager.totalTime;
	}

	//Sets the triggers for pressing the corresponding keys
	//123: Perhaps a good practice thing to do would be to create a "close scene" function that takes as parameter the answer and closes everything (including keysON=false) and then forwards to 
	//changeToNextScene(answer) on game manager
	private void setKeyInput(){

		if (GameManager.escena == "Trial") {
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				GameManager.saveTimeStamp("ParticipantSkip");
				GameManager.changeToNextScene (0,0);
			}
		}
		else if (GameManager.escena == "TrialAnswer") 
		{
			//1: No/Yes 0: Yes/No
			if (randomYes == 1) {
				if (Input.GetKeyDown (KeyCode.LeftArrow)) {
					//Left
					//GameManager.changeToNextScene (0, randomYes);
					keysON = false;
					answer=0;
					GameObject boto = GameObject.Find("LEFTbutton") as GameObject;
					highlightButton(boto);
					GameManager.changeToNextScene (0,1);
					} 
				else if (Input.GetKeyDown (KeyCode.RightArrow)) {
					//Right
					//GameManager.changeToNextScene (1, randomYes);
					keysON = false;
					answer=1;
					GameObject boto = GameObject.Find("RIGHTbutton") as GameObject;
					highlightButton(boto);
					GameManager.changeToNextScene (1,1);
					}
			} 
			else if (randomYes == 0) {
				if (Input.GetKeyDown (KeyCode.LeftArrow)) {
					//Left
					//GameManager.changeToNextScene (1, randomYes);
					keysON = false;
					answer=1;
					GameObject boto = GameObject.Find("LEFTbutton") as GameObject;
					highlightButton(boto);
					GameManager.changeToNextScene (1,0);
					} 
				else if (Input.GetKeyDown (KeyCode.RightArrow)) {
					//Right
					//GameManager.changeToNextScene (0, randomYes);
					keysON = false;
					answer = 0;
					GameObject boto = GameObject.Find("RIGHTbutton") as GameObject;
					highlightButton(boto);
					GameManager.changeToNextScene (0,0);
				}
			}
		} 
		else if (GameManager.escena == "SetUp") {
			if (Input.GetKeyDown (KeyCode.Space)) {
				GameManager.setTimeStamp ();
				GameManager.changeToNextScene (0,0);
			}
		}
	}

	private void highlightButton(GameObject butt){
		Text texto = butt.GetComponentInChildren<Text> ();
		texto.color = Color.gray;


	}

	public void setupInitialScreen(){

		//Button 
		GameObject start = GameObject.Find("Start") as GameObject;
		start.SetActive (false);

		//start.btnLeft.GetComponentInChildren<Text>().text = "No";

		InputField pID = GameObject.Find ("ParticipantID").GetComponent<InputField>();

		InputField.SubmitEvent se = new InputField.SubmitEvent();
		//se.AddListener(submitPID(start));
		se.AddListener((value)=>submitPID(value,start));
		pID.onEndEdit = se;


		//pID.onSubmit.AddListener((value) => submitPID(value));

	}

	private void submitPID(string pIDs, GameObject start){

		//Debug.Log (pIDs);

		GameObject pID = GameObject.Find ("ParticipantID");
		pID.SetActive (false);
		GameObject pIDText = GameObject.Find ("ParticipantIDText");
		pIDText.SetActive (false);

		//Set Participant ID
		GameManager.participantID=pIDs;

		//Activate Start Button and listener
		//GameObject start = GameObject.Find("Start");
		start.SetActive (true);
		keysON = true;

	}

	public static string getItemCoordinates(){
		string coordinates = "";
		foreach (Item it in items)
		{
			Debug.Log ("item");
			Debug.Log (it.center);
			Debug.Log (it.coordWeight1);
			coordinates = coordinates + "(" + it.center.x + "," + it.center.y + ")";
		}
		return coordinates;
	}

	// Use this for initialization
	void Start () {
		//GameManager.saveTimeStamp(GameManager.escena);
	}

	// Update is called once per frame
	void Update () {
		
		if (keysON) {
			setKeyInput ();
		}

	}



}