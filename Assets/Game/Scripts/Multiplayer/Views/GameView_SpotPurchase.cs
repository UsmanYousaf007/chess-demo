using System.Collections.Generic;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        public Signal<SpotPurchaseView.PowerUpSections> openSpotPurchaseSignal = new Signal<SpotPurchaseView.PowerUpSections>();


    }
}
