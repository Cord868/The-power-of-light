using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    int progressAmout;
    public Slider progressSlider;

    public GameObject player;
    public GameObject LoadCanvas;
    public List<GameObject> levels;

    private int currentLevelIndex = 0;

    public GameObject gameOverScreen;
    public TMP_Text survivedText;
    private int survivedLevelsCount;

    public static event Action OnReset;

    void Start()
    {
        progressAmout = 0;
        Gem.OnGetCollect += IncreaseProgressAmout;
        progressSlider.value = 0;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayedDied += GameOverScreen;
        LoadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        MusicManager.PauseBackgroundMusic();
        survivedText.text = "ВЫ ПРОШЛИ " + survivedLevelsCount + " УРОВНЕЙ";
        //if (survivedLevelsCount != 1) survivedText.text += "S";
        Time.timeScale = 0; 
    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        MusicManager.PlayBackgroundMusic(true);
        survivedLevelsCount = 0;
        LoadLevel(0, false);
        OnReset.Invoke();
        Time.timeScale = 1;
    }

    void IncreaseProgressAmout(int amout)
    {
        progressAmout += amout;
        progressSlider.value = progressAmout;

        Debug.Log($"Прогресс: {progressAmout} / 100");
        if (progressAmout >= 100)
        {
            LoadCanvas.SetActive(true);
            Debug.Log("Уровень завершен");
        }
    }

    void LoadLevel(int level, bool wantSurvivedIncrease)
    {
        LoadCanvas.SetActive(false);

        levels[currentLevelIndex].gameObject.SetActive(false);
        levels[level].gameObject.SetActive(true);

        player.transform.position = new Vector3(0, 0, 0);

        currentLevelIndex = level;
        progressAmout = 0;
        progressSlider.value = 0;
        if(wantSurvivedIncrease) survivedLevelsCount++;
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1;
        LoadLevel(nextLevelIndex, true);
    }
}
