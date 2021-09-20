namespace Sneakers
{
    public static class GameConstants
    {
        public const float SuperCloseDistanceSqr = 0.0001f;
        
        public const string CurrentLevelStorageName = "storage/level";
        public const string CurrentWashTrackLevelStorageName = "storage/washtrack";
        public const string CurrentLaceTrackLevelStorageName = "storage/lacetrack";
        public const string CoinsStorageName = "storage/coins";
        public const string CollectedLegendarySneakersStorageName = "storage/legendaries";
        public const string BestResultStorageName = "storage/best";
        
        // bonuses
        public const string TrackFreezeBonusCountStorageName = "storage/bonus/trackfreeze";
        public const string QuickFixWashBonusCountStorageName = "storage/bonus/quickfixwash";
        public const string QuickFixLaceBonusCountStorageName = "storage/bonus/quickfixlace";
        public const string AutoUtilizationBonusCountStorageName = "storage/bonus/autounitization";
        public const string UndoBonusCountStorageName = "storage/bonus/undo";
    }
}