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
    public class RestorePurchasesCommand : Command
    {
        // Services
        [Inject] public IStoreService storeService { get; set; }

        public override void Execute()
        {
            storeService.RestorePurchases();
        }
    }
}
