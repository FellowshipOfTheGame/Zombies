using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeScreen : MonoBehaviour
{
    [Header("0 = Damage, 1 = Fire Rate, 2 = Ammo, 3 = Health")]

    [SerializeField] private GameObject[] upgradeTypesUI = new GameObject[4];
    [SerializeField] private List<TextMeshProUGUI> upgradeTypesText = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> upgradeTypesLevel = new List<TextMeshProUGUI>();
    [SerializeField] private List<GameObject> upgradeTypesLevelGreatIndicator = new List<GameObject>();

    private Upgrade upgrade;

    private void Start()
    {     
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        upgrade = playerObject.GetComponent<Upgrade>();

        for (int i = 0; i < upgradeTypesUI.Length; i++)
        {
            GameObject upgradeType = upgradeTypesUI[i];
            GameObject upgradeTypeValue = upgradeType.transform.Find("Valor").gameObject;
            GameObject upgradeTypeLevel = upgradeType.transform.Find("Nivel").gameObject;
            GameObject upgradeTypeLevelGreatIndicator = upgradeType.transform.Find("Nivel5").gameObject;

            if (!upgradeTypeValue || !upgradeTypeLevel || !upgradeTypeLevelGreatIndicator)
                Debug.LogError("Upgrade type UI not found");

            upgradeTypesText.Add(upgradeTypeValue.GetComponent<TextMeshProUGUI>());
            upgradeTypesLevel.Add(upgradeTypeLevel.GetComponent<TextMeshProUGUI>());
            upgradeTypesLevelGreatIndicator.Add(upgradeTypeLevelGreatIndicator);
        }

        upgradeTypesText[0].text = upgrade.damage.currentValue.ToString();
        upgradeTypesLevel[0].text = "Nível " + upgrade.damage.currentLevel;
        upgradeTypesText[1].text = upgrade.fireRate.currentValue.ToString();
        upgradeTypesLevel[1].text = "Nível " + upgrade.fireRate.currentLevel;
        upgradeTypesText[2].text = upgrade.ammo.currentValue.ToString();
        upgradeTypesLevel[2].text = "Nível " + upgrade.ammo.currentLevel;
        upgradeTypesText[3].text = upgrade.health.currentValue.ToString();
        upgradeTypesLevel[3].text = "Nível " + upgrade.health.currentLevel;
    }

    public void UpgradeDamage()
    {
        upgrade.damage.LevelUp();
        upgradeTypesText[0].text = upgrade.damage.currentValue.ToString();
        upgradeTypesLevel[0].text = "Nível " + upgrade.damage.currentLevel;
        if (upgrade.upgradeTypesOnLevelGreat.Contains(upgrade.damage))
            upgradeTypesLevelGreatIndicator[0].SetActive(true);
    }

    public void UpgradeFireRate()
    {
        upgrade.fireRate.LevelUp();
        upgradeTypesText[1].text = upgrade.fireRate.currentValue.ToString();
        upgradeTypesLevel[1].text = "Nível " + upgrade.fireRate.currentLevel;
        if (upgrade.upgradeTypesOnLevelGreat.Contains(upgrade.fireRate))
            upgradeTypesLevelGreatIndicator[1].SetActive(true);
    }

    public void UpgradeAmmo()
    {
        upgrade.ammo.LevelUp();
        upgradeTypesText[2].text = upgrade.ammo.currentValue.ToString();
        upgradeTypesLevel[2].text = "Nível " + upgrade.ammo.currentLevel;
        if (upgrade.upgradeTypesOnLevelGreat.Contains(upgrade.ammo))
            upgradeTypesLevelGreatIndicator[2].SetActive(true);
    }

    public void UpgradeHealth()
    {
        upgrade.health.LevelUp();
        upgradeTypesText[3].text = upgrade.health.currentValue.ToString();
        upgradeTypesLevel[3].text = "Nível " + upgrade.health.currentLevel;
        if (upgrade.upgradeTypesOnLevelGreat.Contains(upgrade.health))
            upgradeTypesLevelGreatIndicator[3].SetActive(true);
    }
}
