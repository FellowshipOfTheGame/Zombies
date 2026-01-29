using System.Collections;
using UnityEngine;
using static InterfacesMNG;


public class Health : MonoBehaviour, IGet, ICombat
{
     [Header("Variables")]
     [SerializeField] protected int maxHealth = 200; 
     protected int health;
     protected int shield;
     
     [Header("References")]
     [SerializeField] protected GameObject wsText;

     [Header("Interfaces")]
     protected int poisonStacks;
     protected Coroutine poisonC;
     protected int damageToBleed;
     protected int bleedIndex;
     protected Coroutine bleedC;
     
     
     protected virtual void Start()
     {
          health = maxHealth;
     }
     
     protected void OnDisable()
     {
          StopAllCoroutines();
          poisonStacks = 0;
          poisonC = null;
          damageToBleed = 0;
          bleedIndex = 0;
          bleedC = null;
     }
     
     protected virtual void Morreu()
     {
          print("morreu");
     }
     
     protected void FloatingDamage(int damage, Vector3 hitPosition, Transform playerCamera, Color textColor)
     {
          // float distanceScaler = Mathf.Log(10f * Vector3.Distance(hitPosition, playerCamera.position), 2f);
          float distance = Vector3.Distance(hitPosition, playerCamera.position)/2;
          
          GameObject fdInstance = Instantiate(wsText, hitPosition, Quaternion.identity);
          fdInstance.GetComponent<FloatingDamage>().Initialize(playerCamera, distance, damage, textColor);
     }
     
     
     // //// //
     // IGet //
     // //// //
     
     public int GetHealth() => health;
     public int GetMaxHealth() => maxHealth;
     public float GetHealthRatio() => health / (float)maxHealth;
     public int GetStacks() => poisonStacks;
     
     
     // ///////////////// //
     // ICombat functions //
     // ///////////////// //
     
     public virtual void AddHealth(int addHealth)
     {
          health = Mathf.Clamp(health + addHealth, 0, maxHealth);
     }
     
     // caso precise curar outros players e ver a cura
     // public virtual void AddHealth(int addHealth, Transform playerCamera, Color textColor)
     // {
     //      health = Mathf.Clamp(health + addHealth, 0, maxHealth);
     //      FloatingDamage(addHealth, transform.position, playerCamera, textColor);
     // }
     
     public virtual void AddShield(int addShield)
     {
          shield += addShield;
     }
     
     public virtual void TakeDamage(int damage, Vector3 hitPosition, Transform playerCamera, Color textColor)
     {
          if (damage <= shield)
          {
               shield -= damage;
               textColor *= 0.5f;
          }
          else
          {
               health -= damage - shield;
               shield = 0;
               if (health <= 0) Morreu();
          }
          
          FloatingDamage(damage, hitPosition, playerCamera, textColor);
          print(health);
     }
     
     public virtual void TrueDamage(int damage, Vector3 hitPosition, Transform playerCamera, Color textColor)
     {
          health -= damage;
          if (health <= 0) Morreu();
          FloatingDamage(damage, hitPosition, playerCamera, textColor);
     }
     
     
     // //////////////// //
     // ICombat routines //
     // //////////////// //
     
     public void DelayedDamage(int damage, float delay, Transform playerCamera, Color textColor)
     {
          StartCoroutine(Echo(damage, delay, playerCamera, textColor));
     }
     protected virtual IEnumerator Echo(int damage, float delay, Transform playerCamera, Color textColor)
     {
          yield return new WaitForSeconds(delay);
          TakeDamage(damage, transform.position, playerCamera, textColor);
     }
     
     
     public void DamageOverTime(int dps, float duration, Transform playerCamera, Color textColor)
     {
          StartCoroutine(DotTicker(dps, duration, playerCamera, textColor));
     }
     protected virtual IEnumerator DotTicker(int dps, float duration, Transform playerCamera, Color textColor)
     {
          float tickTime = 1f / dps;
          duration -= tickTime;
          for ( ; duration > 0; duration -= tickTime)
          {
               yield return new WaitForSeconds(tickTime);
               TakeDamage(1, transform.position, playerCamera, textColor);
          }
     }
     
     
     public virtual void PoisonDamage(int stacks, float halfLife, Transform playerCamera, Color textColor)
     {
          poisonStacks += stacks;
          poisonC ??= StartCoroutine(PoisonTicker(halfLife, playerCamera, textColor)); // se nao tem corrotina, comeca uma
     }
     protected virtual IEnumerator PoisonTicker(float halfLife, Transform playerCamera, Color textColor)
     {
          while (poisonStacks > 0)
          {
               yield return new WaitForSeconds(halfLife);
               TakeDamage(poisonStacks, transform.position, playerCamera, textColor);
               poisonStacks /= 2;
          }
          poisonC = null;
     }
     
     
     public virtual void BleedDamage(int bleedDamage, float tickTime, Transform playerCamera, Color textColor)
     {
          damageToBleed += bleedDamage;
          bleedIndex = 0;
          bleedC ??= StartCoroutine(BleedTicker(tickTime, playerCamera, textColor)); // se nao tem corrotina, comeca uma
     }
     protected virtual IEnumerator BleedTicker(float tickTime, Transform playerCamera, Color textColor)
     {
          for ( ; damageToBleed > 0; ++bleedIndex)
          {
               yield return new WaitForSeconds(tickTime);
               
               // uma porcentagem do bleedDamage que vai aumentando com o tempo pra curva de dano nao ser infinita/longa
               int tickDamage = Mathf.CeilToInt((0.30f + bleedIndex*0.04f) * damageToBleed);
               TakeDamage(tickDamage, transform.position, playerCamera, textColor);
               damageToBleed -= tickDamage;
          }
          bleedC = null;
     }
     
     
     public virtual void Hemorrhage(int bleedDamage, float execute, Transform playerCamera, Color textColor)
     {
          damageToBleed += bleedDamage;
          bleedIndex = 0;
          bleedC ??= StartCoroutine(HemorrhageTicker(execute, playerCamera, textColor)); // se nao tem corrotina, comeca uma
     }
     protected virtual IEnumerator HemorrhageTicker(float execute, Transform playerCamera, Color textColor)
     {
          for ( ; damageToBleed > 0; ++bleedIndex)
          {
               yield return new WaitForSeconds(0.5f);
               
               // uma porcentagem do bleedDamage que vai aumentando com o tempo pra curva de dano nao ser infinita/longa
     
               int tickDamage = Mathf.CeilToInt((0.30f + bleedIndex*0.05f) * damageToBleed);
               TakeDamage(tickDamage, transform.position, playerCamera, textColor);
               if (health/(float)maxHealth <= execute/100) TrueDamage(health, transform.position, playerCamera, Color.black);
               damageToBleed -= tickDamage;
          }
          bleedC = null;
     }
     
     
     public void HealingOverTime(int hps, float duration, Transform playerCamera, Color textColor)
     {
          StartCoroutine(HotTicker(hps, duration));
     }
     protected virtual IEnumerator HotTicker(int hps, float duration)
     {
          float tickTime = 1f / hps;
          for ( ; duration > 0; duration -= tickTime)
          {
               yield return new WaitForSeconds(tickTime);
               AddHealth(1);
          }
     }
     
     
     public void ShieldOverTime(int sps, float duration, Transform playerCamera, Color textColor)
     {
          StartCoroutine(ShieldTicker(sps, duration));
     }
     protected virtual IEnumerator ShieldTicker(int sps, float duration)
     {
          float tickTime = 1f / sps;
          for ( ; duration > 0; duration -= tickTime)
          {
               yield return new WaitForSeconds(tickTime);
               AddShield(1);
          }
     }
}
