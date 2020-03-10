using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace WebFramework
{
    public abstract class ControllerBase
    {       
        public string Root
        {
            get 
            {   
                Assembly assembly = Assembly.GetEntryAssembly();                       
                return assembly.Location.ToString();                          
            }            
        }        
        
        public string View(string viewName, string controllerName)
        {
            string contents;
            if (viewName.Equals("Dummy"))
            {
                contents = "<ul>";
                for (int i = 0; i < 5; i++)
                {
                    contents += $"<li>{i}</li>";

                }
                contents += "</ul>";
            }
            else
            {
                contents = File.ReadAllText($@"./Views/{controllerName}/{viewName}.html");
            }            
            string html = InjectToLayout(contents);
            return html;
        }

        public string View(string viewName, string controllerName, object model)
        {
            List<string> placeholders = new List<string>();

            string html = "";
            string layoutFile = $@"./Views/Shared/Layout.html";
            if (!File.Exists(layoutFile)) return null;
            string layout = File.ReadAllText(layoutFile);
            placeholders = GetPlaceholders(layout);
            foreach (var item in placeholders)
            {
                html = html + layout.Replace($"@{{{item}}}", model.ToString());
            }          
            
            
            return html;
        }
        

        private string InjectToLayout(string view)
        {
            string layoutFile = $@"./Views/Shared/Layout.html";
            if (!File.Exists(layoutFile)) return null;
            string layout = File.ReadAllText(layoutFile);
            string html = layout.Replace("@RenderBody()", view);
            return html;
        }

        private List<string> GetPlaceholders(string view)
        {
            var regex = new Regex(@"@{(?<name>\w+)}");
            return regex.Matches(view)
                .Select(x => x.Groups["name"].Value)
                .ToList();
        }        
    }
}
