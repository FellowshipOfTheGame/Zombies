
public class EnemyHealth : Health
{
     // protected override void Start()
     // {
     //      base.Start();
     //      // gameRule = GameObject.Find("GameManager").GetComponent<GameRules>();
     // }

     protected override void Morreu()
     {
          // fazer alguma coisa pra drop pool, pontuacao, etc
          // EventsMNG.EnemyDied();
          Destroy(gameObject);
     }
     
}
