// using System;
// using System.ComponentModel;
// using Tests.NetworkTest.Serializers;

using System.Collections;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class PlayerHealth : MonoBehaviour
{
     [Header("Variables")]
     [SerializeField] private int maxHealth = 200; 
                      private int currentHealth;
     [SerializeField] private float regenPerSecond = 1f;
     [SerializeField] private float regenIncreaseRate = 0.25f;
     [SerializeField] private float regenTarget = 10f;
     [SerializeField] private float regenDelayTime = 4f;
     // private readonly Color orange = new(1.0f, 0.25f, 0.0f);
     
     [Header("References")] 
     [SerializeField] private GameObject worldSpaceUIPrefab;
     
     [Header("Declarations")]
     private PlayerHUD playerHUD;
     private Coroutine regenC;
     // private GameRules gameRule;

     private void Start()
     {
          playerHUD = GetComponent<PlayerHUD>();
          
          currentHealth = maxHealth;
          // gameRule = GameObject.Find("GameManager").GetComponent<GameRules>();
          playerHUD.Health(currentHealth);
     }

     private void Update()
     {
          if (Input.GetKeyDown(KeyCode.Minus)) { TakeDamage(5, transform.position, transform); }
     }

     public void TakeDamage(int damage, Vector3 hitPosition, Transform textRotateTarget)
     {
          currentHealth -= damage; FloatingDamage(damage, hitPosition, textRotateTarget, Color.white);

          if (regenC != null) { StopCoroutine(regenC); }
          
          if (currentHealth <= 0)
          {
               // Morreu();
               currentHealth = 0;
          }
          playerHUD.Health(currentHealth);
          Debug.Log(currentHealth);

          regenC = StartCoroutine(RegenDelay());
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
          while (currentHealth < maxHealth)
          {
               Debug.Log(currentRegenPerSecond.ToString("F2"));
               ++currentHealth; playerHUD.Health(currentHealth);
               yield return new WaitForSeconds(regenTickTime);
               currentRegenPerSecond += regenIncreaseRate * (1- Mathf.Pow(currentRegenPerSecond/regenTarget, 2) );
               regenTickTime = 1 / currentRegenPerSecond;
          }
     }
     
     // public void Morreu()
     // {
     //      ConnectionSingleton.Instance.Connection.UDP_Send_Message(
     //           new Message("DIE", new byte[]{0}));
     //      gameRule.pontuacao++;
     // }
     
     
     private void FloatingDamage(int damage, Vector3 hitPosition, Transform textRotateTarget, Color textColor)
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
          
          //implementacao com getchild, provavelmente vai ter que usar um setactive pra que instancie desligado por padrao
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
}
