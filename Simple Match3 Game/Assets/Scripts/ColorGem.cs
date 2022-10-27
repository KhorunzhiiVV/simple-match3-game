using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ColorGem : MonoBehaviour
{
    public enum ColorType
    {
        RED,
        GREEN,
        BLUE,
        YELLOW,
        MAGENTA
    }

    

    [System.Serializable]
    public struct ColorSprite
    {
        [SerializeField] public ColorType color;
        [SerializeField] public Sprite gemSprite;
    }

	[System.Serializable]
	public struct SpriteMaterial
	{
		[SerializeField] public ColorType color;
		[SerializeField] public Material gemMaterial;
	}

	[SerializeField] private ColorSprite[] _colorSprites;
	[SerializeField] private SpriteMaterial[] _spriteMaterials;

	private Dictionary<ColorType, Sprite> _spriteRendererDict;
    private Dictionary<ColorType, Material> _spriteMaterialDict;

    private SpriteRenderer _spriteRenderer;
    
    private ColorType color;
    public ColorType Color
    {
        get { return color; }
        set { SetColor(value); }
    }

    public int NumColors
    {
        get { return _colorSprites.Length; }
    }

    public void SetColor(ColorType newColor)
    {
        color = newColor;

        if (_spriteRendererDict.ContainsKey(newColor))
        {
            _spriteRenderer.sprite = _spriteRendererDict[color];
		}
		if (_spriteMaterialDict.ContainsKey(newColor))
		{
			_spriteRenderer.material = _spriteMaterialDict[color];
		}
	}


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

		_spriteRendererDict = new Dictionary<ColorType, Sprite>();
        _spriteMaterialDict = new Dictionary<ColorType, Material>();

		for (int i = 0; i < _colorSprites.Length; i++)
        {
            if (!_spriteRendererDict.ContainsKey(_colorSprites[i].color))
            {
                _spriteRendererDict.Add(_colorSprites[i].color, _colorSprites[i].gemSprite);
            }
        }
		for (int i = 0; i < _spriteMaterials.Length; i++)
		{
			if (!_spriteMaterialDict.ContainsKey(_spriteMaterials[i].color))
			{
				_spriteMaterialDict.Add(_spriteMaterials[i].color, _spriteMaterials[i].gemMaterial);
			}
		}
	}

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
