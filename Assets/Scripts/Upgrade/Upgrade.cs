using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// TODO: Liberar mouse quando a UI estiver ativa
public class Upgrade : MonoBehaviour
{
    public class UpgradeType
    {
        private readonly Upgrade upgrade; // Reference to the Upgrade instance
        public readonly string name;
        private float startingValue;
        private readonly float growthBase;
        private readonly bool isInteger;
        public int currentLevel { get; private set; }
        public float currentValue { get; private set; }

        public UpgradeType(string name, float startingValue, float growthBase, bool isInteger, Upgrade upgrade)
        {
            this.name = name;
            this.startingValue = startingValue;
            this.growthBase = growthBase;
            this.isInteger = isInteger;
            this.upgrade = upgrade;
            currentLevel = 1;
            currentValue = startingValue;
        }

        private void LogGrowthFloat()
        {
            currentValue = startingValue + Mathf.Log(currentLevel, growthBase);
            currentValue = Mathf.Round(currentValue * 100) / 100;
        }

        private void LogGrowthInt()
        {
            float aux = startingValue + Mathf.Log(currentLevel, growthBase);
            if (aux - currentValue < 1)
                currentValue++; // Aumenta o valor em 1 se a diferenca for menor que 1
            else
                currentValue = Mathf.Round(aux);
        }

        public void LevelUp()
        {
            currentLevel++;

            // Aumenta o valor logaritimicamente
            if (isInteger)
            {
                LogGrowthInt();
            }
            else
            {
                LogGrowthFloat();
            }

            print(name + " nivel " + currentLevel + " = " + currentValue);

            if (currentLevel == upgrade.levelGreat && upgrade.upgradeTypesOnLevelGreat.Count < 2)
            {
                // Adiciona o tipo de upgrade na lista de upgrades no nivel grande
                print("Nivel grande atingido para " + name);
                if (!upgrade.upgradeTypesOnLevelGreat.Contains(this))
                    upgrade.upgradeTypesOnLevelGreat.Add(this);
            }
        }

        public void SetStartingValue(float value)
        {
            startingValue = value;
            currentValue = value;
        }
    }


    [SerializeField] private int levelGreat = 5; // nivel grande

    [SerializeField] private float startingDamage = 1f;
    [SerializeField] private float startingFireRate = 1f;
    [SerializeField] private int startingAmmo = 10;
    [SerializeField] private float startingHealth = 10f;

    [Header("Base logaritimica")]
    [SerializeField] private float damageGrowthBase = 10f;
    [SerializeField] private float fireRateGrowthBase = 10f;
    [SerializeField] private float ammoGrowthBase = 10f;
    [SerializeField] private float healthGrowthBase = 10f;

    public List<UpgradeType> upgradeTypesOnLevelGreat = new();
    private bool upgradedWeapon = false; // arma atualizada

    public UpgradeType damage;
    public UpgradeType fireRate;
    public UpgradeType ammo;
    public UpgradeType health;

    private void Awake()
    {
        damage = new UpgradeType("Damage", startingDamage, damageGrowthBase, false, this);
        fireRate = new UpgradeType("Fire Rate", startingFireRate, fireRateGrowthBase, false, this);
        ammo = new UpgradeType("Ammo", startingAmmo, ammoGrowthBase, true, this);
        health = new UpgradeType("Health", startingHealth, healthGrowthBase, false, this);
    }

    public void UpgradeWeapon()
    {
        if(upgradedWeapon)
        {
            return;
        }

        if(upgradeTypesOnLevelGreat.Count >= 2)
        {
            List<string> namesOfUpgrades = new();

            for (int i = 0; i < 2; i++)
            {
                namesOfUpgrades[i] = upgradeTypesOnLevelGreat[i].name;
            }

            // chain de if ta feio. tem algum jeito melhor?
            if(namesOfUpgrades.Contains("Damage") && namesOfUpgrades.Contains("Fire Rate"))
            {
                Debug.Log("Dano e Fire Rate");
            }
            else if (namesOfUpgrades.Contains("Ammo") && namesOfUpgrades.Contains("Fire Rate"))
            {
                Debug.Log("Municao e Fire Rate");
            }
            else if (namesOfUpgrades.Contains("Health") && namesOfUpgrades.Contains("Fire Rate"))
            {
                Debug.Log("Vida e Fire Rate");
            }
            else if (namesOfUpgrades.Contains("Damage") && namesOfUpgrades.Contains("Ammo"))
            {
                Debug.Log("Dano e Municao");
            }
            else if (namesOfUpgrades.Contains("Health") && namesOfUpgrades.Contains("Ammo"))
            {
                Debug.Log("Vida e Municao");
            }
            else if (namesOfUpgrades.Contains("Damage") && namesOfUpgrades.Contains("Health"))
            {
                Debug.Log("Dano e Vida");
            }

            upgradedWeapon = true;
        }
    }
}
