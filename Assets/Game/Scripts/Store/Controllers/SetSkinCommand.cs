/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine;

namespace TurboLabz.InstantGame
{
    public class SetSkinCommand : Command
    {
        // Parameter
        [Inject] public string skinId { get; set; }

        // Dispatch 
        [Inject] public LoadSkinRefsSignal loadSkinRefsSignal { get; set; }
        [Inject] public SkinUpdatedSignal skinUpdatedSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            playerModel.activeSkinId = skinId;
            loadSkinRefsSignal.Dispatch(skinId);
            skinUpdatedSignal.Dispatch();

            Resources.UnloadUnusedAssets();
        }
    }
}
