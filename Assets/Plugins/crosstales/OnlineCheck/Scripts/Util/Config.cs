﻿namespace Crosstales.OnlineCheck.Util
{
    /// <summary>Configuration for the asset.</summary>
    public static class Config
    {

        #region Variables

        /// <summary>Enable or disable debug logging for the asset.</summary>
        public static bool DEBUG = Constants.DEFAULT_DEBUG || Constants.DEV_DEBUG;

        /// <summary>Enable or disable the ensuring the name of the OnlineCheck gameobject.</summary>
        public static bool ENSURE_NAME = Constants.DEFAULT_ENSURE_NAME;

        /// <summary>Is the configuration loaded?</summary>
        public static bool isLoaded = false;

        #endregion

#if UNITY_EDITOR
        #region Public static methods

        /// <summary>Resets all changeable variables to their default value.</summary>
        public static void Reset()
        {
            if (!Constants.DEV_DEBUG)
                DEBUG = Constants.DEFAULT_DEBUG;

            ENSURE_NAME = Constants.DEFAULT_ENSURE_NAME;
        }

        /// <summary>Loads the all changeable variables.</summary>
        public static void Load()
        {
            if (!Constants.DEV_DEBUG)
            {
                if (Common.Util.CTPlayerPrefs.HasKey(Constants.KEY_DEBUG))
                {
                    DEBUG = Common.Util.CTPlayerPrefs.GetBool(Constants.KEY_DEBUG);
                }
            }
            else
            {
                DEBUG = Constants.DEV_DEBUG;
            }

            if (Common.Util.CTPlayerPrefs.HasKey(Constants.KEY_ENSURE_NAME))
            {
                ENSURE_NAME = Common.Util.CTPlayerPrefs.GetBool(Constants.KEY_ENSURE_NAME);
            }

            isLoaded = true;
        }

        /// <summary>Saves the all changeable variables.</summary>
        public static void Save()
        {
            if (!Constants.DEV_DEBUG)
                Common.Util.CTPlayerPrefs.SetBool(Constants.KEY_DEBUG, DEBUG);

            Common.Util.CTPlayerPrefs.SetBool(Constants.KEY_ENSURE_NAME, ENSURE_NAME);

            Common.Util.CTPlayerPrefs.Save();
        }

        #endregion

#endif
    }
}
// © 2017-2019 crosstales LLC (https://www.crosstales.com)