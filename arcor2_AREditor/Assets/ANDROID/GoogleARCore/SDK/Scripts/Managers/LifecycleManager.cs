//-----------------------------------------------------------------------
// <copyright file="LifecycleManager.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCoreInternal {
    using UnityEngine;

    internal class LifecycleManager {
        private static ILifecycleManager s_Instance;

        public static ILifecycleManager Instance {
            get {
                if (s_Instance == null) {
                    if (Application.platform == RuntimePlatform.IPhonePlayer) {
                        s_Instance = ARCoreIOSLifecycleManager.Instance;
                    } else {
                        s_Instance = ARCoreAndroidLifecycleManager.Instance;
                    }
                }

                return s_Instance;
            }
        }
    }
}
