using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroScript : MonoBehaviour {

	public Autowalk walkingScript;
	public PanelDispenserHandler panelDispenserScript;

	public Text introText;
	public Text leftStartText;
	public Text rightStartText;

	public Text continueText;

	public GameObject smallCoin;
	public GameObject smallCoinCluster;
	public GameObject coinCluster;

	private string[] textObjects;

	private int currentTextIndex = 0;

	// Use this for initialization
	void Start () {
		textObjects = new string[]{
			"There are 9 different rooms in this exhibit. Each representing a different profession",
			"In front of each room is a panel. Aim at the panel and tap to start the simulation",
			"3 different unit sizes exist for the coins dropped",
			"The gold coin represents 1 penny",
			"This small cluster of coins represents 25 cents",
			"This large cluster of coins is the equivalent of 2 dollars",
			"There is also a seperate area which allows you to compare different salaries.",
			"There are panels on the left and right sides of the income simulator where salaries can be selected.",
			"Once you dismiss this message, you may tap to toggle walking on and off."
		};
			
		walkingScript.enabled = false;
		panelDispenserScript.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Cardboard.SDK.Triggered) {
			if (currentTextIndex < textObjects.Length) {
				displayText(textObjects[currentTextIndex]);
				currentTextIndex++;
			}
			else {
				introText.enabled = false;

				continueText.enabled = false;
				leftStartText.enabled = true;
				rightStartText.enabled = true;

				walkingScript.enabled = true;
				panelDispenserScript.enabled = true;
			}
		}
	}

	void displayText(string text) {
		introText.text = text;
	}
}
