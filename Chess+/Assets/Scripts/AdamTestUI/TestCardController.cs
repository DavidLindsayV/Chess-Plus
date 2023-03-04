using UnityEngine;
using UnityEngine.UI;

public class TestCardController : MonoBehaviour
{
    public Canvas canvas;
    public GameObject[] buttonPrefabs;
    public int numButtons = 4;
    public float buttonSpacing = 10f;
    public float bottomPosition = -600;

    void Start()
    {
        // Calculate the total width of the buttons based on their width and spacing
        float totalWidth = 0f;
        foreach (GameObject buttonPrefab in buttonPrefabs)
        {
            RectTransform buttonRect = buttonPrefab.GetComponent<RectTransform>();
            totalWidth += buttonRect.rect.width*2 + buttonSpacing;
        }
        // Calculate the x position of the first button based on the total width and the canvas size
        float startX = canvas.pixelRect.width / 2f - totalWidth / 2f;

        // Shuffle the button prefabs array so we get a random order each time
        ShuffleArray(buttonPrefabs);

        // Instantiate the button prefabs and position them at the bottom of the canvas
        for (int i = 0; i < numButtons; i++)
        {
            GameObject buttonPrefab = buttonPrefabs[i % buttonPrefabs.Length];
            GameObject buttonInstance = Instantiate(buttonPrefab);
            buttonInstance.transform.SetParent(canvas.transform, false);
            RectTransform buttonRect = buttonInstance.GetComponent<RectTransform>();
            buttonRect.anchoredPosition = new Vector2((i * (buttonRect.rect.width * 2f)) + (buttonSpacing*i) -  startX, bottomPosition);
        }
    }

    void Update(){
        
    }

    void ShuffleArray<T>(T[] array)
    {
        // Fisher-Yates shuffle algorithm
        for (int i = array.Length - 1; i >= 1; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = array[j];
            array[j] = array[i];
            array[i] = temp;
        }
    }
}
