
using System.Collections;
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

     protected override void Start()
     {
          base.Start();
          playerHUD = GetComponent<PlayerHUD>();
          playerHUD.Health(health);
     }
     
     private void Update()
     {
          if (Input.GetKeyDown(KeyCode.Equals)) AddShield(5);
          if (Input.GetKeyDown(KeyCode.Minus)) TakeDamage(5, transform.position, transform, Color.white);
     }
     
     public override void AddShield(int addShield)
     {
          shield += addShield;
          playerHUD.ShowShield();
          playerHUD.Shield(shield);
     }
     
     public override void TakeDamage(int damage, Vector3 hitPosition, Transform textRotateTarget, Color textColor)
     {
          if (damage < shield)
          {
               shield -= damage;
               FloatingDamage(damage, hitPosition, textRotateTarget, 0.75f*Color.white);
               playerHUD.Shield(shield);
          }
          else if (damage == shield)
          {
               shield = 0; playerHUD.HideShield();
               FloatingDamage(damage, hitPosition, textRotateTarget, 0.75f*Color.white);
          }
          else
          {
               if (regenC != null) StopCoroutine(regenC);
               
               health -= damage - shield;
               playerHUD.Health(health);
               FloatingDamage(damage, hitPosition, textRotateTarget, Color.white);
               
               shield = 0; playerHUD.HideShield();
               
               regenC = StartCoroutine(RegenDelay());
               if (health <= 0) print("morreu");
          }
          
          print(health);
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
               Debug.Log(currentRegenPerSecond.ToString("F2"));
               ++health; playerHUD.Health(health);
               yield return new WaitForSeconds(regenTickTime);
               currentRegenPerSecond += regenIncreaseRate * (1- Mathf.Pow(currentRegenPerSecond/regenTarget, 2) );
               regenTickTime = 1 / currentRegenPerSecond;
          }
          regenC = null;
     }
     
}
