/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-23 18:17:09 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class PlayerProfileView : View
    {
        [System.Serializable]
        public struct _PlayerInfo
        {
            public Image profilePicture;
            public Image profilePictureBorder;
            public Image countryFlag;
            public Text nameLabel;
            public Text tagLabel;
            public Image leagueBadge;
            public Text leagueNameLabel;
            public Image xpBarFill;
            public Text levelLabel;
            public Text xpLabel;
        }

        [System.Serializable]
        public struct _CellInfo
        {
            public Text titleLabel;
            public Text valueLabel;
        }

        private struct RoomTitleCellInfo
        {
            public string roomId;
            public long gameDuration;
            public int trophiesWon;
            public string roomTitleId;
        }

        // TODO: Remove this injection, views cannot inject services or models
        [Inject] public ILocalizationService localizationService { get; set; }

        // Sprite cache is taken from the SpriteCache gameobject in the scene
        public SpriteCache spriteCache;

        public RoomTitleCell roomTitleCellPrefab;
        public GameObject separatorPrefab;

        public _PlayerInfo player;

        public _CellInfo currency1;
        public _CellInfo currency2;
        public _CellInfo currency1Winnings;
        public _CellInfo winRate;
        public _CellInfo gamesWon;
        public _CellInfo gamesLost;
        public _CellInfo gamesDrawn;

        public Text roomTitlesHeaderLabel;
        public Text noRoomTitlesLabel;

        public GameObject noRoomTitlesCell;
        public GameObject roomTitleCells;

        public Button backButton;

        public Signal backButtonClickedSignal = new Signal();

        public void Init()
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        public void UpdateView(PlayerProfileVO vo)
        {
            UpdateProfilePicture(vo.profilePicture);
            UpdateProfilePictureBorder(vo.playerModel.profilePictureBorder);

            player.countryFlag.sprite = spriteCache.GetCountryFlag(vo.countryId);
            player.nameLabel.text = vo.name;
            player.tagLabel.text = "#" + vo.tag;

            player.leagueBadge.sprite = spriteCache.GetLeagueBadge(vo.leagueId);
            player.leagueNameLabel.text = localizationService.Get(vo.leagueId);
            player.levelLabel.text = localizationService.Get(LocalizationKey.PP_LEVEL_LABEL, vo.level);

            // If the player has reached the maximum level then we don't display
            // the XP. Instead we notify of the max level and show the XP bar as
            // full.
            if (vo.hasReachedMaxLevel)
            {
                player.xpBarFill.fillAmount = 1f;
                player.xpLabel.text = localizationService.Get(LocalizationKey.PP_XP_MAX_LEVEL_LABEL);
            }
            else
            {
                // Normalize xp to start from zero for the level to display on
                // player profile.
                // Add 1 to levelEndXp because we need to display the starting
                // XP of the next level as the ending XP of the current level.
                int levelEndXp = (vo.levelEndXp - vo.levelStartXp) + 1;
                int xp = vo.xp - vo.levelStartXp;

                player.xpBarFill.fillAmount = (float)xp / levelEndXp;
                player.xpLabel.text = localizationService.Get(LocalizationKey.PP_XP_LABEL, xp, levelEndXp);
            }

            currency1.titleLabel.text = localizationService.Get(LocalizationKey.PP_CURRENCY_1_TITLE_LABEL);
            currency1.valueLabel.text = vo.currency1.ToString("N0");

            currency2.titleLabel.text = localizationService.Get(LocalizationKey.PP_CURRENCY_2_TITLE_LABEL);
            currency2.valueLabel.text = vo.currency2.ToString("N0");

            currency1Winnings.titleLabel.text = localizationService.Get(LocalizationKey.PP_CURRENCY_1_WINNINGS_TITLE_LABEL);
            currency1Winnings.valueLabel.text = vo.currency1Winnings.ToString("N0");

            winRate.titleLabel.text = localizationService.Get(LocalizationKey.PP_WIN_RATE_TITLE_LABEL);
            float percentage = (vo.totalGames == 0) ? 0 : ((float)vo.totalGamesWon / vo.totalGames) * 100;
            winRate.valueLabel.text = localizationService.Get(LocalizationKey.PP_WIN_RATE_LABEL, percentage);

            gamesWon.titleLabel.text = localizationService.Get(LocalizationKey.PP_GAMES_WON_TITLE_LABEL);
            gamesWon.valueLabel.text = localizationService.Get(LocalizationKey.PP_GAMES_WON_LABEL, vo.totalGamesWon, vo.totalGames);

            gamesLost.titleLabel.text = localizationService.Get(LocalizationKey.PP_GAMES_LOST_TITLE_LABEL);
            gamesLost.valueLabel.text = localizationService.Get(LocalizationKey.PP_GAMES_LOST_LABEL, vo.totalGamesLost, vo.totalGames);

            gamesDrawn.titleLabel.text = localizationService.Get(LocalizationKey.PP_GAMES_DRAWN_TITLE_LABEL);
            gamesDrawn.valueLabel.text = localizationService.Get(LocalizationKey.PP_GAMES_DRAWN_LABEL, vo.totalGamesDrawn, vo.totalGames);
            
            roomTitlesHeaderLabel.text = localizationService.Get(LocalizationKey.PP_ROOM_TITLES_HEADER_LABEL);
            noRoomTitlesLabel.text = localizationService.Get(LocalizationKey.PP_NO_ROOM_TITLES_LABEL);

            IList<RoomTitleCellInfo> roomTitleInfos = new List<RoomTitleCellInfo>();

            foreach (RoomRecordVO recordVO in vo.roomRecords.Values)
            {
                string roomTitleId = recordVO.roomTitleId;

                if (roomTitleId != RoomTitleId.NONE)
                {
                    RoomTitleCellInfo info;
                    info.roomId = recordVO.id;
                    info.gameDuration = recordVO.gameDuration;
                    info.trophiesWon = recordVO.trophiesWon;
                    info.roomTitleId = roomTitleId;

                    roomTitleInfos.Add(info);
                }
            }

            // Destroy all old room title cells if there are any. Leave the
            // first 2 children as they are not room title cells.
            UnityUtil.DestroyChildren(roomTitleCells.transform, 2);

            if (roomTitleInfos.Count == 0)
            {
                noRoomTitlesCell.SetActive(true);
            }
            else
            {
                noRoomTitlesCell.SetActive(false);

                for (int i = 0; i < roomTitleInfos.Count; ++i)
                {
                    RoomTitleCellInfo info = roomTitleInfos[i];

                    // TODO(mubeeniqbal): Create a common method to convert
                    // gameDuration into minutes. One hindsight gameDuration
                    // must be a TimeSpan object which will automatically handle
                    // all conversions.
                    int gameDurationMinutes = (int)(info.gameDuration / 60000);

                    RoomTitleCell cell = Instantiate<RoomTitleCell>(roomTitleCellPrefab);
                    cell.roomFlag.sprite = spriteCache.GetRoomFlagMinor(info.roomId);
                    cell.roomNameLabel.text = localizationService.Get(info.roomId);
                    cell.roomDurationLabel.text = localizationService.Get(LocalizationKey.PP_ROOM_DURATION_LABEL, gameDurationMinutes);
                    cell.trophiesWonLabel.text = localizationService.Get(LocalizationKey.PP_TROPHIES_WON_LABEL, info.trophiesWon);
                    cell.roomTitleLabel.text = localizationService.Get(info.roomTitleId);

                    // It is very important that worldPositionStays is set to
                    // false otherwise the cell will have incorrect size on
                    // screen sizes/resolutions other than the one it is
                    // originally designed for.
                    cell.transform.SetParent(roomTitleCells.transform, false);

                    // Don't put a separator after the last cell.
                    if (i < (roomTitleInfos.Count - 1))
                    {
                        GameObject separator = Instantiate<GameObject>(separatorPrefab);

                        // It is very important that worldPositionStays is set to
                        // false otherwise the cell will have incorrect size on
                        // screen sizes/resolutions other than the one it is
                        // originally designed for.
                        separator.transform.SetParent(roomTitleCells.transform, false);
                    }
                }
            }
        }

        public void UpdateProfilePicture(Sprite sprite)
        {
            player.profilePicture.sprite = sprite;
            player.profilePicture.gameObject.SetActive(sprite != null);
        }

        public void UpdateProfilePictureBorder(Sprite sprite)
        {
            player.profilePictureBorder.sprite = sprite;
            player.profilePictureBorder.gameObject.SetActive(sprite != null);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnBackButtonClicked()
        {
            backButtonClickedSignal.Dispatch();
        }
    }
}
