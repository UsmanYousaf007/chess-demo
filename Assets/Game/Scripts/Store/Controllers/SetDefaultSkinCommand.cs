using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using UnityEngine;

namespace TurboLabz.InstantGame
{
    public class SetDefaultSkinCommand : Command
    {
        //Dispatch Signals
        [Inject] public SavePlayerInventorySignal savePlayerInventorySignal { get; set; }
        [Inject] public SetSkinSignal setSkinSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        public override void Execute()
        {
            //Set default skin in case subscription is expired
            //and player has active skin which is not yet unlocked by the reward system
            if (!playerModel.HasSubscription() &&
                !playerModel.OwnsVGood(playerModel.activeSkinId))
            {
                setSkinSignal.Dispatch(metaDataModel.store.GetItemBySkinIndex(0).key);
                savePlayerInventorySignal.Dispatch("");
            }

            Resources.UnloadUnusedAssets();
        }
    }
}
