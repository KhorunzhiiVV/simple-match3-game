using UnityEngine;

public class CharacterDefense : MonoBehaviour
{
	[SerializeField] private int _defense = 5;
	[SerializeField] private float _defenseMultiplier;
	[SerializeField] private Armor _armor;

	public int ApplyDefense(int damage)
	{
		int reducedDamage = Mathf.RoundToInt(Mathf.Clamp(damage - _defense * (1 + _defenseMultiplier), 1, damage));
		return reducedDamage;
	}

	private void OnArmorEquip (Armor armor)
	{
		_armor = armor;
		_defense += armor.Defence;
	}

	public void ModifyMultipliers(float defense)
	{
		Mathf.Clamp(_defenseMultiplier += defense, 0, int.MaxValue);
	}
}
