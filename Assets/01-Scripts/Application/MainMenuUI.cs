using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VContainer;

namespace AV.Framework.Application
{
    public sealed class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private List<Button> levelButtons = new List<Button>();
        [SerializeField] private Button quitButton;

        private LevelController levelController;
        private GameFlow gameFlow;
        private readonly List<UnityAction> levelButtonActions = new List<UnityAction>();

        [Inject]
        private void Initialize(LevelController levelController, GameFlow gameFlow)
        {
            this.levelController = levelController;
            this.gameFlow = gameFlow;

            ConfigureLevelButtons();
            ConfigureQuitButton();

            gameFlow.StateChanged += OnGameFlowStateChanged;
            SetVisible(gameFlow.State == GameFlowState.MainMenu);
        }

        private void OnDestroy()
        {
            if (gameFlow != null) gameFlow.StateChanged -= OnGameFlowStateChanged;

            for (int index = 0; index < levelButtons.Count && index < levelButtonActions.Count; index++)
            {
                if (levelButtons[index] != null && levelButtonActions[index] != null)
                {
                    levelButtons[index].onClick.RemoveListener(levelButtonActions[index]);
                }
            }

            if (quitButton != null) quitButton.onClick.RemoveListener(OnQuitClicked);
            levelButtonActions.Clear();
        }

        private void ConfigureLevelButtons()
        {
            levelButtonActions.Clear();

            for (int index = 0; index < levelButtons.Count; index++)
            {
                int levelIndex = index;
                Button button = levelButtons[index];

                if (button == null)
                {
                    levelButtonActions.Add(null);
                    continue;
                }

                UnityAction action = () => levelController.StartLevel(levelIndex);
                levelButtonActions.Add(action);
                button.onClick.AddListener(action);
                button.interactable = levelIndex < levelController.LevelCount;
            }
        }

        private void ConfigureQuitButton()
        {
            if (quitButton == null) return;

            quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnQuitClicked()
        {
            UnityEngine.Application.Quit();
        }

        private void OnGameFlowStateChanged(GameFlowState state)
        {
            SetVisible(state == GameFlowState.MainMenu);
        }

        private void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }
}
