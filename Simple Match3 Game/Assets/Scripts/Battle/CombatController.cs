using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatState
{
	Start,
	PlayerTurn,
	EnemyTurn,
	Won,
	Lost
}

public class CombatController : MonoBehaviour
{
	[SerializeField] private List<Enemy> _enemiesWave;
	[SerializeField] private GameObject _playerPrefab;
	[SerializeField] private float _comboMultiplier = 0.1f;
	[SerializeField] private float _damageMultiplier = 0.05f;
	[SerializeField] private float _defenseMultiplier = 0.05f;
	[SerializeField] private float _maxHealthMultiplier = 0.05f;
	[SerializeField] private float _critChanceMultiplier = 0.05f;
	[SerializeField] private float _coinRewardMultiplier = 0.1f;

	[SerializeField] private Character _player;
	[SerializeField] private Character _currentTarget;
	[SerializeField] private UIController _battlefieldUIController;
	
	private Board _board;
	private List<Gem> _matchedGems;

	private float _damageMultiplierByCombo;
	private float _damageMultiplierByGems;
	private float _defenseMultiplierByGems;
	private float _maxHealthMultiplierByGems;
	private float _critChanceMultiplierByGems;
	private float _coinRewardMultiplierByGems;

	private CombatState _state;
	public CombatState State
	{
		get { return _state; }
	}
	
	private static CombatController _combatController;

	public static CombatController GetInstance() 
	{
		return _combatController;
	}

	private void Awake()
	{
		_combatController = this;
		_board= FindObjectOfType<Board>();
		_battlefieldUIController= FindObjectOfType<UIController>();
	}

	private void OnEnable()
	{
		_board.OnMatchFound += MatchedGemsProcessing;
		_board.OnPlayerSwapGems += ResetMultiplierCounet;
		_board.OnBoardRefill += StateChangeAfterBoardRefill;
	}

	private void OnDisable()
	{

		_board.OnMatchFound -= MatchedGemsProcessing;
		_board.OnPlayerSwapGems -= ResetMultiplierCounet;
		_board.OnBoardRefill -= StateChangeAfterBoardRefill;
	}


	private void Start()
	{
		_state = CombatState.Start;
		_damageMultiplierByCombo = -_comboMultiplier;
		_board.StartFill();
	}

	private void ResetMultiplierCounet()
	{
		_damageMultiplierByCombo = -_comboMultiplier;
	}

	private void StateChangeAfterBoardRefill()
	{
		if (_state == CombatState.Start)
		{
			_player.SetBattlefieldBonus(_damageMultiplierByGems, _defenseMultiplierByGems, _critChanceMultiplierByGems, _maxHealthMultiplierByGems);
			_battlefieldUIController.SetBattlefieldBonus(
				_damageMultiplierByGems, 
				_defenseMultiplierByGems, 
				_critChanceMultiplierByGems, 
				_maxHealthMultiplierByGems, 
				_coinRewardMultiplierByGems);
			_state = CombatState.PlayerTurn;
		}
		else
		{
			_state = CombatState.EnemyTurn;
			StartCoroutine(EnemyTurn());
		}
	}

	private IEnumerator EnemyTurn()
	{
		yield return new WaitForSeconds(2f);
		_state = CombatState.PlayerTurn;
	}

	private void MatchedGemsProcessing(List<Gem> matchedGems, int damageMultiplierForCombo)
	{
		StartCoroutine(MatchedGemsProcessingCoroutine(matchedGems, damageMultiplierForCombo));
	}

	private IEnumerator MatchedGemsProcessingCoroutine(List<Gem> matchedGems, int damageMultiplierForCombo)
	{
		_damageMultiplierByCombo += damageMultiplierForCombo * _comboMultiplier;

		if (_state == CombatState.PlayerTurn)
		{
			foreach (var gem in matchedGems)
			{
				yield return _player.Attack(gem.ColorComponent.Color, _damageMultiplierByCombo);
			}
		}
		else if (_state == CombatState.Start)
		{
			foreach (var gem in matchedGems)
			{
				switch (gem.ColorComponent.Color)
				{
					case ColorGem.ColorType.RED:
						_maxHealthMultiplierByGems += _maxHealthMultiplier;
						break;
					case ColorGem.ColorType.GREEN:
						_damageMultiplierByGems += _damageMultiplier;
						break;
					case ColorGem.ColorType.BLUE:
						_defenseMultiplierByGems += _defenseMultiplier;
						break;
					case ColorGem.ColorType.YELLOW:
						_coinRewardMultiplierByGems += _coinRewardMultiplier;
						break;
					case ColorGem.ColorType.MAGENTA:
						_critChanceMultiplierByGems += _critChanceMultiplier;
						break;
					case ColorGem.ColorType.ANY:
						break;
					default:
						break;
				}
			}
		}
		else
		{
			yield return null;
		}
	}
}
