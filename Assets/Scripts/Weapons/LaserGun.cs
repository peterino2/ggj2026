using UnityEngine;

public class LaserGun : WeaponBase
{
    [SerializeField] LaserBeam LaserBeamPrefab;

    [SerializeField] float PulseFireThreshold = 2.0f;

    [SerializeField] float PulsePowerPerSecond = 1.0f;

    LaserBeam Beam;

    float CurPulsePower = 0.0f;

    bool Firing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Beam = Instantiate(LaserBeamPrefab);
        Beam.gameObject.SetActive(false);
        Beam.gameObject.transform.position = gameObject.transform.position;
        Beam.gameObject.transform.SetParent(gameObject.transform);
        
    }

    public override void ApplyPulse(float pulsePower)
    {
        base.ApplyPulse(pulsePower);

        CurPulsePower += pulsePower;
    }

    // Update is called once per frame
    void Update()
    {
        // TEST CODE REMOVE
        // CurPulsePower += Time.deltaTime * PulsePowerPerSecond * 7.5f;

        if(Firing)
        {
            CurPulsePower -= PulsePowerPerSecond * Time.deltaTime;

            if(CurPulsePower <= 0.0f)
            {
                Firing = false;
                Beam.gameObject.SetActive(false);
            }
        }
        else
        {
            if(CurPulsePower >= PulseFireThreshold)
            {
                Firing = true;
                Beam.gameObject.SetActive(true);
            }
        }

        if(Firing)
        {
            Beam.SetCurrentPower(CurPulsePower);
        }
    }
}
