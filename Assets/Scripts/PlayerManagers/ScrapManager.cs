using UnityEngine;
using UnityEngine.Pool;

public class ScrapManager : MonoBehaviour
{
    static ScrapManager _instance;

    

    public static ScrapManager Get()
    {
        return _instance;
    }

    private ObjectPool<Scrap> ScrapPool;

    [SerializeField] private Scrap ScrapPrefab;

    private void Awake()
    {
        _instance = this;

        ScrapPool = new ObjectPool<Scrap>(CreateScrap);
    }

    private Scrap CreateScrap()
    {
        Scrap NewScrap = Instantiate(ScrapPrefab);
        NewScrap.gameObject.SetActive(false);
        NewScrap.SetOwner(ScrapPool);

        return NewScrap;
    }

    public void SpawnScrap(float Experience)
    {


        Scrap SpawnedScrap = ScrapPool.Get();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
