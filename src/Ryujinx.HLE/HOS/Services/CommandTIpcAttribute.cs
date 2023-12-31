﻿using System;

namespace Ryujinx.HLE.HOS.Services
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandTipcAttribute : Attribute
    {
        public readonly int Id;

        public CommandTipcAttribute(int id) => Id = id;
    }
}
