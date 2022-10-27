using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;

public class Board : MonoBehaviour
{
	[SerializeField] private int _xDimension;
	[SerializeField] private int _yDimension;
    [SerializeField] private GameObject _backgroundPrefab;
    [SerializeField] private GameObject _gemPrefab;
    [SerializeField] private GemPrefabs[] _gemPrefabs;
    [SerializeField] private float _fillTime = 0.1f;

    private Gem[,] _gems;

    private Gem _pressedGem;
    private Gem _dePressedGem;

    public enum GemTypes
    {
        EMPTY,
        NORMAL,     //Just a regular gem
        EPIC,       //Destroy +4 gems in all directions
        LEGENDARY,  //Destroy all gems of the same color
        COUNT
    }

    [System.Serializable]
    public struct GemPrefabs
    {
        public GemTypes Type;
        public GameObject PiecePrefab;
    }



    private Dictionary<GemTypes, GameObject> _gemPrefabDict;

    private void Awake()
    {
        _gemPrefabDict = new Dictionary<GemTypes, GameObject>();
        for (int i = 0; i < _gemPrefabs.Length; i++)
        {
			if (!_gemPrefabDict.ContainsKey(_gemPrefabs[i].Type))
			{
				_gemPrefabDict.Add(_gemPrefabs[i].Type, _gemPrefabs[i].PiecePrefab);
			}
		}
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
                SpawnNewGem(x, y, GemTypes.EMPTY);
			}
		}

        StartCoroutine(Fill());
	}

    // Update is called once per frame
    private void Update()
    {
        
    }

    public IEnumerator Fill ()
    {
        bool needsRefill = true;

        while (needsRefill)
            {

			yield return new WaitForSeconds(_fillTime  + 0.25f);

			while (FillStep())
            {
                yield return new WaitForSeconds(_fillTime);
			}
			List<Gem> matches = GetMatch();
			needsRefill = ClearAllValidMatches(matches);
        }
    }

    public bool FillStep ()
    {
        bool movedGem = false;

        for (int y = 1 ; y < _yDimension; y++)
        {
            for (int x = 0; x < _xDimension; x++)
            {
                Gem gem = _gems[x, y];

                if (gem.IsMovable())
                {
					Gem gemBelow = _gems[x, y - 1];

                    if (gemBelow.GemType == GemTypes.EMPTY)
                    {
                        Destroy(gemBelow.gameObject); 
                        gem.MovableComponent.Move(x, y - 1, _fillTime);
                        _gems[x, y - 1] = gem;
                        SpawnNewGem (x, y, GemTypes.EMPTY);
                        movedGem = true;
                    }
				}
            }
        }
        for (int x = 0; x < _xDimension; x++)
        {
            Gem gemBelow = _gems[x, _yDimension - 1];
            if (gemBelow.GemType == GemTypes.EMPTY)
            {
                Destroy(gemBelow.gameObject);
                GameObject newPiece = (GameObject)Instantiate(_gemPrefabDict[GemTypes.NORMAL], new Vector3 (x, _yDimension, 0), Quaternion.identity);
                newPiece.transform.parent = transform;

                _gems[x, _yDimension - 1] = newPiece.GetComponent<Gem>();
                _gems[x, _yDimension - 1].Init(x, _yDimension, this, GemTypes.NORMAL);
				_gems[x, _yDimension - 1].MovableComponent.Move (x, _yDimension - 1, _fillTime);
                _gems[x, _yDimension - 1].ColorComponent.SetColor((ColorGem.ColorType)Random.Range(0, _gems[x, _yDimension - 1].ColorComponent.NumColors));
                movedGem = true;

			}
        }

        return movedGem;
    }

    public Gem SpawnNewGem (int x, int y, GemTypes gemType)
    {
        GameObject newGem = (GameObject)Instantiate(_gemPrefabDict[gemType], new Vector3(x, y, 0), Quaternion.identity);
        newGem.transform.parent = transform;

        _gems[x, y] = newGem.GetComponent<Gem>();
        _gems[x, y].Init(x, y, this, gemType);

        return _gems[x, y];
    }

    public bool IsAdjacent (Gem gem1, Gem gem2)
    {
        return (gem1.X == gem2.X && (int)Mathf.Abs(gem1.Y - gem2.Y) == 1)
            || (gem1.Y == gem2.Y && (int)Mathf.Abs(gem1.X - gem2.X) == 1);

	}

    public void SwapGems (Gem gem1, Gem gem2)
    {
        if (gem1.IsMovable() && gem2.IsMovable())
        {
            

			_gems[gem2.X, gem2.Y] = gem1;
            _gems[gem1.X, gem1.Y] = gem2;
			List<Gem> matchedGems1 = GetMatch();


			if (matchedGems1 != null)
            {
				int gem1X = gem1.X;
				int gem1Y = gem1.Y;

				gem1.MovableComponent.Move(gem2.X, gem2.Y, _fillTime);
				gem2.MovableComponent.Move(gem1X, gem1Y, _fillTime);

				ClearAllValidMatches(matchedGems1);

                _pressedGem = null;
				_dePressedGem = null;

				StartCoroutine(Fill());
			}
            else
            {
				_gems[gem1.X, gem1.Y] = gem1;
				_gems[gem2.X, gem2.Y] = gem2;

				_pressedGem = null;
				_dePressedGem=null;
			}

            
		}
        
    }

    public void PressedGem (Gem gem)
    {
		if (_pressedGem != null && IsAdjacent(_pressedGem, gem))
		{
			_dePressedGem = _pressedGem;
			_pressedGem = gem;
			SwapGems(_pressedGem, _dePressedGem);

		}
		else
		{

			_pressedGem = gem;
			_dePressedGem = gem;
		}
		 
	}

    public List<Gem> GetMatch ()
    {
        List<Gem> matchedGemList = new List<Gem>();


        ColorGem.ColorType color;
        int matchedGroupCount = 0;

        //Checking horizontal matches
        for (int y = 0; y < _yDimension; y++)
        {
            int matchedGemCount = 0;
            for (int x = 0; x < _xDimension; x++)
            {
                
                if (x == 0)
                    color = _gems[x, y].ColorComponent.Color;
                else
                    color = _gems[x - 1, y].ColorComponent.Color;
                
                if (_gems[x,y].ColorComponent.Color == color)
                {
                    matchedGemCount++;
                }
                else
                {
                    if (matchedGemCount > 2)
                    {
                        matchedGroupCount++;
                        for (int i = 0; i < matchedGemCount; i++)
                        {
                            matchedGemList.Add(_gems[x - matchedGemCount + i, y]);
                        }
                        if (matchedGemCount == 4)
                        {
							if (_pressedGem == null && _dePressedGem == null)
							{
								Debug.Log(matchedGemCount + " in horizontal row1");
								int randomX = Random.Range(x - matchedGemCount, x);
								Debug.Log("Ipic at " + randomX + " " + y);
								_gems[randomX, y].IsEpic = true;
							}
							else
							{
								if (_pressedGem.ColorComponent.Color == color)
								{
									_pressedGem.IsEpic = true;
								}
								else
								{
									_dePressedGem.IsEpic = true;
								}
							}
						}
						if (matchedGemCount > 4)
						{
							if (_pressedGem == null && _dePressedGem == null)
							{
								Debug.Log(matchedGemCount + " in horizontal row2");
								int randomX = Random.Range(x - matchedGemCount, x);
								Debug.Log("Legendary at " + randomX + " " + y);
								_gems[randomX, y].IsLegendary = true;
							}
							else
								{
									if (_pressedGem.ColorComponent.Color == color)
									{
										_pressedGem.IsLegendary = true;
									}
									else
									{
										_dePressedGem.IsLegendary = true;
									}
							}
						}
					}
                    matchedGemCount = 1;
                }
                if (x == _xDimension - 1 && matchedGemCount > 2)
                {
					for (int i = 1; i <= matchedGemCount; i++)
					{
						matchedGemList.Add(_gems[x - matchedGemCount + i, y]);
					}
					if (matchedGemCount == 4)
					{
						if (_pressedGem == null && _dePressedGem == null)
						{
							Debug.Log(matchedGemCount + " in horizontal row3");
							int randomX = Random.Range(x - matchedGemCount + 1, x + 1);
							Debug.Log("Epic at " + randomX + " " + y);
							_gems[randomX, y].IsEpic = true;
						}
						else
						{
							if (_pressedGem.ColorComponent.Color == color)
							{
								_pressedGem.IsEpic = true;
							}
							else
							{
								_dePressedGem.IsEpic = true;
							}
						}
					}
					if (matchedGemCount > 4)
					{
						if (_pressedGem == null && _dePressedGem == null)
						{
							Debug.Log(matchedGemCount + " in horizontal row4");
							int randomX = Random.Range(x - matchedGemCount + 1, x + 1);
							Debug.Log("Legendary at " + randomX + " " + y);
							_gems[randomX, y].IsLegendary = true;
						}
						else
						{
							if (_pressedGem.ColorComponent.Color == color)
							{
								_pressedGem.IsLegendary = true;
							}
							else
							{
								_dePressedGem.IsLegendary = true;
							}
						}
					}
				}

            }
        }

		//Checking vertical matches
		for (int x = 0; x < _xDimension; x++)
		{
			int matchedGemCount = 0;
			for (int y = 0; y < _yDimension; y++)
			{

				if (y == 0)
					color = _gems[x, y].ColorComponent.Color;
				else
					color = _gems[x, y - 1].ColorComponent.Color;

				if (_gems[x, y].ColorComponent.Color == color)
				{
					matchedGemCount++;
				}
				else
				{
					if (matchedGemCount > 2)
					{
						matchedGroupCount++;
						for (int i = 0; i < matchedGemCount; i++)
						{
							matchedGemList.Add(_gems[x, y - matchedGemCount + i]);
						}
						if (matchedGemCount == 4)
						{
							if (_pressedGem == null && _dePressedGem == null)
							{
								Debug.Log(matchedGemCount + " in vert row1");
								int randomY = Random.Range(y - matchedGemCount, y);
								Debug.Log("Epic at " + x + " " + randomY);
								_gems[x, randomY].IsEpic = true;
							}
							else
							{
								if (_pressedGem.ColorComponent.Color == color)
								{
									_pressedGem.IsEpic = true;
								}
								else
								{
									_dePressedGem.IsEpic = true;
								}
							}
						}
						if (matchedGemCount > 4)
						{
							if (_pressedGem == null && _dePressedGem == null)
							{
								Debug.Log(matchedGemCount + " in vert row2");
								int randomY = Random.Range(y - matchedGemCount, y);
								Debug.Log("Legendary at " + x + " " + randomY);
								_gems[x, randomY].IsLegendary = true;
							}
							else
							{
								if (_pressedGem.ColorComponent.Color == color)
								{
									_pressedGem.IsLegendary = true;
								}
								else
								{
									_dePressedGem.IsLegendary = true;
								}
							}
						}
					}
					matchedGemCount = 1;
				}
				if (y == _yDimension - 1 && matchedGemCount > 2)
				{
					for (int i = 1; i <= matchedGemCount; i++)
					{
						matchedGemList.Add(_gems[x, y - matchedGemCount + i]);
					}
					if (matchedGemCount == 4)
					{
						if (_pressedGem == null && _dePressedGem == null)
						{
							Debug.Log(matchedGemCount + " in vert row3");
							int randomY = Random.Range(y - matchedGemCount + 1, y + 1);
							Debug.Log("Epic at " + x + " " + randomY);
							_gems[x, randomY].IsEpic = true;
						}
						else
						{
							if (_pressedGem.ColorComponent.Color == color)
							{
								_pressedGem.IsEpic = true;
							}
							else
							{
								_dePressedGem.IsEpic = true;
							}
						}
					}
					if (matchedGemCount > 4)
					{
						
						if (_pressedGem == null && _dePressedGem == null)
						{
							Debug.Log(matchedGemCount + " in vert row4");
							int randomY = Random.Range(y - matchedGemCount + 1, y + 1);
							Debug.Log("Legendary at " + x + " " + randomY);
							_gems[x, randomY].IsLegendary = true;
						}
						else
						{
							if (_pressedGem.ColorComponent.Color == color)
							{
								_pressedGem.IsLegendary = true;
							}
							else
							{
								_dePressedGem.IsLegendary = true;
							}
						}
					}
				}

			}
		}
        HashSet<Gem> matchedGemHash = new HashSet<Gem>(matchedGemList);
		HashSet<Gem> intersectingGems = new HashSet<Gem>(matchedGemList.GroupBy(x => x).Where(g => g.Count() > 1).Select(i => i.Key).ToList());
        
        foreach (Gem gem in intersectingGems)
        {
            gem.IsLegendary = true;
        }

		if (matchedGemHash.Count > 2)
        {
            return matchedGemHash.ToList();
		}
        return null;
    }

	public List<Gem> GetMatch(Gem gem)
	{
		HashSet<Gem> matchedGemHash = new HashSet<Gem>();

		Debug.Log("GetMatch, check horiz match");//check horizontally
		List<Gem> horizontalMatches = CheckForHorizontalMatches(gem);
		if (horizontalMatches != null)
		{ 
			foreach (Gem gem1 in horizontalMatches)
			{
				List<Gem> tempHotizontalMatchList = CheckForHorizontalMatches(gem1);
				if (tempHotizontalMatchList != null)
					matchedGemHash.UnionWith(tempHotizontalMatchList);
				Debug.Log(matchedGemHash.Count);
			}
		}
		Debug.Log("GetMatch, check horiz match");//check vertically
		List<Gem>verticalMatches = CheckForVerticalMatches(gem);

		if (verticalMatches != null)
		{
			foreach (Gem gem2 in verticalMatches)
			{
				List<Gem>tempVertMatchList = CheckForHorizontalMatches(gem2);
				if (tempVertMatchList != null)
					matchedGemHash.UnionWith(tempVertMatchList);
				Debug.Log(matchedGemHash.Count);
			}
		}
		if (matchedGemHash != null && matchedGemHash.Count > 2)
		{
			Debug.Log(matchedGemHash.Count);
			return matchedGemHash.ToList();
		}
		return null;
	}

	private List<Gem> CheckForVerticalMatches(Gem gem)
	{
		ColorGem.ColorType color = gem.ColorComponent.Color;
		for (int yDir = 0; yDir <= 1; yDir++)
		{
			List<Gem> verticalMatches = new List<Gem>();
			if (yDir == 0)
			{
				if (gem.Y != _yDimension - 1)
				{
					for (int y = gem.Y + 1; y < _yDimension; y++)
					{
						if (_gems[gem.X, y].ColorComponent.Color == color)
						{
							verticalMatches.Add(_gems[gem.X, y]);
						}
						else
						{
							break;
						}
					}
				}
			}
			if (yDir == 1)
			{
				if (gem.Y != 0)
				{

					for (int y = gem.Y - 1; y >= 0; y--)
					{
						if (_gems[gem.X, y].ColorComponent.Color == color)
						{
							verticalMatches.Add(_gems[gem.X, y]);
						}
						else
						{
							break;
						}
					}
				}
			}
			if (verticalMatches.Count > 2)
			{
				Debug.Log("check vert match: " + verticalMatches.Count);
				return verticalMatches;
			}
		}
		return null;
	}

	private List<Gem> CheckForHorizontalMatches(Gem gem)
	{
		ColorGem.ColorType color = gem.ColorComponent.Color;
		for (int xDir = 0; xDir <= 1; xDir++)
		{
			List<Gem> horizontalMatches = new List<Gem>();
			horizontalMatches.Add(gem);
			if (xDir == 0)
			{
				if (gem.X != _xDimension - 1)
				{
					for (int x = gem.X + 1; x < _xDimension; x++)
					{
						if (_gems[x, gem.Y].ColorComponent.Color == color)
						{
							horizontalMatches.Add(_gems[x, gem.Y]);
						}
						else
						{
							break;
						}
					}
				}
			}
			if (xDir == 1)
			{
				if (gem.X != 0)
				{
					for (int x = gem.X - 1; x >= 0; x--)
					{
						if (_gems[x, gem.Y].ColorComponent.Color == color)
						{
							horizontalMatches.Add(_gems[x, gem.Y]);
						}
						else
						{
							break;
						}
					}
				}
			}
			if (horizontalMatches.Count > 2)
			{
				Debug.Log("check horiz match: " + horizontalMatches.Count);
				return horizontalMatches;
			}
		}
		return null;
	}


	private List<Gem> FindConnectedGems (Gem gem)
    {
        List<Gem> connectedGems = new List<Gem>();
        ColorGem.ColorType color = gem.ColorComponent.Color;
        
        List<Gem> checkForConnection = new List<Gem>();

        checkForConnection.Add(gem);

		foreach (Gem notCheckedGem in checkForConnection)
		{
			    
			for (int dirX = 0; dirX <= 1; dirX++) // horizontal check
			{

				for (int xOffset = 1; xOffset < _xDimension; xOffset++)
				{
					int x;

					if (dirX == 0)
						x = notCheckedGem.X - xOffset; //left
					else
						x = notCheckedGem.X + xOffset; //right
					if (x < 0 || x >= _xDimension)
					{
						break;
					}
					if (_gems[x, notCheckedGem.Y].ColorComponent.Color == color)
					{
						if (!checkForConnection.Contains(_gems[x, notCheckedGem.Y]))
						{
							checkForConnection.Add(_gems[x, notCheckedGem.Y]);
						}
					}
					else
					{
						break;
					}
				}
			}

			for (int dirY = 0; dirY <= 1; dirY++) // vertical
			{

				for (int yOffset = 1; yOffset < _yDimension; yOffset++)
				{
					int y;

					if (dirY == 0)
						y = notCheckedGem.Y - yOffset; //down
					else
						y = notCheckedGem.Y + yOffset; //up
					if (y < 0 || y >= _yDimension)
					{
						break;
					}
					if (_gems[notCheckedGem.X, y].ColorComponent.Color == color)
					{
						if (!checkForConnection.Contains(_gems[notCheckedGem.X, y]))
						{
							checkForConnection.Add(_gems[notCheckedGem.X, y]);
						}
					}
					else
					{
						break;
					}
				}
			}
		}

		return checkForConnection;
    }

    private bool ClearAllValidMatches (List<Gem> matches)
    {
        bool needsRefill = false;

				//List<Gem> matches = GetMatch();
				
				if (matches != null)
				{
					foreach (var gem in matches)
					{
						if (ClearGem(gem.X, gem.Y))
							needsRefill = true;
					}
				}
			

        return needsRefill;
    }

	private bool ClearGem(int x, int y, GemTypes gemType = GemTypes.EMPTY)
	{
		if (_gems[x,y].IsClearable() && !_gems[x,y].ClearableGem.IsBeingCleared)
		{
			if (_gems[x, y].IsEpic || _gems[x, y].IsLegendary)
			{
				ColorGem.ColorType color = _gems[x, y].ColorComponent.Color;
				
				if (_gems[x, y].IsEpic)
				{
					_gems[x, y].ClearableGem.Clear();
					Gem newGem = SpawnNewGem(x, y, GemTypes.EPIC);
					newGem.ColorComponent.SetColor(color);
					newGem.IsLegendary = false;
					newGem.IsEpic = false;
				}
				if (_gems[x, y].IsLegendary)
				{				
					_gems[x, y].ClearableGem.Clear();
					Gem newGem = SpawnNewGem(x, y, GemTypes.LEGENDARY);
					newGem.ColorComponent.SetColor(color);
					newGem.IsLegendary = false;
					newGem.IsEpic = false;
				}
				return false;
			}
			_gems[x, y].ClearableGem.Clear();
            SpawnNewGem(x, y, gemType);
            return true;
		}
        return false;
	}

	public void EpicClear (Gem epicGem)
	{
		if (epicGem.X != 0)
		{
			ClearGem(epicGem.X - 1, epicGem.Y);
		}
		if (epicGem.X != _xDimension - 1)
		{
			ClearGem(epicGem.X + 1, epicGem.Y);
		}
		if (epicGem.Y != 0)
		{
			ClearGem(epicGem.X, epicGem.Y - 1);
		}
		if (epicGem.Y != _yDimension - 1)
		{
			ClearGem(epicGem.X, epicGem.Y + 1);
		}
	}

	public void LegendaryClear (Gem gem)
	{
		ColorGem.ColorType color = gem.ColorComponent.Color;

		for (int x = 0; x < _xDimension; x++)
		{
			for (int y = 0; y < _yDimension; y++)
			{
				if (_gems[x,y].IsColored() && _gems[x, y].ColorComponent.Color == color && !_gems[x, y].ClearableGem.IsBeingCleared)
				{
					
					ClearGem(_gems[x, y].X, _gems[x, y].Y);
				}
			}
		}
	}

}
