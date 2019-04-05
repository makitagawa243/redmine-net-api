using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot(RedmineKeys.PHONE)]

    public class Phone : IXmlSerializable, IEquatable<Phone>
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(RedmineKeys.VALUE)]
        public string Value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(RedmineKeys.KIND)]
        public string Kind { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public Phone(XmlReader reader)
        {
            Value = reader.GetAttribute(RedmineKeys.VALUE);
            Kind = reader.GetAttribute(RedmineKeys.KIND);
            reader.Read();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            reader.Read();

            while (!reader.EOF)
            {
                if (reader.IsEmptyElement && !reader.HasAttributes)
                {
                    reader.Read();
                    continue;
                }

                switch (reader.Name)
                {
                    case RedmineKeys.VALUE:
                        Value = reader.ReadContentAsString();
                        break;
                    case RedmineKeys.KIND:
                        Value = reader.ReadContentAsString();
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(RedmineKeys.VALUE, Value);
            writer.WriteElementString(RedmineKeys.KIND, Kind);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Phone other)
        {
            return Value == other.Value && Kind == other.Kind;
        }
    }
}
