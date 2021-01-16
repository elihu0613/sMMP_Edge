﻿using Lib.Common.Components.Agreements;
using System;
using System.Reflection;

namespace Edge.Zeus.Panels
{
    internal class SimpleFactory
    {
        internal static IProtocol BuildCommunicationStation(string service) => (IProtocol)Activator.CreateInstance(Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + service.Split(',')[1]).GetType(service.Split(',')[0]));
    }
}
