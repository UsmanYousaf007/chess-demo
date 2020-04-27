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

        // Dispatch Signals
        public Signal backButtonPressedSignal = new Signal();
        public Signal<string> onSubmitSearchSignal = new Signal<string>();
        public Signal<string> onUnblockButtonPressedSignal = new Signal<string>();

        // Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        public void Init()
        {
            ResetSearch();
            sectionHeaderText.text = localizationService.Get(LocalizationKey.FRIENDS_BLOCKED);
            inputField.onEndEdit.AddListener(OnSearchSubmit);
            backButton.onClick.AddListener(OnBackButtonPressed);
            cancelSearchButton.onClick.AddListener(OnCancelSearchButtonPressed);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateView(Dictionary<string, Friend> blockedFriends)
        {
            ClearFriendsBarContainer();
            AddToFriendsBarContrainer(blockedFriends);
        }

        private void OnSearchSubmit(string s)
        {
            var searchQuery = inputField.text;
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
            onSubmitSearchSignal.Dispatch("");
            audioService.PlayStandardClick();
        }

        private void ResetSearch()
        {
            searchBoxText.text = localizationService.Get(LocalizationKey.FRIENDS_BLOCK_SEARCH);
            cancelSearchButton.gameObject.SetActive(false);
        }

        private void OnUnblockButtonPressed(string friendId)
        {
            onUnblockButtonPressedSignal.Dispatch(friendId);
            audioService.PlayStandardClick();
        }

        private void ClearFriendsBarContainer()
        {
            foreach (Transform bar in friendBarContainer)
            {
                Destroy(bar.gameObject);
            }
        }

        private void AddToFriendsBarContrainer(Dictionary<string, Friend> blockedFriends)
        {
            var count = 0;
            foreach (var entry in blockedFriends)
            {
                count++;

                var bar = Instantiate(friendBarPrefab, friendBarContainer) as GameObject;

                foreach (var skinLink in bar.GetComponentsInChildren<SkinLink>())
                {
                    skinLink.InitPrefabSkin();
                }

                var simpleFriendBar = bar.GetComponent<FriendBarSimple>();
                simpleFriendBar.Init(entry.Value);
                simpleFriendBar.UpdateMasking(count == blockedFriends.Count, true);
                simpleFriendBar.unblockButton.onClick.AddListener(() => OnUnblockButtonPressed(entry.Value.playerId));
                simpleFriendBar.unblockButtonLabel.text = localizationService.Get(LocalizationKey.FRIENDS_UNBLOCK);
            }
        }
    }
}
