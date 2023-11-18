namespace ZoDream.Shared.Translators
{
    public static class JavaScriptHelper
    {
        public static string Value(string value)
        {
            value = value.Replace("'", "\\'");
            return $"'{value}'";
        }

        public static string Value(string target, string value, bool isInput = false)
        {
            var tag = isInput ? "value" : "innerText";
            return $"{target}.{tag}={Value(value)};";
        }

        public static string Callback(string target, bool isInput = false)
        {
            var tag = isInput ? "value" : "innerText";
            return $"zreBridger.Callback({target}.{tag});";
        }

        public static string EmitPaste(string target, string text)
        {
            return "var data = new DataTransfer();"
                + $"data.setData('text/plain', {Value(text)});" 
                + "var pasteEvent = new ClipboardEvent('paste', {clipboardData: data});" 
                + "pasteEvent.initEvent('paste', true, false);"
                 + target + ".focus();"
                + target + ".dispatchEvent(pasteEvent);";
        }

        public static string EmitKeyEnter(string target)
        {
            return "var event = new KeyboardEvent('keydown', {key: 'Enter', code: 'Enter'});"
                + "event.initEvent('keydown', true, false);"
                 + target + ".focus();"
                + target + ".dispatchEvent(event);";
        }

        public static string EmitBlur(string target)
        {
            return "var event = document.createEvent('HTMLEvents');"
                + "event.initEvent('blur', true, false);"
                + target + ".focus();"
               + target + ".dispatchEvent(event);";
        }

        public static string EmitInput(string target)
        {
            return "var event = document.createEvent('KeyboardEvent');"
                + "event.initEvent('input', true, false);"
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

        public static string LoopChange(string target, string callback, bool isInput = false)
        {
            var tag = isInput ? "value" : "innerText";
            return $"var oldVal = {target} ? {target}.{tag} : undefined;" +
                "var loopC = 0;" +
                "var loopH = setInterval(function() {" +
                "loopC ++;" +
                $"var newVal = {target} ? {target}.{tag} : undefined;" +
                "if (newVal == oldVal && loopC < 100) {return;}" +
                "clearInterval(loopH);" +
                "if (!newVal) {return;}" +
                callback + "();}, 500);";
        }
    }
}
