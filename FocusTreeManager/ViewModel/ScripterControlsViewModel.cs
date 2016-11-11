using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace FocusTreeManager.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ScripterControlsViewModel : ViewModelBase
    {
        const string XML_FILES_PATH = @"Common\ScripterControls\";

        public enum ScripterType
        {
            FocusTree,
            Event,
            EventOption,
            EventDescription,
            Generic
        }

        public enum ControlType
        {
            Assignation,
            Block,
            Condition
        }

        public struct ControlInfo
        {
            public string control;
            public string controlName;
            public ControlType controlType;
        }

        public static readonly Brush[] CONDITIONS_COLORS = 
        {
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#b71c1c")),
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#f44336")),
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffff"))
        };

        public static readonly Brush[] BLOCKS_COLORS = 
        {
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#3f51b5")),
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#1a237e")),
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffff"))
        };

        public static readonly Brush[] ASSIGNATIONS_COLORS = 
        {
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffeb3b")),
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#f57f17")),
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"))
        };

        private ObservableCollection<AssignationModel> conditions;

        public ObservableCollection<AssignationModel> Conditions
        {
            get
            {
                return conditions;
            }
        }

        private ObservableCollection<AssignationModel> blocks;

        public ObservableCollection<AssignationModel> Blocks
        {
            get
            {
                return blocks;
            }
        }

        private ObservableCollection<AssignationModel> assigantions;

        public ObservableCollection<AssignationModel> Assignations
        {
            get
            {
                return assigantions;
            }
        }

        public List<ControlInfo> CommonControls
        {
            get
            {
                return getCommonControls();
            }
        }

        private ScripterType currentType = ScripterType.Generic;

        public ScripterType CurrentType
        {
            get
            {
                return currentType;
            }
            set
            {
                currentType = value;
                BuildCommonControls();
            }
        }

        /// <summary>
        /// Initializes a new instance of the ScripterControlsViewModel class.
        /// </summary>
        public ScripterControlsViewModel()
        {
            conditions = new ObservableCollection<AssignationModel>();
            blocks = new ObservableCollection<AssignationModel>();
            assigantions = new ObservableCollection<AssignationModel>();
            BuildCommonControls();
        }

        private void BuildCommonControls()
        {
            conditions.Clear();
            blocks.Clear();
            assigantions.Clear();
            //Load the required XMl File
            XmlDocument doc = new XmlDocument();
            doc.Load(XML_FILES_PATH + currentType.ToString() + ".xml");
            //Load all the children of the root node
            foreach (XmlNode node in doc.DocumentElement.SelectNodes("//Assignations/Control"))
            {
                assigantions.Add(new AssignationModel()
                {
                    LocalizationKey = node.Attributes["Text"].Value,
                    Code = node.Attributes["Code"].Value,
                    IsNotEditable = Convert.ToBoolean(node.Attributes["IsNotEditable"].Value),
                    CanHaveChild = Convert.ToBoolean(node.Attributes["CanHaveChild"].Value),
                    BackgroundColor = ASSIGNATIONS_COLORS[0],
                    BorderColor = ASSIGNATIONS_COLORS[1],
                    Color = ASSIGNATIONS_COLORS[2],
                });
            }
            foreach (XmlNode node in doc.DocumentElement.SelectNodes("//Blocks/Control"))
            {
                conditions.Add(new AssignationModel()
                {
                    LocalizationKey = node.Attributes["Text"].Value,
                    Code = node.Attributes["Code"].Value,
                    IsNotEditable = Convert.ToBoolean(node.Attributes["IsNotEditable"].Value),
                    CanHaveChild = Convert.ToBoolean(node.Attributes["CanHaveChild"].Value),
                    BackgroundColor = CONDITIONS_COLORS[0],
                    BorderColor = CONDITIONS_COLORS[1],
                    Color = CONDITIONS_COLORS[2],
                });
            }
            foreach (XmlNode node in doc.DocumentElement.SelectNodes("//Conditions/Control"))
            {
                Blocks.Add(new AssignationModel()
                {
                    LocalizationKey = node.Attributes["Text"].Value,
                    Code = node.Attributes["Code"].Value,
                    IsNotEditable = Convert.ToBoolean(node.Attributes["IsNotEditable"].Value),
                    CanHaveChild = Convert.ToBoolean(node.Attributes["CanHaveChild"].Value),
                    BackgroundColor = BLOCKS_COLORS[0],
                    BorderColor = BLOCKS_COLORS[1],
                    Color = BLOCKS_COLORS[2],
                });
            }
        }

        private List<ControlInfo> getCommonControls()
        {
            List<ControlInfo> array = new List<ControlInfo>();
            foreach (AssignationModel model in assigantions)
            {
                //If known control
                if (model.IsNotEditable)
                {
                    array.Add(new ControlInfo()
                    {
                        control = model.Code.Split('=')[0].Trim(),
                        controlName = model.LocalizationKey,
                        controlType = ControlType.Assignation
                    });
                }
            }
            foreach (AssignationModel model in blocks)
            {
                //If known control
                if (model.IsNotEditable)
                {
                    array.Add(new ControlInfo()
                    {
                        control = model.Code.Split('=')[0].Trim(),
                        controlName = model.LocalizationKey,
                        controlType = ControlType.Block
                    });
                }
            }
            foreach (AssignationModel model in conditions)
            {
                //If known control
                if (model.IsNotEditable)
                {
                    array.Add(new ControlInfo()
                    {
                        control = model.Code.Split('=')[0].Trim(),
                        controlName = model.LocalizationKey,
                        controlType = ControlType.Condition
                    });
                }

            }
            return array;
        }
    }
}