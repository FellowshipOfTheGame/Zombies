using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerHealth : Health
{
     [Header("Variables")]
     [SerializeField] private float regenPerSecond = 1f;
     [SerializeField] private float regenIncreaseRate = 0.25f;
     [SerializeField] private float regenTarget = 10f;
     [SerializeField] private float regenDelayTime = 4f;
     
     [Header("Declarations")]
     private PlayerHUD playerHUD;
     private Coroutine regenC;
     
     [Header("Effects")] // text update to unique effects / effects that cannot repeat / that cannot overwrite each other
     private string bleedPrefix;
     private TextMeshProUGUI bleedEntry;
     private TextMeshProUGUI poisonEntry;
     
     
     protected override void Start()
     {
          base.Start();
          playerHUD = GetComponent<PlayerHUD>();
          playerHUD.Health(health);
     }
     
     private void Update()
     {
          if (Input.GetKeyDown(KeyCode.Minus)) TakeDamage(5, transform.position, transform, Color.white);
          if (Input.GetKeyDown(KeyCode.Equals)) AddShield(10);
          if (Input.GetKeyDown(KeyCode.Alpha0)) DamageOverTime(5, 5f, transform, new Color(0.8f, 0.3f, 0.9f));
          if (Input.GetKeyDown(KeyCode.Alpha9)) BleedDamage(25, 0.75f, transform, new Color(1.0f, 0.1f, 0.1f));
          if (Input.GetKeyDown(KeyCode.Alpha8)) Hemorrhage(25, 5f, transform, new Color(.69f, 0.0f, 0.0f));
          if (Input.GetKeyDown(KeyCode.Alpha7)) DelayedDamage(10,1f, transform, new Color(1.0f, 1.0f, 0.1f));
          if (Input.GetKeyDown(KeyCode.Alpha6)) PoisonDamage(25, 0.75f, transform, new Color(0.1f, 1.0f, 0.1f));
          if (Input.GetKeyDown(KeyCode.Alpha5)) HealingOverTime(5,5f, transform, new Color(0, .69f, 0));
          if (Input.GetKeyDown(KeyCode.Alpha4)) ShieldOverTime(5,5f, transform, Color.white);
     }
     private IEnumerator RegenDelay()
     {
          yield return new WaitForSeconds(regenDelayTime);
          regenC = StartCoroutine(RegenHealth());
     }
     
     private IEnumerator RegenHealth()
     {
          float currentRegenPerSecond = regenPerSecond;
          float regenTickTime = 1/currentRegenPerSecond;
          while (health < maxHealth) // vida++ com ticks de tempo cada vez menores ate que chegue na vida maxima
          {
               ++health; playerHUD.Health(health);
               yield return new WaitForSeconds(regenTickTime);
               currentRegenPerSecond += regenIncreaseRate * (1- Mathf.Pow(currentRegenPerSecond/regenTarget, 2) );
               regenTickTime = 1 / currentRegenPerSecond;
          }
          regenC = null;
     }
     
     
     // ////////////////////////////// //
     // Overwrites for HUD integration //
     // ////////////////////////////// //
     
     public override void AddShield(int addShield)
     {
          shield += addShield;
          playerHUD.ShowShield();
          playerHUD.Shield(shield);
     }
     
     
     public override void TakeDamage(int damage, Vector3 hitPosition, Transform playerCamera, Color textColor)
     {
          if (damage < shield)
          {
               shield -= damage;
               FloatingDamage(damage, hitPosition, playerCamera, 0.75f*Color.white);
               playerHUD.Shield(shield);
          }
          else if (damage == shield)
          {
               shield = 0;
               playerHUD.HideShield();
               FloatingDamage(damage, hitPosition, playerCamera, 0.75f*Color.white);
          }
          else
          {
               if (regenC != null) StopCoroutine(regenC);
               
               health -= damage - shield;
               playerHUD.Health(health);
               FloatingDamage(damage, hitPosition, playerCamera, Color.white);
               
               shield = 0; playerHUD.HideShield();
               
               regenC = StartCoroutine(RegenDelay());
               if (health <= 0) print("morreu");
          }
          
          print(health);
     }
     
     
     protected override IEnumerator Echo(int damage, float delay, Transform playerCamera, Color textColor)
     {
          TextMeshProUGUI effectEntry = playerHUD.AddEffect(textColor);
          effectEntry.text = "Echo: " + damage;
          
          yield return StartCoroutine(base.Echo(damage, delay, playerCamera, textColor));
          
          playerHUD.RemoveEffect(effectEntry);
     }
     
     
     // ////////////////////////////// //
     // constant damage/heal over time //
     // ////////////////////////////// //
     
     protected override IEnumerator DotTicker(int dps, float duration, Transform playerCamera, Color textColor)
     {
          TextMeshProUGUI effectEntry = playerHUD.AddEffect(textColor);
          effectEntry.text = "Dot: " + (dps * duration).ToString("F0");
          
          float tickTime = 1f / dps;
          duration -= tickTime;
          for ( ; duration >= 0; duration -= tickTime)
          {
               yield return new WaitForSeconds(tickTime);
               TakeDamage(1, transform.position, playerCamera, textColor);
               effectEntry.text =  "Dot: " + (dps * duration).ToString("F0");
          }
          playerHUD.RemoveEffect(effectEntry);
     }
     
     
     protected override IEnumerator HotTicker(int hps, float duration)
     {
          TextMeshProUGUI effectEntry = playerHUD.AddEffect(Color.green * 0.69f);
          effectEntry.text = "healing: " + hps*duration;
          
          float tickTime = 1f / hps;
          duration -= tickTime;
          for ( ; duration >= 0; duration -= tickTime)
          {
               yield return new WaitForSeconds(tickTime);
               AddHealth(1);
               playerHUD.Health(health);
               effectEntry.text = "healing: " + (hps * duration).ToString("F0");
          }
          playerHUD.RemoveEffect(effectEntry);
     }
     
     
     protected override IEnumerator ShieldTicker(int sps, float duration)
     {
          TextMeshProUGUI effectEntry = playerHUD.AddEffect(Color.white);
          effectEntry.text = "Shielding: " + (sps * duration).ToString("F0");
          
          float tickTime = 1f / sps;
          duration -= tickTime;
          for ( ; duration >= 0; duration -= tickTime)
          {
               yield return new WaitForSeconds(tickTime);
               AddShield(1);
               
               effectEntry.text =  "Shielding: " + (sps * duration).ToString("F0");
          }
          playerHUD.RemoveEffect(effectEntry);
     }
     
     
     // //////////////////////////////// //
     // tick-based damage/heal over time //
     // //////////////////////////////// //
     
     public override void PoisonDamage(int stacks, float halfLife, Transform playerCamera, Color textColor)
     {
          poisonStacks += stacks;
          if (poisonC == null) StartCoroutine(PoisonTicker(halfLife, playerCamera, textColor));
          else poisonEntry.text = "Poisoned: " + poisonStacks;
     }
     
     protected override IEnumerator PoisonTicker(float halfLife, Transform playerCamera, Color textColor)
     {
          poisonEntry = playerHUD.AddEffect(textColor);
          poisonEntry.text = "Poisoned: " + poisonStacks;
          
          while (poisonStacks > 0)
          {
               yield return new WaitForSeconds(halfLife);
               TakeDamage(poisonStacks, transform.position, playerCamera, textColor);
               poisonStacks /= 2;
               
               poisonEntry.text = "Poisoned: " + poisonStacks;
          }
          poisonC = null;
          playerHUD.RemoveEffect(poisonEntry);
     }
     
     
     public override void BleedDamage(int bleedDamage, float tickTime, Transform playerCamera, Color textColor)
     {
          damageToBleed += bleedDamage;
          bleedIndex = 0;
          
          if (bleedC == null)
          {
               bleedPrefix = "Bleeding: ";
               bleedC = StartCoroutine(BleedTicker(tickTime, playerCamera, textColor));
          }
          else bleedEntry.text = bleedPrefix + damageToBleed;
     }
     
     protected override IEnumerator BleedTicker(float tickTime, Transform playerCamera, Color textColor)
     {
          bleedEntry = playerHUD.AddEffect(textColor);
          bleedEntry.text = "Bleeding: " + damageToBleed;
          
          for ( ; damageToBleed > 0; ++bleedIndex)
          {
               yield return new WaitForSeconds(tickTime);
               
               // uma porcentagem do bleedDamage que vai aumentando com o tempo pra curva de dano nao ser infinita/longa
               int tickDamage = Mathf.CeilToInt((0.30f + bleedIndex*0.04f) * damageToBleed);
               TakeDamage(tickDamage, transform.position, playerCamera, textColor);
               damageToBleed -= tickDamage;
               
               bleedEntry.text = "Bleeding: " + damageToBleed;
          }
          bleedC = null;
          playerHUD.RemoveEffect(bleedEntry);
     }
     
     
     public override void Hemorrhage(int bleedDamage, float execute, Transform playerCamera, Color textColor)
     {
          damageToBleed += bleedDamage;
          bleedIndex = 0;
          
          if (bleedC == null)
          {
               bleedPrefix = "Hemorrhage: ";
               bleedC = StartCoroutine(HemorrhageTicker(execute, playerCamera, textColor));
          }
          else bleedEntry.text = bleedPrefix + damageToBleed;
     }
     
     protected override IEnumerator HemorrhageTicker(float execute, Transform playerCamera, Color textColor)
     {
          bleedEntry = playerHUD.AddEffect(textColor);
          bleedEntry.text = "Hemorrhage: " + damageToBleed;
          
          for ( ; damageToBleed > 0; ++bleedIndex)
          {
               yield return new WaitForSeconds(0.5f);
               
               // uma porcentagem do bleedDamage que vai aumentando com o tempo pra curva de dano nao ser infinita/longa
               int tickDamage = Mathf.CeilToInt((0.30f + bleedIndex*0.05f) * damageToBleed);
               TakeDamage(tickDamage, transform.position, playerCamera, textColor);
               if (health / (float)maxHealth <= execute / 100)
               {
                    TrueDamage(health, transform.position, playerCamera, Color.black);
                    damageToBleed = 0;
                    break;
               }
               damageToBleed -= tickDamage;

               bleedEntry.text = "Hemorrhage: " + damageToBleed;
          }
          bleedC = null;
          playerHUD.RemoveEffect(bleedEntry);
     }
}
