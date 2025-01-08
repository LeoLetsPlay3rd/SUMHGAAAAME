using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSceneLoader : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) // Press 'L' to test scene loading
        {
            Debug.Log("Testing scene load: NextScene");
            SceneManager.LoadScene("Phase2"); // Replace with your scene name
        }
    }
}

