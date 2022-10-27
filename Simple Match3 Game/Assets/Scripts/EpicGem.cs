using UnityEngine;

public class EpicGem : ClearableGem
{
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public override void Clear()
    {
        base.Clear();
        gem.BoardRef.EpicClear(gem);
    }
}
