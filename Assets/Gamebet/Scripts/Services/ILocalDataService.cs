/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-10 10:47:41 UTC+05:00
/// 
/// @description
/// [add_description_here]
using System.Collections.Generic;

namespace TurboLabz.Gamebet
{
    /// ************************************************************************
    /// Note: Always wrap interaction with the local data service
    /// in try catch blocks!
    /// ************************************************************************

    public interface ILocalDataService
    {
        ILocalDataWriter OpenWriter(string filename);
        ILocalDataReader OpenReader(string filename);
        bool FileExists(string filename);
        void DeleteFile(string filename);

        // Run Unit Test
        void TestService();
    }

    public interface ILocalDataWriter
    {
        void Write<T>(string key, T value);
        void WriteList<T>(string key, List<T> value);
        void Close();
    }

    public interface ILocalDataReader
    {
        T Read<T>(string key);
        List<T> ReadList<T>(string key);
        bool HasKey(string key);
        void Close();
    }
}
