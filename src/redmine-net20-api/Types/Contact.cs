using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using System.IO;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot(RedmineKeys.CONTACT)]
    public class Contact : Identifiable<Contact>, IXmlSerializable, IEquatable<Contact>, ICloneable
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.NAME)]
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlArray(RedmineKeys.PROJECTS)]
        [XmlArrayItem(RedmineKeys.PROJECT)]
        public IList<IdentifiableName> Projects { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.COMPANY)]
        public IdentifiableName Company { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.FIRSTNAME)]
        public string FirstName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.LASTNAME)]
        public string LastName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.GENDER)]
        public bool Gender { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlArray(RedmineKeys.MAILS)]
        [XmlArrayItem(RedmineKeys.MAIL)]
        public IList<string> Mails { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlArray(RedmineKeys.PHONES)]
        [XmlArrayItem(RedmineKeys.PHONE)]
        public IList<Phone> Phones { get; set; }
        
        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        [XmlArray(RedmineKeys.CUSTOM_FIELDS)]
        [XmlArrayItem(RedmineKeys.CUSTOM_FIELD)]
        public IList<IssueCustomField> CustomFields { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [XmlElement(RedmineKeys.CREATED_ON, IsNullable = true)]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the updated on.
        /// </summary>
        /// <value>The updated on.</value>
        [XmlElement(RedmineKeys.UPDATED_ON, IsNullable = true)]
        public DateTime? UpdatedOn { get; set; }

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
                    case RedmineKeys.ID:
                        Id = reader.ReadElementContentAsInt();
                        break;
                    case RedmineKeys.NAME:
                        Name = reader.ReadElementContentAsString();
                        break;
                    case RedmineKeys.FIRSTNAME:
                        FirstName = reader.ReadElementContentAsString();
                        break;
                    case RedmineKeys.LASTNAME:
                        LastName = reader.ReadElementContentAsString();
                        break;
                    case RedmineKeys.GENDER:
                        Gender = reader.ReadElementContentAsBoolean();
                        break;
                    case RedmineKeys.MAILS:
                        {
                            Mails = new List<string>();
                            var xml = reader.ReadOuterXml();
                            using (var sr = new StringReader(xml))
                            {
                                var r = new XmlTextReader(sr);
                                r.ReadToFollowing("mail");
                                while (!r.EOF)
                                {
                                    if (r.NodeType == XmlNodeType.EndElement)
                                    {
                                        r.ReadEndElement();
                                        continue;
                                    }
                                    if (r.NodeType != XmlNodeType.None)
                                    {
                                        Mails.Add(r.ReadElementContentAsString("mail", ""));
                                    }
                                    else
                                    {
                                        r.Read();
                                    }
                                }
                            }
                        }
                        break;
                    case RedmineKeys.PHONES:
                        {
                            // TODO TBD Phone has "value" attribute. it seems to cause the error of deserialize. To avoid, Phone instance is generated in this method. should be found more smart way...
                            Phones = new List<Phone>();
                            var xml = reader.ReadOuterXml();
                            using (var sr = new StringReader(xml))
                            {
                                var r = new XmlTextReader(sr);
                                r.ReadStartElement();
                                while (!r.EOF)
                                {
                                    if (r.NodeType == XmlNodeType.EndElement)
                                    {
                                        r.ReadEndElement();
                                        continue;
                                    }
                                    Phone temp = new Phone(r);
                                    if (temp != null) Phones.Add(temp);
                                }
                            }
                        }

                        break;
                    case RedmineKeys.PROJECTS:
                        {
                            // TODO TBD projects are IdentifiableName's list, it seems to cause same problem as Phone. should be found more smart way...
                            Projects = new List<IdentifiableName>();
                            var xml = reader.ReadOuterXml();
                            using (var sr = new StringReader(xml))
                            {
                                var r = new XmlTextReader(sr);
                                r.ReadStartElement();
                                while (!r.EOF)
                                {
                                    if (r.NodeType == XmlNodeType.EndElement)
                                    {
                                        r.ReadEndElement();
                                        continue;
                                    }
                                    IdentifiableName temp = null;

                                    if (r.IsEmptyElement && r.HasAttributes)
                                    {
                                        temp = new IdentifiableName(r);
                                    }

                                    if (temp != null) Projects.Add(temp);
                                    if (!r.IsEmptyElement) r.Read();
                                }
                            }
                        }
                        break;

                    case RedmineKeys.CREATED_ON:
                        CreatedOn = reader.ReadElementContentAsNullableDateTime();
                        break;

                    case RedmineKeys.UPDATED_ON:
                        UpdatedOn = reader.ReadElementContentAsNullableDateTime();
                        break;

                    case RedmineKeys.CUSTOM_FIELDS:
                        CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>();
                        break;



                    default:
                        reader.Read();
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
            writer.WriteElementString(RedmineKeys.NAME, Name);
            writer.WriteElementString(RedmineKeys.FIRSTNAME, FirstName);
            writer.WriteElementString(RedmineKeys.LASTNAME, LastName);
            writer.WriteElementString(RedmineKeys.GENDER, Gender.ToString());
            writer.WriteArray(Mails, RedmineKeys.MAILS);
            writer.WriteArray(Phones, RedmineKeys.PHONES);
            writer.WriteArray(Projects, RedmineKeys.PROJECTS);
            writer.WriteArray(CustomFields, RedmineKeys.CUSTOM_FIELDS);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var issue = new Contact
            {
                Id = Id,
                Name = Name,
                FirstName = FirstName,
                LastName = LastName,
                Gender = Gender,
                Mails = Mails,
                Phones = Phones,
                Projects = Projects,
                CustomFields = CustomFields
            };
            return issue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Contact other)
        {
            if (other == null) return false;
            return (
                Id == other.Id
            && Name == other.Name
            && FirstName == other.FirstName
            && LastName == other.LastName
            && Gender == other.Gender
            && (Mails != null ? Mails.Equals<string>(other.Mails) : other.Mails == null)
            && (Phones != null ? Phones.Equals<Phone>(other.Phones) : other.Phones == null)
            && (Projects != null ? Projects.Equals<IdentifiableName>(other.Projects) : other.Mails == null)
            && (CustomFields != null ? CustomFields.Equals<IssueCustomField>(other.CustomFields) : other.CustomFields == null)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Contact: {10}, Name={0}, FirstName={1}, LastName={2}, Gender={3}, Mails={4}, Phones={5} Projects={6}, CustomFields={7}, CreatedOn={8}, UpdatedOn={9}",
                Name, FirstName, LastName, Gender, Mails, Phones, Projects,
                CustomFields, CreatedOn, UpdatedOn, base.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();

            hashCode = HashCodeHelper.GetHashCode(Name, hashCode);
            hashCode = HashCodeHelper.GetHashCode(FirstName, hashCode);
            hashCode = HashCodeHelper.GetHashCode(LastName, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Gender, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Mails, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Phones, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Projects, hashCode);
            hashCode = HashCodeHelper.GetHashCode(CustomFields, hashCode);
            return hashCode;
        }
    }
}
