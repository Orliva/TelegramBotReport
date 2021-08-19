namespace TelegramBotServer.Enum
{
    public enum TimeInterval : byte
    {
        Week = 1,
        All = 2
    }

    public enum Answer : byte
    {
        Hardware = 1,
        Software = 2,
        Cartridge = 3,
        BigEvent = 4,
        PersonTasks = 5,
        ExtraWork = 6,
        EndReport = 7,
        None = 8,
        EditProfile = 9
    }
}
