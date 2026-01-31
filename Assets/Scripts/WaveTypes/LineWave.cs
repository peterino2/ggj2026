// Enemies spawn in a line. With the option to add a diagonal offset for each subsequent spawn.

using UnityEngine;

public class LineWave : Wave
{
    public float frequency = 1; // In seconds. 0.1 means 10 enemies per second.
    public float diagonal = 0; // Will move each spawn a little bit to create a diagonal line

    private float spawnDelay = 0;
    private float y = 0;

    private void Start()
    {
        y = getRandomY();
    }

    public override void Update()
    {
        spawnDelay -= Time.deltaTime;
        while (spawnDelay < 0)
        {
            spawnDelay += frequency;
            Spawn(y);

            y += diagonal;

            // Change direction if the line hit an edge
            if ((diagonal > 0 && y >= spawnRange.max) ||
                (diagonal < 0 && y <= spawnRange.min))
            {
                diagonal = -diagonal;
            }
        }

        base.Update();
    }
}
