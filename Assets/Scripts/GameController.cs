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
        private readonly BonusesController _bonusesController;

        private readonly MainMenuUiController _mainMenuUiController;
        private readonly TutorialUiController _tutorialUiController;
        private readonly WinUiController _winUiController;
        private readonly LoseUiController _loseUiController;
        private readonly LegendUiController _legendUiController;
        private readonly UpgradeShopUiController _upgradeShopUiController;

        private readonly CheatPanelUiController _cheatPanelUiController;

        private readonly GameModel _model;

//#if UNITY_EDITOR
        private static GameController _instance;
        //#endif

        public GameController()
        {
//#if UNITY_EDITOR
            _instance = this;
//#endif
            _model = new GameModel();
            _view = Object.FindObjectOfType<GameView>();
            
            BonusesController.Context bonusesControllerContext = new BonusesController.Context(_model,
                _view.GameConfig.BonusesParameters, SwitchFrozenState, WashAllSneakers, LaceAllSneakers, SwitchAutoUtilization, UndoBadSorting);
            _bonusesController = new BonusesController(bonusesControllerContext);

            SortingView sortingView = Object.FindObjectOfType<SortingView>();
            SortingController.Context sortingControllerContext = new SortingController.Context(sortingView, _model,
                _view.GameConfig.BonusesParameters,
                _bonusesController,
                ShowLegend, ApplyBonus);
            _sortingController = new SortingController(sortingControllerContext);

            _mainMenuUiController = new MainMenuUiController(_view.MainMenuUi);
            _tutorialUiController = new TutorialUiController(_view.TutorialUi);
            _winUiController = new WinUiController(_view.WinUi);
            _loseUiController = new LoseUiController(_view.LoseUi);
            _legendUiController = new LegendUiController(_view.LegendUi);
            _upgradeShopUiController = new UpgradeShopUiController(_view.UpgradeShopUi);

            _cheatPanelUiController = new CheatPanelUiController(_view.CheatPanelUi, _view.GameConfig.ShowCheatPanel);

            ShowMainMenu(_model.CurrentLevel, _view.GameConfig.Levels[_model.CurrentLevel - 1]);
        }

        public void OuterUpdate(float frameLength)
        {
            _bonusesController.OuterUpdate(frameLength);
            
            if (!_model.IsPlayingState(_model.CurrentState))
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
                _view.GameConfig.WashTrackLevels[_model.WashTrackLevelReactiveProperty.Value],
                _view.GameConfig.LaceTrackLevels[_model.LaceTrackLevelReactiveProperty.Value]);

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

                    if (_view.GameConfig.Levels[_model.CurrentLevel - 1].ShowUpgradeShop)
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
            _sortingController.CountScore();
            SaveProgress();

            _winUiController.Show(_sortingController.Score,
                () =>
                {
                    _winUiController.Hide();
                    _sortingController.Clear();
                    StartLevel(_view.GameConfig.Levels[_model.CurrentLevel - 1]);
                },
                () =>
                {
                    _winUiController.Hide();
                    _sortingController.Clear();
                    ShowMainMenu(_model.CurrentLevel, _view.GameConfig.Levels[_model.CurrentLevel - 1]);
                });
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
                    StartLevel(_view.GameConfig.Levels[_model.CurrentLevel - 1]);
                },
                () =>
                {
                    _loseUiController.Hide();
                    _sortingController.Clear();
                    ShowMainMenu(_model.CurrentLevel, _view.GameConfig.Levels[_model.CurrentLevel - 1]);
                });
        }

        private void ShowLegend(int legendarySneakerId)
        {
            ChangeState(GameState.CollectLegend);
            // find config of legendary sneaker
            SneakerConfig legendarySneakerConfig = Array.Find(_view.GameConfig.Levels[_model.CurrentLevel - 1].PossibleSneakers,
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

            _upgradeShopUiController.Show(_model.CoinsReactiveProperty,
                _view.GameConfig.WashTrackLevels,
                _view.GameConfig.LaceTrackLevels,
                _model.WashTrackLevelReactiveProperty, _model.LaceTrackLevelReactiveProperty,
                () =>
                {
                    if (!_model.SpendMoney(_view.GameConfig.WashTrackLevels[_model.WashTrackLevelReactiveProperty.Value].Price))
                        return;
                    
                    _model.UpgradeWashMachine();
                    _sortingController.UpgradeWashTrack(_view.GameConfig.WashTrackLevels[_model.WashTrackLevelReactiveProperty.Value]);
                },
                () =>
                {
                    if (!_model.SpendMoney(_view.GameConfig.LaceTrackLevels[_model.LaceTrackLevelReactiveProperty.Value].Price))
                        return;
                    
                    _model.UpgradeLaceMachine();
                    _sortingController.UpgradeLaceTrack(_view.GameConfig.LaceTrackLevels[_model.LaceTrackLevelReactiveProperty.Value]);
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
            if (_model.CurrentLevel < _view.GameConfig.Levels.Length)
            {
                _model.NextLevel();
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
            _model.AddMoney(_sortingController.Score);
        }

        private void ChangeState(GameState newState)
        {
            _model.ChangeState(newState);
            Time.timeScale = _model.IsPlayingState(_model.CurrentState) ? 1 : 0;
        }

        private void SwitchFrozenState(bool enabled)
        {
            if (enabled)
                ChangeState(GameState.Frozen);
            // don't change state if some other activated
            else if (_model.CurrentState == GameState.Frozen)
                ChangeState(GameState.Playing);
        }

        private void WashAllSneakers()
        {
            _sortingController.WashAllSneakers();
        }

        private void LaceAllSneakers()
        {
            _sortingController.LaceAllSneakers();
        }

        private void SwitchAutoUtilization(bool isActive)
        {
            _sortingController.SwitchAutoUtilization(isActive);
        }

        private void UndoBadSorting()
        {
            _sortingController.UndoBadSorting();
        }

        private void ApplyBonus(BonusType bonusType)
        {
            _bonusesController.ApplyBonus(bonusType);
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
            _instance._model.DropProgress();
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
            _instance._model.AddMoney(100);
        }
        
        public static void RemoveCoinsInEditor()
        {
            if (_instance._model.CoinsReactiveProperty.Value >= 100)
                _instance._model.SpendMoney(100);
        }
    }
}