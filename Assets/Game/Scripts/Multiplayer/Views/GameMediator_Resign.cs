/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-19 11:47:36 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.MPChess
{
    public partial class GameMediator
    {
        // Dispatch signals
        [Inject] public ResignSignal resignSignal { get; set; }

        public void OnRegisterResign()
        {
            view.InitResign();
            view.resignSignal.AddListener(OnResign);
        }

        public void OnRemoveResign()
        {
            view.CleanupResign();
            view.resignSignal.RemoveListener(OnResign);
        }

        private void OnResign()
        {
            resignSignal.Dispatch();
        }
    }
}
