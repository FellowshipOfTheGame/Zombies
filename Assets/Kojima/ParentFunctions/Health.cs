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
     private int poisonStacks;
     private Coroutine poisonCoroutine;
     private int damageToBleed;
     private int bleedIndex;
     private Coroutine bleedCoroutine;
     
     
     protected virtual void Start()
     {
          health = maxHealth;
     }
     
     // ICombat
     public virtual void AddHealth(int addHealth)
     {
          health = Mathf.Clamp(health + addHealth, 0, maxHealth);
     }
     
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
     
     private IEnumerator Echo(int damage, float delay, Transform textRotateTarget, Color textColor)
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
          for ( ; duration > 0; duration -= 0.2f)
          {
               yield return new WaitForSeconds(0.2f);
               TakeDamage(Mathf.RoundToInt(dps/5f), transform.position, textRotateTarget, textColor);
          }
     }
     
     
     public virtual void PoisonDamage(int stacks, float halfLife, Transform textRotateTarget, Color textColor)
     {
          poisonStacks += stacks;
          poisonCoroutine ??= StartCoroutine(PoisonTicker(halfLife, textRotateTarget, textColor)); // se nao tem corrotina, comeca uma
     }
     
     protected virtual IEnumerator PoisonTicker(float decayTime, Transform textRotateTarget, Color textColor)
     {
          while (poisonStacks != 0)
          {
               yield return new WaitForSeconds(decayTime);
               TakeDamage(poisonStacks, transform.position, textRotateTarget, textColor);
               poisonStacks /= 2;
          }
          poisonCoroutine = null;
     }
     
     public virtual void BleedDamage(int bleedDamage, float tickTime, Transform textRotateTarget, Color textColor)
     {
          damageToBleed += bleedDamage;
          bleedIndex = 0;
          bleedCoroutine ??= StartCoroutine(BleedTicker(tickTime, textRotateTarget, textColor)); // se nao tem corrotina, comeca uma
     }
     
     protected virtual IEnumerator BleedTicker(float tickTime, Transform textRotateTarget, Color textColor)
     {
          for ( ; damageToBleed > 0; ++bleedIndex)
          {
               yield return new WaitForSeconds(tickTime);
               
               // clamp so pra ter certeza de nao passar de 1
               // int tickDamage = Mathf.CeilToInt(Math.Clamp((0.30f + bleedIndex * 0.04f), 0.3f, 1f) * damageToBleed);
               
               // uma porcentagem do bleedDamage que vai aumentando com o tempo pra curva de dano nao ser infinita/longa
               int tickDamage = Mathf.CeilToInt((0.30f + bleedIndex*0.04f) * damageToBleed);
               TakeDamage(tickDamage, transform.position, textRotateTarget, textColor);
               damageToBleed -= tickDamage;
          }
          bleedCoroutine = null;
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
          Destroy(wsInstance, 0.5f);
          
          TextMeshProUGUI textMesh = wsInstance.GetComponentInChildren<TextMeshProUGUI>();
          textMesh.text = damage.ToString();
          wsInstance.GetComponentInChildren<RotateText>().textRotateTarget = textRotateTarget;
          wsInstance.transform.rotation = textRotateTarget.rotation;
          textMesh.color = textColor;
          
          //cria um vetor pra forca e um pra direcao perpend. \ inverte o lado \ multiplica os componentes \ atribui a forca
          Vector3 impulse = new Vector3(Random.Range(2f, 4f), Random.Range(2f, 4f), 5f);
          Vector3 forceDirection = Vector3.Cross(textRotateTarget.forward, wsInstance.transform.up).normalized;
          forceDirection *= Random.Range(0, 1f)>0.5f ? 1f : -1f; forceDirection.y += 1f;
          impulse = Vector3.Scale(forceDirection, impulse);
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
          poisonCoroutine = null;
          damageToBleed = 0;
          bleedIndex = 0;
          bleedCoroutine = null;
     }
}
