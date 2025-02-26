// Copyright (c) Timofei Zhakov. All rights reserved.

using Windows.ApplicationModel;

namespace MrBoom
{
    public static class Version
    {
        public static readonly string VersionString =
            $"{Package.Current.Id.Version.Major}." +
            $"{Package.Current.Id.Version.Minor}." +
            $"{Package.Current.Id.Version.Build}";
    }
}
