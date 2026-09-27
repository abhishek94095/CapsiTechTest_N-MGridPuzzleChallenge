using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace AV.Framework.Application
{
    public sealed class GameResultUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject inGamePanel;

        [Header("Win Panel")]
        [SerializeField] private TextMeshProUGUI winTitleText;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button winRetryButton;
        [SerializeField] private Button winMainMenuButton;

        [Header("Game Over Panel")]
        [SerializeField] private TextMeshProUGUI gameOverTitleText;
        [SerializeField] private Button gameOverRetryButton;
        [SerializeField] private Button gameOverMainMenuButton;

        private GameFlow gameFlow;
        private LevelController levelController;

        private void Awake()
        {
            HideAllResultPanels();
        }

        [Inject]
        private void Initialize(GameFlow gameFlow, LevelController levelController)
        {
            this.gameFlow = gameFlow;
            this.levelController = levelController;

            ConfigureButtons();

            gameFlow.StateChanged += OnGameFlowStateChanged;
            UpdatePanels(gameFlow.State);
        }

        private void OnDestroy()
        {
            if (gameFlow != null) gameFlow.StateChanged -= OnGameFlowStateChanged;

            RemoveButtonListeners();
        }

        [ContextMenu("Validate References")]
        private void ValidateReferences()
        {
            Debug.Assert(winPanel != null, $"{nameof(GameResultUI)}: Win Panel is not assigned.", this);
            Debug.Assert(gameOverPanel != null, $"{nameof(GameResultUI)}: Game Over Panel is not assigned.", this);
            Debug.Assert(winTitleText != null, $"{nameof(GameResultUI)}: Win Title Text is not assigned.", this);
            Debug.Assert(nextLevelButton != null, $"{nameof(GameResultUI)}: Next Level Button is not assigned.", this);
            Debug.Assert(winRetryButton != null, $"{nameof(GameResultUI)}: Win Retry Button is not assigned.", this);
            Debug.Assert(winMainMenuButton != null, $"{nameof(GameResultUI)}: Win Main Menu Button is not assigned.", this);
            Debug.Assert(gameOverTitleText != null, $"{nameof(GameResultUI)}: Game Over Title Text is not assigned.", this);
            Debug.Assert(gameOverRetryButton != null, $"{nameof(GameResultUI)}: Game Over Retry Button is not assigned.", this);
            Debug.Assert(gameOverMainMenuButton != null, $"{nameof(GameResultUI)}: Game Over Main Menu Button is not assigned.", this);
        }

        private void ConfigureButtons()
        {
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            winRetryButton.onClick.AddListener(OnRetryLevelClicked);
            winMainMenuButton.onClick.AddListener(OnMainMenuClicked);
            gameOverRetryButton.onClick.AddListener(OnRetryLevelClicked);
            gameOverMainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        private void RemoveButtonListeners()
        {
            nextLevelButton.onClick.RemoveListener(OnNextLevelClicked);
            winRetryButton.onClick.RemoveListener(OnRetryLevelClicked);
            winMainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
            gameOverRetryButton.onClick.RemoveListener(OnRetryLevelClicked);
            gameOverMainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        }

        private void OnNextLevelClicked()
        {
            if (gameFlow.State != GameFlowState.Won) return;

            levelController.NextLevel();
        }

        private void OnRetryLevelClicked()
        {
            if (gameFlow.State != GameFlowState.Won && gameFlow.State != GameFlowState.Lost) return;

            levelController.RetryLevel();
        }

        private void OnMainMenuClicked()
        {
            levelController.ReturnToMainMenu();
        }

        private void OnGameFlowStateChanged(GameFlowState state)
        {
            UpdatePanels(state);
        }

        private void UpdatePanels(GameFlowState state)
        {
            HideAllResultPanels();

            if (state == GameFlowState.Won)
            {
                ShowWinPanel();
                return;
            }

            if (state == GameFlowState.Lost)
            {
                ShowGameOverPanel();
            }

            if (inGamePanel != null) inGamePanel.SetActive(GameFlowState.Playing == state);
        }

        private void HideAllResultPanels()
        {
            if (winPanel != null) winPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
        }

        private void ShowWinPanel()
        {
            if (winPanel == null || winTitleText == null || nextLevelButton == null) return;

            winTitleText.text = "Level Complete";
            nextLevelButton.interactable = levelController.HasNextLevel;
            winPanel.SetActive(true);
        }

        private void ShowGameOverPanel()
        {
            if (gameOverPanel == null || gameOverTitleText == null) return;

            gameOverTitleText.text = "Game Over";
            gameOverPanel.SetActive(true);
        }
    }
}
