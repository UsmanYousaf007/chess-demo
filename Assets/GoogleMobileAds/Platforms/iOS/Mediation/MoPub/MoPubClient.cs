﻿// Copyright 2018 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if UNITY_IOS

using UnityEngine;
using System.Collections.Generic;

using GoogleMobileAds.Common.Mediation.MoPub;

namespace GoogleMobileAds.iOS.Mediation.MoPub
{
    public class MoPubClient : IMoPubClient
    {
        private static MoPubClient instance = new MoPubClient();
        private MoPubClient() { }

        public static MoPubClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void Initialize(string moPubAdUnitID)
        {
            Externs.GADMMoPubInitialize(moPubAdUnitID);
        }

        public bool IsInitialized()
        {
            return Externs.GADMMoPubIsInitialized();
        }
    }
}

#endif
