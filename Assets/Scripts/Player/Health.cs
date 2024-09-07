// using System;
// using System.ComponentModel;
// using Tests.NetworkTest.Serializers;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class Health : MonoBehaviour
{
     // private float maxHealth = 100f;
     public int health = 100;
     public GameObject worldSpaceUIPrefab;
     // private readonly Color orange = new(1.0f, 0.25f, 0.0f);
     private bool isCrit; //implementar os dois na funcao TakeDamage
     private bool isHeadshot;
     // private GameRules gameRule;
     
     // void Start()
     // {
     //      gameRule = GameObject.Find("GameManager").GetComponent<GameRules>();
     // }

     public void TakeDamage(int damage, Vector3 position, Transform textRotateTarget)
     {
          GameObject wsInstance = Instantiate(worldSpaceUIPrefab, position, Quaternion.identity);
          Destroy(wsInstance, 0.5f);
          
          TextMeshProUGUI textMesh = wsInstance.GetComponentInChildren<TextMeshProUGUI>();
          textMesh.text = damage.ToString();
          wsInstance.GetComponentInChildren<RotateText>().textRotateTarget = textRotateTarget;
          wsInstance.transform.rotation = textRotateTarget.rotation;
          
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
          
          health -= damage;
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
}
