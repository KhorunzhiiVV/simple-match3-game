using UnityEngine;

public class Health : MonoBehaviour
{
    
    [SerializeField]private int _maxHealth = 10;
    [SerializeField]private ColorGem.ColorType _color;

    private int _currentHealth;

	private void Start()
	{
        _currentHealth = _maxHealth;
	}

	public void TakeDamage(int damage, ColorGem.ColorType color)
    {
        if (_color == color)
        {
            _currentHealth -= Mathf.RoundToInt(damage * 0.5f);
        }
        else
        {
            _currentHealth -= damage;
		}
    }
}
