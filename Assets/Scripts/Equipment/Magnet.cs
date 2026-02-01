using UnityEngine;

public class Magnet : EquipmentBase
{
    [SerializeField] float RangePerPulsePower = 100.0f;

    public override void ApplyPulse(float pulsePower)
    {
        float RangeSquared = RangePerPulsePower * RangePerPulsePower;
        foreach(Scrap scrap in ScrapManager.Get().GetActiveScrap())
        {
            if(Vector3.SqrMagnitude(transform.position - scrap.transform.position) <= RangeSquared)
            {
                scrap.Magnetize(pulsePower);
            }
        }
    }
}
