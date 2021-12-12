using System;
using System.Collections.Generic;
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
        private readonly LegendaryInventoryUiController _legendaryInventoryUiController;
        private readonly SettingsUiController _settingsUiController;
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

        private LevelConfig CurrentLevelConfig => _view.GameConfig.Levels[_model.CurrentLevel - 1];

        public GameController()
        {
//#if UNITY_EDITOR
            _instance = this;
//#endif
            _model = new GameModel();
            _view = Object.FindObjectOfType<GameView>();

            // ui
            _mainMenuUiController = new MainMenuUiController(_view.MainMenuUi);
            _legendaryInventoryUiController = new LegendaryInventoryUiController(_view.LegendaryInventoryUi);
            _settingsUiController = new SettingsUiController(_view.SettingsUi);
            _tutorialUiController = new TutorialUiController(_view.TutorialUi);
            _winUiController = new WinUiController(_view.WinUi);
            _loseUiController = new LoseUiController(_view.LoseUi);
            _legendUiController = new LegendUiController(_view.LegendUi);
            UpgradeShopUiController.Context upgradeShopControllerContext = new UpgradeShopUiController.Context(
                _view.UpgradeShopUi,
                _view.GameConfig.WashTrackLevels, _view.GameConfig.LaceTrackLevels,
                _view.GameConfig.RegularModeBonusesConfig.BonusesParameters,
                _model.CoinsReactiveProperty,
                _model.WashTrackLevelReactiveProperty,
                _model.LaceTrackLevelReactiveProperty,
                _model.TrackFreezeBonusCountReactiveProperty,
                _model.QuickFixBonusCountReactiveProperty,
                _model.AutoUtilizationBonusCountReactiveProperty,
                _model.UndoBonusCountReactiveProperty,
                BuyBonus);
            _upgradeShopUiController = new UpgradeShopUiController(upgradeShopControllerContext);

            _cheatPanelUiController = new CheatPanelUiController(_view.CheatPanelUi, _view.GameConfig.ShowCheatPanel);
            
            // bonuses
            BonusesController.Context bonusesControllerContext = new BonusesController.Context(_model,
                SwitchFrozenState,
                WashAllDirtySneakers, LaceAllUnlacedSneakers, SetQuickWash, SetQuickLace,
                SwitchAutoUtilization, WasteAllWastedSneakers, UndoBadSorting);
            _bonusesController = new BonusesController(bonusesControllerContext);
            
            // sorting
            SortingView sortingView = Object.FindObjectOfType<SortingView>();
            SortingController.Context sortingControllerContext = new SortingController.Context(sortingView, _model,
                _bonusesController,
                ShowLegend, ApplyBonus);
            _sortingController = new SortingController(sortingControllerContext);

            ShowMainMenu(_model.CurrentLevel, CurrentLevelConfig);
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
            
            _mainMenuUiController.Show(currentLevel,
                currentLevel >= _view.GameConfig.LevelToEnableEndlessMode,
                () =>
            {
                StartLevel(currentLevel, levelConfig, _view.GameConfig.RegularModeBonusesConfig);
                _mainMenuUiController.Hide();
            },
            () =>
            {
                StartEndlessMode();
                _mainMenuUiController.Hide();
            },
                () =>
                {
                    ShowLegendaryInventory();
                    _mainMenuUiController.Hide();
                },
                () =>
                {
                    ShowSettings();
                    _mainMenuUiController.Hide();
                });
        }

        private void StartLevel(int levelNumber, LevelConfig levelConfig, BonusesParametersConfig bonusesParametersConfig)
        {
            _sortingController.Init(levelNumber, levelConfig,
                _view.GameConfig.WashTrackLevels[_model.WashTrackLevelReactiveProperty.Value],
                _view.GameConfig.LaceTrackLevels[_model.LaceTrackLevelReactiveProperty.Value],
                bonusesParametersConfig.BonusesParameters);

            if (!String.IsNullOrEmpty(levelConfig.TutorialText))
            {
                ShowTutorial(levelConfig);
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

        private void StartEndlessMode()
        {
            StartLevel(-1, _view.GameConfig.EndlessLevel, _view.GameConfig.EndlessModeBonusesConfig);
        }

        private void ShowLegendaryInventory()
        {
            Dictionary<int, int> currentLegends = StringToDict(PlayerPrefs.GetString(GameConstants.CollectedLegendarySneakersStorageName, ""));
            
            _legendaryInventoryUiController.Show(currentLegends, () =>
            {
                ShowMainMenu(_model.CurrentLevel, CurrentLevelConfig);
                _legendaryInventoryUiController.Hide();
            });
        }

        private void ShowSettings()
        {
            _settingsUiController.Show(() =>
            {
                ShowMainMenu(_model.CurrentLevel, CurrentLevelConfig);
                _settingsUiController.Hide();
            });
        }

        private void ShowTutorial(LevelConfig levelConfig)
        {
            ChangeState(GameState.Tutorial);
            _tutorialUiController.Show(levelConfig.TutorialHeader, levelConfig.TutorialText,
                () =>
                {
                    _tutorialUiController.Hide();

                    if (levelConfig.ShowUpgradeShop)
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
                    StartLevel(_model.CurrentLevel, CurrentLevelConfig, _view.GameConfig.RegularModeBonusesConfig);
                });
        }

        private void LoseLevel()
        {
            ChangeState(GameState.LoseLevel);
            SaveResources();
            
            _loseUiController.Show(() =>
                {
                    // play ads, continue
                },
                () =>
                {
                    _loseUiController.Hide();
                    _sortingController.Clear();
                    
                    if (_sortingController.CurrentLevelConfig.NumberOfSneakers > 0)
                        StartLevel(_model.CurrentLevel, CurrentLevelConfig, _view.GameConfig.RegularModeBonusesConfig);
                    else
                        StartEndlessMode();
                },
                () =>
                {
                    _loseUiController.Hide();
                    _sortingController.Clear();
                    ShowMainMenu(_model.CurrentLevel, CurrentLevelConfig);
                });
        }

        private void ShowLegend(int legendarySneakerId)
        {
            ChangeState(GameState.CollectLegend);
            // find config of legendary sneaker
            SneakerConfig legendarySneakerConfig = Array.Find(CurrentLevelConfig.PossibleSneakers,
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

            _upgradeShopUiController.Show(
                _sortingController.CurrentLevelConfig.FreezeTrackBonusLimitations,
                _sortingController.CurrentLevelConfig.QuickFixBonusLimitations,
                _sortingController.CurrentLevelConfig.AutoUtilizationBonusLimitations,
                _sortingController.CurrentLevelConfig.UndoBonusLimitations,
                () =>
                {
                    int washTrackNextLevelIndex = _model.WashTrackLevelReactiveProperty.Value + 1;
                    
                    if (!_model.SpendMoney(_view.GameConfig.WashTrackLevels[washTrackNextLevelIndex].Price))
                        return;
                    
                    _model.UpgradeWashMachine();
                    _sortingController.UpgradeWashTrack(_view.GameConfig.WashTrackLevels[_model.WashTrackLevelReactiveProperty.Value]);
                },
                () =>
                {
                    int laceTrackNextLevelIndex = _model.LaceTrackLevelReactiveProperty.Value + 1;
                    
                    if (!_model.SpendMoney(_view.GameConfig.LaceTrackLevels[laceTrackNextLevelIndex].Price))
                        return;
                    
                    _model.UpgradeLaceMachine();
                    _sortingController.UpgradeLaceTrack(_view.GameConfig.LaceTrackLevels[_model.LaceTrackLevelReactiveProperty.Value]);
                },
                () =>
                {
                    _upgradeShopUiController.Hide();
                    ChangeState(GameState.Playing);
                });
        }

        private void BuyBonus(BonusShopType bonusType)
        {
            int bonusPrice = _view.GameConfig.RegularModeBonusesConfig.BonusesParameters.GetBonusPrice(bonusType);
            
            if (!_model.SpendMoney(bonusPrice))
                return;

            switch (bonusType)
            {
                case BonusShopType.TrackFreeze:
                    _model.AddBonus(BonusShopType.TrackFreeze);
                    break;
                case BonusShopType.QuickFix:
                    _model.AddBonus(BonusShopType.QuickFix);
                    break;
                case BonusShopType.AutoUtilization:
                    _model.AddBonus(BonusShopType.AutoUtilization);
                    break;
                case BonusShopType.Undo:
                    _model.AddBonus(BonusShopType.Undo);
                    break;
            }
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

            SaveResources();
        }

        private void SaveResources()
        {
            // save coins
            _model.AddMoney(_sortingController.Score);
            
            // save best in endless mode
            if (_sortingController.CurrentLevelConfig.NumberOfSneakers < 0)
            {
                _model.SetBestResult(_sortingController.Score);
            }
            
            // bonuses
            PlayerPrefs.SetInt(GameConstants.TrackFreezeBonusCountStorageName, _model.TrackFreezeBonusCountReactiveProperty.Value);
            PlayerPrefs.SetInt(GameConstants.QuickFixBonusCountStorageName, _model.QuickFixBonusCountReactiveProperty.Value);
            PlayerPrefs.SetInt(GameConstants.AutoUtilizationBonusCountStorageName, _model.AutoUtilizationBonusCountReactiveProperty.Value);
            PlayerPrefs.SetInt(GameConstants.UndoBonusCountStorageName, _model.UndoBonusCountReactiveProperty.Value);
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

        private void WashAllDirtySneakers()
        {
            _sortingController.WashAllDirtySneakers();
        }

        private void LaceAllUnlacedSneakers()
        {
            _sortingController.LaceAllUnlacedSneakers();
        }

        private void WasteAllWastedSneakers()
        {
            _sortingController.WasteAllWastedSneakers();
        }

        private void SetQuickWash(float processDuration)
        {
            _sortingController.SetQuickWash(processDuration);
        }

        private void SetQuickLace(float processDuration)
        {
            _sortingController.SetQuickLace(processDuration);
        }

        private void SwitchAutoUtilization(bool isActive)
        {
            _sortingController.SwitchAutoUtilization(isActive);
        }

        private void UndoBadSorting()
        {
            _sortingController.UndoBadSorting();
        }

        private void ApplyBonus(BonusShopType bonusType)
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