using System;
using System.Collections.Generic;
using Match3.Overlay.Advertisement.Placements;
using Match3.Foundation.Base.MoneyHandling;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.NeighborhoodChallenge;
using Match3.LiveOps.DogTraining.Game;
using Match3.Overlay.Advertisement.Placements.Base;
using Match3.Overlay.Analytics.ResourcesAnalytics;
using Match3.Presentation.Gameplay;
using Match3.Presentation.NeighborhoodChallenge;
using UnityEngine;

public enum page_types { surface, match, pre_match, }
public enum page_names { match, match_detail, result_lose, result_win, task_page, heart_popup, ticket_popup, levelInfo_popup , main_menu, rate_us, shop, splash, spinner, undefined, lucky_spinner }
public enum activity_types { ad, gameplay, progress, shop, sink_source, social, surfase, tutorial, spinner }
public enum activity_ad_names { ad_complete, ad_incomplete, ad_no, ad_noad, ad_request, ad_yes }
public enum activity_gameplay_names { load_level, ai_moves, blocked, create_explosive, create_rainbow, double_rainbow, explosion_bomb, explosion_rocket, explostion_dynamite, explostion_tnt, mission_done, mission_step, move_failed, normal_success, rainbow_use, reward_move, special_move, use_hint }
public enum activity_progress_names { dialogue_move_pre_post, heart_close, heart_popup, heart_refill_ad, heart_refill_coin, home_tap, level_back, level_continue, level_info_close, level_info_open, level_info_powerup_sellect, level_info_powerup_sellect_lock, level_info_start, level_lose, level_result_back, level_reply, level_win, load_level, map_change_object, map_select_object, task_done, task_failed, task_manager_close, task_manager_open, task_day_reward }
public enum activity_shop_names { purchase_failed, purchase_success, purchase_success_direct, shop_open, shop_powerup_close, shop_powerup_failed, shop_powerup_popup, shop_powerup_success, shop_instagram, shop_telegram }
public enum activity_sink_source_names { sink, source }
public enum activity_social_names { rate_us_close, rate_us_confirm, rate_us_popup }
public enum activity_surfase_names { app_close, app_load, app_minimize, app_open }
public enum activity_tutorial_names { tutorial, tutorial_finish, tutorial_start }
public enum Activity_spinner_names { spinner_open, spinner_close, spinner_stop, spinner_confirm }
public enum activity_map_names { map_select_object, map_change_object, map_home_tap }


public static class AnalyticsDataMaker
{
    public static class LevelType
    {
        public const string Campaign = "Campaign";
        public const string Neighbourhood = "Neighbourhood";
        public const string DogTraining = "DogTraining";
    }

    //private static Func<int> GetInGameTime;
    private static Func<int> MoveCount;

    public const string SINK_VALUE = "sink";
    public const string SOURCE_VALUE = "source";
    public const string COIN_CURRENCY = "coin";
    public const string HEART_CURRENCY = "heart";

    public const string POWERUP_TYPE = "power_up";

    public const string HAMMER_POWERUP = "hammer";
    public const string BROOM_POWERUP = "sweeper";
    public const string HAND_POWERUP = "hand";

    // param names
    public readonly static string currency2_inv = "currency2_inv";
    public readonly static string currency1_inv = "currency1_inv";
    public readonly static string player_level = "player_level";
    public readonly static string player_xp = "player_xp";
    public readonly static string currency3_inv = "currency3_inv";
    public readonly static string cur_star = "cur_star";
    public readonly static string cur_stage = "cur_stage";

    public readonly static string flag_type = "flag_type";
    public readonly static string flag_name = "flag_name";
    public readonly static string ui_ux_type = "ui_ux_type";
    public readonly static string ui_ux_group = "ui_ux_group";
    public readonly static string ui_ux_action = "ui_ux_action";
    public readonly static string general_type = "general_type";
    public readonly static string general_name = "general_name";
    public readonly static string page_number = "page_number";
    public readonly static string page_source = "page_source";
    public readonly static string item_type = "item_type";
    public readonly static string item_lvl = "item_lvl";
    public readonly static string item_name = "item_name";
    public readonly static string activity_type = "activity_type";
    public readonly static string activity_step = "activity_step";
    public readonly static string activity_name = "activity_name";
    public readonly static string activity_source = "activity_source";
    public readonly static string sku_name = "sku_name";
    public readonly static string price = "price";
    public readonly static string session = "session";
    public readonly static string sink_source = "sink_source";
    public readonly static string currency_name = "currency_name";
    public readonly static string amount = "amount";
    public readonly static string event_id = "event_id";
    public readonly static string is_first = "is_first";
    public readonly static string revenue = "revenue";
    public readonly static string detail = "detail";
    readonly static string page_type = "page_type";
    public readonly static string match_id = "match_id";

    public readonly static string WasSuccessful = "wasSuccessful"; 
    public readonly static string ShareSegmentName = "shareSegmentName";
    public readonly static string ShareChannelName = "shareChannelName";
    public readonly static string ReferralGoalPrizeId = "ReferralGoalPrizeId";
    public readonly static string WonLevel = "wonLevel";
    public readonly static string ReferredPlayersCount = "ReferredPlayersCount";
    public readonly static string FaultyBehaviourDetectionMode = "FaultyBehaviourDetectionMode";
    
    public readonly static string WarningSource = "WarningSource";
    public readonly static string WarningLevelExitResult = "WarningLevelExitResult";
    public readonly static string WarningGameOverResult = "WarningGameOverResult";
    
    /*
    public readonly static string TOTAL_COIN_GAIN = "total coin gain";
    public readonly static string TOTAL_COIN_USED = "total coin used";
    public readonly static string IN_GAME_TIME = "time in game";
    public readonly static string ACTION = "action";
    public readonly static string TYPE = "type";
    public readonly static string POWERUP = "powerup";
    public readonly static string COMPONENT = "Component";
    public readonly static string MEASURE = "measure";
    public readonly static string GAME_PAGE = "game page";
    */

    public static void Setup(Func<int> _GetInGameTime, Func<int> _MoveCount)
    {
        //GetInGameTime = _GetInGameTime;
        MoveCount = _MoveCount;
    }

    public static void AddSet_currency2_inv(Dictionary<string, object> analyticParam)
    {
        analyticParam.Add(currency2_inv, Base.gameManager.profiler.CoinCount);
    }

    public static void AddSet_currency1_inv(Dictionary<string, object> analyticParam)
    {
        analyticParam.Add(currency1_inv, Base.gameManager.profiler.LifeCount);
    }

    public static void AddSet_player_level(Dictionary<string, object> analyticParam)
    {
        analyticParam.Add(player_level, GetCampaignLastLevelNumber());
    }

    public static void AddSet_player_xp(Dictionary<string, object> analyticParam)
    {
        analyticParam.Add(player_xp, Mathf.Max(Base.gameManager.taskManager.DoneTasksCount, 0));
    }

    public static void AddSet_currency3_inv(Dictionary<string, object> analyticParam)
    {
        analyticParam.Add(currency3_inv, Base.gameManager.profiler.StarCount);
    }

    public static void AddSet_cur_star(Dictionary<string, object> analyticParam)
    {
        analyticParam.Add(cur_star, Base.gameManager.profiler.LastDoneConfigId);
    }

    public static void AddSet_cur_stage(Dictionary<string, object> analyticParam)
    {
        analyticParam.Add(cur_stage, Base.gameManager.taskManager.CurrentDay);
    }

    public static void AddGainedCoinSet(Dictionary<string, object> analyticParam)
    {
        //analyticParam.Add(TOTAL_COIN_GAIN, Base.gameManager.profiler.TotalGainedCoin);
    }

    public static void AddUsedCoinSet(Dictionary<string, object> analyticParam)
    {
        //analyticParam.Add(TOTAL_COIN_USED, Base.gameManager.profiler.TotalUsedCoin);
    }

    public static void AddInGameTimeSet(Dictionary<string, object> analyticParam)
    {
        //analyticParam.Add(IN_GAME_TIME, GetInGameTime());
    }

    public static void AddSet_page_type_and_page_name(Dictionary<string, object> analyticParam, page_names page_name)
    {
        switch (page_name)
        {
            case page_names.match:
                analyticParam.Add(page_type, "match");
                break;
            case page_names.match_detail:
                analyticParam.Add(page_type, "pre_match");
                break;
            case page_names.result_lose:
            case page_names.result_win:
                analyticParam.Add(page_type, "result");
                break;
            case page_names.heart_popup:
            case page_names.main_menu:
            case page_names.rate_us:
            case page_names.splash:
            case page_names.spinner:
                analyticParam.Add(page_type, "surface");
                break;
            case page_names.shop:
                analyticParam.Add(page_type, "shop");
                break;
            default:
                break;
        }
        analyticParam.Add("page_name", page_name.ToString());
    }

    public static int GetMoveCount() { return MoveCount(); }

    public static int GetCampaignLastLevelNumber()
    {
        return Base.gameManager.profiler.LastUnlockedLevel + 1;
    }

    public static string GetCurrentLevelType()
    {
        switch (Base.gameManager.CurrentState)
        {
            case NeighborhoodChallengeGameplayState _:
                return LevelType.Neighbourhood;
            case CampaignGameplayState _:
                return LevelType.Campaign;
            case DogTrainingGameplayState _:
                return LevelType.DogTraining;
            case GameplayState _:
                return Base.gameManager.CurrentState.name;
        }

        return "ResultNone";
    }

    static string IsFirstCompleteAdString = "IsFirstCompleteAd";
    public static bool IsFirstCompleteAd
    {
        get { return PlayerPrefs.GetInt(IsFirstCompleteAdString, 1) == 1; }
        set { PlayerPrefs.SetInt(IsFirstCompleteAdString, value ? 1 : 0); }
    }

    public static void FillSink(ref Dictionary<string, object> analyticParam, string currency, int amount)
    {
        analyticParam.Add(AnalyticsDataMaker.sink_source, AnalyticsDataMaker.SINK_VALUE);
        analyticParam.Add(AnalyticsDataMaker.currency_name, currency);
        analyticParam.Add(AnalyticsDataMaker.amount, -amount);
    }

    public static void FillSource(ref Dictionary<string, object> analyticParam, string currency, int amount)
    {
        analyticParam.Add(AnalyticsDataMaker.sink_source, AnalyticsDataMaker.SOURCE_VALUE);
        analyticParam.Add(AnalyticsDataMaker.currency_name, currency);
        analyticParam.Add(AnalyticsDataMaker.amount, amount);
    }

    public static void ExtractRevenueAndCurrency(Money money, out float revenue, out string currenyCode)
    {
        if (money.CurrencyCode().Equals("IRR"))
        {
            revenue = (float)money.Amount() / 10000f;
            currenyCode = "USD";
        }
        else
        {
            revenue = (float)money.Amount();
            currenyCode = money.CurrencyCode();
        }
    }
}

public abstract class AnalyticsData_Share : AnalyticsDataBase
{
    public AnalyticsData_Share()
    {
        AnalyticsDataMaker.AddSet_currency2_inv(analyticParam);
        AnalyticsDataMaker.AddSet_currency1_inv(analyticParam);
        AnalyticsDataMaker.AddSet_player_xp(analyticParam);
        AnalyticsDataMaker.AddSet_currency3_inv(analyticParam);
        AnalyticsDataMaker.AddSet_player_level(analyticParam);
        AnalyticsDataMaker.AddSet_cur_star(analyticParam);
        AnalyticsDataMaker.AddSet_cur_stage(analyticParam);
        analyticParam.Add("player_rank", Base.gameManager.profiler.PlayCount);
    }
}

#region game_state
public abstract class AnalyticsData_GameStart : AnalyticsData_Share
{
    public AnalyticsData_GameStart()
    {
        analyticParam.Add(AnalyticsDataMaker.activity_type, "surface");
    }
}
public class AnalyticsData_App_Open : AnalyticsData_GameStart
{
    public AnalyticsData_App_Open()
    {
        analyticParam.Add(AnalyticsDataMaker.general_type, "session");
        analyticParam.Add(AnalyticsDataMaker.general_name, "session_start");
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.splash);
        analyticParam.Add(AnalyticsDataMaker.activity_name, "app_open");
    }
}

public class AnalyticsData_App_Load : AnalyticsData_GameStart
{
    public AnalyticsData_App_Load()
    {
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.main_menu);
        analyticParam.Add(AnalyticsDataMaker.activity_name, "app_load");
    }
}

public class AnalyticsData_App_Close : AnalyticsData_GameStart
{
    public AnalyticsData_App_Close()
    {
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.main_menu);
        analyticParam.Add(AnalyticsDataMaker.activity_name, "app_close");
    }
}

public class AnalyticsData_App_Minimize : AnalyticsData_GameStart
{
    public AnalyticsData_App_Minimize(page_names page_name)
    {
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_name);
        analyticParam.Add(AnalyticsDataMaker.activity_name, "app_minimize");
    }
}
#endregion

#region user_snap_shot
public abstract class AnalyticsData_Snapshot : AnalyticsDataBase
{
    public AnalyticsData_Snapshot()
    {
        AnalyticsDataMaker.AddSet_player_xp(analyticParam);
        AnalyticsDataMaker.AddSet_currency1_inv(analyticParam);
        AnalyticsDataMaker.AddGainedCoinSet(analyticParam);
        AnalyticsDataMaker.AddUsedCoinSet(analyticParam);
        AnalyticsDataMaker.AddSet_player_level(analyticParam);
        AnalyticsDataMaker.AddInGameTimeSet(analyticParam);
    }
}

public class AnalyticsData_Snapshot_Open : AnalyticsData_Snapshot { }

public class AnalyticsData_Snapshot_Result : AnalyticsData_Snapshot { }
#endregion

#region tutorial
public abstract class AnalyticsData_Tutorial : AnalyticsData_Share
{
    public AnalyticsData_Tutorial()
    {
        analyticParam.Add(AnalyticsDataMaker.activity_type, activity_types.tutorial);
    }
}

public class AnalyticsData_Tutorial_Step : AnalyticsData_Tutorial
{
    public readonly int step;

    public AnalyticsData_Tutorial_Step(int step)
    {
        this.step = step;
        analyticParam.Add(AnalyticsDataMaker.activity_step, step);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_tutorial_names.tutorial.ToString());
    }
}

public class AnalyticsData_DayOne_End : AnalyticsData_Tutorial
{
    public AnalyticsData_DayOne_End()
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, "DayOneEnd");
    }
}

public class AnalyticsData_Tutorial_Start : AnalyticsData_Tutorial
{
    public AnalyticsData_Tutorial_Start()
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_tutorial_names.tutorial_start.ToString());
    }
}

public class AnalyticsData_Tutorial_Finish : AnalyticsData_Tutorial
{
    public AnalyticsData_Tutorial_Finish()
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_tutorial_names.tutorial_finish.ToString());
    }
}
#endregion

#region heart_refill
public abstract class AnalyticsData_HeartRefill : AnalyticsData_Share
{
    public AnalyticsData_HeartRefill()
    {
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.heart_popup);
    }
}

public class AnalyticsData_Heart_Refill_Coin : AnalyticsData_HeartRefill
{
    public AnalyticsData_Heart_Refill_Coin(int cost)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_type, "progress");
        analyticParam.Add(AnalyticsDataMaker.activity_name, "heart_refill_coin");
        analyticParam.Add(AnalyticsDataMaker.currency_name, "coin");
        analyticParam.Add(AnalyticsDataMaker.sink_source, AnalyticsDataMaker.SINK_VALUE);
        analyticParam.Add(AnalyticsDataMaker.amount, cost);
    }
}
#endregion

#region level_info
public abstract class AnalyticsData_LevelInfo : AnalyticsData_Share
{
    public AnalyticsData_LevelInfo()
    {
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.match_detail);
        analyticParam.Add(AnalyticsDataMaker.activity_type, activity_types.progress);
        analyticParam.Add(AnalyticsDataMaker.page_number, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
    }
}

public class AnalyticsData_LevelInfo_Open : AnalyticsData_LevelInfo
{
    public AnalyticsData_LevelInfo_Open()
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_progress_names.level_info_open);
    }
}

public class AnalyticsData_LevelInfo_Start : AnalyticsData_LevelInfo
{
    public AnalyticsData_LevelInfo_Start()
    {
        analyticParam.Add(AnalyticsDataMaker.general_type, "progress");
        analyticParam.Add(AnalyticsDataMaker.general_name, "match_start");
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_progress_names.level_info_start);
    }
}

public class AnalyticsData_LevelInfo_Close : AnalyticsData_LevelInfo
{
    public AnalyticsData_LevelInfo_Close()
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_progress_names.level_info_close);
    }
}

public class AnalyticsData_LevelInfo_Booster_Select : AnalyticsData_LevelInfo
{
    public AnalyticsData_LevelInfo_Booster_Select(string powerup_name)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_progress_names.level_info_powerup_sellect);
        analyticParam.Add(AnalyticsDataMaker.item_type, "power_up");
        analyticParam.Add(AnalyticsDataMaker.item_name, powerup_name);
    }
}

public class AnalyticsData_LevelInfo_Booster_Select_Lock : AnalyticsData_LevelInfo
{
    public AnalyticsData_LevelInfo_Booster_Select_Lock(string powerup_name)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_progress_names.level_info_powerup_sellect_lock);
        analyticParam.Add(AnalyticsDataMaker.item_type, "power_up");
        analyticParam.Add(AnalyticsDataMaker.item_name, powerup_name);
    }
}

#endregion

#region level_entry

public abstract class AnalyticsData_LevelEntry_Base : AnalyticsData_Share
{
    public readonly string entryType;
    public readonly int levelNumber;

    public static class EntryType
    {
        public const string Load = "Load";
        public const string Abort = "Abort";
        public const string Win = "Win";
        public const string Lose = "Lose";
        public const string GiveUp = "GiveUp";
        public const string Retry = "Retry";
        public const string ExtraMove = "ExtraMove";
        public const string DoubleBomb = "DoubleBomb";
        public const string Rainbow = "Rainbow";
        public const string TNTRainbow = "TNTRainbow";
    }

    protected AnalyticsData_LevelEntry_Base(string entryType, int levelNumber)
    {
        this.entryType = entryType;
        this.levelNumber = levelNumber;
    }

}

public abstract class AnalyticsData_Global_LevelEntry : AnalyticsData_LevelEntry_Base
{
    protected AnalyticsData_Global_LevelEntry(string entryType, int levelNumber) : base(entryType, levelNumber)
    {
        analyticParam.Add(AnalyticsDataMaker.page_number, AnalyticsDataMaker.GetMoveCount());
        analyticParam.Add(AnalyticsDataMaker.activity_type, activity_types.progress);
        analyticParam.Add(AnalyticsDataMaker.activity_step, levelNumber);
        analyticParam.Add(AnalyticsDataMaker.match_id, AnalyticsManager.MatchId());
    }
}

public class AnalyticsData_Global_LevelEntry_Win : AnalyticsData_Global_LevelEntry
{
    public readonly int extraMoveRetriesCount;
    public AnalyticsData_Global_LevelEntry_Win(int score, int extraMoveRetriesCount, int levelNumber) : base(EntryType.Win, levelNumber)
    {
        this.extraMoveRetriesCount = extraMoveRetriesCount;

        analyticParam.Add(AnalyticsDataMaker.general_type, "progress");
        analyticParam.Add(AnalyticsDataMaker.general_name, "match_win");
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.result_win);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_progress_names.level_win);

        AnalyticsDataMaker.FillSource(ref analyticParam, AnalyticsDataMaker.COIN_CURRENCY, score);
    }
}

public class AnalyticsData_Global_LevelEntry_Lose : AnalyticsData_Global_LevelEntry
{
    public AnalyticsData_Global_LevelEntry_Lose(int levelNumber) : base(EntryType.Lose, levelNumber)
    {
        analyticParam.Add(AnalyticsDataMaker.general_type, "progress");
        analyticParam.Add(AnalyticsDataMaker.general_name, "match_lose");
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.result_lose);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_progress_names.level_lose);
    }
}

public class AnalyticsData_Global_LevelEntry_GiveUp : AnalyticsData_Global_LevelEntry
{
    public readonly int extraMoveRetriesCount;

    public AnalyticsData_Global_LevelEntry_GiveUp(int extraMoveRetriesCount, int levelNumber) : base(EntryType.GiveUp, levelNumber)
    {
        this.extraMoveRetriesCount = extraMoveRetriesCount;

        analyticParam.Add(AnalyticsDataMaker.general_type, "progress");
        analyticParam.Add(AnalyticsDataMaker.general_name, "match_lose");
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.match);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_progress_names.level_result_back);
    }
}

public class AnalyticsData_Global_LevelEntry_Continue_Extra_Move : AnalyticsData_Global_LevelEntry
{
    public AnalyticsData_Global_LevelEntry_Continue_Extra_Move(int cost, int levelNumber) : base(EntryType.ExtraMove, levelNumber)
    {
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.result_lose);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_progress_names.level_continue);
        //analyticParam.Add(AnalyticsDataMaker.currency_name, "coin");
        //analyticParam.Add(AnalyticsDataMaker.amount, cost);

        AnalyticsDataMaker.FillSink(ref analyticParam, AnalyticsDataMaker.COIN_CURRENCY, cost);
    }
}

public class AnalyticsData_Global_LevelEntry_Retry : AnalyticsData_Global_LevelEntry
{
    public readonly int extraMoveRetriesCount;

    public AnalyticsData_Global_LevelEntry_Retry(int extraMoveRetriesCount, int lostHeartsCount, int levelNumber) : base(EntryType.Retry, levelNumber)
    {
        this.extraMoveRetriesCount = extraMoveRetriesCount;

        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.result_lose);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_progress_names.level_reply);

        AnalyticsDataMaker.FillSink(ref analyticParam, AnalyticsDataMaker.HEART_CURRENCY, lostHeartsCount);
    }
}

public class AnalyticsData_Global_LevelEntry_Abort : AnalyticsData_Global_LevelEntry
{
    public AnalyticsData_Global_LevelEntry_Abort(int levelNumber)  : base(EntryType.Abort, levelNumber)
    {
    }
}

public class AnalyticsData_Global_LevelEntry_Start : AnalyticsData_Global_LevelEntry
{
    public AnalyticsData_Global_LevelEntry_Start(int levelNumber) : base(EntryType.Load, levelNumber)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.load_level);
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.match);

        analyticParam[AnalyticsDataMaker.activity_type] = activity_types.gameplay;
        analyticParam[AnalyticsDataMaker.page_number] = levelNumber;
    }
}

public class AnalyticsData_Global_LevelEntry_DoubleBomb : AnalyticsData_Global_LevelEntry
{
    public AnalyticsData_Global_LevelEntry_DoubleBomb(int levelNumber) : base(EntryType.DoubleBomb, levelNumber)
    {
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.match);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.load_level);
        analyticParam[AnalyticsDataMaker.activity_type] = activity_types.gameplay;
        analyticParam[AnalyticsDataMaker.page_number] = levelNumber;
    }
}

public class AnalyticsData_Global_LevelEntry_Rainbow : AnalyticsData_Global_LevelEntry
{
    public AnalyticsData_Global_LevelEntry_Rainbow(int levelNumber) : base(EntryType.Rainbow, levelNumber)
    {
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.match);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.load_level);
        analyticParam[AnalyticsDataMaker.activity_type] = activity_types.gameplay;
        analyticParam[AnalyticsDataMaker.page_number] = levelNumber;
    }
}

public class AnalyticsData_Global_LevelEntry_TNTRainbow : AnalyticsData_Global_LevelEntry
{
    public AnalyticsData_Global_LevelEntry_TNTRainbow(int levelNumber) : base(EntryType.TNTRainbow, levelNumber)
    {
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.match);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.load_level);
        analyticParam[AnalyticsDataMaker.activity_type] = activity_types.gameplay;
        analyticParam[AnalyticsDataMaker.page_number] = levelNumber;
    }
}



public abstract class AnalyticsData_Specific_LevelEntry : AnalyticsData_LevelEntry_Base
{
    public readonly string specificCategoryName;

    protected AnalyticsData_Specific_LevelEntry(string entryType, string specificCategoryName, int levelNumber) : base(entryType, levelNumber)
    {
        this.specificCategoryName = specificCategoryName;
    }
}

public class AnalyticsData_Specific_LevelEntry_Win : AnalyticsData_Specific_LevelEntry
{
    public AnalyticsData_Specific_LevelEntry_Win(string specificCategoryName, int levelNumber) : base(EntryType.Win, specificCategoryName, levelNumber)
    {
    }
}

public class AnalyticsData_Specific_LevelEntry_Lose : AnalyticsData_Specific_LevelEntry
{
    public AnalyticsData_Specific_LevelEntry_Lose(string specificCategoryName, int levelNumber) : base(EntryType.Lose, specificCategoryName, levelNumber)
    {
    }
}

public class AnalyticsData_Specific_LevelEntry_GiveUp : AnalyticsData_Specific_LevelEntry
{
    public AnalyticsData_Specific_LevelEntry_GiveUp(string specificCategoryName, int levelNumber) : base(EntryType.GiveUp, specificCategoryName, levelNumber)
    {
    }
}

public class AnalyticsData_Specific_LevelEntry_Continue_Extra_Move : AnalyticsData_Specific_LevelEntry
{
    public AnalyticsData_Specific_LevelEntry_Continue_Extra_Move(string specificCategoryName, int levelNumber) : base(EntryType.ExtraMove, specificCategoryName, levelNumber)
    {
    }
}

public class AnalyticsData_Specific_LevelEntry_Retry : AnalyticsData_Specific_LevelEntry
{
    public AnalyticsData_Specific_LevelEntry_Retry(string specificCategoryName, int levelNumber) : base(EntryType.Retry, specificCategoryName, levelNumber)
    {
    }
}

public class AnalyticsData_Specific_LevelEntry_Abort : AnalyticsData_Specific_LevelEntry
{
    public AnalyticsData_Specific_LevelEntry_Abort(string specificCategoryName, int levelNumber) : base(EntryType.Abort, specificCategoryName, levelNumber)
    {
    }
}

public class AnalyticsData_Specific_LevelEntry_Start : AnalyticsData_Specific_LevelEntry
{
    public AnalyticsData_Specific_LevelEntry_Start(string specificCategoryName, int levelNumber) : base(EntryType.Load, specificCategoryName, levelNumber)
    {
    }
}
#endregion

#region ingame
public abstract class AnalyticsData_InGame : AnalyticsData_Share
{
    public readonly int levelNumber;

    public AnalyticsData_InGame()
    {
        levelNumber = AnalyticsDataMaker.GetCampaignLastLevelNumber();
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.match);
        analyticParam.Add(AnalyticsDataMaker.activity_type, activity_types.gameplay);
        analyticParam.Add(AnalyticsDataMaker.page_number, levelNumber);
        analyticParam.Add(AnalyticsDataMaker.match_id, AnalyticsManager.MatchId());
    }

    protected override int NamePrefixLength()
    {
        return 20;// Length of AnalyticsData_InGame
    }
}

public class AnalyticsData_Ingame_UseHint : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_UseHint()
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "hint");
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.use_hint);
    }
}

public class AnalyticsData_Ingame_Normal_Success : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_Normal_Success()
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "move");
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.normal_success);
        SendWhenOffline = false;
    }
}

public class AnalyticsData_Ingame_Special_Move : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_Special_Move()
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "move");
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.special_move);
    }
}

public class AnalyticsData_Ingame_Move_Failed : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_Move_Failed()
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "move");
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.move_failed);

        SendWhenOffline = false;
    }
}

public class AnalyticsData_Ingame_Explosion_Rocket : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_Explosion_Rocket()
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "explosive");
        analyticParam.Add(AnalyticsDataMaker.item_name, "rocket");
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.explosion_rocket);

        SendWhenOffline = false;
    }
}

public class AnalyticsData_Ingame_Explosion_Bomb : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_Explosion_Bomb()
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "explosive");
        analyticParam.Add(AnalyticsDataMaker.item_name, "bomb");
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.explosion_bomb);

        SendWhenOffline = false;
    }
}

public class AnalyticsData_Ingame_Explosion_Dynamite : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_Explosion_Dynamite()
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "explosive");
        analyticParam.Add(AnalyticsDataMaker.item_name, "dynamite");
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.explostion_dynamite);

        SendWhenOffline = false;

    }
}

public class AnalyticsData_Ingame_Explosion_Tnt : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_Explosion_Tnt()
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "explosive");
        analyticParam.Add(AnalyticsDataMaker.item_name, "tnt");
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.explostion_tnt);

        SendWhenOffline = false;
    }
}

public class AnalyticsData_Ingame_Use_Double_Rainbow : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_Use_Double_Rainbow()
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "explosive");
        analyticParam.Add(AnalyticsDataMaker.item_name, "rainbow");
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.double_rainbow);
    }
}

public class AnalyticsData_Ingame_Use_Rainbow : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_Use_Rainbow(string item)
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "explosive");
        analyticParam.Add(AnalyticsDataMaker.activity_source, item);
        analyticParam.Add(AnalyticsDataMaker.item_name, "rainbow");
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.rainbow_use);

        SendWhenOffline = false;
    }
}

public class AnalyticsData_Ingame_Mission_Done : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_Mission_Done(GoalType goalType)
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "mission");
        analyticParam.Add(AnalyticsDataMaker.item_name, goalType);
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.mission_done);
    }

    public AnalyticsData_Ingame_Mission_Done(string goalType)
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "mission");
        analyticParam.Add(AnalyticsDataMaker.item_name, goalType);
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.mission_done);
    }
}
public class AnalyticsData_Ingame_Mission_Step : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_Mission_Step(GoalType goalType, int counter, int total)
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "mission");
        analyticParam.Add(AnalyticsDataMaker.item_lvl, counter);
        analyticParam.Add(AnalyticsDataMaker.item_name, goalType);
        analyticParam.Add(AnalyticsDataMaker.activity_source, total);
        analyticParam.Add(AnalyticsDataMaker.activity_step, counter);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.mission_step);

        SendWhenOffline = false;
    }

    public AnalyticsData_Ingame_Mission_Step(string goalType, int counter, int total)
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "mission");
        analyticParam.Add(AnalyticsDataMaker.item_lvl, counter);
        analyticParam.Add(AnalyticsDataMaker.item_name, goalType);
        analyticParam.Add(AnalyticsDataMaker.activity_source, total);
        analyticParam.Add(AnalyticsDataMaker.activity_step, counter);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.mission_step);

        SendWhenOffline = false;
    }
}

public class AnalyticsData_Ingame_Create_Booster : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_Create_Booster(ExplosiveItemType explosiveItemType, bool isUserMove)
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "explosive");
        analyticParam.Add(AnalyticsDataMaker.item_name, explosiveItemType);
        analyticParam.Add(AnalyticsDataMaker.activity_source, isUserMove ? "user" : "ai");
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.create_explosive);

        SendWhenOffline = false;
    }

    public AnalyticsData_Ingame_Create_Booster(string explosiveItemType, bool isUserMove)
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "explosive");
        analyticParam.Add(AnalyticsDataMaker.item_name, explosiveItemType);
        analyticParam.Add(AnalyticsDataMaker.activity_source, isUserMove ? "user" : "ai");
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.create_explosive);

        SendWhenOffline = false;
    }
}

public class AnalyticsData_Ingame_Blocked : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_Blocked()
    {
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.blocked);
    }
}

public class AnalyticsData_Ingame_In_Level_Back : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_In_Level_Back(string type)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, type);
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
    }
}

public class AnalyticsData_Ingame_Extra_Move : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_Extra_Move()
    {
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.reward_move);
    }
}

public class AnalyticsData_Ingame_Create_Rainbow : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_Create_Rainbow()
    {
        analyticParam.Add(AnalyticsDataMaker.item_type, "explosive");
        analyticParam.Add(AnalyticsDataMaker.item_name, "rainbow");
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.create_rainbow);

        SendWhenOffline = false;
    }
}

public class AnalyticsData_Ingame_AiMove : AnalyticsData_InGame
{
    public AnalyticsData_Ingame_AiMove()
    {
        analyticParam.Add(AnalyticsDataMaker.activity_step, AnalyticsDataMaker.GetCampaignLastLevelNumber());
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_gameplay_names.ai_moves);
    }
}

public abstract class AnalyticsData_InGame_PowerUp_Shop : AnalyticsData_InGame
{
    public AnalyticsData_InGame_PowerUp_Shop(string powerUpName)
    {
        analyticParam[AnalyticsDataMaker.item_type] = AnalyticsDataMaker.POWERUP_TYPE;
        analyticParam[AnalyticsDataMaker.item_name] = powerUpName;
        analyticParam[AnalyticsDataMaker.activity_type] = activity_types.shop;
        analyticParam[AnalyticsDataMaker.activity_step] = AnalyticsDataMaker.GetCampaignLastLevelNumber();
        analyticParam[AnalyticsDataMaker.activity_name] = DefaultActivityName();
    }

}

public class AnalyticsData_InGame_PowerUp_Success : AnalyticsData_InGame_PowerUp_Shop
{
    public AnalyticsData_InGame_PowerUp_Success(int cost, string powerUpName) : base(powerUpName)
    {
        AnalyticsDataMaker.FillSink(ref analyticParam, AnalyticsDataMaker.COIN_CURRENCY, cost);
    }
}

public class AnalyticsData_InGame_PowerUp_Popup : AnalyticsData_InGame_PowerUp_Shop
{
    public AnalyticsData_InGame_PowerUp_Popup(string powerUpName) : base(powerUpName)
    {
    }
}

public class AnalyticsData_InGame_PowerUp_Failed : AnalyticsData_InGame_PowerUp_Shop
{
    public AnalyticsData_InGame_PowerUp_Failed(string powerUpName) : base(powerUpName)
    {
    }
}

public class AnalyticsData_InGame_PowerUp_Close : AnalyticsData_InGame_PowerUp_Shop
{
    public AnalyticsData_InGame_PowerUp_Close(string powerUpName) : base(powerUpName)
    {
    }
}

public abstract class AnalyticsData_InGame_Powerup_Activation : AnalyticsData_InGame
{
    public AnalyticsData_InGame_Powerup_Activation(string powerUpName)
    {
        analyticParam[AnalyticsDataMaker.item_type] = AnalyticsDataMaker.POWERUP_TYPE;
        analyticParam[AnalyticsDataMaker.item_name] = powerUpName;
        analyticParam[AnalyticsDataMaker.activity_type] = activity_types.gameplay;
        analyticParam[AnalyticsDataMaker.activity_step] = AnalyticsDataMaker.GetCampaignLastLevelNumber();
        analyticParam[AnalyticsDataMaker.activity_name] = DefaultActivityName();
    }
}

public class AnalyticsData_InGame_Powerup_Hammer : AnalyticsData_InGame_Powerup_Activation
{
    public AnalyticsData_InGame_Powerup_Hammer() : base(AnalyticsDataMaker.HAMMER_POWERUP)
    {
    }
}

public class AnalyticsData_InGame_Powerup_Sweeper : AnalyticsData_InGame_Powerup_Activation
{
    public AnalyticsData_InGame_Powerup_Sweeper() : base(AnalyticsDataMaker.BROOM_POWERUP)
    {
    }
}

public class AnalyticsData_InGame_Powerup_Hand : AnalyticsData_InGame_Powerup_Activation
{
    public AnalyticsData_InGame_Powerup_Hand() : base(AnalyticsDataMaker.HAND_POWERUP)
    {
    }
}
#endregion

#region ad
public abstract class AnalyticsData_Advertisement : AnalyticsData_Share
{
    public readonly string adPlacementName;
    public readonly AdvertisementPlacementType advertisementType;

    public AnalyticsData_Advertisement(string adNetworkName, page_names page_name, string adPlacementName, AdvertisementPlacementType adType)
    {
        this.adPlacementName = adPlacementName;
        this.advertisementType = adType;

        analyticParam.Add(AnalyticsDataMaker.activity_source, adNetworkName);
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_name);
        analyticParam.Add(AnalyticsDataMaker.item_type, "ad");
        analyticParam.Add(AnalyticsDataMaker.item_name, "rewarded");
        analyticParam.Add(AnalyticsDataMaker.activity_type, activity_types.ad);
    }

    public string AdNetworkName()
    {
        return analyticParam[AnalyticsDataMaker.activity_source].ToString();
    }
}

public class AnalyticsData_Advertisement_Request : AnalyticsData_Advertisement
{
    public AnalyticsData_Advertisement_Request(string adNetworkName, page_names page_name, string adPlacementName, AdvertisementPlacementType adType)
        : base(adNetworkName, page_name, adPlacementName, adType)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_ad_names.ad_request);
    }
}

public class AnalyticsData_Advertisement_Complete : AnalyticsData_Advertisement
{
    public AnalyticsData_Advertisement_Complete(string adNetworkName, page_names page_name, int coinCount, string adPlacementName, AdvertisementPlacementType adType)
        : base(adNetworkName, page_name, adPlacementName, adType)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_ad_names.ad_complete);
        analyticParam.Add(AnalyticsDataMaker.general_type, "monetize");
        analyticParam.Add(AnalyticsDataMaker.general_name, "ad_complete");
        analyticParam.Add(AnalyticsDataMaker.sink_source, AnalyticsDataMaker.SOURCE_VALUE);
        analyticParam.Add(AnalyticsDataMaker.currency_name, "coin");
        analyticParam.Add(AnalyticsDataMaker.amount, coinCount);
    }
}

public class AnalyticsData_Advertisement_InComplete : AnalyticsData_Advertisement
{
    public AnalyticsData_Advertisement_InComplete(string adNetworkName, page_names page_name, string adPlacementName, AdvertisementPlacementType adType)
        : base(adNetworkName, page_name, adPlacementName, adType)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_ad_names.ad_incomplete);
    }
}

public class AnalyticsData_Advertisement_NoAd : AnalyticsData_Advertisement
{
    public AnalyticsData_Advertisement_NoAd(string adNetworkName, page_names page_name, string adPlacementName, AdvertisementPlacementType adType)
        : base(adNetworkName, page_name, adPlacementName, adType)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_ad_names.ad_noad);
    }
}
#endregion

#region map_action
public abstract class AnalyticsData_MapAction : AnalyticsData_Share
{
    public AnalyticsData_MapAction(string action)
    {
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.main_menu);
        analyticParam.Add(AnalyticsDataMaker.item_type, "map_object");
        analyticParam.Add(AnalyticsDataMaker.item_name, action);
    }
}

public class AnalyticsData_Map_SelectObject : AnalyticsData_MapAction
{
    public AnalyticsData_Map_SelectObject(string action, int selectedIndex, bool isFirstTime) : base(action)
    {
        analyticParam.Add(AnalyticsDataMaker.item_lvl, selectedIndex);
        analyticParam.Add(AnalyticsDataMaker.activity_source, isFirstTime);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_map_names.map_select_object);
    }
}

public class AnalyticsData_Map_ChangeObject : AnalyticsData_MapAction
{
    public AnalyticsData_Map_ChangeObject(string action, int selectedIndex, bool isFirstTime) : base(action)
    {
        analyticParam.Add(AnalyticsDataMaker.item_lvl, selectedIndex);
        analyticParam.Add(AnalyticsDataMaker.activity_source, isFirstTime);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_map_names.map_change_object);
    }
}

public class AnalyticsData_Map_HomeTap : AnalyticsData_MapAction
{
    public AnalyticsData_Map_HomeTap(string action) : base(action)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_map_names.map_home_tap);
    }
}
#endregion

#region task
public abstract class AnalyticsData_TaskAction : AnalyticsData_Share
{
    public AnalyticsData_TaskAction()
    {
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.task_page);
        analyticParam.Add(AnalyticsDataMaker.activity_type, activity_types.progress);
    }
}

public class AnalyticsData_TaskAction_ManagerOpen : AnalyticsData_TaskAction
{
    public AnalyticsData_TaskAction_ManagerOpen()
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_progress_names.task_manager_open);

    }
}

public class AnalyticsData_TaskAction_Done : AnalyticsData_TaskAction
{
    public AnalyticsData_TaskAction_Done(string taskString, int taskIndex)
    {
        analyticParam.Add(AnalyticsDataMaker.page_number, taskIndex);
        analyticParam.Add(AnalyticsDataMaker.page_source, taskString);
        analyticParam.Add(AnalyticsDataMaker.item_type, "task");
        analyticParam.Add(AnalyticsDataMaker.item_lvl, taskIndex);
        analyticParam.Add(AnalyticsDataMaker.item_name, taskString);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_progress_names.task_done);
    }
}

public class AnalyticsData_TaskAction_Failed : AnalyticsData_TaskAction
{
    public AnalyticsData_TaskAction_Failed(string taskString, int taskIndex)
    {
        analyticParam.Add(AnalyticsDataMaker.page_number, taskIndex);
        analyticParam.Add(AnalyticsDataMaker.page_source, taskString);
        analyticParam.Add(AnalyticsDataMaker.item_type, "task");
        analyticParam.Add(AnalyticsDataMaker.item_lvl, taskIndex);
        analyticParam.Add(AnalyticsDataMaker.item_name, taskString);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_progress_names.task_failed);
    }
}

public class AnalyticsData_TaskAction_ManagerClose : AnalyticsData_TaskAction
{
    public AnalyticsData_TaskAction_ManagerClose()
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_progress_names.task_manager_close);
    }
}

public class AnalyticsData_TaskAction_Reward : AnalyticsData_TaskAction
{
    public AnalyticsData_TaskAction_Reward()
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_progress_names.task_day_reward);
        analyticParam.Add(AnalyticsDataMaker.item_type, "reward");
        analyticParam.Add(AnalyticsDataMaker.activity_step, Base.gameManager.taskManager.CurrentDay);
    }
}
#endregion

#region resources
public class AnalyticsData_Sink_Source : AnalyticsData_Share
{
    public AnalyticsData_Sink_Source(int amount, string type)
    {
        analyticParam.Add(AnalyticsDataMaker.sink_source, amount > 0 ? AnalyticsDataMaker.SOURCE_VALUE : AnalyticsDataMaker.SINK_VALUE);
        analyticParam.Add(AnalyticsDataMaker.amount, Mathf.Abs(amount));
        analyticParam.Add(AnalyticsDataMaker.event_id, type);
        analyticParam.Add(AnalyticsDataMaker.currency_name, "coin");
    }
}

public class AnalyticsData_ResourcesEvent : AnalyticsDataBase
{
    public enum SinkSourceType
    {
        Sink,
        Source
    }

    public readonly string resourceCurrencyType;
    public readonly float amount;
    public readonly string itemType;
    public readonly string itemId;
    public readonly SinkSourceType sinkSourceType;

    public AnalyticsData_ResourcesEvent(string itemType, string itemId, string resourceCurrencyType, float amount)
    {
        this.resourceCurrencyType = resourceCurrencyType;
        this.amount = Mathf.Abs(amount);
        this.itemType = itemType;
        this.itemId = itemId;
        this.sinkSourceType = amount > 0 ? SinkSourceType.Source : SinkSourceType.Sink;

        ResourcesAnalyticsLogger.LogInfo(message: $"Creating A Resources Event. Currency: {resourceCurrencyType}, ItemType: {itemType}, ItemId: {itemId}, Amount: {amount}, Sink/Source: {(amount > 0 ? AnalyticsDataMaker.SOURCE_VALUE : AnalyticsDataMaker.SINK_VALUE)}");
    }
}
#endregion

#region story
public class AnalyticsData_Story : AnalyticsData_Share
{
    public AnalyticsData_Story(string action, string text)
    {
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.main_menu);
        analyticParam.Add(AnalyticsDataMaker.item_name, text);
        analyticParam.Add(AnalyticsDataMaker.item_type, "story");
        analyticParam.Add(AnalyticsDataMaker.activity_type, activity_types.progress);
        analyticParam.Add(AnalyticsDataMaker.activity_name, action);
        analyticParam.Add(AnalyticsDataMaker.activity_step, text);

        SendWhenOffline = false;
    }
}
#endregion

#region purchase
public abstract class AnalyticsData_Purchase : AnalyticsData_Share
{
    public AnalyticsData_Purchase(string gamePage)
    {
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.shop);
        analyticParam.Add(AnalyticsDataMaker.page_source, gamePage);
        analyticParam.Add(AnalyticsDataMaker.activity_type, activity_types.shop);
    }
}

public class AnalyticsData_ShopOpen : AnalyticsData_Purchase
{
    public AnalyticsData_ShopOpen(string gamePage, string cohortGroupString) : base(gamePage)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_step, cohortGroupString);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_shop_names.shop_open.ToString());
    }
}

public class AnalyticsData_Purchase_Success : AnalyticsData_Purchase
{
    private static class PurchaseHappeningType
    {
        public const string FirstTime = "FirstTime";
        public const string NotFirstTime = "NotFirstTime";
    }

    public readonly float revenue;
    public readonly string revenueCurrency;
    public readonly string purchaseHappeningType;
    public readonly int lastUnlockedLevel;


    public AnalyticsData_Purchase_Success(string gamePage, string sku, Money money, string storeToken, int lastUnlockedLevel, bool isFirstTime) : base(gamePage)
    {
        AnalyticsDataMaker.ExtractRevenueAndCurrency(money, out revenue, out revenueCurrency);
        purchaseHappeningType = isFirstTime ? PurchaseHappeningType.FirstTime : PurchaseHappeningType.NotFirstTime;
        this.lastUnlockedLevel = lastUnlockedLevel;

        analyticParam.Add(AnalyticsDataMaker.item_name, sku);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_shop_names.purchase_success.ToString());
        analyticParam.Add(AnalyticsDataMaker.general_type, "monetize");
        analyticParam.Add(AnalyticsDataMaker.general_name, "purchase");
        analyticParam.Add(AnalyticsDataMaker.revenue, revenue);
        analyticParam.Add(AnalyticsDataMaker.sku_name, sku);
        analyticParam.Add(AnalyticsDataMaker.detail, storeToken);
        analyticParam.Add(AnalyticsDataMaker.currency_name, "coin");
        analyticParam.Add(AnalyticsDataMaker.sink_source, AnalyticsDataMaker.SOURCE_VALUE);
    }

    public string SKU()
    {
        return (string)analyticParam[AnalyticsDataMaker.item_name];
    }
}


public class AnalyticsData_Purchase_Success_Direct : AnalyticsData_Purchase_Success
{
    public AnalyticsData_Purchase_Success_Direct(string gamePage, string sku, Money realmoney, string storeToken, int lastUnlockedLevel, bool isFirstTime) :
        base(gamePage, sku, realmoney, "GOOGLEPLAY_" + storeToken, lastUnlockedLevel, isFirstTime)
    {
    }
}

public class AnalyticsData_Purchase_Failed : AnalyticsData_Purchase
{
    public AnalyticsData_Purchase_Failed(string gamePage, string sku, Money cost, string failedReason) : base(gamePage)
    {
        analyticParam.Add(AnalyticsDataMaker.item_name, sku);
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_shop_names.purchase_failed.ToString());
    }
}

public class AnalyticsData_Purchase_Instagram : AnalyticsData_Purchase
{
    public AnalyticsData_Purchase_Instagram(string gamePage, int coinCount) : base(gamePage)
    {
        analyticParam.Add(AnalyticsDataMaker.item_name, "instagram");
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_shop_names.shop_instagram);
        analyticParam.Add(AnalyticsDataMaker.sink_source, AnalyticsDataMaker.SOURCE_VALUE);
        analyticParam.Add(AnalyticsDataMaker.currency_name, "coin");
        analyticParam.Add(AnalyticsDataMaker.amount, coinCount);
    }
}

public class AnalyticsData_Purchase_Telegram : AnalyticsData_Purchase
{
    public AnalyticsData_Purchase_Telegram(string gamePage, int coinCount) : base(gamePage)
    {
        analyticParam.Add(AnalyticsDataMaker.item_name, "telegram");
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_shop_names.shop_telegram);
        analyticParam.Add(AnalyticsDataMaker.sink_source, AnalyticsDataMaker.SOURCE_VALUE);
        analyticParam.Add(AnalyticsDataMaker.currency_name, "coin");
        analyticParam.Add(AnalyticsDataMaker.amount, coinCount);
    }
}


#endregion

#region shop_booster
public abstract class AnalyticsData_Shop_Booster : AnalyticsData_Share
{
    public AnalyticsData_Shop_Booster(int coin, int boosterId)
    {
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.match);
        analyticParam.Add(AnalyticsDataMaker.page_source, "booster");
        analyticParam.Add(AnalyticsDataMaker.item_type, "booster");
        analyticParam.Add(AnalyticsDataMaker.item_name, boosterId);
        analyticParam.Add(AnalyticsDataMaker.activity_type, activity_types.shop);
        analyticParam.Add(AnalyticsDataMaker.currency_name, "coin");
        analyticParam.Add(AnalyticsDataMaker.amount, coin);
        analyticParam[AnalyticsDataMaker.page_number] = AnalyticsDataMaker.GetCampaignLastLevelNumber();
        analyticParam["page_name"] = page_names.match_detail;
    }
}

public class AnalyticsData_Shop_Booster_Popup : AnalyticsData_Shop_Booster
{
    public AnalyticsData_Shop_Booster_Popup(int coin, int boosterId) : base(coin, boosterId)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_shop_names.shop_powerup_popup);
    }
}

public class AnalyticsData_Shop_Booster_Success : AnalyticsData_Shop_Booster
{
    public AnalyticsData_Shop_Booster_Success(int coin, int boosterId) : base(coin, boosterId)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_shop_names.shop_powerup_success);
    }
}

public class AnalyticsData_Shop_Booster_Failed : AnalyticsData_Shop_Booster
{
    public AnalyticsData_Shop_Booster_Failed(int coin, int boosterId) : base(coin, boosterId)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_shop_names.shop_powerup_failed);
    }
}

public class AnalyticsData_Shop_Booster_Close : AnalyticsData_Shop_Booster
{
    public AnalyticsData_Shop_Booster_Close(int coin, int boosterId) : base(coin, boosterId)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_shop_names.shop_powerup_close);
    }
}
#endregion

#region rate
public abstract class AnalyticsData_Rate : AnalyticsData_Share
{
    public AnalyticsData_Rate()
    {
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.rate_us);
        analyticParam.Add(AnalyticsDataMaker.activity_type, activity_types.social);
    }
}

public class AnalyticsData_Rate_Popup : AnalyticsData_Rate
{
    public AnalyticsData_Rate_Popup()
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_social_names.rate_us_popup);

    }
}

public class AnalyticsData_Rate_Confirm : AnalyticsData_Rate
{
    public AnalyticsData_Rate_Confirm(int star)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_social_names.rate_us_confirm);
        analyticParam.Add(AnalyticsDataMaker.item_lvl, star);
        analyticParam.Add(AnalyticsDataMaker.item_type, "rate_star");
        analyticParam.Add(AnalyticsDataMaker.item_name, "rate_star");
    }
}

public class AnalyticsData_Rate_Close : AnalyticsData_Rate
{
    public AnalyticsData_Rate_Close()
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, activity_social_names.rate_us_close);
    }
}
#endregion

#region spinner
public abstract class AnalyticsData_Spinner : AnalyticsData_Share
{
    public AnalyticsData_Spinner()
    {
        AnalyticsDataMaker.AddSet_page_type_and_page_name(analyticParam, page_names.spinner);
        analyticParam.Add(AnalyticsDataMaker.activity_type, activity_types.spinner);
    }
}

public class AnalyticsData_Spinner_Reward : AnalyticsData_Spinner
{
    public AnalyticsData_Spinner_Reward(int itemIndex, int coinReward)
    {
        analyticParam.Add(AnalyticsDataMaker.activity_name, "win");
        analyticParam.Add(AnalyticsDataMaker.item_type, "reward");
        analyticParam.Add(AnalyticsDataMaker.item_name, itemIndex);
        analyticParam.Add(AnalyticsDataMaker.currency_name, "coin");
        analyticParam.Add(AnalyticsDataMaker.sink_source, AnalyticsDataMaker.SOURCE_VALUE);
        analyticParam.Add(AnalyticsDataMaker.amount, coinReward);
    }
}
#endregion

#region flags
public abstract class AnalyticsData_Flag : AnalyticsData_Share
{
    public AnalyticsData_Flag()
    {

    }
}

public class AnalyticsData_Flag_First_Open : AnalyticsData_Flag
{
    public AnalyticsData_Flag_First_Open()
    {
        analyticParam.Add(AnalyticsDataMaker.flag_type, "progress");
        analyticParam.Add(AnalyticsDataMaker.flag_name, "first_open");
    }
}

public class AnalyticsData_Flag_First_CompleteAd : AnalyticsData_Flag
{
    public AnalyticsData_Flag_First_CompleteAd()
    {
        analyticParam.Add(AnalyticsDataMaker.flag_type, "monetize");
        analyticParam.Add(AnalyticsDataMaker.flag_name, "first_ad");
    }
}

public class AnalyticsData_Flag_First_TaskDone : AnalyticsData_Flag
{
    public AnalyticsData_Flag_First_TaskDone()
    {
        analyticParam.Add(AnalyticsDataMaker.flag_type, "progress");
        analyticParam.Add(AnalyticsDataMaker.flag_name, "first_task");
    }
}

public class AnalyticsData_Flag_First_Purchase : AnalyticsData_Flag
{
    public readonly float revenue;
    public readonly string revenueCurrency;


    public AnalyticsData_Flag_First_Purchase(string sku, Money realMoney)
    {
        AnalyticsDataMaker.ExtractRevenueAndCurrency(realMoney, out revenue, out revenueCurrency);

        analyticParam.Add(AnalyticsDataMaker.sku_name, sku);
        analyticParam.Add(AnalyticsDataMaker.revenue, revenue);
        analyticParam.Add(AnalyticsDataMaker.flag_type, "monetize");
        analyticParam.Add(AnalyticsDataMaker.flag_name, "first_purchase");
    }

    public string SKU()
    {
        return (string) analyticParam[AnalyticsDataMaker.sku_name];
    }
}

//public class AnalyticsData_Flag_First_TextFix : AnalyticsData_Flag_First_Purchase
//{
//    public AnalyticsData_Flag_First_TextFix(string sku, Money realMoney) : base(sku, realMoney)
//    {
//    }
//}

public class AnalyticsData_Flag_Cohort_Assign : AnalyticsData_Flag
{
    public AnalyticsData_Flag_Cohort_Assign(string cohortGroup)
    {
        analyticParam.Add(AnalyticsDataMaker.flag_type, "cohort");
        analyticParam.Add(AnalyticsDataMaker.flag_name, cohortGroup);
    }
}
#endregion


#region LiveOps

public class AnalyticsData_WinStreak_Load : AnalyticsDataBase
{
    public readonly string winStreakType;
    public readonly int winStreakStep;

    public AnalyticsData_WinStreak_Load(string winStreakType, int winStreakStep)
    {
        this.winStreakType = winStreakType;
        this.winStreakStep = winStreakStep;
    }
}

#endregion

#region ReferralMarketing

public abstract class AnalyticsData_Referral : AnalyticsDataBase {}


public class AnalyticsData_Referral_SharingResult : AnalyticsData_Referral
{
    public AnalyticsData_Referral_SharingResult(bool wasSuccessful, string segmentCaller, string sharingChannel)
    {
        analyticParam.Add(AnalyticsDataMaker.WasSuccessful, wasSuccessful);
        analyticParam.Add(AnalyticsDataMaker.ShareSegmentName, segmentCaller);
        analyticParam.Add(AnalyticsDataMaker.ShareChannelName, sharingChannel);
    }

    public string ShareStatus => (bool)analyticParam[AnalyticsDataMaker.WasSuccessful] == true ? "Succeeded" : "Failed";
    public string ShareSegmentName => (string)analyticParam[AnalyticsDataMaker.ShareSegmentName];
    public string ShareChannelName => (string)analyticParam[AnalyticsDataMaker.ShareChannelName];
}

public class AnalyticsData_Referral_UseCode : AnalyticsData_Referral
{
    public AnalyticsData_Referral_UseCode(string fromSegmentName)
    {
        analyticParam.Add(AnalyticsDataMaker.ShareSegmentName, fromSegmentName);
    }
    
    public string FromSegmentName => (string)analyticParam[AnalyticsDataMaker.ShareSegmentName];
}

public class AnalyticsData_Referral_NewUserUsedCode : AnalyticsData_Referral
{
    public AnalyticsData_Referral_NewUserUsedCode(string fromSegmentName)
    {
        analyticParam.Add(AnalyticsDataMaker.ShareSegmentName, fromSegmentName);
    }
    
    public string FromSegmentName => (string)analyticParam[AnalyticsDataMaker.ShareSegmentName];
}

public class AnalyticsData_Referral_ClaimReward : AnalyticsData_Referral
{
    public AnalyticsData_Referral_ClaimReward(int prizeId)
    {
        analyticParam.Add(AnalyticsDataMaker.ReferralGoalPrizeId, prizeId.ToString());
    }

    public string PrizeId => (string) analyticParam[AnalyticsDataMaker.ReferralGoalPrizeId];
}

public class AnalyticsData_Referral_FirstPurchase : AnalyticsData_Referral  {}

public class AnalyticsData_Referral_PurchaseSuccess : AnalyticsData_Referral
{
    public readonly float revenue;
    public readonly string revenueCurrency;
    
    public AnalyticsData_Referral_PurchaseSuccess(string sku, Money money)
    {
        AnalyticsDataMaker.ExtractRevenueAndCurrency(money, out revenue, out revenueCurrency);

        analyticParam.Add(AnalyticsDataMaker.item_name, sku);
        analyticParam.Add(AnalyticsDataMaker.revenue, revenue);
        analyticParam.Add(AnalyticsDataMaker.sku_name, sku);
    }
}

public class AnalyticsData_Referral_GameProgress : AnalyticsData_Referral
{
    public AnalyticsData_Referral_GameProgress(int level)
    {
        analyticParam.Add(AnalyticsDataMaker.WonLevel, level.ToString());
    }

    public string WonLevel => (string) analyticParam[AnalyticsDataMaker.WonLevel];
}

public class AnalyticsData_Referral_ReferredPlayersCount : AnalyticsData_Referral
{
    public AnalyticsData_Referral_ReferredPlayersCount(int referredCount)
    {
        analyticParam.Add(AnalyticsDataMaker.ReferredPlayersCount, referredCount.ToString());
    }

    public string ReferredPlayersCount => (string) analyticParam[AnalyticsDataMaker.ReferredPlayersCount];
}

public class AnalyticsData_Referral_ReferralReminderOpen : AnalyticsData_Referral
{
    public bool isReferralCenterOpened { private set; get; }

    public AnalyticsData_Referral_ReferralReminderOpen(bool isReferralCenterOpened)
    {
        this.isReferralCenterOpened = isReferralCenterOpened;
    }
}

#endregion


#region NeighbourHood

public class AnalyticsData_NeighbourhoodChangeLevel : AnalyticsDataBase
{
    public readonly int previousLevel;

    public AnalyticsData_NeighbourhoodChangeLevel(int previousLevel)
    {
        this.previousLevel = previousLevel;
    }
}

public class AnalyticsData_NeighbourhoodTicketBuy : AnalyticsDataBase
{
    public readonly int neighbourhoodScore;

    public AnalyticsData_NeighbourhoodTicketBuy(int neighbourhoodScore)
    {
        this.neighbourhoodScore = neighbourhoodScore;
    }
}

#endregion


#region Warnings

public static class WarningSource
{
    public const string DogTraining = "DogTraining";
    public const string SeasonPass = "SeasonPass";
}

public class AnalyticsData_Warning : AnalyticsDataBase
{
    public AnalyticsData_Warning(string warningSource)
    {
        analyticParam.Add(AnalyticsDataMaker.WarningSource, warningSource);
    }
    
    public string EventSource => analyticParam[AnalyticsDataMaker.WarningSource].ToString();
}

public class AnalyticsData_Warning_LevelExit : AnalyticsData_Warning
{
    public string Placement = "ExitLevel";
    
    public AnalyticsData_Warning_LevelExit(string warningSource, bool result): base(warningSource)
    {
        analyticParam.Add(AnalyticsDataMaker.WarningLevelExitResult, result);
    }
    
    public bool Result => (bool) analyticParam[AnalyticsDataMaker.WarningLevelExitResult];
}

public class AnalyticsData_Warning_GameOver : AnalyticsData_Warning
{
    public string Placement = "GameOver";
    
    public AnalyticsData_Warning_GameOver(string warningSource, GameOverPopupResult result): base(warningSource)
    {
        analyticParam.Add(AnalyticsDataMaker.WarningGameOverResult, result);
    }
    
    public GameOverPopupResult Result => (GameOverPopupResult) analyticParam[AnalyticsDataMaker.WarningGameOverResult];
}

#endregion


#region Other

public class AnalyticsData_MerchButtonClick : AnalyticsDataBase
{
    public string UserPayingState { get; }

    public AnalyticsData_MerchButtonClick()
    {
        UserPayingState = IsPayingUser() ? "Paying" : "NonPaying";

        bool IsPayingUser()
        {
            return Base.gameManager.profiler.PurchaseCount > 0;
        }
    }
}

public class AnalyticsData_Flag_FaultyBehaviourDetected : AnalyticsData_Flag
{
    public readonly string detectionMode;

    public AnalyticsData_Flag_FaultyBehaviourDetected(string faultyBehaviourDetectionMode)
    {
        analyticParam.Add(AnalyticsDataMaker.flag_name , "faultyBehaviour_DetectionMode");
        analyticParam.Add(AnalyticsDataMaker.flag_type , "general");
        detectionMode = faultyBehaviourDetectionMode;
    }
}

public class AnalyticsData_TimeCheatDetected : AnalyticsDataBase
{
    public AnalyticsData_TimeCheatDetected()
    {
    }
}

public class AnalyticsData_Flag_StoreSwitched : AnalyticsData_Flag
{
    public readonly string previousStoreName;

    public AnalyticsData_Flag_StoreSwitched(string previousStoreName)
    {
        analyticParam.Add(AnalyticsDataMaker.flag_name, "storeSwitch");
        analyticParam.Add(AnalyticsDataMaker.flag_type, "general");
        this.previousStoreName = previousStoreName;
    }
}
#endregion

#region BoardPreview
public class AnalyticsData_BoardPreview : AnalyticsDataBase
{
    public AnalyticsData_BoardPreview()
    {
    }
}

public class AnalyticsData_BoardPreview_LoseLevelShown : AnalyticsData_BoardPreview
{
    public AnalyticsData_BoardPreview_LoseLevelShown()
    {
    }
}

public class AnalyticsData_BoardPreview_ResumeLevelWithExtraMove : AnalyticsData_BoardPreview
{
    public AnalyticsData_BoardPreview_ResumeLevelWithExtraMove()
    {
    }
}

public class AnalyticsData_BoardPreview_PreviewGameBoardClicked : AnalyticsData_BoardPreview
{
    public AnalyticsData_BoardPreview_PreviewGameBoardClicked()
    {
    }
}
#endregion