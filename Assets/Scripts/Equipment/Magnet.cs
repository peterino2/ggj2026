using UnityEngine;

public class Magnet : EquipmentBase
{
    [SerializeField] float RangePerPulsePower = 100.0f;

    public override void ApplyPulse(float pulsePower)
    {
        float RangeSquared = pulsePower * RangePerPulsePower;
        RangeSquared = RangeSquared * RangeSquared;

        foreach(Scrap scrap in ScrapManager.Get().GetActiveScrap())
        {
            if(Vector3.SqrMagnitude(transform.position - scrap.transform.position) <= RangeSquared)
            {
                scrap.Magnetize(pulsePower);
            }
        }
    }
}
