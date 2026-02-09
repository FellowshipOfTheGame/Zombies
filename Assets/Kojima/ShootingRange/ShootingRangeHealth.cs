using System.Collections;
using UnityEngine;

public class ShootingRangeHealth : Health, InterfacesMNG.IGet
{
	// escutar a tecla R pra dar mais municao pro player
	// fazer algo que mostra dps, moa e spread
	// fazer algo que mostra os hitpoints
	//fazer uma relacao de distancia pra aumentar o tamanho dos pontos dependendo da distancia do player

	private float ratio = 1f;
	private int accumulatedDamage;
	private int accumulatedHealing;
	private Transform player;
	[SerializeField] private GameObject hitDot;
	[SerializeField] private Transform hitDotHolder;
	
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			accumulatedDamage = 0;
			foreach (Transform dot in hitDotHolder) Destroy(dot.gameObject);
		}
	}
	
	public override void TakeDamage(int damage, Vector3 hitPosition, Transform playerCamera, Color textColor)
	{
		accumulatedDamage += damage; 
		print("acc: " + accumulatedDamage + " / dmg: " + damage);
		FloatingDamage(damage, hitPosition, playerCamera, textColor);
		GameObject newDot = Instantiate(hitDot, hitPosition, Quaternion.identity, hitDotHolder);
		newDot.transform.localScale *= ratio;
	}
	
	public override void AddHealth(int addHealth)
	{
		accumulatedHealing += addHealth;
		print(accumulatedHealing);
	}

	public void UpdateDotSize(float distance)
	{
		ratio = distance / 10f;
		foreach (Transform dot in hitDotHolder) dot.localScale = Vector3.one * ratio;
	}
	
	
	// /////////////////////// //
	// Special damage handling //
	// /////////////////////// //
	
	protected override IEnumerator Echo(int damage, float delay, Transform playerCamera, Color textColor)
	{
		yield return new WaitForSeconds(delay);
		accumulatedDamage += damage;
	}
	
	protected override IEnumerator DotTicker(int dps, float duration, Transform playerCamera, Color textColor)
	{
		float tickTime = 1f / dps;
		duration -= tickTime;
		for ( ; duration >= 0; duration -= tickTime)
		{
			yield return new WaitForSeconds(tickTime);
			accumulatedDamage += 1;
		}
	}
	
	protected override IEnumerator HotTicker(int hps, float duration)
	{
		float tickTime = 1f / hps;
		duration -= tickTime;
		for ( ; duration >= 0; duration -= tickTime)
		{
			yield return new WaitForSeconds(tickTime);
			AddHealth(1);
		}
	}
	
	
	// //////////////////////////////// //
	// tick-based damage/heal over time //
	// //////////////////////////////// //
	
	public override void PoisonDamage(int stacks, float halfLife, Transform playerCamera, Color textColor)
	{
		poisonStacks += stacks;
		poisonC ??= StartCoroutine(PoisonTicker(halfLife, playerCamera, textColor));
	}
	
	protected override IEnumerator PoisonTicker(float halfLife, Transform playerCamera, Color textColor)
	{
		while (poisonStacks > 0)
		{
			yield return new WaitForSeconds(halfLife);
			accumulatedDamage += poisonStacks;
			poisonStacks /= 2;
		}
		poisonC = null;
	}
	
	
	public override void BleedDamage(int bleedDamage, float tickTime, Transform playerCamera, Color textColor)
	{
		damageToBleed += bleedDamage;
		bleedIndex = 0;
		
		bleedC ??= StartCoroutine(BleedTicker(tickTime, playerCamera, textColor));
	}
	
	protected override IEnumerator BleedTicker(float tickTime, Transform playerCamera, Color textColor)
	{
		for ( ; damageToBleed > 0; ++bleedIndex)
		{
			yield return new WaitForSeconds(tickTime);
			
			int tickDamage = Mathf.CeilToInt((0.30f + bleedIndex*0.04f) * damageToBleed);
			accumulatedDamage += tickDamage;
			damageToBleed -= tickDamage;
		}
		bleedC = null;
	}
	
	
	public override void Hemorrhage(int bleedDamage, float execute, Transform playerCamera, Color textColor)
	{
		damageToBleed += bleedDamage;
		bleedIndex = 0;
		
		bleedC ??= StartCoroutine(HemorrhageTicker(execute, playerCamera, textColor));
	}
	
	protected override IEnumerator HemorrhageTicker(float execute, Transform playerCamera, Color textColor)
	{
		for ( ; damageToBleed > 0; ++bleedIndex)
		{
			yield return new WaitForSeconds(0.5f);
			
			int tickDamage = Mathf.CeilToInt((0.30f + bleedIndex*0.05f) * damageToBleed);
			accumulatedDamage += tickDamage;
			damageToBleed -= tickDamage;
		}
		bleedC = null;
	}
	
	
	public new int GetHealth() => 1;
	public new int GetMaxHealth() => 1;
	public new float GetHealthRatio() => 0.01f;
	public new int GetStacks() => poisonStacks;
}
