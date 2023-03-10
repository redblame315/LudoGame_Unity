public class Enums
{

}


public enum AdLocation
{
    GameStart,
    GameOver,
    LevelComplete,
    Pause,
    FacebookFriends,
    GameFinishWindow,
    StoreWindow,
    GamePropertiesWindow

};

public enum MyGameType
{
    TwoPlayer, FourPlayer, Private
};

public enum MyGameMode
{
    Classic, Master, Quick
}

public enum EnumPhoton
{
    ReadyToPlay = 179,
    BeginPrivateGame = 171,
    NextPlayerTurn = 172,
    StartWithBots = 173,
    StartGame = 174,
    SendChatMessage = 175,
    SendChatEmojiMessage = 176,
    AddFriend = 177,
    FinishedGame = 178,
}

public enum EnumGame
{
    DiceRoll = 50,
    PawnMove = 51,
    PawnRemove = 52,
}