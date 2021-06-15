using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using RMon.Globalization;
using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Globalization
{
    class FileProvider : IGlobalizationProvider
    {
        private readonly object _syncObject = new ();
        private IDictionary<string, string> _dictionary;
        private readonly string _folderName;
        private readonly CultureInfo _cultureInfo;

        public FileProvider(string folderName, string culture)
        {
            _folderName = folderName;
            _cultureInfo = new CultureInfo(culture);
        }

        public void Load()
        {
            lock (_syncObject)
            {
                if (_dictionary != null)
                    return;

                _dictionary = new Dictionary<string, string>();
                try
                {
                    var files = Directory.GetFiles(_folderName, "*.lang");
                    foreach (var file in files)
                    {
                        var dic = StringsPackage.LoadFromDicFile(file);
                        foreach (var word in dic)
                        {
                            if (!_dictionary.ContainsKey(word.Key))
                                _dictionary.Add(word);
                        }
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        public string GetText(string code)
        {
            if(_dictionary == null)
                Load();

            if (_dictionary.ContainsKey(code))
                return _dictionary[code];

            return null;
        }

        public IFormatProvider GetFormatProvider()
        {
            return _cultureInfo;
        }
    }
}
