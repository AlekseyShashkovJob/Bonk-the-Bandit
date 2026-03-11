using TMPro;
using UnityEngine;
using System.Collections;

namespace GameCore.Core
{
    public enum GameDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public int CurrentScore { get; private set; }
        public int TotalScore { get; private set; }
        public GameDifficulty CurrentDifficulty { get; private set; } = GameDifficulty.Easy;

        [SerializeField] private View.UI.UIScreen _difficultyScreen;
        [SerializeField] private View.UI.UIScreen _winScreen;
        [SerializeField] private View.UI.UIScreen _loseScreen;
        [SerializeField] private Misc.SceneManagment.SceneLoader _sceneLoader;
        [SerializeField] private Mole.MoleFieldController _controller;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _scoreText;

        private const float MAX_TIME = 60.0f;
        private Coroutine _timerCoroutine;
        private float _timeLeft;
        private bool _isGameActive = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                LoadData();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void Start()
        {
            Time.timeScale = 1.0f;
            ShowDifficultyScreen();
        }

        public void ShowDifficultyScreen()
        {
            _difficultyScreen.StartScreen();
            _isGameActive = false;
        }

        public void SetDifficulty(int difficultyIndex)
        {
            CurrentDifficulty = (GameDifficulty)difficultyIndex;
            _difficultyScreen.CloseScreen();
            StartGame();
        }

        public void StartGame()
        {
            _isGameActive = true;
            _controller.OnGameStart(); // Запуск
            CurrentScore = 0;
            _timeLeft = MAX_TIME;
            UpdateScoreUI();
            UpdateTimerUI();

            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _timerCoroutine = StartCoroutine(TimerRoutine());
        }

        public void AddScore(int amount = 1)
        {
            if (!_isGameActive) return;
            CurrentScore += amount;
            UpdateScoreUI();
        }

        public void RestartGame()
        {
            Time.timeScale = 1.0f;
            _sceneLoader.ChangeScene(Misc.Data.SceneConstants.GAME_SCENE);
        }

        public void FinishGame()
        {
            _isGameActive = false;
            Time.timeScale = 0.0f;

            bool isWin = CurrentScore >= TotalScore;
            if (isWin)
            {
                TotalScore = CurrentScore;
                SaveData();
                _winScreen.StartScreen();
            }
            else
            {
                SaveData();
                _loseScreen.StartScreen();
            }
            CurrentScore = 0;
        }

        private IEnumerator TimerRoutine()
        {
            while (_timeLeft > 0)
            {
                _timeLeft -= Time.deltaTime;
                UpdateTimerUI();
                yield return null;
            }

            _timeLeft = 0;
            UpdateTimerUI();
            OnTimeOver();
        }

        private void UpdateScoreUI()
        {
            _scoreText.text = $"{CurrentScore}";
        }

        private void UpdateTimerUI()
        {
            int seconds = Mathf.CeilToInt(_timeLeft);
            _timerText.text = $"{seconds}";
        }

        private void OnTimeOver()
        {
            StopTimer();
            FinishGame();
        }

        private void StopTimer()
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }

        private void SaveData()
        {
            PlayerPrefs.SetInt(GameConstants.TOTAL_SCORE_KEY, TotalScore);
            PlayerPrefs.Save();
        }

        private void LoadData()
        {
            TotalScore = PlayerPrefs.GetInt(GameConstants.TOTAL_SCORE_KEY, 0);
        }
    }
}