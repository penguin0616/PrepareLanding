﻿using System;
using System.Collections.Generic;


namespace PrepareLanding.Filters
{
    public enum FilterHeaviness
    {
        Light = 0,
        Medium = 1,
        Heavy = 2
    }

    public interface ITileFilter
    {
        string SubjectThingDef { get; }

        string RunningDescription { get; }

        string AttachedProperty { get; }

        Action<PrepareLandingUserData, List<int>> FilterAction { get; }

        FilterHeaviness Heaviness { get; }

        List<int> FilteredTiles { get; }

        void Filter(PrepareLandingUserData userData, List<int> inputList);
    }
}