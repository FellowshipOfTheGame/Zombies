// using System;
// using System.ComponentModel;
// using Tests.NetworkTest.Serializers;

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Collider = UnityEngine.Collider;
using Random = UnityEngine.Random;

/// <summary>
/// 
///     Dependencias:
///     Esse script trabalha em conjunto com o script de interfaces e canvasses de UI.
///
///     Funcao do script:
///     Esse script serve para contabilizar a vida dos inimigos e para chamar as interacoes
/// de dano especial, como ricochete e dano pela vida perdida.
/// 
/// </summary>

public class EnemyHealth : MonoBehaviour, InterfacesMNG.IDmg, InterfacesMNG.IDmgSpecial
{
     [Header("InternalVariables")]
     public int maxHealth = 100;
     private int health;
     public LayerMask playerLayerMask;
     
     [Header("SpecialVariables")]
     public float targetRadius = 5f;
     private float missileSide;
     private int stacks = -1;
     
     [Header("Prefabs")]
     public GameObject worldSpaceUIPrefab;
     public GameObject missilePrefab;
     public GameObject ricochetPrefab;
     
     [Header("Colors")]
     private readonly Color orange = new (1f, 0.55f, 0.25f);
     private readonly Color purple = new(0.7f, 0.35f, 1.0f);

     [Header("Scripts")]
     private WaveMNG waveMNG;
     // private GameRules gameRule;
     
     
     private void Start()
     {
          health = maxHealth;
          
          int randomValue = Random.Range(0, 2);
          missileSide = randomValue == 0 ? 1 : -1;
          
          // gameRule = GameObject.Find("GameManager").GetComponent<GameRules>();
          waveMNG = GetComponent<WaveMNG>();
     }

     public void TakeDmg(int damage, Vector3 hitPosition, Transform textRotateTarget, Color textColor)
     {
          FloatingDamage(damage, hitPosition, textRotateTarget, textColor);
          
          health -= damage;
          if (health <= 0)
          {
               Morreu();
          }
          Debug.Log(health);
     }
     
     public void TakeDmgSpecial(int damage, Vector3 hitPosition, Transform textRotateTarget, Color textColor,
          string special, int percentage)
     {
          health -= damage; FloatingDamage(damage, hitPosition, textRotateTarget, textColor); //dano normal
          
          if (special == "Missile")
          {
               Missile(damage/4, targetRadius, textRotateTarget);
          }
          else if (special == "Ricochet")
          {
               Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, targetRadius, playerLayerMask);
               List<Transform> nearbyEnemies = new List<Transform>();
               foreach (Collider ncollider in nearbyColliders)
               {
                    if (ncollider.transform != transform)
                    {
                         nearbyEnemies.Add(ncollider.transform);
                    }
               }
               // select two random enemies
               Transform enemy1;
               switch (nearbyEnemies.Count)
               {
                    case 0:
                         break;
                    case 1:
                         enemy1 = nearbyEnemies[Random.Range(0, nearbyEnemies.Count)].GetChild(0);
                         Ricochet(enemy1, damage/2, hitPosition, textRotateTarget);
                         break;
                    default:
                         enemy1 = nearbyEnemies[Random.Range(0, nearbyEnemies.Count)].GetChild(0);
                         Transform enemy2 = nearbyEnemies[Random.Range(0, nearbyEnemies.Count)].GetChild(0);
                         Ricochet(enemy1, damage/2, hitPosition, textRotateTarget);
                         Ricochet(enemy2, damage/2, hitPosition, textRotateTarget);
                         Debug.Log(enemy1);
                         break;
               }
          }
          else 
          {
               switch (special)
               {
                    // ricochet - purple
                    case "MaxHealth":
                         textColor = Color.green;
                         damage = Mathf.FloorToInt(maxHealth * percentage/100f);
                         break;
                    
                    case "MissingHealth":
                         textColor = Color.red;
                         damage = Mathf.FloorToInt((maxHealth - health) * percentage/100f);
                         break;
                    
                    case "Extra":
                         textColor = Color.grey;
                         damage = Mathf.FloorToInt(damage * percentage/100f);
                         break;
                    
                    case "Stack":
                         textColor = Color.grey;
                         stacks += 2; damage = stacks; Debug.Log(stacks);
                         break;
               }
               health -= damage; FloatingDamage(damage, hitPosition, textRotateTarget, textColor);
          }
          
          if (health <= 0)
          {
               Morreu();
          }
          Debug.Log(health);
     }
     
     
     private void Morreu()
     {
          // ConnectionSingleton.Instance.Connection.UDP_Send_Message(
          //      new Message("DIE", new byte[]{0}));
          // gameRule.pontuacao++;

          EventsMNG.EnemyDied();
          Destroy(gameObject);
     }
     
     
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
          Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, radius, playerLayerMask);
          List<Transform> nearbyEnemies = new List<Transform>();
          foreach (Collider ncollider in nearbyColliders)
          {
               if (ncollider.transform != transform)
               {
                    nearbyEnemies.Add(ncollider.transform);
               }
          }
          
          // select two random enemies
          Transform enemy1;
          switch (nearbyEnemies.Count)
          {
               case 0:
                    InstantiateMissile(transform, damage, textRotateTarget);
                    break;
               case 1:
                    enemy1 = nearbyEnemies[Random.Range(0, nearbyEnemies.Count)];
                    InstantiateMissile(enemy1, damage, textRotateTarget);
                    break;
               default:
                    enemy1 = nearbyEnemies[Random.Range(0, nearbyEnemies.Count)];
                    Transform enemy2 = nearbyEnemies[Random.Range(0, nearbyEnemies.Count)];
                    InstantiateMissile(enemy1, damage, textRotateTarget);
                    InstantiateMissile(enemy2, damage, textRotateTarget);
                    break;
          }
          
          // // calculate the two closest enemies manually
          // Transform closestEnemy1 = null, closestEnemy2 = null;
          // float closestDistance1 = 2 * radius, closestDistance2 = 2 * radius;
          // foreach (Collider nearbyEnemy in nearbyColliders)
          // {
          //      float distance = Vector3.Distance(transform.position, nearbyEnemy.transform.position);
          //      if (distance < closestDistance1 && distance > 0.55f)  //novo 1º mais perto
          //      {
          //           closestEnemy2 = closestEnemy1;  //passa o 1º pro 2º lugar
          //           closestDistance2 = closestDistance1;
          //           closestEnemy1 = nearbyEnemy.transform;
          //           closestDistance1 = distance;
          //      }
          //      else if (distance < closestDistance2 && distance > 0.55f)  //novo 2º mais perto
          //      {
          //           closestEnemy2 = nearbyEnemy.transform;
          //           closestDistance2 = distance;
          //      }
          // }
          // //                                     sem alvos ? self target : missel no alvo
          // InstantiateMissile(closestDistance1 > radius ? transform : closestEnemy1, damage, textRotateTarget);
          //
          // if (closestDistance2 > radius) return;
          // InstantiateMissile(closestEnemy2, damage, textRotateTarget);
     }
     
     private void InstantiateMissile(Transform target, int damage, Transform textRotateTarget)
     {
          Vector3 spawnP = transform.position + 0.6f * missileSide * textRotateTarget.right;
          Quaternion spawnR = Quaternion.Euler(0, textRotateTarget.eulerAngles.y, missileSide * -Random.Range(85f,100f));
          GameObject missile = Instantiate(missilePrefab, spawnP, spawnR);
          missile.GetComponent<MissileTargeting>().Setter(damage, target, textRotateTarget, orange);
          missileSide *= -1f;
     }
     
     private void Ricochet(Transform target, int damage, Vector3 hitPosition, Transform textRotateTarget)
     {
          Vector3 direction = (target.position - transform.position)/2.1f;
          // slight offset to prevent self collision
          if (Physics.Raycast(hitPosition + direction.normalized, direction, 
                   out RaycastHit targetHit, 1.5f * targetRadius))
          {
               GameObject hitObject = targetHit.collider.gameObject;
               if (hitObject.CompareTag("Player"))
               {
                    hitObject.GetComponent<InterfacesMNG.IDmg>().TakeDmg(damage, targetHit.point, textRotateTarget, purple);
               }
          }
          // instancia o ricochetePF entre this.transform e target.transform
          GameObject ricochetI = Instantiate(ricochetPrefab, hitPosition + direction,
               Quaternion.LookRotation(direction));
          // escala o ricochete pra ficar do tamanho certo
          ricochetI.transform.localScale = new Vector3(ricochetI.transform.localScale.x, 
               ricochetI.transform.localScale.y, direction.magnitude);
          Destroy(ricochetI, 0.1f);
     }
}
