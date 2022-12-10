using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text _dmgBonus;
	[SerializeField] private TMP_Text _defBonus;
	[SerializeField] private TMP_Text _critBonus;
	[SerializeField] private TMP_Text _hpBonus;
	[SerializeField] private TMP_Text _coinRewardBonus;

	[SerializeField] private TMP_Text _playerHP;
	[SerializeField] private TMP_Text _enemyHP;

	

	public void SetBattlefieldBonus (float dmg, float def, float crit, float hp, float coinRewards)
    {
		_dmgBonus.text = $"Damage: +{Mathf.Round(dmg * 100)}%";
		_defBonus.text = $"Defense: +{Mathf.Round(def * 100)}%";
		_critBonus.text = $"Crit. Chance: +{Mathf.Round(crit * 100)}%";
		_hpBonus.text = $"Max. HP: +{Mathf.Round(hp * 100)}%";
		_coinRewardBonus.text = $"Coin Rewards: +{Mathf.Round(coinRewards * 100)}%";
	}

	public void SetPlayerHPText(int health, int maxHealth)
	{
		_playerHP.text = $"{health}/{maxHealth}";
	}

	public void SetEnemyHPText(int health, int maxHealth)
	{
		_enemyHP.text = $"{health}/{maxHealth}";
	}
}
