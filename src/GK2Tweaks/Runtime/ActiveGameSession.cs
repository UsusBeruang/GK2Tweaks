namespace GK2Tweaks.Runtime
{
    internal static class ActiveGameSession
    {
        private const int GameplayState = 1;

        public static bool TryGet(out MainGame game)
        {
            game = MainGame.Instance;
            return game != null && (int)game.gameState == GameplayState;
        }

        public static bool TryGetSave(
            out SaveSlotData slot,
            out GameSave save)
        {
            slot = null;
            save = null;

            if (!TryGet(out MainGame game))
                return false;

            slot = game.SaveSlotData;
            save = game.GameSave;
            return slot != null && save != null;
        }
    }
}
