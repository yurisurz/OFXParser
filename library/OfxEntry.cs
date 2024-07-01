using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OFXParser
{
    /// <summary>
    /// Represents a single entry/node in the OFX file
    /// </summary>
    public class OfxEntry
    {

        /// <summary>
        /// The tag of the entry
        /// </summary>
        public string Tag{get;set;}
        
        /// <summary>
        /// The value of the entry when it is a leaf node
        /// </summary>
        public object Value{get;set;}

        /// <summary>
        /// The children nodes of the entry when it is a parent node
        /// </summary>
        public List<OfxEntry> Children{get;set;}

        /// <summary>
        /// Pointer to the parent node of the entry, when null it is the root node, normally OFX node
        /// </summary>
        public OfxEntry? Parent{get;set;}


        /// <summary>
        /// Constructor for the OfxEntry class
        /// </summary>
        /// <param name="tag">OFX node TAG name</param>
        /// <param name="value">OFX node value</param>
        public OfxEntry(string tag, object value)
        {
            Tag = tag;
            Value = value;
            Children = new List<OfxEntry>();
        }

        /// <summary>
        /// Constructor for the OfxEntry class with parent node
        /// </summary>
        /// <param name="tag">OFX node TAG name</param>
        /// <param name="value">OFX node value</param>
        /// <param name="parent">OFX parent node reference</param>
        public OfxEntry(string tag, object value, OfxEntry parent)
        {
            Tag = tag;
            Value = value;
            Children = new List<OfxEntry>();
            Parent = parent;
        }
    }
}