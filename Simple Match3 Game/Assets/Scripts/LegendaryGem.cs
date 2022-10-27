using UnityEngine;

public class LegendaryGem : ClearableGem
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
        gem.BoardRef.LegendaryClear(gem);
    }
}
