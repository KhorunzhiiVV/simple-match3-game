using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Vector2 _damage;

    public int GetDamage()
    {
        return Mathf.RoundToInt( Random.Range(_damage.x, _damage.y));
    }
}
