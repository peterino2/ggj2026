// Spawn a single enemy

using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "Waves/Single")]
public class SingleWave : Wave
{
    public override bool Update()
    {
        SpawnRandom();
        return true;
    }
}
