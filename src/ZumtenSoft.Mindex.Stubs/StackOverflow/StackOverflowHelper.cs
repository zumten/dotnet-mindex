using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace ZumtenSoft.Mindex.Stubs.StackOverflow
{
    public static class StackOverflowHelper
    {
        public static IEnumerable<StackOverflowUser> LoadUsers(string fileName)
        {
            using (FileStream file = File.OpenRead(fileName))
            using (XmlReader reader = XmlReader.Create(file))
            {
                reader.MoveToContent();
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element
                        && reader.Name == "row"
                        && XNode.ReadFrom(reader) is XElement element)
                    {
                        yield return new StackOverflowUser
                        {
                            Id = (int)element.Attribute("Id"),
                            Reputation = (int)element.Attribute("Reputation"),
                            CreationDate = (DateTime)element.Attribute("CreationDate"),
                            LastAccessDate = (DateTime)element.Attribute("LastAccessDate"),
                            Location = Intern((string)element.Attribute("Location")),
                            WebsiteUrl = Intern((string)element.Attribute("WebsiteUrl")),
                            DisplayName = Intern((string)element.Attribute("DisplayName")),
                            Views = (int)element.Attribute("Views"),
                            UpVotes = (int)element.Attribute("UpVotes"),
                            DownVotes = (int)element.Attribute("DownVotes")
                        };
                    }
                }
            }
        }

        private static string Intern(string str)
        {
            return str == null ? null : String.Intern(str);
        }
    }
}
