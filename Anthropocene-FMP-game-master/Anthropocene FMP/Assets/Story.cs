using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Story : MonoBehaviour
{
    [TextArea(3, 10)]
    public string[] sentences;
    public TextMeshProUGUI textBox;

    bool currentlyRunning = false;
    int currentText = 0;

    private void Update()
    {
        if (currentText < sentences.Length)
        {
            if (!currentlyRunning)
            {
                StartCoroutine(displayText(sentences[currentText]));
                currentText++;
            }
        }
        else
        {
            SceneManager.LoadScene(2);
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(2);
        }
    }

    IEnumerator displayText(string nextText)
    {
        currentlyRunning = true;
        StringBuilder text = new StringBuilder();
        foreach (char c in nextText)
        {
            text.Append(c);
            textBox.SetText(text);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.4f);
        currentlyRunning = false;
    }
}
