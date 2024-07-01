using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OFXParser
{
    /// <summary>
    /// Interface for Parsers of leaf node values in OFX file
    /// </summary>
    public interface IOfxTagValueParser
    {
        object ParseValue(string value);
    }

    /// <summary>
    /// Parser for OFX date values
    /// (Parsing of time and zone offset is not implemented yet)
    /// </summary>
    public class OfxDateValueParser : IOfxTagValueParser
    {
        public object ParseValue(string value)
        {
            // OFX dates are in the format yyyyMMddHHmmss[]
            // Example: 20240408100000[-03:EST]

            // Split the string to separate the datetime part from the timezone offset
            int indexOfBracket = value.IndexOf('[');
            string dateTimePart = value.Substring(0, indexOfBracket);
            string offsetPart = value.Substring(indexOfBracket); //currently ignored

            // Parse the datetime part
            DateTime localDateTime = DateTime.ParseExact(dateTimePart, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            DateTime localDate=new DateTime(localDateTime.Year,localDateTime.Month,localDateTime.Day,0,0,0);

            return localDate;
        }
    }

    /// <summary>
    /// Parser for OFX amount values using invariant culture
    /// </summary>
    public class OfxAmountValueParser : IOfxTagValueParser
    {
        public object ParseValue(string value)
        {
            if (decimal.TryParse(value, CultureInfo.InvariantCulture, out decimal amount))
            {
                return amount;
            }

            return null;
        }
    }


    /// <summary>
    /// Parser for OFX string/untyped values
    /// </summary>
    public class OfxStringValueParser : IOfxTagValueParser
    {
        public object ParseValue(string value)
        {
            return value; // No conversion needed for string values
        }
    }


    /// <summary>
    ///  Parser of leaf node values in OFX file
    /// </summary>
    public class OfxValueParser
    {
        private readonly Dictionary<string, IOfxTagValueParser> parsers = new Dictionary<string, IOfxTagValueParser>();

        public OfxValueParser()
        {
            // Register parsers for different tag names
            RegisterParser("DTSTART",new OfxDateValueParser());
            RegisterParser("DTEND",new OfxDateValueParser());
            RegisterParser("DTPOSTED",new OfxDateValueParser());
            RegisterParser("DTSERVER",new OfxDateValueParser());
            RegisterParser("DTASOF",new OfxDateValueParser());

            RegisterParser("TRNAMT",new OfxAmountValueParser());
            RegisterParser("BALAMT",new OfxAmountValueParser());
            // Additional parsers can be registered here
        }

        /// <summary>
        /// Register a parser for a specific tag name
        /// </summary>
        /// <param name="tagName">OFX tag that will trigger this value parser</param>
        /// <param name="parser">Reference to value parser for specified TAG</param>
        /// <exception cref="ArgumentException">Triggered when specific tagName passed is already in use by another parser OR tagName is null or empty</exception>
        public void RegisterParser(string tagName, IOfxTagValueParser parser)
        {
            if (parsers.ContainsKey(tagName))
            {
                throw new ArgumentException($"Parser for tag '{tagName}' is already registered");
            }

            if(string.IsNullOrEmpty(tagName))
            {
                throw new ArgumentException("Parser tag name cannot be null or empty");
            }

            parsers[tagName] = parser;
        }

        /// <summary>
        /// Parse the value of a specific tag
        /// </summary>
        /// <param name="tagName">TAG name</param>
        /// <param name="value">Value to be parsed</param>
        /// <returns></returns>
        public object ParseValue(string tagName, string value)
        {
            if (parsers.TryGetValue(tagName, out IOfxTagValueParser parser))
            {
                return parser.ParseValue(value);
            }
            // Fallback to string parser if no specific parser is found
            return new OfxStringValueParser().ParseValue(value);
        }
    }
}