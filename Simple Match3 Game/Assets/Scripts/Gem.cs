using UnityEngine;

public class Gem : MonoBehaviour
{
    private int _x;
    public int X
    {
        get { return _x; }
        set
        {
            if (IsMovable())
            {
                _x = value;
            }
        }
    }

    private int _y;
    public int Y 
    { 
        get { return _y; }
		set
		{
			if (IsMovable())
			{
				_y = value;
			}
		}
	}

    private Board _board;
    public Board BoardRef { get { return _board; } }

    private Board.GemTypes _gemType;
    public Board.GemTypes GemType { get { return _gemType; } }

    private MovableGem _movableComponent;
    public MovableGem MovableComponent { get { return _movableComponent; } }

	private ColorGem _colorComponent;
	public ColorGem ColorComponent { get { return _colorComponent; } }

    private ClearableGem _clearableGem;
    public ClearableGem ClearableGem { get { return _clearableGem; } }

	private bool _isEpic = false;
    public bool IsEpic
    {
        get { return _isEpic; }
        set 
        { 
            _isEpic = value;
            if (_isLegendary == true)
            {
                _isLegendary = false;
            }
        }
    }
	private bool _isLegendary = false;
    public bool IsLegendary
    {
        get { return _isLegendary; }
        set
        {
            _isLegendary = value;
            if (_isEpic == true)
            {
                _isEpic = false;
            }
        }
    }


	private void Awake()
    {
        _movableComponent = GetComponent<MovableGem>();
        _colorComponent = GetComponent<ColorGem>();
        _clearableGem = GetComponent<ClearableGem>();
    }

    public void Init (int x, int y, Board board, Board.GemTypes gemType)
    {
        _x = x;
        _y = y; 
        _board = board;
        _gemType = gemType;
    }

    public bool IsMovable ()
    {
        return _movableComponent != null;
    }
	public bool IsColored()
	{
		return _colorComponent != null;
	}

    public bool IsClearable()
    {
        return _clearableGem != null;
    }

    private void OnMouseDown()
    {
        _board.PressedGem(this);
    }


}
