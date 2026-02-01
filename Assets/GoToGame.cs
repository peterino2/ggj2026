using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void GoGame()
    {
        SceneManager.LoadScene("DeckBuilderGym");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
