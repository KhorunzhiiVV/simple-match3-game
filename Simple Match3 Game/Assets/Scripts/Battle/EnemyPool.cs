using UnityEngine;

public class EnemyPool : MonoBehaviour
{
	[SerializeField] private GameObject _enemyPrefab;
	private GameObject[] _enemyPool;

	
	public void PopulatePool(int size)
	{
		
		_enemyPool = new GameObject[size];

		for (int i = 0; i < size; i++)
		{
			_enemyPool[i] = Instantiate(_enemyPrefab, transform);
			_enemyPool[i].SetActive(false);
		}
	}

	private GameObject EnableEnemyInPool()
	{
		foreach (GameObject enemy in _enemyPool)
		{
			if (enemy.activeInHierarchy == false)
			{
				enemy.SetActive(true);
				return enemy;
			}
		}
		return null;
	}

	public GameObject InstantiateEnemy()
	{
		return EnableEnemyInPool();
	}

}
