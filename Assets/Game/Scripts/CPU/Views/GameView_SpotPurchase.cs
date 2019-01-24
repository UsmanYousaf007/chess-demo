using strange.extensions.signal.impl;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.CPU
{
    public partial class GameView
    {
        public Signal<SpotPurchaseView.PowerUpSections> openSpotPurchaseSignal = new Signal<SpotPurchaseView.PowerUpSections>();
    }
}
