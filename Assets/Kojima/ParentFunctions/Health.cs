using System.Collections;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;
using static InterfacesMNG;


public class Health : MonoBehaviour, IGet, ICombat
{
     [Header("Variables")]
     [SerializeField] protected int maxHealth = 200; 
     protected int health;
     protected int shield;
     
     [Header("References")] 
     [SerializeField] private GameObject worldSpaceUIPrefab;

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
     
     // ICombat
     public virtual void AddHealth(int addHealth)
     {
          health = Mathf.Clamp(health + addHealth, 0, maxHealth);
     }
     
     // caso precise curar outros players e ver a cura
     // public virtual void AddHealth(int addHealth, Transform textRotateTarget, Color textColor)
     // {
     //      health = Mathf.Clamp(health + addHealth, 0, maxHealth);
     //      FloatingDamage(addHealth, transform.position, textRotateTarget, textColor);
     // }
     
     public virtual void AddShield(int addShield)
     {
          shield += addShield;
     }
     
     public virtual void TakeDamage(int damage, Vector3 hitPosition, Transform textRotateTarget, Color textColor)
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
          
          FloatingDamage(damage, hitPosition, textRotateTarget, textColor);
          print(health);
     }
     
     public virtual void TrueDamage(int damage, Vector3 hitPosition, Transform textRotateTarget, Color textColor)
     {
          health -= damage;
          if (health <= 0) Morreu();
          FloatingDamage(damage, hitPosition, textRotateTarget, textColor);
     }
     
     public void DelayedDamage(int damage, float delay, Transform textRotateTarget, Color textColor)
     {
          StartCoroutine(Echo(damage, delay, textRotateTarget, textColor));
     }
     
     protected virtual IEnumerator Echo(int damage, float delay, Transform textRotateTarget, Color textColor)
     {
          yield return new WaitForSeconds(delay);
          TakeDamage(damage, transform.position, textRotateTarget, textColor);
     }
     
     public void DamageOverTime(int dps, float duration, Transform textRotateTarget, Color textColor)
     {
          StartCoroutine(DotTicker(dps, duration, textRotateTarget, textColor));
     }
     
     protected virtual IEnumerator DotTicker(int dps, float duration, Transform textRotateTarget, Color textColor)
     {
          float tickTime = 1f / dps;
          for ( ; duration > 0; duration -= tickTime)
          {
               yield return new WaitForSeconds(tickTime);
               TakeDamage(1, transform.position, textRotateTarget, textColor);
          }
     }
     
     
     public virtual void PoisonDamage(int stacks, float halfLife, Transform textRotateTarget, Color textColor)
     {
          poisonStacks += stacks;
          poisonC ??= StartCoroutine(PoisonTicker(halfLife, textRotateTarget, textColor)); // se nao tem corrotina, comeca uma
     }
     
     protected virtual IEnumerator PoisonTicker(float halfLife, Transform textRotateTarget, Color textColor)
     {
          while (poisonStacks > 0)
          {
               yield return new WaitForSeconds(halfLife);
               TakeDamage(poisonStacks, transform.position, textRotateTarget, textColor);
               poisonStacks /= 2;
          }
          poisonC = null;
     }
     
     public virtual void BleedDamage(int bleedDamage, float tickTime, Transform textRotateTarget, Color textColor)
     {
          damageToBleed += bleedDamage;
          bleedIndex = 0;
          bleedC ??= StartCoroutine(BleedTicker(tickTime, textRotateTarget, textColor)); // se nao tem corrotina, comeca uma
     }

     public virtual void Hemorrhage(int bleedDamage, float execute, Transform textRotateTarget, Color textColor)
     {
          damageToBleed += bleedDamage;
          bleedIndex = 0;
          bleedC ??= StartCoroutine(HemorrhageTicker(execute, textRotateTarget, textColor)); // se nao tem corrotina, comeca uma
     }
     
     protected virtual IEnumerator BleedTicker(float tickTime, Transform textRotateTarget, Color textColor)
     {
          for ( ; damageToBleed > 0; ++bleedIndex)
          {
               yield return new WaitForSeconds(tickTime);
               
               // uma porcentagem do bleedDamage que vai aumentando com o tempo pra curva de dano nao ser infinita/longa
               int tickDamage = Mathf.CeilToInt((0.30f + bleedIndex*0.04f) * damageToBleed);
               TakeDamage(tickDamage, transform.position, textRotateTarget, textColor);
               damageToBleed -= tickDamage;
          }
          bleedC = null;
     }
     
     protected virtual IEnumerator HemorrhageTicker(float execute, Transform textRotateTarget, Color textColor)
     {
          for ( ; damageToBleed > 0; ++bleedIndex)
          {
               yield return new WaitForSeconds(0.5f);
               
               // uma porcentagem do bleedDamage que vai aumentando com o tempo pra curva de dano nao ser infinita/longa
               int tickDamage = Mathf.CeilToInt((0.30f + bleedIndex*0.05f) * damageToBleed);
               TakeDamage(tickDamage, transform.position, textRotateTarget, textColor);
               if (health/(float)maxHealth <= execute/100) TrueDamage(health, transform.position, textRotateTarget, Color.black);
               damageToBleed -= tickDamage;
          }
          bleedC = null;
     }
     
     public void HealingOverTime(int hps, float duration, Transform textRotateTarget, Color textColor)
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

     public void ShieldOverTime(int sps, float duration, Transform textRotateTarget, Color textColor)
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
     
     
     // IGet
     public int GetHealth() => health;
     public int GetMaxHealth() => maxHealth;
     public float GetHealthRatio() => health / (float)maxHealth;
     public int GetStacks() => poisonStacks;
     
     
     protected virtual void Morreu()
     {
          print("morreu");
     }
     
     protected virtual void FloatingDamage(int damage, Vector3 hitPosition, Transform textRotateTarget, Color textColor)
     {
          GameObject wsInstance = Instantiate(worldSpaceUIPrefab, hitPosition, Quaternion.identity);
          float ratio = Mathf.Log(1.75f * Vector3.Distance(hitPosition, textRotateTarget.position) + 1.25f);
          
          TextMeshProUGUI textMesh = wsInstance.GetComponentInChildren<TextMeshProUGUI>();
          textMesh.text = damage.ToString();
          wsInstance.GetComponentInChildren<RotateText>().textRotateTarget = textRotateTarget;
          wsInstance.transform.rotation = textRotateTarget.rotation;
          wsInstance.transform.localScale = Vector3.one * ratio;
          textMesh.color = textColor;
          
          //cria um vetor pra forca e um pra direcao perpend. \ inverte o lado \ multiplica os componentes \ atribui a forca
          Vector3 impulse = new Vector3(Random.Range(2f, 4f), Random.Range(2f, 4f), 5f);
          Vector3 forceDirection = Vector3.Cross(textRotateTarget.forward, wsInstance.transform.up).normalized;
          forceDirection *= Random.Range(0, 1f)>0.5f ? 1f : -1f; forceDirection.y += 1f;
          impulse = Vector3.Scale(forceDirection, impulse) * (ratio/3f + 0.8f);
          Rigidbody textRB = wsInstance.GetComponentInChildren<Rigidbody>();
          textRB.AddForce(impulse, ForceMode.Impulse);
          
          //implementacao com getChild, provavelmente vai ter que usar um setActive pra que instancie desligado por padrao
          //pra pegar o filho tem que usar o .transform.GetChild(i) e depois pegar o GO referente a esse transform
          // Transform floatingDmgTF = worldSpaceUIPrefab.transform.GetChild(0);
          // GameObject fDmgGO = floatingDmgTF.gameObject; //pega o FloatingDamage do worldSpaceUI prefab
          // GameObject fDmgInstance = Instantiate(fDmgGO, hit.point, transform.rotation, wsCanvas.transform);
          // Destroy(fDmgInstance, 0.65f);
          // fDmgInstance.GetComponent<RotateText>().textRotateTarget = transform;
          //
          // TextMeshPro fDmgTextMesh = fDmgInstance.GetComponent<TextMeshPro>();
          // fDmgTextMesh.text = damage.ToString();
          //
          // Vector2 impulse = new Vector2(Random.Range(2f, 5f), Random.Range(2f, 5f));
          // impulse.x *= Random.Range(0,1f)>0.5f ? 1f : -1f;
          // fDmgGO.GetComponent<Rigidbody2D>().AddForce(impulse, ForceMode2D.Impulse);
          
          // isCrit = true;
          // isHeadshot = true;
          // switch ((isCrit ? 1 : 0) + (isHeadshot ? 2 : 0))
          // {
          //      case 1: textMesh.color = orange;         textMesh.text += "!"; break; //crit
          //      case 2: textMesh.color = Color.yellow; textMesh.text += "!"; break; //headshot
          //      case 3: textMesh.color = Color.red;    textMesh.text += "!!";
          //              textMesh.fontStyle = FontStyles.Bold; break;
          // }
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
}
