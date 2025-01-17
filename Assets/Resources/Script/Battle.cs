﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

public class Battle : MonoBehaviour
{

    public static Dictionary<int, C_BattleMgr> playerListCamp1 = new Dictionary<int, C_BattleMgr>();
    public static Dictionary<int, C_BattleMgr> playerListCamp2 = new Dictionary<int, C_BattleMgr>();

    public static C_BattleMgr localPlayerBattleInfo;

    public static Transform localPlayerCameraTrans;
    public static Camera localPlayerCamera;
    public static int localPlayerCamp = -1;

    // battle state
    public static bool started = false;
    public static bool inRoom = false;
    public static bool freezing = false;

    public static PlaneHUDMgr planeHUDMgr;
    public static HUDMgr hudMgr;
    public static ScoreboardMgr scoreboardMgr;
    public static BornPointsMgr bornMgr;
    public static NetworkBattleMgr battleMgr;
    public static PhotonEngine photonEngine;


    // temp setting
    public static float relativeRate = 1f;
    public static float mouseSpeedPrimary = 1f;
    public static float mouseSpeedAiming = 1f;

    // save
    public static bool login = false;
    public static PlayerBasic playerBasicSave;
    public static PlayerBattle playerBattleSave;
    public static SystemSetting systemSettingSave;
    public static string savePath = "";

    public static C_BattleMgr GetPlayerInfoByRoomID(int sourceID, int camp)
    {
        if (camp == 1)
        {
            return playerListCamp2.ContainsKey(sourceID) ? playerListCamp2[sourceID] : null;
        }
        if (camp == 2)
        {
            return playerListCamp1.ContainsKey(sourceID) ? playerListCamp1[sourceID] : null;
        }
        return null;
    }

    public static void SavePlayerBasicData()
    {
        string str = JsonConvert.SerializeObject(playerBasicSave);
        File.WriteAllText(savePath + "/PlayerBasic.cfg", str);
    }

    public static void SavePlayerBattleData()
    {
        string str = JsonConvert.SerializeObject(playerBattleSave);
        File.WriteAllText(savePath + "/PlayerBattle.cfg", str);
    }
    public static void SaveSystemSettingData()
    {
        string str = JsonConvert.SerializeObject(systemSettingSave);
        File.WriteAllText(savePath + "/SystemSetting.cfg", str);
    }


    public static void PlayerJoin(int camp, int roomID, C_BattleMgr battleMgr)
    {
        if (camp == 1)
        {
            if (playerListCamp1.ContainsKey(roomID))
            {
                playerListCamp1[roomID] = battleMgr;
            }
            else
            {
                playerListCamp1.Add(roomID, battleMgr);
            }
        }
        else if (camp == 2)
        {
            if (playerListCamp2.ContainsKey(roomID))
            {
                playerListCamp2[roomID] = battleMgr;
            }
            else
            {
                playerListCamp2.Add(roomID, battleMgr);
            }
        }

    }

    public static void playerExit(int camp, int roomID)
    {

        if (camp == 1)
        {
            if (playerListCamp1.ContainsKey(roomID))
            {
                playerListCamp1.Remove(roomID);
                foreach (var item in scoreboardMgr.topPanel)
                {
                    item.Init();
                }
            };
        }
        else if (camp == 2)
        {
            if (playerListCamp2.ContainsKey(roomID))
            {
                playerListCamp2.Remove(roomID);
                foreach (var item in scoreboardMgr.bottomPanel)
                {
                    item.Init();
                }
            };
        }

    }

    // 玩家中途加入时获取人数较少的一方的阵营id
    //public static int GetWeakCamp()
    //{
    //    var camp1 = campNumber.ContainsKey(1) ? campNumber[1] : 0;
    //    var camp2 = campNumber.ContainsKey(2) ? campNumber[2] : 0;
    //    return camp1 <= camp2 ? 1 : 2;
    //}

    public static void ReflashScoreboard()
    {
        int index1 = 0;
        int index2 = 0;
        foreach (var item in PhotonNetwork.playerList)
        {
            var camp = int.Parse(item.CustomProperties["team"].ToString());
            var battleInfo = (item.CustomProperties["battle"].ToString()).Split('#');
            var playerName = item.IsLocal ? ("<color=#ffcf69>" + item.NickName.Split('#')[0] + "</color>") : item.NickName.Split('#')[0];
            if (camp == 1)
            {
                // bug ******Index was out of range.
                scoreboardMgr.topPanel[index1].Init(playerName, battleInfo[0], battleInfo[1], battleInfo[2], battleInfo[3]);
                index1++;
            }
            else if (camp == 2)
            {
                // bug ******Index was out of range.
                scoreboardMgr.bottomPanel[index2].Init(playerName, battleInfo[0], battleInfo[1], battleInfo[2], battleInfo[3]);
                index2++;
            }
        }
    }

    public static void ClearBattlefield()
    {
        var list = playerListCamp1.Values.ToArray();
        foreach (var item in list)
        {
            if (item != null)
            {
                Object.Destroy(item.gameObject);
            }
        }
        playerListCamp1.Clear();

        list = playerListCamp2.Values.ToArray();
        foreach (var item in list)
        {
            if (item != null)
            {
                Object.Destroy(item.gameObject);
            }
        }
        playerListCamp2.Clear();

        Effect.Clear();

    }

    public static void UpdateFriendlyMark()
    {

        foreach (var item in playerListCamp1.Values)
        {
            item.SetFirendlyMark();
        }
        foreach (var item in playerListCamp2.Values)
        {
            item.SetFirendlyMark();
        }

    }

}



