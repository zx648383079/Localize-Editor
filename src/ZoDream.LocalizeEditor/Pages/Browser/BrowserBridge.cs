using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Translators;

namespace ZoDream.LocalizeEditor.Pages
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class BrowserBridge : ITranslatorBridge
    {
        public ContentReadyEventHandler? ContentReady;

        public void Callback(string content)
        {
            ContentReady?.Invoke(content);
            Debug.WriteLine(content);
        }
    }

    public delegate void ContentReadyEventHandler(string html);
}
