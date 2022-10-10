using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
	[SerializeField] private int _xDimension;
	[SerializeField] private int _yDimension;
    [SerializeField] private GameObject _backgroundPrefab;
    [SerializeField] private GameObject _gemPrefab;

    private Gem[,] _gems;

    public enum GemTypes
    {
        NORMAL,     //Just a regular gem
        EPIC,       //Destroy +4 gems in all directions
        LEGENDARY   //Destroy all gems of the same color
    }


	// Start is called before the first frame update
	private void Start()
    {
        for (int x = 0; x < _xDimension; x++)
        {
            for (int y = 0; y < _yDimension; y++)
            {
                GameObject background = (GameObject) Instantiate(_backgroundPrefab, new Vector3(x , y , 0), Quaternion.identity);
                background.transform.parent = transform;
            }
        }

        _gems = new Gem[_xDimension,_yDimension];

		for (int x = 0; x < _xDimension; x++)
		{
			for (int y = 0; y < _yDimension; y++)
			{
				GameObject gem = (GameObject)Instantiate(_gemPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                gem.name = $"{x}, {y}";
				gem.transform.parent = transform;
                _gems[x, y] = gem.GetComponent<Gem>();

                if (_gems[x, y].IsMovable())
                {
                    _gems[x,y].MovableComponent.Move(x, y);
                }

                if (_gems[x, y].IsColored())
                {
                    _gems[x, y].ColorComponent.SetColor((ColorGem.ColorType)Random.Range(0, _gems[x, y].ColorComponent.NumColors));
                }
			}
		}
	}

    // Update is called once per frame
    private void Update()
    {
        
    }
}
