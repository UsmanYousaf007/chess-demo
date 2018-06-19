/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-03-21 10:43:27 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine.UI;
using TurboLabz.Chess;

namespace TurboLabz.CPU
{
    public partial class GameView
    {
        public Text notation;
        private string defaultNotation;

        public void OnParentShowNotation()
        {
            ClearNotation();
        }

        public void UpdateNotation(MoveVO moveVO)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(defaultNotation);
            List<string> notationList = moveVO.notation;

            for (int i = 0; i < notationList.Count; ++i)
            {
                if (i % 2 == 0)
                {
                    builder.Append("  " + ((i / 2) + 1) + ".");
                }

                builder.Append(" " + notationList[i]);
            }

            notation.text = builder.ToString();
        }

        private void SetIntroNotation()
        {
            defaultNotation = DateTime.Now.ToString("MMMM dd, yyyy") + 
                " [x]" +
                " VS " +
                " [x]* ";

            notation.text = defaultNotation;
        }

        private void ClearNotation()
        {
            notation.text = "";
        }
    }
}
