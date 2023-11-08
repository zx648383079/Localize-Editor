using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Translators
{
    public static class JavaScriptHelper
    {

        public static string Callback(string target, bool isInput = false)
        {
            var tag = isInput ? "value" : "innerText";
            return $"zreBridger.Callback({target}.{tag});";
        }

        public static string Paste(string target, string text)
        {
            return "var data = new DataTransfer();"
                + $"data.setData('text/plain', '{text}');" 
                + "var pasteEvent = new ClipboardEvent('paste', {clipboardData: data});" 
                + "pasteEvent.initEvent('paste', true, false);"
                 + target + ".focus();"
                + target + ".dispatchEvent(pasteEvent);";
        }

        public static string KeyEnter(string target)
        {
            return "var event = new KeyboardEvent('keydown', {key: 'Enter', code: 'Enter'});"
                + "event.initEvent('keydown', true, false);"
                 + target + ".focus();"
                + target + ".dispatchEvent(event);";
        }

        public static string Blur(string target)
        {
            return "var event = document.createEvent('HTMLEvents');"
                + "event.initEvent('blur', true, false);"
                + target + ".focus();"
               + target + ".dispatchEvent(event);";
        }

        public static string ListenChange(string target, string callback)
        {
            return "var observer = new MutationObserver(function(){" +
                    callback + "();observer.disconnect()" + 
                "});" +
                "observer.observe(" +
                target +
                ", {childList: true});";
        }
    }
}
