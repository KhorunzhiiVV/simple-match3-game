using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterAttack), typeof(CharacterDefense))]
public class Character : MonoBehaviour
{
	[SerializeField] private int _maxHealth = 100;
	[SerializeField] private float _maxHealthMultiplier;
	
	[SerializeField] private Transform _spawnPosition;
	[SerializeField] private CharacterAttack _characterAttack;
	[SerializeField] private CharacterDefense _characterDefense;
	[SerializeField] private ColorGem.ColorType _color;
	[SerializeField] private Character _target;
	[SerializeField] private DamagePopup _damagePopupPrefab;

	private UIController _battlefieldUIController;
	private int _currentHealth;
	private float _damageMultiplierForSameColorAttack = 0.5f;

	private void Awake()
	{
		_characterAttack= GetComponent<CharacterAttack>();
		_characterDefense= GetComponent<CharacterDefense>();
		_battlefieldUIController = FindObjectOfType<UIController>();
	}
	private void Start()
	{
		_maxHealth = Mathf.RoundToInt(_maxHealth * (1 + _maxHealthMultiplier));
		_currentHealth = _maxHealth;
		UpdateBattlefieldUI();
	}

	public IEnumerator TakeDamage(int damage, ColorGem.ColorType color)
	{
		int finalDamage;
		int reducedDamage = _characterDefense.ApplyDefense(damage);
		//print("Player reduced damage = " + reducedDamage);
		if (_color == color)
		{
			reducedDamage = Mathf.CeilToInt(reducedDamage * _damageMultiplierForSameColorAttack);
		}
		bool isCritical = _target.GetComponent<CharacterAttack>().IsCritical();
		if (isCritical)
		{
			finalDamage = reducedDamage * 2;
			yield return StartCoroutine(InstantiateDamagePopup(finalDamage, isCritical));
		}
		else
		{
			finalDamage = reducedDamage;
			yield return StartCoroutine(InstantiateDamagePopup(finalDamage, isCritical));
		}
		//print("Player after crit damage = " + finalDamage);

		_currentHealth -= finalDamage;
		UpdateBattlefieldUI();
		print("Player smashed for " + finalDamage + " damage.");
		yield return new WaitForSeconds(0.2f);
	}

	private IEnumerator InstantiateDamagePopup(int finalDamage, bool isCritical)
	{
		DamagePopup damagePopup = Instantiate(_damagePopupPrefab, transform.localPosition + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity);
		damagePopup.Setup(finalDamage, isCritical);
		yield return new WaitForSeconds(0.1f);
	}

	private void UpdateBattlefieldUI()
	{
		if (this.CompareTag("Player"))
		{
			_battlefieldUIController.SetPlayerHPText(_currentHealth, _maxHealth);
		}
		else
		{
			_battlefieldUIController.SetEnemyHPText(_currentHealth, _maxHealth);
		}
	}

	public Coroutine Attack(ColorGem.ColorType color, float damageMultiplier)
	{
		int calculatedDamage = _characterAttack.CalculateDamage(damageMultiplier);
		//print("Player calculated damage = " + calculatedDamage);
		return StartCoroutine(_target.TakeDamage(calculatedDamage, color));
	}

	public void SetTarget(Character target)
	{
		_target = target;
	}

	public void SetBattlefieldBonus(float damage, float def, float critChance, float hp)
	{
		_maxHealthMultiplier += hp;
		_characterAttack.ModifyMultipliers(damage, critChance);
		_characterDefense.ModifyMultipliers(def);
	}
}
