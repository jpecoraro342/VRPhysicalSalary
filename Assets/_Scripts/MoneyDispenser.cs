using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoneyDispenser : MonoBehaviour
{

	[SerializeField] float salaryAmount = 120000f;

	[SerializeField] GameObject penny;
	[SerializeField] GameObject nickel;
	[SerializeField] GameObject dime;
	[SerializeField] GameObject quarter;
	[SerializeField] GameObject coinCluster;

	// What is the maximum x and z that we can spawn the coins in around the central dispensing game object?
	[SerializeField] float containingWidth;
	[SerializeField] float containingLength;

	[SerializeField] GameObject coinContainer;

	private bool dispensing;

	private float pennyValue = 0.01f;
	private float nickelValue = 0.05f;
	private float dimeValue = 0.1f;
	private float quarterValue = 0.25f;

	private Coroutine pennyCoroutine;
	private Coroutine nickelCoroutine;
	private Coroutine dimeCoroutine;
	private Coroutine quarterCoroutine;

	private float timeSinceLastSpawn;

	private float dollarsEarned = 0;

	// rate = coins per minute
	private Dictionary<GameObject, float> coinRate;


	// Use this for initialization
	void Start ()
	{
		coinRate = new Dictionary<GameObject, float> () { 
			{ penny , 0 },
			{ nickel , 0 },
			{ dime , 0 },
			{ quarter , 0 }
		};

		dispensing = false;
		timeSinceLastSpawn = 0;
		startDispensing ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (dispensing) {
			timeSinceLastSpawn += Time.deltaTime;
			var pennySpawnTime = 60 / coinRate [penny];

			float numberOfItemsToSpawn = timeSinceLastSpawn / pennySpawnTime;
			if (numberOfItemsToSpawn >= 100) {
				timeSinceLastSpawn = 0;
				for (var i = 0; i < Mathf.Round(numberOfItemsToSpawn/100); i++) {
					spawnCoinInRandomLocation (coinCluster);
					dollarsEarned += 1f;
					Debug.Log("Dollars Earned " + dollarsEarned);
				}
			}
			else if (numberOfItemsToSpawn >= 1) {
				// Debug.Log("Time since last spawn = " + timeSinceLastSpawn + " Spawning " + numberOfItemsToSpawn + " pennies");
				timeSinceLastSpawn = 0;
				for (var i = 0; i < Mathf.Round(numberOfItemsToSpawn); i++) {
					spawnCoinInRandomLocation (penny);
					dollarsEarned += 0.01f;
					Debug.Log("Dollars Earned " + dollarsEarned);
				}
			}
		}
	}

	private IEnumerator runDemo ()
	{
		startDispensing ();

		yield return new WaitForSeconds (5);

		restartDispensingWithSalary (50000f);

		yield return new WaitForSeconds (10);

		stopDispensing ();
		removeAllCoins ();
	}

	private IEnumerator spawnCoinAtRate (GameObject coin, float rate)
	{
		if (rate == 0) {
			yield break;
		}

		var waitTime = 60 / rate;

		while (true) {
			yield return new WaitForSeconds (waitTime);
			spawnCoinInRandomLocation (coin);
		}
	}

	private void calculateCoinRates (float salary)
	{
		var salaryPerWeek = salary / 52.0f;
		var salaryPerDay = salaryPerWeek / 5.0f;
		var penniesPerDay = salaryPerDay / pennyValue;

		var penniesPerMinute = penniesPerDay / 8.0f / 60.0f;
		setCoinRate (penny, penniesPerMinute);

		Debug.Log ("Pennies per minute = " + penniesPerMinute);

		// TODO: Set this up for a variety of coins, not just pennies
	}

	private void resetCoinRates ()
	{
		setCoinRate (penny, 0f);
		setCoinRate (nickel, 0f);
		setCoinRate (dime, 0f);
		setCoinRate (quarter, 0f);
	}

	private void setCoinRate (GameObject coin, float rate)
	{
		coinRate [coin] = rate;
	}

	private void startSpawningAllCoinObjects ()
	{
		pennyCoroutine = StartCoroutine (spawnCoinAtRate (penny, coinRate [penny]));
		nickelCoroutine = StartCoroutine (spawnCoinAtRate (nickel, coinRate [nickel]));
		dimeCoroutine = StartCoroutine (spawnCoinAtRate (dime, coinRate [dime]));
		quarterCoroutine = StartCoroutine (spawnCoinAtRate (quarter, coinRate [quarter]));
	}

	private void spawnCoinInRandomLocation (GameObject coin)
	{
		var location = Vector3.zero;

		var maxX = containingWidth / 2.0f;
		var minX = -1 * maxX;

		var maxZ = containingLength / 2.0f;
		var minZ = -1 * maxZ;

		// set x and z to random numbers within location bounds
		location.x = Random.Range (minX, maxX);
		location.z = Random.Range (minZ, maxZ);

		spawnCoinAtLocation (coin, location);
	}

	private void spawnCoinAtLocation (GameObject coin, Vector3 location)
	{
		var eulerAngles = new Vector3(90, 0, 90);
		var newCoin = Instantiate (coin, location, Quaternion.identity) as GameObject;
		newCoin.transform.parent = coinContainer.transform;
		newCoin.transform.localPosition = location;

		newCoin.transform.localEulerAngles = eulerAngles;
		Debug.Log(newCoin.transform.rotation);
	}

	private void StopAllCoroutines ()
	{

		if (pennyCoroutine != null) {
			StopCoroutine (pennyCoroutine);
		}

		if (nickelCoroutine != null) {
			StopCoroutine (nickelCoroutine);
		}

		if (dimeCoroutine != null) {
			StopCoroutine (dimeCoroutine);
		}

		if (quarterCoroutine != null) {
			StopCoroutine (quarterCoroutine);
		}
	}

	// Public Methods

	public void restartDispensingWithSalary (float salary)
	{
		stopDispensing ();
		salaryAmount = salary;
		startDispensing ();
	}

	public void toggleDispensing ()
	{
		if (dispensing) {
			stopDispensing ();
		} else {
			startDispensing ();
		}
	}

	public void startDispensing ()
	{
		calculateCoinRates (salaryAmount);
		// startSpawningAllCoinObjects();

		dispensing = true;
	}

	public void stopDispensing ()
	{
		resetCoinRates ();
		// StopAllCoroutines();

		dispensing = false;
	}

	public void removeAllCoins ()
	{
		Transform[] childTransforms = coinContainer.GetComponentsInChildren<Transform> ();

		foreach (Transform coin in childTransforms) {
			Destroy (coin.gameObject);
		}
	}
}
