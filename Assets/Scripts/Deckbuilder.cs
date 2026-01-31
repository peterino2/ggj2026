using UnityEngine;

public class Deckbuilder : MonoBehaviour
{
    private static Deckbuilder gDeckBuilder;
    
    // to use me 
    // do Deckbuilder.GetInstance().GraphUpdates += this.graphUpdate
    public delegate void GraphUpdate(Deckbuilder deckbuilder);

    public GraphUpdate GraphUpdates;

    public Deckbuilder GetInstance()
    {
        return gDeckBuilder;
    }

    void Awake()
    {
        gDeckBuilder = this;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is create
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
