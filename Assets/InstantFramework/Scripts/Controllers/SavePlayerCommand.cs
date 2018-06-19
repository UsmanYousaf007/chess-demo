/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
	public class SavePlayerCommand : Command
	{
		// Models
		[Inject] public IPlayerModel playerModel { get; set; }

		// Services
		[Inject] public ILocalDataService localDataService { get; set; }

		public override void Execute()
		{
			playerModel.SaveToFile();
		}
	}
}
