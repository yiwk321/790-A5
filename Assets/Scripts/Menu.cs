using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static string text = "";

    private void Start()
    {
        var textBox = GameObject.Find("WinLose");
        if(textBox != null) {
            textBox.GetComponent<Text>().text = text;
        }
    }

    public void StartGame(){
        SceneManager.LoadScene("parkour");
    }

    public void win(){
        text = "You win!";
        SceneManager.LoadScene("StartScene");
    }

    public void lose(){
        text = "You lose";
        SceneManager.LoadScene("StartScene");
    }
}
