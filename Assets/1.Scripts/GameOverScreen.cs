using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
 public void RestartButton()
 {
	 SceneManager.LoadScene("IngameSceme");
 }
 
 public void RankingButton()
 {
	 SceneManager.LoadScene("RankingScene");
 }

 public void ExitButton()

 {
    SceneManager.LoadScene("MainScene");
 }

}
