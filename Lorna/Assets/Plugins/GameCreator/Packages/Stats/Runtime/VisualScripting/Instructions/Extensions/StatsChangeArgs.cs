using UnityEngine;

public class StatsChangeArgs {
    public StatsChangeArgs() {
    }
    public bool HitterIsPlayer = false;
    public UnityEngine.Vector3 position = default;

    public Collider         HittedCollider = null;
    //public AttackStrikeType AtkStrikeType;
    public float            DmNumber;
}