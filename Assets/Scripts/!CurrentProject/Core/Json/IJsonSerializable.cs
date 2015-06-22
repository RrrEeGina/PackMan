
namespace Pacman
{
    public interface IJsonSerializable
    {
        JsonObjectType JsonType { get; }

        IJsonObject GetJsonObject();
    }
}
