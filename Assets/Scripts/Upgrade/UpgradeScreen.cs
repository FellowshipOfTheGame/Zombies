using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WeaponsNS;

public class UpgradeScreen : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject playerObject;
    private Movement playerMovement;
    private WeaponScript playerWeapon;
    private SwitchScript playerSwitch;
    private WeaponInfoStruct weapon;

    [Header("0 = Damage, 1 = Fire Rate, 2 = Ammo, 3 = Health")]

    [SerializeField] private GameObject[] upgradeTypesUI = new GameObject[4];
    [SerializeField] private List<TextMeshProUGUI> upgradeTypesText = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> upgradeTypesLevel = new List<TextMeshProUGUI>();
    [SerializeField] private List<GameObject> upgradeTypesLevelGreatIndicator = new List<GameObject>();

    private Upgrade upgrade;

    private void Start()
    {     
        upgrade = playerObject.GetComponent<Upgrade>();
        playerMovement = playerObject.GetComponent<Movement>();
        playerWeapon = playerObject.GetComponent<WeaponScript>();
        playerSwitch = playerObject.GetComponent<SwitchScript>();

        Debug.Log(playerObject.transform.childCount);

        // child 0 = camera, child 1 = arma
        weapon = playerObject.transform.GetChild(0).GetChild(1).GetComponent<IWeaponDataProvider>().GetWeaponData();

        upgrade.damage.SetStartingValue(weapon.damage);
        upgrade.fireRate.SetStartingValue(weapon.fireRate);
        upgrade.ammo.SetStartingValue(weapon.magSize);
        // provisorio, ate botar no script onde ta a vida
        upgrade.health.SetStartingValue(100);

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

        if(canvas.activeSelf)
            canvas.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            bool pausing = !canvas.activeSelf; // verdadeiro se estiver ativando o menu
            canvas.SetActive(pausing);
            Time.timeScale = pausing ? 0 : 1;
            playerMovement.enabled = !pausing;
            playerWeapon.enabled = !pausing;
            playerSwitch.enabled = !pausing;

            //libera o cursor
            Cursor.lockState = pausing ? CursorLockMode.None : CursorLockMode.Locked;

            //mostra o cursor
            Cursor.visible = pausing;
        }
    }

    public void UpgradeDamage()
    {
        // Arrumar: Dano da arma é em int, mas o upgrade é em float
        upgrade.damage.LevelUp();

        upgradeTypesText[0].text = upgrade.damage.currentValue.ToString();
        upgradeTypesLevel[0].text = "Nível " + upgrade.damage.currentLevel;
        if (upgrade.upgradeTypesOnLevelGreat.Contains(upgrade.damage))
            upgradeTypesLevelGreatIndicator[0].SetActive(true);

        weapon.damage = (int)upgrade.damage.currentValue;
    }

    public void UpgradeFireRate()
    {
        upgrade.fireRate.LevelUp();

        upgradeTypesText[1].text = upgrade.fireRate.currentValue.ToString();
        upgradeTypesLevel[1].text = "Nível " + upgrade.fireRate.currentLevel;
        if (upgrade.upgradeTypesOnLevelGreat.Contains(upgrade.fireRate))
            upgradeTypesLevelGreatIndicator[1].SetActive(true);

        weapon.fireRate = upgrade.fireRate.currentValue;
    }

    public void UpgradeAmmo()
    {
        upgrade.ammo.LevelUp();

        upgradeTypesText[2].text = upgrade.ammo.currentValue.ToString();
        upgradeTypesLevel[2].text = "Nível " + upgrade.ammo.currentLevel;
        if (upgrade.upgradeTypesOnLevelGreat.Contains(upgrade.ammo))
            upgradeTypesLevelGreatIndicator[2].SetActive(true);

        weapon.magSize = (int)upgrade.ammo.currentValue;
        weapon.ammo = weapon.magSize;
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
