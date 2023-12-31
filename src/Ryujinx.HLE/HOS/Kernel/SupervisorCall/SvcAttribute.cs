﻿using System;

namespace Ryujinx.HLE.HOS.Kernel.SupervisorCall
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SvcAttribute : Attribute
    {
        public int Id { get; }

        public SvcAttribute(int id)
        {
            Id = id;
        }
    }
}
