using UnityEngine;

public class Enemy : Character
{
	
	private Health _health;

	private void Awake()
	{
		_health = GetComponent<Health>();
	}

	
	
}
