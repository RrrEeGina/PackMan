using System.Collections.Generic;

namespace Pacman
{
    public class JsonSavedGame : IJsonObject
    {
        public GameLevel Level;
        public JsonMap Map;
        public JsonPlayer Player;
        public List<JsonNPC> NPCList;
    }
}
