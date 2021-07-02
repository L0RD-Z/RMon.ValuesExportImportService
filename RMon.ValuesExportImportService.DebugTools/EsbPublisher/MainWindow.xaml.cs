using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EsbPublisher.Controls;
using RMon.Values.ExportImport.Core;

namespace EsbPublisher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private MainLogic _logic;
        private readonly Dictionary<Type, UserControl> _parseControls;
        private UserControl _selectedParseControl;

        public MainLogic Logic
        {
            get => _logic;
            set
            {
                if (_logic != value)
                {
                    _logic = value;
                    OnPropertyChanged(nameof(Logic));
                }
            }
        }


        public UserControl SelectedParseControl
        {
            get => _selectedParseControl;
            set
            {
                if (_selectedParseControl != value)
                {
                    _selectedParseControl = value;
                    OnPropertyChanged(nameof(SelectedParseControl));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            
            Logic = new MainLogic();
            Logic.ParseLogic.ChangeParseType += ParseLogic_ChangeParseType;

            _parseControls = new Dictionary<Type, UserControl>()
            {
                {typeof(ParseXml80020Control), new ParseXml80020Control(Logic.ParseLogic.Xml80020Logic)},
                {typeof(ParseMatrix24X31Control), new ParseMatrix24X31Control(Logic.ParseLogic.Matrix24X31Logic)},
                {typeof(ParseMatrix31X24Control), new ParseMatrix31X24Control(Logic.ParseLogic.Matrix31X24Logic)},
            };
            SelectedParseControl = ParseFormatToUserControlConvert(Logic.ParseLogic.SelectedFileType);


            Task.Run(async () =>
            {
                await Logic.ExportLogic.LoadDataAsync().ConfigureAwait(false);
            });
        }

        private void ParseLogic_ChangeParseType(object sender, ValuesParseFileFormatType e)
        {
            SelectedParseControl = ParseFormatToUserControlConvert(e);
        }

        UserControl ParseFormatToUserControlConvert(ValuesParseFileFormatType format) =>
            format switch
            {
                ValuesParseFileFormatType.Xml80020 => _parseControls[typeof(ParseXml80020Control)],
                ValuesParseFileFormatType.Matrix24X31 => _parseControls[typeof(ParseMatrix24X31Control)],
                ValuesParseFileFormatType.Matrix31X24 => _parseControls[typeof(ParseMatrix31X24Control)],
                ValuesParseFileFormatType.Table => null,
                ValuesParseFileFormatType.Flexible => null,
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };


        #region Комманды

        #region Экспорт

        private void Export_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
        private async void Export_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await Logic.ExportLogic.SendTaskAsync().ConfigureAwait(true);
        }

        #endregion

        #region Отмена экспорта

        private void CancelExport_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = Logic?.ExportTaskAbortEnabled ?? false;
        }

        [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
        private async void CancelExport_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await Logic.ExportLogic.SendTaskAbortAsync().ConfigureAwait(true);
        }

        #endregion


        

        #region Парсинг

        private void Parse_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void Parse_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await Logic.ParseLogic.SendTaskAsync().ConfigureAwait(true);
        }

        #endregion

        #region Отмена парсинга

        private void CancelParse_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void CancelParse_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await Logic.ParseLogic.SendTaskAbortAsync().ConfigureAwait(true);
        }

        #endregion




        #region Импорт

        private void Import_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
        private async void Import_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await Logic.SendImportTaskAsync().ConfigureAwait(true);
        }

        #endregion

        #region Отмена импорта

        private void CancelImport_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = Logic?.ImportTaskAbortEnabled ?? false;
        }

        [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
        private async void CancelImport_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await Logic.SendImportTaskAbortAsync().ConfigureAwait(true);
        }

        #endregion

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        
    }
}
