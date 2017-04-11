using FocusTreeManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FocusTreeManager.CodeStructures;
using System.IO;
using FocusTreeManager.DataContract;
using FocusTreeManager.CodeStructures.CodeExceptions;
using FocusTreeManager.Model.TabModels;

namespace FocusTreeManager.Parsers
{
    public static class EventParser
    {
        private static readonly string[] ALL_PARSED_ELEMENTS =
        {
            "id", "title", "picture", "major", "is_triggered_only",
            "hidden", "fire_only_once", "desc", "option"
        };

        public static string ParseEventForCompare(EventTabModel model)
        {
            EventContainer container = new EventContainer(model);
            return Parse(container.EventList.ToList(), container.EventNamespace);
        }

        public static string ParseEventScriptForCompare(string filename)
        {
            if (!File.Exists(filename))
            {
                return "";
            }
            Script script = new Script();
            script.Analyse(File.ReadAllText(filename));
            return ParseEventForCompare(CreateEventFromScript(filename, script));
        }

        public static Dictionary<string, string> ParseAllEvents(List<EventContainer> Containers)
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
                        text.AppendLine(desc.InternalScript.Parse(null, 2));
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
                        text.AppendLine(option.InternalScript.Parse(null, 2));
                        text.AppendLine("\t}");
                    }
                }
                text.AppendLine(item.InternalScript.Parse());
                text.AppendLine("\t}");
            }
            return text.ToString();
        }

        public static EventTabModel CreateEventFromScript(string fileName, Script script)
        {
            EventTabModel container = new EventTabModel(Path.GetFileNameWithoutExtension(fileName))
            {
                EventNamespace = script.FindValue("add_namespace").Parse()
            };
            foreach (ICodeStruct codeStruct in script.FindAllValuesOfType<CodeBlock>("news_event"))
            {
                CodeBlock block = (CodeBlock)codeStruct;
                //Check if the event is only a call, if it isn't, it must have a least one option
                if (block.FindAssignation("option") == null)
                {
                    break;
                }
                EventModel newEvent = new EventModel();
                try
                {
                    newEvent.Type = Event.EventType.news_event;
                    newEvent.Id = Script.TryParse(block, "id");
                    //An event can have no picture.
                    newEvent.Picture = Script.TryParse(block, "picture", null, false)?.Replace("GFX_", "");
                    newEvent.IsMajor = block.FindValue("major") != null 
                        && YesToBool(Script.TryParse(block, "major"));
                    newEvent.IsHidden = block.FindValue("hidden") != null 
                        && YesToBool(Script.TryParse(block, "hidden"));
                    newEvent.IsTriggeredOnly = block.FindValue("is_triggered_only") != null 
                        && YesToBool(Script.TryParse(block, "is_triggered_only"));
                    newEvent.IsFiredOnce = block.FindValue("fire_only_once") != null 
                        && YesToBool(Script.TryParse(block, "fire_only_once"));
                    foreach (ICodeStruct desc in block.FindAllValuesOfType<ICodeStruct>("desc"))
                    {
                        newEvent.Descriptions.Add(new EventDescriptionModel
                        {
                            InternalScript = desc.GetContentAsScript(new string[0], script.Comments)
                        });
                    }
                    foreach (ICodeStruct option in block.FindAllValuesOfType<ICodeStruct>("option"))
                    {
                        newEvent.Options.Add(new EventOptionModel
                        {
                            Name = Script.TryParse(option, "name"),
                            InternalScript = option.GetContentAsScript(new[] { "name" }, script.Comments)
                        });
                    }
                    //Get all core scripting elements
                    Script InternalEventScript = block.
                        GetContentAsScript(ALL_PARSED_ELEMENTS.ToArray(), script.Comments);
                    newEvent.InternalScript = InternalEventScript;
                    container.EventList.Add(newEvent);
                }
                catch (SyntaxException e)
                {
                    //TODO: Add language support
                    ErrorLogger.Instance.AddLogLine("Invalid syntax for event "
                        + Script.TryParse(block, "id") + ", please double-check the syntax.");
                    ErrorLogger.Instance.AddLogLine("\t" + e.Message);
                }
                catch (Exception)
                {
                    //TODO: Add language support
                    ErrorLogger.Instance.AddLogLine("Invalid syntax for event "
                        + block.FindValue("id").Parse() + ", please double-check the syntax.");
                }
            }
            foreach (ICodeStruct codeStruct in script.FindAllValuesOfType<CodeBlock>("country_event"))
            {
                CodeBlock block = (CodeBlock)codeStruct;
                //Check if the event is only a call, if it isn't, it must have a least one option
                if (block.FindAssignation("option") == null)
                {
                    break;
                }
                EventModel newEvent = new EventModel();
                try
                { 
                    newEvent.Type = Event.EventType.country_event;
                    newEvent.Id = Script.TryParse(block, "id");
                    //An event can have no picture.
                    newEvent.Picture = Script.TryParse(block, "picture", null, false)?.Replace("GFX_", "");
                    newEvent.IsMajor = block.FindValue("major") != null && 
                        YesToBool(Script.TryParse(block, "major"));
                    newEvent.IsHidden = block.FindValue("hidden") != null && 
                        YesToBool(Script.TryParse(block, "hidden"));
                    newEvent.IsTriggeredOnly = block.FindValue("is_triggered_only") != null && 
                        YesToBool(Script.TryParse(block, "is_triggered_only"));
                    newEvent.IsFiredOnce = block.FindValue("fire_only_once") != null && 
                        YesToBool(Script.TryParse(block, "fire_only_once"));
                    foreach (ICodeStruct option in block.FindAllValuesOfType<ICodeStruct>("option"))
                    {
                        newEvent.Options.Add(new EventOptionModel
                        {
                            Name = Script.TryParse(option, "name"),
                            InternalScript = option.GetContentAsScript(new[] { "name" })
                        });
                    }
                    //Get all core scripting elements
                    Script InternalEventScript = block.
                        GetContentAsScript(ALL_PARSED_ELEMENTS.ToArray(), script.Comments);
                    newEvent.InternalScript = InternalEventScript;
                    container.EventList.Add(newEvent);
                }
                catch (SyntaxException e)
                {
                    //TODO: Add language support
                    ErrorLogger.Instance.AddLogLine("Invalid syntax for event "
                        + Script.TryParse(block, "id") + ", please double-check the syntax.");
                    ErrorLogger.Instance.AddLogLine("\t" + e.Message);
                }
                catch (Exception)
                {
                    //TODO: Add language support
                    ErrorLogger.Instance.AddLogLine("Invalid syntax for event "
                        + block.FindValue("id").Parse() + ", please double-check the syntax.");
                }
            }
            return container;
        }

        private static bool YesToBool(string value)
        {
            return value == "yes";
        }
    }
}
