/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 13:37:03 UTC+05:00
/// 
/// @description
/// [add_description_here]
using System.Collections.Generic;
using TurboLabz.TLUtils;
using System.Text;

namespace TurboLabz.InstantFramework
{
    // Navigator State
    public class NS
    {
        protected NavigatorCommand cmd;

        public void SetCommand(NavigatorCommand cmd)
        {
            this.cmd = cmd;
        }

        public virtual void RenderDisplayOnEnter()
        {   
        }

        public virtual NS HandleEvent(NavigatorEvent evt)
        {
            return null;
        }

        protected void ShowView(NavigatorViewId id)
        {
            AddToHistory(id);

            if (!TrimStack(id))
            {
                Show(id);
            }
        }

        protected void ShowDialog(NavigatorViewId id)
        {
            AddToHistory(id);
            Show(id);
        }

        private void AddToHistory(NavigatorViewId id)
        {
            if (id == NavigatorViewId.CPU_LOBBY)
            {
                cmd.navigatorModel.history.Clear();
            }

            cmd.navigatorModel.history.Add(GetTopOfStack());
        }

        protected NavigatorViewId CameFrom(params NavigatorViewId[] ids)
        {
            List<NavigatorViewId> history = cmd.navigatorModel.history;

            for (int i = history.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < ids.Length; j++)
                {
                    if (history[i] == ids[j])
                    {
                        return ids[j];
                    }
                }
            }

            Assertions.Assert(false, "Searching for unknown view id in navigator");

            return NavigatorViewId.NONE;
        }

        protected bool IsPreviousView(NavigatorViewId id)
        {
            List<NavigatorViewId> history = cmd.navigatorModel.history;
            int count = history.Count;

            if (count == 0)
            {
                return false;
            }
            else
            {
                return (history[count - 1] == id);
            }
        }

        private void Show(NavigatorViewId id)
        {
            cmd.navigatorModel.viewStack.Add(id);
            cmd.showViewSignal.Dispatch(id);
        }

        private bool TrimStack(NavigatorViewId id)
        {
            List<NavigatorViewId> viewStack = cmd.navigatorModel.viewStack;

            int removeIndex = 0;
            int removeCount = 0;
            bool stackHasTargetView = false;

            for (int i = viewStack.Count - 1; i >= 0; i--)
            {
                if (viewStack[i] != id)
                {
                    cmd.hideViewSignal.Dispatch(viewStack[i]);
                    removeIndex = i;
                    removeCount++;
                }
                else
                {
                    stackHasTargetView = true;
                    break;
                }
            }

            viewStack.RemoveRange(removeIndex, removeCount);
            return stackHasTargetView;
        }

        private NavigatorViewId GetTopOfStack()
        {
            List<NavigatorViewId> viewStack = cmd.navigatorModel.viewStack;

            if (viewStack.Count == 0)
            {
                return NavigatorViewId.NONE;
            }
            else
            {
                return viewStack[viewStack.Count - 1];
            }
        }
    }
}

