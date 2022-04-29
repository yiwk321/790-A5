using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static string text = "";
    public string gameScene = "Level2";
    public string startScene = "StartScene";
    public string bossScene = "BossScene";

    private void Start()
    {
        var textBox = GameObject.Find("WinLose");
        if(textBox != null) {
            textBox.GetComponent<Text>().text = text;
        }
    }

    public void StartGame(){
        SceneManager.LoadScene(gameScene);
    }

    public void win(){
        text = "You win!";
        SceneManager.LoadScene(startScene);
    }

    public void lose(){
        text = "You lose";
        SceneManager.LoadScene(startScene);
    }

    public void loadBossScene(){
        SceneManager.LoadScene(bossScene);
    }
    
    public void reload(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}