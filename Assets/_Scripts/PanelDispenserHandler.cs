using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; 

public class PanelDispenserHandler : MonoBehaviour {

    [SerializeField] private List<GameObject> panels;
    public List<string> titles;
    public List<float> salaries;

	[SerializeField] private Text leftTitleText;
	[SerializeField] private Text rightTitleText;

	[SerializeField] private Text leftSalaryText;
	[SerializeField] private Text rightSalaryText;

	// Use this for initialization
	void Start () {
        titles = new List<string>();
        titles.Add("Fast Food Worker");
        titles.Add("Janitor/Custodian");
        titles.Add("Retail Salesperson");
        titles.Add("Public School Teacher");
        titles.Add("Hospital Nurse");
        titles.Add("Software Engineer");
        titles.Add("NFL Player");
        titles.Add("CEO/Executive");
        titles.Add("Mark Zuckerberg");

        salaries = new List<float>();
        salaries.Add(16000);
        salaries.Add(24000);
        salaries.Add(32000);
        salaries.Add(37000);
        salaries.Add(68000);
        salaries.Add(80000);
        salaries.Add(1800000);
        salaries.Add(13800000);
        salaries.Add(11500000000);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void toggleDispensing(MoneyDispenser md) {
        md.toggleDispensing();
    }

    public void toggleComparatorDispensing(GameObject panelItem, MoneyDispenser md) {
        var index = panels.IndexOf(panelItem);
        var salary = salaries[index % 9];
        var title = titles[index % 9];
        
		if (index > 8) {
			leftTitleText.text = title;
			leftSalaryText.text = string.Format("Average Salary: {0:C}", salary);
		}
		else {
			rightTitleText.text = title;
			rightSalaryText.text = string.Format("Average Salary: {0:C}", salary);
		}

        md.restartDispensingWithSalary(salary);
    }

}
