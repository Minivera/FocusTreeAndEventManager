using FocusTreeManager.Containers;
using FocusTreeManager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FocusTreeManager.CodeStructures;
using System.IO;

namespace FocusTreeManager.Parsers
{
    static class EventParser
    {
        private static readonly string[] CORE_EVENT_SCRIPTS_ELEMENTS =
        {
            "immediate", "mean_time_to_happen", "trigger"
        };

        public static Dictionary<string, string> ParseAllEvents(ObservableCollection<EventContainer> Containers)
        {
            Dictionary<string, string> fileList = new Dictionary<string, string>();
            foreach (EventContainer container in Containers)
            {
                string ID = container.ContainerID.Replace(" ", "_");
                fileList[ID] = Parse(container.EventList.ToList(), container.EventNamespace);
            }
            return fileList;
        }

        public static string Parse(List<Event> listEvent, string EventNamespace)
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("add_namespace = " + EventNamespace);
            foreach (Event item in listEvent)
            {
                text.AppendLine(item.Type.ToString() + " = {");
                text.AppendLine("\tid = " + item.Id);
                text.AppendLine("\ttitle = " + item.Id + ".t");
                text.AppendLine("\tpicture = GFX_" + item.Picture);
                if (item.IsMajor)
                {
                    text.AppendLine("\tmajor = yes");
                }
                if (item.IsTriggeredOnly)
                {
                    text.AppendLine("\tis_triggered_only = yes");
                }
                if (item.IsHidden)
                {
                    text.AppendLine("\thidden = yes");
                }
                if (item.IsFiredOnce)
                {
                    text.AppendLine("\tfire_only_once = yes");
                }
                if (item.Type == Event.EventType.news_event)
                {
                    foreach (EventDescription desc in item.Descriptions)
                    {
                        text.AppendLine("\tdesc = {");
                        text.AppendLine(desc.InternalScript.Parse(2));
                        text.AppendLine("\t}");
                    }
                }
                else
                {
                    text.AppendLine("\tdesc = " + item.Id + ".d");
                }
                if (item.Options.Any())
                {
                    foreach (EventOption option in item.Options)
                    {
                        text.AppendLine("\toption = {");
                        text.AppendLine("\tname = " + option.Name);
                        text.AppendLine(option.InternalScript.Parse(2));
                        text.AppendLine("\t}");
                    }
                }
                text.AppendLine(item.InternalScript.Parse());
                text.AppendLine("\t}");
            }
            return text.ToString();
        }

        public static EventContainer CreateEventFromScript(string fileName, Script script)
        {
            EventContainer container = new EventContainer(Path.GetFileNameWithoutExtension(fileName));
            container.EventNamespace = script.FindValue("add_namespace").Parse();
            foreach (CodeBlock block in script.FindAllValuesOfType<CodeBlock>("news_event"))
            {
                Event newEvent = new Event();
                newEvent.Type = Event.EventType.news_event;
                newEvent.Id = block.FindValue("id").Parse();
                newEvent.Picture = block.FindValue("picture").Parse().Replace("GFX_", "");
                newEvent.IsMajor = block.FindValue("major") != null ? 
                    YesToBool(block.FindValue("major").Parse()) : false;
                newEvent.IsHidden = block.FindValue("hidden") != null ?
                    YesToBool(block.FindValue("hidden").Parse()) : false;
                newEvent.IsTriggeredOnly = block.FindValue("is_triggered_only") != null ?
                    YesToBool(block.FindValue("is_triggered_only").Parse()) : false;
                newEvent.IsFiredOnce = block.FindValue("fire_only_once") != null ?
                    YesToBool(block.FindValue("fire_only_once").Parse()) : false;
                foreach (ICodeStruct desc in block.FindAllValuesOfType<ICodeStruct>("desc"))
                {
                    newEvent.Descriptions.Add(new EventDescription
                    {
                        InternalScript = desc.GetContentAsScript(new string[0])
                    });
                }
                foreach (ICodeStruct option in block.FindAllValuesOfType<ICodeStruct>("option"))
                {
                    newEvent.Options.Add(new EventOption
                    {
                        Name = option.FindValue("name").Parse(),
                        InternalScript = option.GetContentAsScript(new string[1] { "name" })
                    });
                }
                //Get all core scripting elements
                Script InternalEventScript = new Script();
                for (int i = 0; i < CORE_EVENT_SCRIPTS_ELEMENTS.Length; i++)
                {
                    ICodeStruct found = block.FindAssignation(CORE_EVENT_SCRIPTS_ELEMENTS[i]);
                    if (found != null)
                    {
                        InternalEventScript.Code.Add(found);
                    }
                }
                newEvent.InternalScript = InternalEventScript;
                container.EventList.Add(newEvent);
            }
            foreach (CodeBlock block in script.FindAllValuesOfType<CodeBlock>("country_event"))
            {
                Event newEvent = new Event();
                newEvent.Type = Event.EventType.country_event;
                newEvent.Id = block.FindValue("id").Parse();
                newEvent.Picture = block.FindValue("picture").Parse().Replace("GFX_", "");
                newEvent.IsMajor = block.FindValue("major") != null ?
                    YesToBool(block.FindValue("major").Parse()) : false;
                newEvent.IsHidden = block.FindValue("hidden") != null ?
                    YesToBool(block.FindValue("hidden").Parse()) : false;
                newEvent.IsTriggeredOnly = block.FindValue("is_triggered_only") != null ?
                    YesToBool(block.FindValue("is_triggered_only").Parse()) : false;
                newEvent.IsFiredOnce = block.FindValue("fire_only_once") != null ?
                    YesToBool(block.FindValue("fire_only_once").Parse()) : false;
                foreach (ICodeStruct option in block.FindAllValuesOfType<ICodeStruct>("option"))
                {
                    newEvent.Options.Add(new EventOption
                    {
                        Name = option.FindValue("name").Parse(),
                        InternalScript = option.GetContentAsScript(new string[1] { "name" })
                    });
                }
                //Get all core scripting elements
                Script InternalEventScript = new Script();
                for (int i = 0; i < CORE_EVENT_SCRIPTS_ELEMENTS.Length; i++)
                {
                    ICodeStruct found = block.FindAssignation(CORE_EVENT_SCRIPTS_ELEMENTS[i]);
                    if (found != null)
                    {
                        InternalEventScript.Code.Add(found);
                    }
                }
                newEvent.InternalScript = InternalEventScript;
                container.EventList.Add(newEvent);
            }
            return container;
        }

        private static bool YesToBool(string value)
        {
            if (value == "yes")
            {
                return true;
            }
            return false;
        }
    }
}
