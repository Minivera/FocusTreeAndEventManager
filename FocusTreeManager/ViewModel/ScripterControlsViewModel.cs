using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

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

        public void BuildCommonControls()
        {
            //Temporary (?) common controls builder
            BuildConditions();
            BuildBlock();
            BuildAssignations();
        }

        private void BuildConditions()
        {
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            //Condition IF
            conditions.Add(new AssignationModel()
            {
                Text = resourceLocalization["Scripter_Condition_If"] as string,
                Code = "if = {}",
                IsNotEditable = true,
                CanHaveChild = true,
                BackgroundColor = CONDITIONS_COLORS[0],
                BorderColor = CONDITIONS_COLORS[1],
                Color = CONDITIONS_COLORS[2],
            });
            //Condition AND
            conditions.Add(new AssignationModel()
            {
                Text = resourceLocalization["Scripter_Condition_AND"] as string,
                Code = "AND = {}",
                IsNotEditable = true,
                CanHaveChild = true,
                BackgroundColor = CONDITIONS_COLORS[0],
                BorderColor = CONDITIONS_COLORS[1],
                Color = CONDITIONS_COLORS[2],
            });
            //Condition OR
            conditions.Add(new AssignationModel()
            {
                Text = resourceLocalization["Scripter_Condition_OR"] as string,
                Code = "OR = {}",
                IsNotEditable = true,
                CanHaveChild = true,
                BackgroundColor = CONDITIONS_COLORS[0],
                BorderColor = CONDITIONS_COLORS[1],
                Color = CONDITIONS_COLORS[2],
            });
            //Condition NOT
            conditions.Add(new AssignationModel()
            {
                Text = resourceLocalization["Scripter_Condition_NOT"] as string,
                Code = "NOT = {}",
                IsNotEditable = true,
                CanHaveChild = true,
                BackgroundColor = CONDITIONS_COLORS[0],
                BorderColor = CONDITIONS_COLORS[1],
                Color = CONDITIONS_COLORS[2],
            });
            //Condition LIMIT
            conditions.Add(new AssignationModel()
            {
                Text = resourceLocalization["Scripter_Condition_Limit"] as string,
                Code = "limit = {}",
                IsNotEditable = true,
                CanHaveChild = true,
                BackgroundColor = CONDITIONS_COLORS[0],
                BorderColor = CONDITIONS_COLORS[1],
                Color = CONDITIONS_COLORS[2],
            });
        }
        
        private void BuildBlock()
        {
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            //Block AI
            Blocks.Add(new AssignationModel()
            {
                Text = resourceLocalization["Scripter_Block_AI"] as string,
                Code = "ai_will_do = {}",
                IsNotEditable = true,
                CanHaveChild = true,
                BackgroundColor = BLOCKS_COLORS[0],
                BorderColor = BLOCKS_COLORS[1],
                Color = BLOCKS_COLORS[2],
            });
            //Block Completion reward
            Blocks.Add(new AssignationModel()
            {
                Text = resourceLocalization["Scripter_Block_Reward"] as string,
                Code = "completion_reward = {}",
                IsNotEditable = true,
                CanHaveChild = true,
                BackgroundColor = BLOCKS_COLORS[0],
                BorderColor = BLOCKS_COLORS[1],
                Color = BLOCKS_COLORS[2],
            });
            //Availability Block
            Blocks.Add(new AssignationModel()
            {
                Text = resourceLocalization["Scripter_Block_Available"] as string,
                Code = "available = {}",
                IsNotEditable = true,
                CanHaveChild = true,
                BackgroundColor = BLOCKS_COLORS[0],
                BorderColor = BLOCKS_COLORS[1],
                Color = BLOCKS_COLORS[2],
            });
            //Bypass Block
            Blocks.Add(new AssignationModel()
            {
                Text = resourceLocalization["Scripter_Block_Bypass"] as string,
                Code = "bypass = {}",
                IsNotEditable = true,
                CanHaveChild = true,
                BackgroundColor = BLOCKS_COLORS[0],
                BorderColor = BLOCKS_COLORS[1],
                Color = BLOCKS_COLORS[2],
            });
            //Cancel Block
            Blocks.Add(new AssignationModel()
            {
                Text = resourceLocalization["Scripter_Block_Cancel"] as string,
                Code = "cancel = {}",
                IsNotEditable = true,
                CanHaveChild = true,
                BackgroundColor = BLOCKS_COLORS[0],
                BorderColor = BLOCKS_COLORS[1],
                Color = BLOCKS_COLORS[2],
            });
            //Custom Block
            Blocks.Add(new AssignationModel()
            {
                Text = resourceLocalization["Scripter_Block_Custom"] as string,
                Code = "block_name = {}",
                IsNotEditable = true,
                CanHaveChild = true,
                BackgroundColor = BLOCKS_COLORS[0],
                BorderColor = BLOCKS_COLORS[1],
                Color = BLOCKS_COLORS[2],
            });
        }

        private void BuildAssignations()
        {
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            //Custom assignation
            assigantions.Add(new AssignationModel()
            {
                Text = resourceLocalization["Scripter_Assignation_Custom"] as string,
                Code = "prop = value",
                IsNotEditable = false,
                CanHaveChild = false,
                BackgroundColor = ASSIGNATIONS_COLORS[0],
                BorderColor = ASSIGNATIONS_COLORS[1],
                Color = ASSIGNATIONS_COLORS[2],
            });
        }
    }
}