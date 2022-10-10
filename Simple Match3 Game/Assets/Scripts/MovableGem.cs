using UnityEngine;

public class MovableGem : MonoBehaviour
{
    private Gem gem;

    private void Awake()
    {
        gem = GetComponent<Gem>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void Move (int newX, int newY)
    {
        gem.X = newX;
        gem.Y = newY;

        gem.transform.position = new Vector3(newX, newY, 0);
    }
}
