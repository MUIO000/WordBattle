using UnityEngine;

public class PlayerUnit : Unit
{
    protected override void Start()
    {
        base.Start();
        isPlayerUnit = true;
    }
}