using UnityEngine;

public class LegendaryGem : ClearableGem
{
    public override void Clear()
    {
        base.Clear();
        _gem.BoardRef.LegendaryClear(_gem);
    }
}
