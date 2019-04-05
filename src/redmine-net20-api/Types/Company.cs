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
    [XmlRoot(RedmineKeys.COMPANY)]
    public class Company : Identifiable<Issue>, IXmlSerializable, IEquatable<Company>, ICloneable
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.NAME)]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.ADDRESS1)]
        public string Address1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.ADDRESS2)]
        public string Address2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.ZIP)]
        public string Zip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.TOWN)]
        public string Town { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.STATE)]
        public string State { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.COUNTRY)]
        public string Country { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.COUNTRY_CODE)]
        public string CountryCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.ISSUE_PRIORITY)]
        public IdentifiableName Priority { get; set; }

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
        /// 
        /// </summary>
        [XmlArray(RedmineKeys.PROJECTS)]
        [XmlArrayItem(RedmineKeys.PROJECT)]
        public IList<IdentifiableName> Projects { get; set; }


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
                    case RedmineKeys.ADDRESS1:
                        Address1 = reader.ReadElementContentAsString();
                        break;
                    case RedmineKeys.ADDRESS2:
                        Address2 = reader.ReadElementContentAsString();
                        break;
                    case RedmineKeys.ZIP:
                        Zip = reader.ReadElementContentAsString();
                        break;
                    case RedmineKeys.TOWN:
                        Town = reader.ReadElementContentAsString();
                        break;
                    case RedmineKeys.STATE:
                        State = reader.ReadElementContentAsString();
                        break;
                    case RedmineKeys.COUNTRY:
                        Country = reader.ReadElementContentAsString();
                        break;
                    case RedmineKeys.COUNTRY_CODE:
                        CountryCode = reader.ReadElementContentAsString();
                        break;
                    case RedmineKeys.ISSUE_PRIORITY:
                        Priority = new IdentifiableName(reader);
                        break;
                    case RedmineKeys.MAILS:
                        Mails = reader.ReadElementContentAsCollection<string>();
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
                                    Phone temp = null;

                                    if (r.IsEmptyElement && r.HasAttributes)
                                    {
                                        temp = new Phone(r);
                                    }

                                    if (temp != null) Phones.Add(temp);
                                    if (!r.IsEmptyElement) r.Read();
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
            writer.WriteElementString(RedmineKeys.ADDRESS1, Address1);
            writer.WriteElementString(RedmineKeys.ADDRESS2, Address2);
            writer.WriteElementString(RedmineKeys.ZIP, Zip);
            writer.WriteElementString(RedmineKeys.TOWN, Town);
            writer.WriteElementString(RedmineKeys.STATE, State);
            writer.WriteElementString(RedmineKeys.COUNTRY, Country);
            writer.WriteElementString(RedmineKeys.COUNTRY_CODE, CountryCode);
            writer.WriteIdIfNotNull(Priority, RedmineKeys.PRIORITY_ID);
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
            var issue = new Company
            {
                Id = Id,
                Name = Name,
                Address1 = Address1,
                Address2 = Address2,
                Zip = Zip,
                Town = Town,
                State = State,
                Country = Country,
                CountryCode = CountryCode,
                Priority = Priority,
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
        public bool Equals(Company other)
        {
            if (other == null) return false;
            return (
                Id == other.Id
            && Name == other.Name
            && Address1 == other.Address1
            && Address2 == other.Address2
            && Zip == other.Zip
            && Town == other.Town
            && State == other.State
            && Country == other.Country
            && CountryCode == other.CountryCode
            && Priority == other.Priority
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
            return string.Format("[Company: {15}, Name={0}, Addresss1={1}, Address2={2}, Zip={3}, Town={4}, State={5}, Country={6}, CountryCode={7}, Priority={8}, Mails={9}, Phones={10} Projects={11}, CustomFields={12}, CreatedOn={13}, UpdatedOn={14}",
                Name, Address1, Address2, Zip, Town, State, Country, CountryCode, Priority, Mails, Phones, Projects,
                CustomFields, CreatedOn, UpdatedOn, base.ToString());
            // TODO phones are missing now.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();

            hashCode = HashCodeHelper.GetHashCode(Name, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Address1, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Address2, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Zip, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Town, hashCode);
            hashCode = HashCodeHelper.GetHashCode(State, hashCode);

            hashCode = HashCodeHelper.GetHashCode(Country, hashCode);
            hashCode = HashCodeHelper.GetHashCode(CountryCode, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Priority, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Mails, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Phones, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Projects, hashCode);
            hashCode = HashCodeHelper.GetHashCode(CustomFields, hashCode);
            return hashCode;
        }
    }
}
