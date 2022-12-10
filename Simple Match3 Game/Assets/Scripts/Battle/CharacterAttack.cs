using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
	[SerializeField] private int _damage = 5;
	[SerializeField] private float _damageMultiplier;
	[SerializeField] private float _critChance;
	
	private Weapon _weapon;

	private void Awake()
	{
		_weapon = GetComponent<Weapon>();
	}

	public int CalculateDamage(float damageMultiplierCombo)
	{
		int damage = ApplyWeaponDamage(_damage);
		int finalDamage = Mathf.RoundToInt(damage * (1 + damageMultiplierCombo) * (1 + _damageMultiplier));
		return finalDamage;
	}

	public bool IsCritical()
	{
		int roll = Random.Range(0, 101);
		return roll < Mathf.CeilToInt(_critChance * 100);
	}

	private int ApplyWeaponDamage(int damage)
	{
		return Mathf.RoundToInt((damage += _weapon.GetDamage())); 
	}

	public void ModifyMultipliers (float damage, float critChance)
	{
		Mathf.Clamp(_damageMultiplier += damage, 0, int.MaxValue);
		Mathf.Clamp(_critChance += critChance, 0, int.MaxValue);
	}
}
