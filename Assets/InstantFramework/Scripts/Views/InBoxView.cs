/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class InBoxView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        public Transform listContainer;
        public GameObject sectionHeader;

        public Text heading;
        public Text stripHeading;

        // Player bar click signal
        [HideInInspector]
        public Signal<InBoxBar> inBoxBarClickedSignal = new Signal<InBoxBar>();
        private Dictionary<string, InBoxBar> inBoxBars = new Dictionary<string, InBoxBar>();

        public void Init()
        {
            heading.text = "InBox";
            stripHeading.text = "Rewards";

            AddInBoxBar("InBoxBarTournamentResult");
            AddInBoxBar("InBoxBarTournamentResult");
            AddInBoxBar("InBoxBarTournamentResult");
            AddInBoxBar("InBoxBarTournamentResult");
            Sort();
        }

        private void Sort()
        {
            List<InBoxBar> items = new List<InBoxBar>();

            // Copy all player bars into a list
            foreach (KeyValuePair<string, InBoxBar> item in inBoxBars)
            {
                items.Add(item.Value);
            }

            // Todo: Sort

            // Adust order
            int index = 0;
            sectionHeader.transform.SetSiblingIndex(index++);

            for (int i = 0; i < items.Count; i++)
            {
                items[i].transform.SetSiblingIndex(index++);
            }
        }

        public void AddInBoxBar(string inBoxBarPrefabName)
        {
            GameObject objPrefab = Resources.Load(inBoxBarPrefabName) as GameObject;

            GameObject obj = GameObject.Instantiate(objPrefab);
            InBoxBar item = obj.GetComponent<InBoxBar>();

            if (item.GetType() == typeof(InBoxBarTournamentResult))
            {
                InBoxBarTournamentResult itemTournamentResult = item as InBoxBarTournamentResult;
                itemTournamentResult.thumbnailBg.sprite = null;// (Resources.Load("PK") as Image).sprite;
                itemTournamentResult.headingText.text = "Tournament Results";
                itemTournamentResult.subHeadingText.text = "Completed 08/10/2020";
                itemTournamentResult.thumbnail.sprite = null;// (Resources.Load("GE") as Image).sprite;
                itemTournamentResult.trophiesCount.text = "5";
            }

            item.button.onClick.AddListener(() => inBoxBarClickedSignal.Dispatch(item));

            item.transform.SetParent(listContainer, false);
            inBoxBars.Add(item.name + inBoxBars.Count.ToString(), item);
        }
    }
}
