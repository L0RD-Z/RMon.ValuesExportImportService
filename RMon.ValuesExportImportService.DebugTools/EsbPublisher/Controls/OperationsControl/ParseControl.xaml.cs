using System;
using System.Collections.Generic;
using System.Windows.Input;
using EsbPublisher.Controls.ParseControls;
using EsbPublisher.Processing;
using RMon.Values.ExportImport.Core;

namespace EsbPublisher.Controls.OperationsControl
{
    /// <summary>
    /// Логика взаимодействия для ParseControl.xaml
    /// </summary>
    public partial class ParseControl : OperationControlBase
    {
        private ParseLogic _logic;
        private readonly Dictionary<Type, ParseControlBase> _parseControls;
        private ParseControlBase _selectedParseControl;

        public ParseLogic Logic
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

        public ParseControlBase SelectedParseControl
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

        public ParseControl(ParseLogic logic)
        {
            InitializeComponent();
            Logic = logic;
            Logic.ChangeParseType += ParseLogic_ChangeParseType;

            _parseControls = new Dictionary<Type, ParseControlBase>()
            {
                {typeof(ParseXml80020Control), new ParseXml80020Control(Logic.Xml80020Logic)},
                {typeof(ParseMatrix24X31Control), new ParseMatrix24X31Control(Logic.Matrix24X31Logic)},
                {typeof(ParseMatrix31X24Control), new ParseMatrix31X24Control(Logic.Matrix31X24Logic)},
                {typeof(ParseTableControl), new ParseTableControl(Logic.TableLogic)}
            };
            SelectedParseControl = ParseFormatToUserControlConvert(Logic.SelectedFileType);
        }


        private void ParseLogic_ChangeParseType(object sender, ValuesParseFileFormatType e)
        {
            SelectedParseControl = ParseFormatToUserControlConvert(e);
        }

        private ParseControlBase ParseFormatToUserControlConvert(ValuesParseFileFormatType format) =>
            format switch
            {
                ValuesParseFileFormatType.Xml80020 => _parseControls[typeof(ParseXml80020Control)],
                ValuesParseFileFormatType.Matrix24X31 => _parseControls[typeof(ParseMatrix24X31Control)],
                ValuesParseFileFormatType.Matrix31X24 => _parseControls[typeof(ParseMatrix31X24Control)],
                ValuesParseFileFormatType.Table => _parseControls[typeof(ParseTableControl)],
                ValuesParseFileFormatType.Flexible => null,
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };


        #region Команды

        #region Парсинг

        private void Parse_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void Parse_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await Logic.SendTaskAsync().ConfigureAwait(true);
        }

        #endregion

        #region Отмена парсинга

        private void CancelParse_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void CancelParse_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await Logic.SendTaskAbortAsync().ConfigureAwait(true);
        }

        #endregion

        #endregion
    }
}
