using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Xml;
using FocusTreeManager.Model;
using GalaSoft.MvvmLight;

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
        private const string XML_FILES_PATH = @"Common\ScripterControls\";

        public struct ControlInfo
        {
            public string control;
            public string controlName;
            public ControlType controlType;
        }

        public static readonly Brush[] CONDITIONS_COLORS = 
        {
            (SolidColorBrush)new BrushConverter().ConvertFrom("#b71c1c"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#f44336"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#ffffff")
        };

        public static readonly Brush[] BLOCKS_COLORS = 
        {
            (SolidColorBrush)new BrushConverter().ConvertFrom("#3f51b5"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#1a237e"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#ffffff")
        };

        public static readonly Brush[] ASSIGNATIONS_COLORS = 
        {
            (SolidColorBrush)new BrushConverter().ConvertFrom("#ffeb3b"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#f57f17"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#000000")
        };

        public ObservableCollection<AssignationModel> Conditions { get; }

        public ObservableCollection<AssignationModel> Blocks { get; }

        public ObservableCollection<AssignationModel> Assignations { get; }

        public List<ControlInfo> CommonControls => getCommonControls();

        public ScripterType CurrentType => new ViewModelLocator().Scripter.ScriptType;

        /// <summary>
        /// Initializes a new instance of the ScripterControlsViewModel class.
        /// </summary>
        public ScripterControlsViewModel()
        {
            Conditions = new ObservableCollection<AssignationModel>();
            Blocks = new ObservableCollection<AssignationModel>();
            Assignations = new ObservableCollection<AssignationModel>();
            BuildCommonControls();
        }

        private void BuildCommonControls()
        {
            Conditions.Clear();
            Blocks.Clear();
            Assignations.Clear();
            //Load the required XMl File
            XmlDocument doc = new XmlDocument();
            doc.Load(XML_FILES_PATH + CurrentType + ".xml");
            //Load all the children of the root node
            XmlNodeList xmlNodeList = doc.DocumentElement?.SelectNodes("//Assignations/Control");
            if (xmlNodeList == null) return;
            foreach (XmlNode node in xmlNodeList)
            {
                if (node.Attributes != null)
                {
                    Assignations.Add(new AssignationModel
                    {
                        LocalizationKey = node.Attributes["Text"].Value,
                        Code = node.Attributes["Code"].Value,
                        IsNotEditable = Convert.ToBoolean(node.Attributes["IsNotEditable"].Value),
                        CanHaveChild = Convert.ToBoolean(node.Attributes["CanHaveChild"].Value),
                        BackgroundColor = ASSIGNATIONS_COLORS[0],
                        BorderColor = ASSIGNATIONS_COLORS[1],
                        Color = ASSIGNATIONS_COLORS[2]
                    });
                }
            }
            XmlNodeList selectNodes = doc.DocumentElement.SelectNodes("//Blocks/Control");
            if (selectNodes != null)
            {
                foreach (XmlNode node in selectNodes)
                {
                    if (node.Attributes != null)
                    {
                        Conditions.Add(new AssignationModel
                        {
                            LocalizationKey = node.Attributes["Text"].Value,
                            Code = node.Attributes["Code"].Value,
                            IsNotEditable = Convert.ToBoolean(node.Attributes["IsNotEditable"].Value),
                            CanHaveChild = Convert.ToBoolean(node.Attributes["CanHaveChild"].Value),
                            BackgroundColor = CONDITIONS_COLORS[0],
                            BorderColor = CONDITIONS_COLORS[1],
                            Color = CONDITIONS_COLORS[2]
                        });
                    }
                }
            }
            XmlNodeList nodeList = doc.DocumentElement.SelectNodes("//Conditions/Control");
            if (nodeList != null)
            {
                foreach (XmlNode node in nodeList)
                {
                    if (node.Attributes != null)
                    {
                        Blocks.Add(new AssignationModel
                        {
                            LocalizationKey = node.Attributes["Text"].Value,
                            Code = node.Attributes["Code"].Value,
                            IsNotEditable = Convert.ToBoolean(node.Attributes["IsNotEditable"].Value),
                            CanHaveChild = Convert.ToBoolean(node.Attributes["CanHaveChild"].Value),
                            BackgroundColor = BLOCKS_COLORS[0],
                            BorderColor = BLOCKS_COLORS[1],
                            Color = BLOCKS_COLORS[2]
                        });
                    }
                }
            }
        }

        private List<ControlInfo> getCommonControls()
        {
            List<ControlInfo> array = (from model in Assignations
                where model.IsNotEditable
                select new ControlInfo
                {
                    control = model.Code.Split('=')[0].Trim(),
                    controlName = model.LocalizationKey, controlType = ControlType.Assignation
                }).ToList();
            array.AddRange(from model in Blocks
                where model.IsNotEditable
                select new ControlInfo
                {
                    control = model.Code.Split('=')[0].Trim(),
                    controlName = model.LocalizationKey, controlType = ControlType.Block
                });
            array.AddRange(from model in Conditions
                where model.IsNotEditable
                select new ControlInfo
                {
                    control = model.Code.Split('=')[0].Trim(),
                    controlName = model.LocalizationKey, controlType = ControlType.Condition
                });
            return array;
        }
    }
}