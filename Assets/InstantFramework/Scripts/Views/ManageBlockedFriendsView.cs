using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class ManageBlockedFriendsView : View
    {
        public Transform friendBarContainer;
        public GameObject friendBarPrefab;
        public TMP_InputField inputField;
        public Button cancelSearchButton;
        public Text searchBoxText;
        public Button backButton;
        public Text sectionHeaderText;
        public GameObject emptyListSection;
        public Text emptyListText;
        public GameObject processing;
        public GameObject uiBlocker;

        // Dispatch Signals
        public Signal backButtonPressedSignal = new Signal();
        public Signal<string> onSubmitSearchSignal = new Signal<string>();
        public Signal<string> onUnblockButtonPressedSignal = new Signal<string>();

        // Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        private string searchQuery = string.Empty;
        private Dictionary<string, FriendBarSimple> bars;
        private GameObjectsPool simpleFriendBarsPool;

        public void Init()
        {
            sectionHeaderText.text = localizationService.Get(LocalizationKey.FRIENDS_BLOCKED);
            inputField.onEndEdit.AddListener(OnSearchSubmit);
            backButton.onClick.AddListener(OnBackButtonPressed);
            cancelSearchButton.onClick.AddListener(OnCancelSearchButtonPressed);
            bars = new Dictionary<string, FriendBarSimple>();
            simpleFriendBarsPool = new GameObjectsPool(friendBarPrefab);
        }

        public void Show()
        {
            ResetSearch();
            gameObject.SetActive(true);
            processing.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateView(Dictionary<string, Friend> blockedFriends)
        {
            processing.SetActive(false);
            uiBlocker.SetActive(false);
            ClearFriendsBarContainer();
            AddToFriendsBarContrainer(blockedFriends);
        }

        private void OnSearchSubmit(string s)
        {
            searchQuery = inputField.text;
            searchQuery = searchQuery.Replace("\n", " ");
            searchBoxText.text = searchQuery;
            onSubmitSearchSignal.Dispatch(searchQuery);
            audioService.PlayStandardClick();
            cancelSearchButton.gameObject.SetActive(true);
        }

        private void OnBackButtonPressed()
        {
            backButtonPressedSignal.Dispatch();
            audioService.PlayStandardClick();
        }

        private void OnCancelSearchButtonPressed()
        {
            ResetSearch();
            onSubmitSearchSignal.Dispatch(string.Empty);
            audioService.PlayStandardClick();
        }

        private void ResetSearch()
        {
            searchQuery = string.Empty;
            emptyListText.text = localizationService.Get(LocalizationKey.FRIENDS_BLOCKED_EMPTY_LIST);
            searchBoxText.text = localizationService.Get(LocalizationKey.FRIENDS_BLOCK_SEARCH);
            cancelSearchButton.gameObject.SetActive(false);
        }

        private void OnUnblockButtonPressed(string friendId)
        {
            uiBlocker.SetActive(true);
            onUnblockButtonPressedSignal.Dispatch(friendId);
            audioService.PlayStandardClick();
        }

        private void ClearFriendsBarContainer()
        {
            foreach (var bar in bars)
            {
                bar.Value.RemoveButtonListeners();
                simpleFriendBarsPool.ReturnObject(bar.Value.gameObject);
            }
            bars.Clear();
        }

        private void AddToFriendsBarContrainer(Dictionary<string, Friend> blockedFriends)
        {
            var count = 0;

            friendBarContainer.gameObject.SetActive(false);
            foreach (var entry in blockedFriends)
            {
                count++;

                var bar = simpleFriendBarsPool.GetObject();

                foreach (var skinLink in bar.GetComponentsInChildren<SkinLink>())
                {
                    skinLink.InitPrefabSkin();
                }

                var simpleFriendBar = bar.GetComponent<FriendBarSimple>();
                simpleFriendBar.Init(entry.Value);
                simpleFriendBar.UpdateMasking(count == blockedFriends.Count, true);
                simpleFriendBar.unblockButton.onClick.AddListener(() => OnUnblockButtonPressed(entry.Value.playerId));
                simpleFriendBar.unblockButtonLabel.text = localizationService.Get(LocalizationKey.FRIENDS_UNBLOCK);
                bars.Add(entry.Value.playerId, simpleFriendBar);

                bar.transform.SetParent(friendBarContainer, false);
                bar.gameObject.SetActive(true);
            }
            friendBarContainer.gameObject.SetActive(true);

            emptyListSection.SetActive(blockedFriends.Count == 0);

            if (!string.IsNullOrEmpty(searchQuery))
            {
                emptyListText.text = $"{localizationService.Get(LocalizationKey.FRIENDS_BLOCKED_EMPTY_LIST)} with name {searchQuery}";
            }
        }

        public void ResetUnblockButton(string playerId)
        {
            TLUtils.LogUtil.LogNullValidation(playerId, "playerId");

            if (playerId != null && !bars.ContainsKey(playerId))
                return;

            uiBlocker.SetActive(false);
            bars[playerId].ResetUnblockButton();
        }
    }
}
