using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
	private TextMeshPro _damageText;
	private float _disappearTimer = 2f;
	private float _disappearSpeed = 2f;
	private float _moveXSpeed;
	private float _moveYSpeed;
	private Color _textColor;

	private void Awake()
	{
		_damageText = GetComponent<TextMeshPro>();
	}
    
	public void Setup (int damage, bool isCritical)
	{
		_damageText.text = damage.ToString();
		_textColor = isCritical ? Color.red : _damageText.color;
		_damageText.color = _textColor;
		_moveXSpeed = Random.Range(-1.5f, 1.2f);
		_moveYSpeed = 2f;
	}

	private void Update()
	{
		transform.position += new Vector3(_moveXSpeed, _moveYSpeed, 0 ) * Time.deltaTime;

		_disappearTimer -= Time.deltaTime;
		if (_disappearTimer < 0)
		{
			_textColor.a -= _disappearSpeed * Time.deltaTime;
			_damageText.color = _textColor;
			if (_textColor.a < 0)
			{
				Destroy(gameObject);
			}
		}
	}
}
