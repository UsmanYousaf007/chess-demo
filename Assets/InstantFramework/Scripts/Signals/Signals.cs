/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

using strange.extensions.signal.impl;
using System.Collections.Generic;
using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public class ReconnectionCompleteSignal : Signal { }
    public class StartSignal : Signal { }
    public class AppEventSignal : Signal<AppEvent> { }
    public class GameAppEventSignal : Signal<AppEvent> { }
    public class InitFacebookSignal : Signal { }
    public class GameDisconnectingSignal : Signal { }
    public class SetUpdateURLSignal : Signal<string> { }
    public class FindMatchSignal : Signal<string> { }
    public class FindMatchCompleteSignal : Signal<string> { }
    public class FindRandomLongMatchCompleteSignal : Signal { }
    public class TapLongMatchSignal : Signal<string, bool> { }
    public class CreateLongMatchSignal : Signal<string, bool> { }
    public class StartLongMatchSignal : Signal<string> { }
    public class MatchFoundSignal : Signal<ProfileVO> { }
    public class GetGameStartTimeSignal : Signal { }
    public class ShowFindMatchSignal : Signal<string> { }
    public class AudioStateChangedSignal : Signal<bool> { }
    public class NavigatorEventSignal : Signal<NavigatorEvent> { }
    public class NavigatorShowViewSignal : Signal<NavigatorViewId> { }
    public class NavigatorHideViewSignal : Signal<NavigatorViewId> { }
    public class NavigatorIgnoreEventSignal : Signal<NavigatorEvent> { }
    public class ModelsResetSignal : Signal { }
    public class ModelsSaveToDiskSignal : Signal { }
    public class ModelsLoadFromDiskSignal : Signal { }
    public class GetInitDataSignal : Signal<bool> { }
    public class GetInitDataCompleteSignal : Signal { }
    public class GetInitDataFailedSignal : Signal<BackendResult> { }
    public class AuthFaceBookSignal : Signal { }
    public class AuthSignInWithAppleSignal : Signal { }
    public class AuthFacebookResultSignal : Signal<AuthFacebookResultVO> { }
    public class AuthSignInWithAppleResultSignal : Signal<AuthSignInWIthAppleResultVO> { }
    public class SignOutSocialAccountSignal : Signal { }
    public class UpdateProfileSignal : Signal<ProfileVO> { }
    public class UpdateTrophiesSignal : Signal<int> { }
    public class UpdateOpponentProfileSignal : Signal<ProfileVO> { }
    public class UpdateChatOpponentPicSignal : Signal<Sprite> { }
    public class RemoteStorePurchaseCompletedSignal : Signal<string> { }
    public class BackendErrorSignal : Signal<BackendResult> { }
    public class ReceptionSignal : Signal<bool> { }
    public class LoadLobbySignal : Signal { }
    public class SavePlayerInventorySignal : Signal<string> { }
    public class SetSkinSignal : Signal<string> { }
    public class SetDefaultSkinSignal : Signal { }
    public class SkinUpdatedSignal : Signal { }
    public class InitBackendOnceSignal : Signal { }
    public class PurchaseStoreItemSignal : Signal<string, bool> { }
    public class PurchaseStoreItemResultSignal : Signal<StoreItem, PurchaseResult> { }
    public class UpdatePurchasedStoreItemSignal : Signal<StoreItem> { }
    public class ConsumeVirtualGoodSignal : Signal<string, int> { }
    public class StartGameSignal : Signal { }
    public class WifiIsHealthySignal : Signal<bool> { }
    public class SplashWifiIsHealthySignal : Signal { }
    public class ShowSplashContentSignal : Signal<bool> { }
    public class ResetCapturedPiecesSignal : Signal { }
    public class UpdateSearchResultsSignal : Signal<bool> { }
    public class SetErrorAndHaltSignal : Signal<BackendResult, string> { }
    public class HaltSignal : Signal<BackendResult> { }
    public class UpdatePlayerRewardsPointsSignal : Signal<float, float> { }
    public class UpdatePlayerInventorySignal : Signal<PlayerInventoryVO> { }
    public class UpdateRemoveAdsSignal : Signal<string, bool> { }
    public class AddFriendsSignal : Signal<Dictionary<string, Friend>, FriendCategory> { }
    public class RefreshCommunitySignal : Signal<bool> { }
    public class SearchFriendSignal : Signal<string, int> { }
    public class RefreshFriendsSignal : Signal { }
    public class RemoveFriendSignal : Signal<string> { }
    public class GetSocialPicsSignal : Signal<Dictionary<string, Friend>> { }
    public class NewFriendSignal : Signal<string, bool> { }
    public class BlockFriendSignal : Signal<string> { }
    public class LoadChatSignal : Signal<string, bool> { }
    public class RemoveCommunityFriendSignal : Signal<string> { }
    public class RemoveRecentlyPlayedSignal : Signal<string, FriendsSubOp> { }
    public class UpdateFriendOnlineStatusSignal : Signal<ProfileVO> { }
    public class ForceUpdateFriendOnlineStatusSignal : Signal<ProfileVO> { }
    public class ClearCommunitySignal : Signal { }
    public class ClearFriendsSignal : Signal { }
    public class ClearFriendSignal : Signal<string> { }
    public class UpdateFriendPicSignal : Signal<string, Sprite> { }
    public class UpdateEloScoresSignal : Signal<EloVO> { }
    public class FriendsShowConnectFacebookSignal : Signal<bool> { }
    public class UpdateProfileDialogSignal : Signal<ProfileDialogVO> { }
    public class UpdateShareDialogSignal : Signal<Sprite> { }
    public class UpdateNewGameDialogSignal : Signal<string> { }
    public class ShowProfileDialogSignal : Signal<string, FriendBar> { }
    public class ShowShareScreenDialogSignal : Signal { }
    public class LoadFriendsSignal : Signal { }
    public class ShareAppSignal : Signal { }
    public class ShowAdSignal : Signal<ResultAdsVO, bool> { }
    public class ShowRewardedAdSignal : Signal<AdPlacements> { }
    public class ToggleBannerSignal : Signal<bool> { }
    public class PauseNotificationsSignal : Signal<bool> { }
    public class RequestToggleBannerSignal : Signal { }
    public class UpdateFriendBarStatusSignal : Signal<LongPlayStatusVO> { }
    public class UpdateFriendBarSignal : Signal<Friend, string> { }
    public class FriendBarBusySignal : Signal<string, bool, CreateLongMatchAbortReason> { }
    public class ToggleFacebookButton : Signal<bool> { }
    public class AcceptSignal : Signal<string> { }
    public class DeclineSignal : Signal<string> { }
    public class CloseStripSignal : Signal<string> { }
    public class UnregisterSignal : Signal<string> { }
    public class SortFriendsSignal : Signal { }
    public class SortCommunitySignal : Signal { }
    public class SortSearchedSignal : Signal<bool> { }
    public class SetActionCountSignal : Signal<int> { }
    public class ShowFriendsHelpSignal : Signal { }
    public class SendChatMessageSignal : Signal<ChatMessage> { }
    public class ReceiveChatMessageSignal : Signal<ChatMessage, bool> { }
    public class DisplayChatMessageSignal : Signal<ChatMessage> { }
    public class ClearActiveChatSignal : Signal<string> { }
    public class ClearUnreadMessagesSignal : Signal<string> { }
    public class AddUnreadMessagesToBarSignal : Signal<string, int> { }
    public class ClearUnreadMessagesFromBarSignal : Signal<string> { }
    public class RestorePurchasesSignal : Signal { }
    public class NewFriendAddedSignal : Signal<string> { }
    public class NotificationRecievedSignal : Signal<NotificationVO> { }
    public class PreShowNotificationSignal : Signal { }
    public class ShowViewBoardResultsPanelSignal : Signal<bool> { }
    public class PostShowNotificationSignal : Signal { }
    public class ResumeMatchSignal : Signal<NavigatorViewId> { }
    public class UpdatePlayerNotificationCountSignal : Signal<int> { }
    public class UpdatePlayerDataSignal : Signal { }
    public class ChallengeAcceptedSignal : Signal { }
    public class ChangeUserDetailsSignal : Signal<string> { }
    public class OpponentPingedForConnectionSignal : Signal<bool, int> { }
    public class ShowInGameProfileSingal : Signal { }
    public class ChessboardBlockerEnableSignal : Signal<bool> { }
    public class ReconnectViewEnableSignal : Signal<bool> { }
    public class UpdateFindViewSignal : Signal<FindViewVO> { }
    public class UpdateConfirmDlgSignal : Signal<ConfirmDlgVO> { }
    public class ContactSupportSignal : Signal { }
    public class FindMatchRequestCompleteSignal : Signal<string> { }
    public class StoreAvailableSignal : Signal<bool> { }
    public class SyncReconnectDataSignal : Signal<string> { }
    public class CancelSearchResultSignal : Signal { }
    public class RewardUnlockedSignal : Signal<string, int> { }
    public class ThemeAlertDisableSignal : Signal { }
    public class ShowPromotionDlgSignal : Signal<IPromise<AdsResult>, InternalAdType> { }
    public class ShowPromotionUpdateDlgSignal : Signal<PromotionVO> { }
    public class ClosePromotionDlgSignal : Signal { }
    public class ClosePromotionUpdateDlgSignal : Signal { }
    public class SubscriptionDlgClosedSignal : Signal { }
    public class ShowAdSkippedDlgSignal : Signal { }
    public class DisableModalBlockersSignal : Signal { }
    public class SelectTierSignal : Signal<string> { }
    public class SetSubscriptionContext : Signal<string> { }
    public class MatchAnalyticsSignal : Signal<MatchAnalyticsVO> { }
    public class ManageBlockedFriendsSignal : Signal<string, bool> { }
    public class UnblockFriendSignal : Signal<string> { }
    public class UpdateManageBlockedFriendsViewSignal : Signal<Dictionary<string, Friend>> { }
    public class ResetUnblockButtonSignal : Signal<string> { }
    public class UpdateOfferDrawSignal : Signal<OfferDrawVO> { }
    public class SkillSelectedSignal : Signal { }
    public class UploadFileSignal : Signal<UploadFileVO> { }
    public class PhotoPickerCompleteSignal : Signal<PhotoVO> { }
    public class VideoEventSignal : Signal<VideoEvent> { }
    public class SaveLastWatchedVideoSignal : Signal<string> { }
    public class LoadArenaSignal : Signal { }
    public class UpdateShopBundlePurchasedViewSignal : Signal<StoreItem> { }
    public class VirtualGoodsTransactionSignal : Signal<VirtualGoodsTransactionVO> { }
    public class VirtualGoodBoughtSignal : Signal<string, int> { }
    public class ShowInventoryRewardedVideoSignal : Signal<InventoryVideoVO> { }
    public class InventoryVideoResultSignal : Signal<InventoryVideoResult, string> { }
    public class VirtualGoodsTransactionResultSignal : Signal<BackendResult> { }
    public class UpdateBottomNavSignal : Signal<BottomNavView.ButtonId> { }
    public class InboxAddMessagesSignal : Signal { }
    public class InboxRemoveMessagesSignal : Signal<string> { }
    public class LoadRewardsSignal : Signal { }
    public class UpdateInboxMessageCountViewSignal : Signal<long> { }
    public class UpdateRewardDlgViewSignal : Signal<RewardDlgVO> { }
    public class LoadRewardDlgViewSignal : Signal<string, Signal> { }
    public class InboxFetchingMessagesSignal : Signal<bool> { }
    public class PlayerModelUpdatedSignal : Signal<IPlayerModel> { }
    public class ProfilePictureLoadedSignal : Signal<string, Sprite> { }
    public class GetProfilePictureSignal : Signal<GetProfilePictureVO> { }
    public class ShowBottomNavSignal : Signal<bool> { }
    public class AppResumedSignal : Signal { }
    public class LoadSpotInventorySignal : Signal<LoadSpotInventoryParams> { }
    public class UpdateSpotInventoryViewSignal : Signal<SpotInventoryVO> { }
    public class SpotInventoryPurchaseCompletedSignal : Signal<string, string> { }
    public class TournamentOverDialogueClosedSignal : Signal { }
    public class ClearInboxSignal : Signal { }
    public class ResetSubscirptionStatusSignal : Signal { }
    public class ActivePromotionSaleSingal : Signal<string> { }
    public class ShowFadeBlockerSignal : Signal { }
    public class PromotionCycleOverSignal : Signal { }
    public class LoginAsGuestSignal : Signal { }
    public class LoadSpotCoinPurchaseSignal : Signal<long> { }
    public class UpdateSpotCoinsPurchaseDlgSignal : Signal<long, List<string>> { }
    public class UpdateSpotCoinsWatchAdDlgSignal : Signal<long, StoreItem, AdPlacements> { }
    public class UpdateRewardDlgV2ViewSignal : Signal<RewardDlgV2VO> { }
    public class RewardedVideoResultSignal : Signal<AdsResult, AdPlacements> { }
    public class RewardedVideoAvailableSignal : Signal<AdPlacements> { }
    public class UpdateLeaguePromotionDlgViewSignal : Signal<RewardDlgVO> { }
    public class LoadCareerCardSignal : Signal { }
    public class RateAppDlgClosedSignal : Signal { }
    public class LobbyChestRewardClaimedSignal : Signal<int> { }
    public class InboxEmptySignal : Signal { }
    public class UpdatePurchaseSuccessDlgSignal : Signal<StoreItem> { }
    public class RankPromotedDlgClosedSignal : Signal { }
    public class OutOfGemsSignal : Signal { }
    public class LobbySequenceEndedSignal : Signal { }
    public class SpotCoinsPurchaseDlgClosedSignal : Signal { }

    // Tournaments
    public class GetAllTournamentsSignal : Signal { }
    public class TournamentOpFailedSignal : Signal { }
    public class GetTournamentsSuccessSignal : Signal { }
    public class UpdateTournamentsSignal : Signal { }
    public class GetTournamentLeaderboardSignal : Signal<string, bool> { }
    public class FetchLiveTournamentRewardsSignal : Signal<string> { }
    public class UpdateTournamentLeaderboardPartialSignal : Signal<string> { }
    public class UpdateTournamentLeaderboardSignal : Signal<string> { }
    public class UpdateTournamentLeaderboardViewSignal : Signal { }
    public class ToggleLeaderboardViewNavButtons : Signal<bool> { }
    public class UpdateTournamentsViewSignal : Signal { }
    public class ResetTournamentsViewSignal : Signal { }
    public class UpdateChampionshipResultDlgSignal : Signal<RewardDlgVO> { }
    public class UpdateLiveTournamentRewardsSuccessSignal : Signal<string> { }
    public class UpdateChestInfoDlgViewSignal : Signal<TournamentReward> { }
    public class UnlockCurrentJoinedTournamentSignal : Signal { }
    public class OnTournamentEndRewardViewClickedSignal : Signal<string> { }

    // SKINS
    public class LoadSkinRefsSignal : Signal<string> { }
    public class RefreshSkinLinksSignal : Signal { }
    public class ShowMaintenanceViewSignal : Signal<int> { }
    public class RatingBoostedSignal : Signal<int, int> { }

    // LeagueProfileStrip
    public class LeagueProfileStripSetOnClickSignal : Signal<Signal> { }
    public class UpdateLeagueProfileStripSignal : Signal<LeagueProfileStripVO> { }
    //public class UpdateLeagueProfileSignal : Signal<string, ProfileVO> { }
    public class UpdateLeagueProfileSignal : Signal<string> { }
    public class SetLeaguesSignal : Signal { }

    //Downloadable Content
    public class DownloadableContentEventSignal : Signal<ContentType?, ContentDownloadStatus> { }

    //App Updates
    public class AppUpdateSignal : Signal<bool> { }

    public class UpdateCareerCardSignal : Signal<CareerCardVO> { }
    public class LoadLeaderboardSignal : Signal { }

    // All star leaderboard
    public class GetAllStarLeaderboardSignal : Signal { }
    public class UpdateAllStarLeaderboardSignal : Signal { }
    public class UpdateTimeSelectDlgSignal : Signal<long> { }

    public class StartLobbyChampionshipTimerSignal : Signal { }
}
