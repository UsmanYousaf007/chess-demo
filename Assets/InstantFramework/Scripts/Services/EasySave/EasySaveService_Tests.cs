/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-10 10:52:49 UTC+05:00
/// 
/// @description
/// Check out the easy save docs for all its capabilities that can extend this service:
/// http://docs.moodkie.com/product/easy-save-2/

using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public partial class EasySaveService : ILocalDataService
    {
        public void TestService()
        {
            Debug.Log("Testing EasySave Service...");

            #region Test1
            // Write key value pair to file
            Debug.Log("---- Test1: Write key value pair to file");
            string test1writerfilename = GetRandomFilename("test1writer");
            ILocalDataWriter test1writer = OpenWriter(test1writerfilename);
            Vector3 test1vector = new Vector3(1, 2, 3);
            test1writer.Write("vector", test1vector);
            test1writer.Close();
            Debug.Log("---- Test1: OK");
            #endregion

            #region Test2
            // Read key value pair from file
            Debug.Log("---- Test2: Read key value pair from file");
            ILocalDataReader test2reader = OpenReader(test1writerfilename);
            Vector3 test2vector = test2reader.Read<Vector3>("vector");
            test2reader.Close();
            if (test2vector.Equals(test1vector))
            {
                Debug.Log("---- Test2: OK");
            }
            else
            {
                Debug.Log("---- Test2: FAILED");
                return;
            }
            #endregion

            #region Test3
            // Open a file that doesn't exist
            Debug.Log("---- Test3: Open a file that doesn't exist");
            bool exceptionCaught = false;
            try
            {
                OpenReader(GetRandomFilename("test3reader"));
            }
            catch (Exception e)
            {
                exceptionCaught = true;
                Debug.Log("*****" + e);
            }
            if (exceptionCaught)
            {
                Debug.Log("---- Test3: OK");
            }
            else
            {
                Debug.Log("---- Test3: FAILED");
                return;
            }
            #endregion

            #region Test4
            // Read a value that doesn't exist
            Debug.Log("---- Test4: Read a value that doesn't exist");
            ILocalDataReader test4reader = OpenReader(test1writerfilename);
            exceptionCaught = false;
            try
            {
                test4reader.Read<Vector3>("foo");
            }
            catch (Exception e)
            {
                exceptionCaught = true;
                Debug.Log("*****" + e);
            }
            test4reader.Close();
            if (exceptionCaught)
            {
                Debug.Log("---- Test4: OK");
            }
            else
            {
                Debug.Log("---- Test4: FAILED");
                return;
            }
            #endregion

            #region Test5
            // Read a value into an incompatible type
            Debug.Log("---- Test5: Read a value into an incompatible type");
            ILocalDataReader test5reader = OpenReader(test1writerfilename);
            exceptionCaught = false;
            try
            {
                test5reader.Read<string>("vector");
            }
            catch (Exception e)
            {
                exceptionCaught = true;
                Debug.Log("*****" + e);
            }
            test5reader.Close();
            if (exceptionCaught)
            {
                Debug.Log("---- Test5: OK");
            }
            else
            {
                Debug.Log("---- Test5: FAILED");
                return;
            }
            #endregion

            #region Test6
            // Write an unsupported type to a file
            Debug.Log("---- Test6: Write an unsupported type to a file");
            Unsupported unsupported = new Unsupported();
            ILocalDataWriter test6writer = OpenWriter(GetRandomFilename("test6writer"));
            exceptionCaught = false;
            try
            {
                test6writer.Write("unsupported", unsupported);
            }
            catch (Exception e)
            {
                exceptionCaught = true;
                Debug.Log("*****" + e);
            }
            test6writer.Close();
            if (exceptionCaught)
            {
                Debug.Log("---- Test6: OK");
            }
            else
            {
                Debug.Log("---- Test6: FAILED");
                return;
            }
            #endregion

            #region Test7
            // Check for file that exists
            Debug.Log("---- Test7: Check for file that exists");
            bool test7exists = FileExists(test1writerfilename);
            if (test7exists)
            {
                Debug.Log("---- Test7: OK");
            }
            else
            {
                Debug.Log("---- Test7: FAILED");
                return;
            }
            #endregion

            #region Test8
            // Check for file that does not exist
            Debug.Log("---- Test8: Check for file that does not exist");
            bool test8exists = FileExists(test1writerfilename + "junk");
            if (!test8exists)
            {
                Debug.Log("---- Test8: OK");
            }
            else
            {
                Debug.Log("---- Test8: FAILED");
                return;
            }
            #endregion

            #region Test9
            // Check for key that exists
            Debug.Log("---- Test9: Check for key that exists");
            ILocalDataReader test9reader = OpenReader(test1writerfilename);
            bool test9haskey = test9reader.HasKey("vector");
            test9reader.Close();
            if (test9haskey)
            {
                Debug.Log("---- Test9: OK");
            }
            else
            {
                Debug.Log("---- Test9: FAILED");
                return;
            }
            #endregion

            #region Test10
            // Check for key that does not exist
            Debug.Log("---- Test10: Check for key that does not exist");
            ILocalDataReader test10reader = OpenReader(test1writerfilename);
            bool test10haskey = test10reader.HasKey("foo");
            test10reader.Close();
            if (!test10haskey)
            {
                Debug.Log("---- Test10: OK");
            }
            else
            {
                Debug.Log("---- Test10: FAILED");
                return;
            }
            #endregion

            #region Test11
            // Delete a file
            Debug.Log("---- Test11: Check delete file");
            bool fileExists = ES2.Exists(test1writerfilename);
            if (!fileExists)
            {
                Debug.Log("---- Test11: FAILED - File does not exist!");
                return;
            }
            ES2.Delete(test1writerfilename);
            fileExists = ES2.Exists(test1writerfilename);
            if (!fileExists)
            {
                Debug.Log("---- Test11: OK");
            }
            else
            {
                Debug.Log("---- Test11: FAILED - File still exists!");
                return;
            }
            #endregion

            #region Test12
            // Write a list to a file
            Debug.Log("---- Test12: Write a list to a file");
            string test12writerfilename = GetRandomFilename("test12writer");
            ILocalDataWriter test12writer = OpenWriter(test12writerfilename);
            List<string> test12List = new List<string>();
            test12List.Add("foo"); test12List.Add("bar");
            test12writer.WriteList<string>("test12List", test12List);
            test12writer.Close();
            Debug.Log("---- Test12: OK");
            #endregion

            #region Test13
            // Read the list from the file
            Debug.Log("---- Test13: Read a list from a file");
            ILocalDataReader test13reader = OpenReader(test12writerfilename);
            List<string> test13List = test13reader.ReadList<string>("test12List");
            test13reader.Close();
            if (test13List[0] == "foo" && test13List[1] == "bar")
            {
                Debug.Log("---- Test13: OK");
            }
            else
            {
                Debug.Log("---- Test13: FAILED");
                return;
            }
            #endregion

            Debug.Log("Testing EasySave Service: All Tests OK.");
        }

        string GetRandomFilename(string salt)
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return salt + unixTimestamp;
        }

        class Unsupported { public int foo = 1; }
    }
}