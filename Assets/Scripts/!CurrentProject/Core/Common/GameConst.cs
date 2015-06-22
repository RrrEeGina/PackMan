
namespace Pacman
{
    public static class GameConst
    {
        #region Technical
        public const float UNITS_PER_CELL = 10;
        public const float CAMERA_Z = -10;

        public const float SQR_INPUT_LIMIT = 0.9f;
        public const string AXIS_X = "Horizontal";
        public const string AXIS_Y = "Vertical";

        public const float JUMP_RATIO_LIMIT = 0.6f;
        public const float JUMP_OFFSET = 0.2f;

        private const float base_accuracy = 0.05f;
        public static float ACCURACY { get { return base_accuracy * UNITS_PER_CELL; } }

        public const string REBUILDING_OBJECT_TAG = "RebuildingObject";

        public const int DEFAULT_SLEEP_INTERVAL = 100;
        #endregion
        #region Gameplay
        #region Player
        public const int LIFE_COUNT = 3;
        #endregion
        #region Speed&Slow
        public const float BASE_PACMAN_SPEED = 5f;
        public const float BASE_SPOOK_SPEED = 5f;

        public const float PACKMAN_SLOW_K = 0.75f;
        public const float SPOOK_SLOW_K = 0.5f;
        #endregion
        #region Size
        public const float COOCKIE_SIZE = 0.25f;
        public const float DRUG_SIZE = 0.5f;
        public const float CHERRY_SIZE = 0.75f;

        public const float PACKMAN_SIZE = 1;
        public const float SPOOK_SIZE = 1;
        #endregion
        #region CherryRoutine
        public const float CHERRY_ENABLE_INTERVAL = 10;
        public const float CHERRY_DISABLE_INTERVAL = 10;
        public const float CHERRY_PULS_START = 5;
        public const float CHERRY_COROUTINE_TICK_RATE = 1;
        #endregion
        #region Score
        public const int COOCKIE_SCORE = 10;
        public const int CHERRY_SCORE = 100;
        public const int CHERRY_SCORE_INCREMENT = 100;
        public const int DRUG_SCORE = 50;
        public const int SPOOK_SCORE = 200;
        public const int PACMAN_SCORE = 500;
        #endregion
        #region Affector/GameLevels
        public static readonly int[] DRUG_DURATION_MS = { 10000, 6500, 4000 };
        #endregion
        #region AI/GameLevels
        public static readonly float[] AI_TARGETING_RATIO = { 0.4f, 0.6f, 0.85f };
        public static readonly float[] AI_NEXT_ACTION_REFRESH_RATE = { 6f, 4f, 3f };
        #endregion
        #endregion
    }
}
