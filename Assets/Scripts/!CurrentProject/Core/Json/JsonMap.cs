using System.Collections.Generic;

namespace Pacman
{
    public struct JsonMap : IJsonObject
    {
        public List<string> ContentLines;
        public JsonCherry Cherry;
        public JsonDrugAffect DrugAffect;
    }
}
