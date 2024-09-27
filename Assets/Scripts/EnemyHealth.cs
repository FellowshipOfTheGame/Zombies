// using System;
// using System.ComponentModel;
// using Tests.NetworkTest.Serializers;

using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class EnemyHealth : MonoBehaviour, Interfaces.IDamage, Interfaces.IDamageSpecial
{
     public int maxHealth = 100;
     private int health;
     public float targetRadius = 10f;
     private float missileSide;
     public GameObject worldSpaceUIPrefab;
     public GameObject missilePrefab;
     public LayerMask playerLayerMask;
     // private readonly Color orange = new(1.0f, 0.25f, 0.0f);
     // private GameRules gameRule;
     
     private void Start()
     {
          health = maxHealth;
          
          int randomValue = Random.Range(0, 2);
          missileSide = randomValue == 0 ? 1 : -1;
          
          // gameRule = GameObject.Find("GameManager").GetComponent<GameRules>();
     }

     public void TakeDamage(int damage, Vector3 hitPosition, Transform textRotateTarget, Color textColor)
     {
          FloatingDamage(damage, hitPosition, textRotateTarget, textColor);
          
          health -= damage;
          if (health <= 0)
          {
               // Morreu();
          }
          Debug.Log(health);
     }
     
     public void TakeDamageSpecial(int damage, Vector3 hitPosition, Transform textRotateTarget, Color textColor,
          string special, int percentage)
     {
          health -= damage; FloatingDamage(damage, hitPosition, textRotateTarget, textColor); //dano normal
          
          if (special == "Missile")
          {
               Missile(damage/4, targetRadius, textRotateTarget);
          }
          else 
          {
               switch (special)
               {
                    case "MaxHealth":
                         textColor = Color.green;
                         damage = Mathf.FloorToInt(maxHealth * percentage/100f);
                         break;
                    
                    case "MissingHealth":
                         textColor = Color.black;
                         damage = Mathf.FloorToInt((maxHealth - health) * percentage/100f);
                         break;
                    
                    case "CurrentHealth":
                         textColor = Color.yellow;
                         damage = Mathf.FloorToInt(health * percentage/100f);
                         break;

                    case "Extra":
                         textColor = Color.grey;
                         damage = Mathf.FloorToInt(damage * percentage/100f);
                         break;
               }
               
               health -= damage; FloatingDamage(damage, hitPosition, textRotateTarget, textColor);
          }
          
          if (health <= 0)
          {
               // Morreu();
          }
          Debug.Log(health);
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

     private void Missile(int damage, float radius, Transform textRotateTarget)
     {
          float closestDistance1 = 2 * radius, closestDistance2 = 2 * radius;
          Transform closestEnemy1 = null, closestEnemy2 = null;
          Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, radius, playerLayerMask);
          
          foreach (Collider nearbyEnemy in nearbyColliders)
          {
               float distance = Vector3.Distance(transform.position, nearbyEnemy.transform.position);
               if (distance < closestDistance1 && distance > 0.55f)  //novo 1º mais perto
               {
                    closestEnemy2 = closestEnemy1;  //passa o 1º pro 2º lugar
                    closestDistance2 = closestDistance1;
                    closestEnemy1 = nearbyEnemy.transform;
                    closestDistance1 = distance;
               }
               else if (distance < closestDistance2 && distance > 0.55f)  //novo 2º mais perto
               {
                    closestEnemy2 = nearbyEnemy.transform;
                    closestDistance2 = distance;
               }
          }
          //                                     sem alvos ? self target : missil no alvo
          InstantiateMissile(closestDistance1 > radius ? transform : closestEnemy1, damage, textRotateTarget);

          if (closestDistance2 > radius) return;
          InstantiateMissile(closestEnemy2, damage, textRotateTarget);
     }
     
     private void InstantiateMissile(Transform target, int damage, Transform textRotateTarget)
     {
          Vector3 spawnP = transform.position + 0.6f * missileSide * textRotateTarget.right;
          Quaternion spawnR = Quaternion.Euler(0, textRotateTarget.eulerAngles.y, missileSide * -Random.Range(85f,95f));
          GameObject missile = Instantiate(missilePrefab, spawnP, spawnR);
          missile.GetComponent<Missile>().Setter(damage, target, textRotateTarget);
          missileSide *= -1f;
     }
}
