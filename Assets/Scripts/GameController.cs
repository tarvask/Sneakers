using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sneakers
{
    public class GameController
    {
        private readonly GameView _view;
        private readonly SortingController _sortingController;

        private readonly MainMenuUiController _mainMenuUiController;
        private readonly TutorialUiController _tutorialUiController;
        private readonly WinUiController _winUiController;
        private readonly LoseUiController _loseUiController;

        private GameState _currentState;
        private int _currentLevel;

#if UNITY_EDITOR
        private static GameController _instance;
#endif
        
        public GameController()
        {
#if UNITY_EDITOR
            _instance = this;
#endif
            _view = Object.FindObjectOfType<GameView>();
            _currentLevel = PlayerPrefs.GetInt(GameConstants.CurrentLevelStorageName, 1);
            
            SortingView sortingView = Object.FindObjectOfType<SortingView>();
            SortingController.Context sortingControllerContext = new SortingController.Context(sortingView);
            _sortingController = new SortingController(sortingControllerContext);
            
            _mainMenuUiController = new MainMenuUiController(_view.MainMenuUi);
            _tutorialUiController = new TutorialUiController(_view.TutorialUi);
            _winUiController = new WinUiController(_view.WinUi);
            _loseUiController = new LoseUiController(_view.LoseUi);

            ShowMainMenu(_currentLevel, _view.GameConfig.Levels[_currentLevel - 1]);
        }

        public void OuterUpdate()
        {
            if (!IsPlayingState(_currentState))
                return;
            
            if (_sortingController.AllSneakersAreSorted)
            {
                WinLevel();
                return;
            }

            if (_sortingController.Lives <= 0)
                LoseLevel();
        }

        private void ShowMainMenu(int currentLevel, LevelConfig levelConfig)
        {
            ChangeState(GameState.MainMenu);
            
            _mainMenuUiController.Show(currentLevel, () =>
            {
                StartLevel(levelConfig);
                _mainMenuUiController.Hide();
            });
        }

        private void StartLevel(LevelConfig levelConfig)
        {
            _sortingController.Init(levelConfig);

            if (!String.IsNullOrEmpty(levelConfig.TutorialText))
            {
                ShowTutorial(levelConfig.TutorialText);
            }
            else
            {
                ChangeState(GameState.Playing);
            }
        }

        private void ShowTutorial(string tutorialText)
        {
            ChangeState(GameState.Tutorial);
            _tutorialUiController.Show(tutorialText,
                () =>
                {
                    _tutorialUiController.Hide();
                    ChangeState(GameState.Playing);
                });
        }

        private void WinLevel()
        {
            ChangeState(GameState.WinLevel);
            
            if (_currentLevel < _view.GameConfig.Levels.Length)
                _currentLevel++;
            
            _winUiController.Show(_sortingController.Score,
                () =>
                {
                    _winUiController.Hide();
                    _sortingController.Clear();
                    StartLevel(_view.GameConfig.Levels[_currentLevel - 1]);
                },
                (() =>
                {
                    _winUiController.Hide();
                    _sortingController.Clear();
                    ShowMainMenu(_currentLevel, _view.GameConfig.Levels[_currentLevel - 1]);
                }));
        }

        private void LoseLevel()
        {
            ChangeState(GameState.LoseLevel);
            
            _loseUiController.Show(() =>
                {
                    // play ads, continue
                },
                () =>
                {
                    _loseUiController.Hide();
                    _sortingController.Clear();
                    StartLevel(_view.GameConfig.Levels[_currentLevel - 1]);
                },
                () =>
                {
                    _loseUiController.Hide();
                    _sortingController.Clear();
                    ShowMainMenu(_currentLevel, _view.GameConfig.Levels[_currentLevel - 1]);
                });
        }

        private void ChangeState(GameState newState)
        {
            _currentState = newState;
            Time.timeScale = IsPlayingState(newState) ? 1 : 0;
        }

        private bool IsPlayingState(GameState state)
        {
            return state == GameState.Playing;
        }

#if UNITY_EDITOR
        [MenuItem("Window/Sneakers/Win Level", false, 0)]
        public static void WinLevelInEditor()
        {
            _instance.WinLevel();
        }

        [MenuItem("Window/Sneakers/Lose Level", false, 0)]
        public static void LoseLevelInEditor()
        {
            _instance.LoseLevel();
        }
#endif
    }
}