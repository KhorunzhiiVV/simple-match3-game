using UnityEngine;

public class EpicGem : ClearableGem
{
    public override void Clear()
    {
        base.Clear();
        _gem.BoardRef.EpicClear(_gem);
    }
}
