﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.LocalizeEditor.Events
{
    public delegate void ValueChangedEventHandler<T>(object sender, T value);
    public delegate void EmptyEventHandler();
}
