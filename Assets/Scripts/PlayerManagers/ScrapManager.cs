using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class ScrapManager : MonoBehaviour
{
    static ScrapManager _instance;

    [SerializeField] private List<float> ScrapXPAmounts;
    [SerializeField] private float ScrapMovementSpeed = 200.0f;
    [SerializeField] private float ScrapCollectionRange = 50.0f;
    [SerializeField] private float ScrapPositionOffset = 30.0f;

    private HashSet<Scrap> ActiveScrap;

    public static ScrapManager Get()
    {
        return _instance;
    }

    private ObjectPool<Scrap> ScrapPool;

    [SerializeField] private List<Scrap> ScrapPrefabs;

    private int CurScrapIndex = 0;

    private void Awake()
    {
        _instance = this;

        ScrapPool = new ObjectPool<Scrap>(CreateScrap, null, ScrapOnRelease);

        ActiveScrap = new HashSet<Scrap>();
    }

    private void ScrapOnRelease(Scrap scrap)
    {
        ActiveScrap.Remove(scrap);
    }

    private Scrap CreateScrap()
    {
        Scrap NewScrap = Instantiate(ScrapPrefabs[CurScrapIndex]);
        NewScrap.gameObject.SetActive(false);
        NewScrap.SetOwner(ScrapPool);
        NewScrap.transform.SetParent(transform.parent);

        ++CurScrapIndex;

        if(CurScrapIndex >= ScrapPrefabs.Count)
        {
            CurScrapIndex = 0;
        }

        return NewScrap;
    }

    public void SpawnScrap(Vector3 Position, float Experience)
    {
        float RemainingXP = Experience;
        for(int i = ScrapXPAmounts.Count - 1; i>=0; i--)
        {
            while(RemainingXP >= ScrapXPAmounts[i])
            {
                Scrap SpawnedScrap = ScrapPool.Get();
                SpawnedScrap.SetXP(ScrapXPAmounts[i]);
                SpawnedScrap.SetVelocity(new Vector3(-ScrapMovementSpeed, 0.0f, 0.0f));
                SpawnedScrap.SetCollectionRange(ScrapCollectionRange);
                SpawnedScrap.SetPlayerTransformReference(transform);
                SpawnedScrap.transform.position = Position + new Vector3(Random.Range(-ScrapPositionOffset, ScrapPositionOffset), Random.Range(-ScrapPositionOffset, ScrapPositionOffset), 0);
                SpawnedScrap.gameObject.SetActive(true);
                ActiveScrap.Add(SpawnedScrap);

                RemainingXP -= ScrapXPAmounts[i];

                if(RemainingXP == 0)
                {
                    return;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
