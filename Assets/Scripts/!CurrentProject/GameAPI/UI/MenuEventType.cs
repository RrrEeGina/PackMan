namespace Pacman
{
    public enum MenuEventType : byte
    {
        Quit = 0,
        SinglePlayerMenu = 1,
        ScoreMenu = 2,
        NewGame = 3,
        SaveGame = 4,
        LoadGame = 5,
        ResumeGame = 6,
        EasyLevel = 7,
        MediumLevel = 8,
        HardLevel = 9,

        Opening = 10,

        ESC_Pressed = 11
    }
}
