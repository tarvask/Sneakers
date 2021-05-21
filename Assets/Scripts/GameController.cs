using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
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
        private readonly LegendUiController _legendUiController;
        private readonly UpgradeShopUiController _upgradeShopUiController;

        private readonly CheatPanelUiController _cheatPanelUiController;

        private GameState _currentState;
        private int _currentLevel;
        private readonly ReactiveProperty<int> _coinsReactiveProperty;
        private readonly ReactiveProperty<int> _washTrackLevel;
        private readonly ReactiveProperty<int> _laceTrackLevel;

//#if UNITY_EDITOR
        private static GameController _instance;
//#endif

        public GameController()
        {
//#if UNITY_EDITOR
            _instance = this;
//#endif
            _view = Object.FindObjectOfType<GameView>();
            _currentLevel = PlayerPrefs.GetInt(GameConstants.CurrentLevelStorageName, 1);
            _coinsReactiveProperty = new ReactiveProperty<int>(PlayerPrefs.GetInt(GameConstants.CoinsStorageName, 0));
            _washTrackLevel = new ReactiveProperty<int>(PlayerPrefs.GetInt(GameConstants.CurrentWashTrackLevelStorageName, 0));
            _laceTrackLevel = new ReactiveProperty<int>(PlayerPrefs.GetInt(GameConstants.CurrentLaceTrackLevelStorageName, 0));
            
            SortingView sortingView = Object.FindObjectOfType<SortingView>();
            SortingController.Context sortingControllerContext = new SortingController.Context(sortingView, ShowLegend);
            _sortingController = new SortingController(sortingControllerContext);
            
            _mainMenuUiController = new MainMenuUiController(_view.MainMenuUi);
            _tutorialUiController = new TutorialUiController(_view.TutorialUi);
            _winUiController = new WinUiController(_view.WinUi);
            _loseUiController = new LoseUiController(_view.LoseUi);
            _legendUiController = new LegendUiController(_view.LegendUi);
            _upgradeShopUiController = new UpgradeShopUiController(_view.UpgradeShopUi);
            
            _cheatPanelUiController = new CheatPanelUiController(_view.CheatPanelUi, _view.GameConfig.ShowCheatPanel);

            ShowMainMenu(_currentLevel, _view.GameConfig.Levels[_currentLevel - 1]);
        }

        public void OuterUpdate()
        {
            if (!IsPlayingState(_currentState))
                return;

            if (_sortingController.Lives <= 0)
            {
                LoseLevel();
                return;
            }

            if (_sortingController.AllSneakersAreSorted)
                WinLevel();
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
            _sortingController.Init(levelConfig,
                _view.GameConfig.WashTrackLevels[_washTrackLevel.Value],
                _view.GameConfig.LaceTrackLevels[_laceTrackLevel.Value]);

            if (!String.IsNullOrEmpty(levelConfig.TutorialText))
            {
                ShowTutorial(levelConfig.TutorialText);
            }
            else if (levelConfig.ShowUpgradeShop)
            {
                ShowUpgradeShop();
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

                    if (_view.GameConfig.Levels[_currentLevel - 1].ShowUpgradeShop)
                    {
                        ShowUpgradeShop();
                    }
                    else
                    {
                        ChangeState(GameState.Playing);
                    }
                });
        }

        private void WinLevel()
        {
            ChangeState(GameState.WinLevel);
            SaveProgress();

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

        private void ShowLegend(int legendarySneakerId)
        {
            ChangeState(GameState.CollectLegend);
            // find config of legendary sneaker
            SneakerConfig legendarySneakerConfig = Array.Find(_view.GameConfig.Levels[_currentLevel - 1].PossibleSneakers,
                (sneakerParams) => sneakerParams.Config.Id == legendarySneakerId).Config;
            _legendUiController.Show(legendarySneakerConfig,
                () =>
                {
                    _legendUiController.Hide();
                    ChangeState(GameState.Playing);
                });
        }

        private void ShowUpgradeShop()
        {
            ChangeState(GameState.UpgradeShop);

            _upgradeShopUiController.Show(_coinsReactiveProperty,
                _view.GameConfig.WashTrackLevels,
                _view.GameConfig.LaceTrackLevels,
                _washTrackLevel, _laceTrackLevel,
                () =>
                {
                    _washTrackLevel.Value++;
                    PlayerPrefs.SetInt(GameConstants.CurrentWashTrackLevelStorageName, _washTrackLevel.Value);
                    _sortingController.UpgradeWashTrack(_view.GameConfig.WashTrackLevels[_washTrackLevel.Value]);
                    
                    _coinsReactiveProperty.Value -= _view.GameConfig.WashTrackLevels[_washTrackLevel.Value].Price;
                    PlayerPrefs.SetInt(GameConstants.CoinsStorageName, _coinsReactiveProperty.Value);
                },
                () =>
                {
                    _laceTrackLevel.Value++;
                    PlayerPrefs.SetInt(GameConstants.CurrentLaceTrackLevelStorageName, _laceTrackLevel.Value);
                    _sortingController.UpgradeLaceTrack(_view.GameConfig.LaceTrackLevels[_laceTrackLevel.Value]);
                    
                    _coinsReactiveProperty.Value -= _view.GameConfig.LaceTrackLevels[_laceTrackLevel.Value].Price;
                    PlayerPrefs.SetInt(GameConstants.CoinsStorageName, _coinsReactiveProperty.Value);
                },
                (() =>
                {
                    _upgradeShopUiController.Hide();
                    ChangeState(GameState.Playing);
                }));
        }

        private void SaveProgress()
        {
            // save current level
            if (_currentLevel < _view.GameConfig.Levels.Length)
            {
                _currentLevel++;
                PlayerPrefs.SetInt(GameConstants.CurrentLevelStorageName, _currentLevel);
            }

            // save legends
            Dictionary<int, int> currentLegends = StringToDict(PlayerPrefs.GetString(GameConstants.CollectedLegendarySneakersStorageName, ""));

            foreach (KeyValuePair<int, int> legendPair in _sortingController.CollectedLegendarySneakers)
            {
                if (currentLegends.ContainsKey(legendPair.Key))
                    currentLegends[legendPair.Key] += legendPair.Value;
                else
                    currentLegends.Add(legendPair.Key, legendPair.Value);
            }
            
            PlayerPrefs.SetString(GameConstants.CollectedLegendarySneakersStorageName, DictToString(currentLegends));
            
            // save coins
            _coinsReactiveProperty.Value += _sortingController.Score;
            PlayerPrefs.SetInt(GameConstants.CoinsStorageName, _coinsReactiveProperty.Value);
        }

        private void DropProgress()
        {
            // level
            _currentLevel = 1;
            PlayerPrefs.SetInt(GameConstants.CurrentLevelStorageName, _currentLevel);
            // tracks
            _washTrackLevel.Value = 0;
            PlayerPrefs.SetInt(GameConstants.CurrentWashTrackLevelStorageName, _washTrackLevel.Value);
            _laceTrackLevel.Value = 0;
            PlayerPrefs.SetInt(GameConstants.CurrentLaceTrackLevelStorageName, _laceTrackLevel.Value);
            // coins
            _coinsReactiveProperty.Value = 0;
            PlayerPrefs.SetInt(GameConstants.CoinsStorageName, 0);
            // legends
            PlayerPrefs.SetString(GameConstants.CollectedLegendarySneakersStorageName, "");
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

        private string DictToString(Dictionary<int, int> dictionary)
        {
            return string.Join(";", dictionary);
        }

        private Dictionary<int, int> StringToDict(string legendarySneakersString)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            string[] items = legendarySneakersString.Split(';');

            foreach (string item in items)
            {
                string[] keyValueTokens = item.Split(',');
                
                if (keyValueTokens.Length == 2)
                    result.Add(int.Parse(keyValueTokens[0].Trim('[')), int.Parse(keyValueTokens[1].Trim(']')));
            }

            return result;
        }

        //[MenuItem("Window/Sneakers/Drop progress", false, 0)]
        public static void DropProgressInEditor()
        {
            _instance.DropProgress();
        }
        
        //[MenuItem("Window/Sneakers/Win Level", false, 0)]
        public static void WinLevelInEditor()
        {
            _instance.WinLevel();
        }

        //[MenuItem("Window/Sneakers/Lose Level", false, 0)]
        public static void LoseLevelInEditor()
        {
            _instance.LoseLevel();
        }

        public static void AddCoinsInEditor()
        {
            _instance._coinsReactiveProperty.Value += 100;
        }
        
        public static void RemoveCoinsInEditor()
        {
            if (_instance._coinsReactiveProperty.Value >= 100)
                _instance._coinsReactiveProperty.Value -= 100;
        }
    }
}