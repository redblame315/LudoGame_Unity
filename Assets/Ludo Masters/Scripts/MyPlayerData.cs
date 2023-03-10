using System.Collections.Generic;
using PlayFab;
//using PlayFab.ClientModels;
using AssemblyCSharp;
using System;
using UnityEngine;
using SocketIO;

public class MyPlayerData
{
    
    public static string TitleFirstLoginKey = "TitleFirstLogin";
    public static string TotalEarningsKey = "TotalEarnings";
    public static string GamesPlayedKey = "blockinfo";
    public static string TwoPlayerWinsKey = "TwoPlayerWins";
    public static string FourPlayerWinsKey = "FourPlayerWins";
    public static string PlayerName = "nickname";
    public static string CoinsKey = "coins";
    public static string ChatsKey = "Chats";
    public static string EmojiKey = "Emoji";
    public static string AvatarIndexKey = "avatar";
    public static string FortuneWheelLastFreeKey = "FortuneWheelLastFreeTime";
	public static string FbId = "FacebookID";


    public SocketIOComponent socketIO;
    public Dictionary<string, string> user_data;
    public Dictionary<string, string> room_data;
    public string[] myGamePlayerList;



    private void Awake()
    {
        socketIO = GameObject.Find("gameSocketIO").gameObject.GetComponent<SocketIOComponent>();
    }

    void Start()
    {
        user_data = new Dictionary<string, string>();
        room_data = new Dictionary<string, string>();
        myGamePlayerList = new string[4];
    }


    public int GetCoins()
    {
        if(user_data.ContainsKey(CoinsKey))
        {
            return int.Parse(user_data["coins"]);
        }
        return 0;
    }

    public string GetLoginRoomId()
    {
        if(user_data.ContainsKey("roomId"))
        {
            return user_data["roomId"];
        }
        return "";
    }


    public string GetAdminCoin()
    {
        if(room_data.ContainsKey("adminCoin"))
        {
            return room_data["adminCoin"];
        }
        return "";
    }

    public string[] GetGamePlayerList()
    {
        return myGamePlayerList;
    }


    public int GetTotalEarnings()
    {
        return /*int.Parse(this.data[TotalEarningsKey].Value)*/0;
    }

    public int GetTwoPlayerWins()
    {
        return /*int.Parse(this.data[TwoPlayerWinsKey].Value)*/0;
    }

    public int GetFourPlayerWins()
    {
        return /*int.Parse(this.data[FourPlayerWinsKey].Value)*/0;
    }

    public int GetWinnerCoin()
    {
        if (room_data.ContainsKey("winnerCoin"))
        {
            return int.Parse(room_data["winnerCoin"]);
        }

        return 0;
        
    }
    public int GetSecondWinnerCoin()
    {
        if (room_data.ContainsKey("secondWinnerCoin"))
        {
            return int.Parse(room_data["secondWinnerCoin"]);
        }

        return 0;
    }

    public int GetPlayedGamesCount()
    {
        /*if (this.data != null)
            return int.Parse(this.data[GamesPlayedKey].Value);
        return -1;*/
        return 0;
    }

    public string GetAvatarIndex()
    {
        if (user_data.ContainsKey("avatar") && user_data["avatar"] != "" && !user_data["avatar"].Contains("FB"))
        {
            string[] avatar = user_data["avatar"].Split('/');
            string[] avatarIndex = avatar[2].Split('.');

            return avatarIndex[0];
        }
        else if(user_data.ContainsKey("avatar") && user_data["avatar"].Contains("FB"))
        {
            return user_data["avatar"];
        }
        return /*this.data[AvatarIndexKey].Value*/"";
    }

    public string GetChats()
    {
        return /*this.data[ChatsKey].Value*/"";
    }

    public string GetEmoji()
    {
        /*if (this.data.ContainsKey(EmojiKey))
            return this.data[EmojiKey].Value;
        else return "error";*/

        return "";
    }

    public string GetPlayerName()
    {
        //if (this.data.ContainsKey(PlayerName))
        //    return this.data[PlayerName].Value;
        //else return "Error";
        return this.user_data["nickname"];
    }

    public string GetPlayerId()
    {
        if (user_data.ContainsKey("id"))
        {
            return user_data["id"];
        }
        return "";
    }

    public int GetRoomId()
    {
        if(room_data.ContainsKey("id"))
        {
            return int.Parse(room_data["id"]);
        }
        return 0;
    }

    public int GetRoomType()
    {
        if (room_data.ContainsKey("type"))
        {
            return int.Parse(room_data["type"]);
        }
        return 0;
    }

    public  int GetEntryCoin()
    {
        if(room_data.ContainsKey("entryCoin"))
        {
            return int.Parse(room_data["entryCoin"]);
        }
        return 0;
    }

	public string GetFbId()
	{
        /*if(this.data.ContainsKey(FbId))
			return this.data[FbId].Value;
		else return "Error";*/
        return "";
	}

    public string GetLastFortuneTime()
    {
        /*if (this.data.ContainsKey(FortuneWheelLastFreeKey))
        {
            return this.data[FortuneWheelLastFreeKey].Value;

        }
        else
        {*/
            string date = DateTime.Now.Ticks.ToString();
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add(FortuneWheelLastFreeKey, date);
            //UpdateUserData(data);
            return date;
        //}
    }



    public MyPlayerData() { }
    public MyPlayerData(/*Dictionary<string, UserDataRecord> data, */bool myData)
    {
        /*this.data = data;*/


        if (myData)
        {
            if (GetAvatarIndex().Equals("fb"))
            {
                GameManager.Instance.avatarMy = GameManager.Instance.facebookAvatar;
            }
            else
            {
                GameManager.Instance.avatarMy = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().avatars[int.Parse(GetAvatarIndex())];
            }

            GameManager.Instance.nameMy = GetPlayerName();
        }
        
    }




    public static Dictionary<string, string> InitialUserData(bool fb)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add(TotalEarningsKey, "0");
        data.Add(ChatsKey, "");
        data.Add(EmojiKey, "");
        if (fb)
        {
            data.Add(CoinsKey, StaticStrings.initCoinsCountFacebook.ToString());
            data.Add(AvatarIndexKey, "fb");
        }
        else
        {
            data.Add(CoinsKey, StaticStrings.initCoinsCountGuest.ToString());
            data.Add(AvatarIndexKey, "0");
        }

        data.Add(GamesPlayedKey, "0");
        data.Add(TwoPlayerWinsKey, "0");
        data.Add(FourPlayerWinsKey, "0");

        data.Add(TitleFirstLoginKey, "1");
        data.Add(FortuneWheelLastFreeKey, DateTime.Now.Ticks.ToString());
        return data;
    }


}