using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MoneyDispenser : MonoBehaviour
{

	[SerializeField] float salaryAmount = 120000f;

	[SerializeField] GameObject penny;
	[SerializeField] GameObject smallCoinCluster;
	[SerializeField] GameObject largeCoinCluster;

	// What is the maximum x and z that we can spawn the coins in around the central dispensing game object?
	[SerializeField] float containingWidth;
	[SerializeField] float containingLength;

	[SerializeField] GameObject coinContainer;

	[SerializeField] Text earningsText;

	private bool dispensing;

	private float pennyValue = 0.01f;
	private float pennySmallClusterValue = 0.5f;
	private float pennyClusterValue = 20f;

	private float pennyRate = 0f;
	private float pennySmallClusterRate = 0f;
	private float pennyClusterRate = 0f;

	private float timeSinceLastPennySpawn;
	private float timeSinceLastPennySmallClusterSpawn;
	private float timeSinceLastPennyClusterSpawn;

	private float dollarsEarned = 0;

	// rate = coins per minute
	private Dictionary<GameObject, float> coinRate;


	// Use this for initialization
	void Start ()
	{
		dispensing = false;
		timeSinceLastPennySpawn = 0;
		timeSinceLastPennySmallClusterSpawn = 0;
		timeSinceLastPennyClusterSpawn = 0;
		// startDispensing ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (dispensing) {
			timeSinceLastPennySpawn += Time.deltaTime;
			timeSinceLastPennySmallClusterSpawn += Time.deltaTime;
			timeSinceLastPennyClusterSpawn += Time.deltaTime;

			var pennySpawnTime = 1 / pennyRate;
			var pennySmallClusterSpawnTime = 1 / pennySmallClusterRate;
			var pennyClusterSpawnTime = 1 / pennyClusterRate;

			float numberOfPenniesToSpawn = timeSinceLastPennySpawn / pennySpawnTime;
			float numberOfPennySmallClustersToSpawn = timeSinceLastPennySmallClusterSpawn / pennySmallClusterSpawnTime;
			float numberOfPennyClustersToSpawn = timeSinceLastPennyClusterSpawn / pennyClusterSpawnTime;

			spawnItems(penny, numberOfPenniesToSpawn, pennyValue, ref timeSinceLastPennySpawn);
			spawnItems(smallCoinCluster, numberOfPennySmallClustersToSpawn, pennySmallClusterValue, ref timeSinceLastPennySmallClusterSpawn);
			spawnItems(largeCoinCluster, numberOfPennyClustersToSpawn, pennyClusterValue, ref timeSinceLastPennyClusterSpawn);
		}
	}

	private void spawnItems(GameObject item, float numberToSpawn, float value, ref float timeObject) {
		var amountJustEarned = 0f;
		if (numberToSpawn >= 1) {
			timeObject = 0f;
			for (var i = 0; i < Mathf.Round(numberToSpawn); i++) {
				spawnCoinInRandomLocation (item);
				amountJustEarned += value;
			}

			dollarsEarned += amountJustEarned;

			// Debug.Log("Total Dollars Earned " + dollarsEarned + "\nAdditional amount earned " + amountJustEarned);
			if (earningsText) {
				earningsText.text = "Simulation Earnings: $" + dollarsEarned.ToString("F2");
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

	private void calculateCoinRates (float salary)
	{
		var salaryPerWeek = salary / 52.0f;
		var salaryPerDay = salaryPerWeek / 5.0f;
		var penniesPerDay = salaryPerDay / pennyValue;

		var penniesPerMinute = penniesPerDay / 8.0f / 60.0f;
		var penniesPerSecond = penniesPerMinute / 60.0f;

		if (penniesPerSecond < 15) {
			pennyRate = penniesPerSecond;
		}
		else if (penniesPerSecond < 351) {
			pennyRate = .25f;
			penniesPerSecond -= .25f;
			pennySmallClusterRate = penniesPerSecond / (pennySmallClusterValue * 100f);
		}
		else {
			pennyRate = .25f;
			penniesPerSecond -= .25f;
			pennySmallClusterRate = .18f;
			penniesPerSecond -= .18f*pennySmallClusterValue;

			pennyClusterRate = penniesPerSecond / (pennyClusterValue * 100f);

		}

		Debug.Log("Pennies per second: " + pennyRate + "\nSmall Clusters per second: " + pennySmallClusterRate + "\nClusters per second: " + pennyClusterRate);
	}

	private void resetCoinRates ()
	{
		pennyRate = 0;
		pennySmallClusterRate = 0;
		pennyClusterRate = 0;
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
	}

	// Public Methods

	public void restartDispensingWithSalary (float salary)
	{
		if (dispensing) {
			stopDispensing ();
		}
		if (salaryAmount.Equals(salary)) {
			return;
		}
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
		dispensing = true;
	}

	public void stopDispensing ()
	{
		resetCoinRates ();
		removeAllCoins();
		dispensing = false;
	}

	public void removeAllCoins ()
	{
		dollarsEarned = 0f;

		Transform[] childTransforms = coinContainer.GetComponentsInChildren<Transform> ();

		foreach (Transform coin in childTransforms) {
			if ( coin.gameObject.GetInstanceID() != coinContainer.GetInstanceID() ) {
				Destroy (coin.gameObject);
			}
		}
	}
}
