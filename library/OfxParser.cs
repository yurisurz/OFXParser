using System.Text;
using System.Text.Json;

namespace OFXParser
{    

    /// <summary>
    /// Read and parse OFX content to High-level object or JSON
    /// </summary>
    public sealed class OfxParser
    {

        private OfxValueParser valueParser=new OfxValueParser();

        /// <summary>
        /// Parse OFX content to high-level object
        /// </summary>
        /// <param name="ofxContent">String containing OFX content to be parsed</param>
        /// <returns></returns>
        /// <exception cref="Exception">Triggered when OFX content is mal-formed</exception>
        public OfxEntry Parse(string ofxContent)
        {
            Stack<string> tags = new Stack<string>();
            OfxEntry root = new OfxEntry("OFX", null);
            OfxEntry currentObject = root;
            int index = 0;

            //replace all line breaks
            ofxContent = ofxContent.Replace("\r", "").Replace("\n", "");

            // Find the start of the OFX content
            int startIndex = ofxContent.IndexOf("<OFX>");
            if (startIndex == -1) return null; // OFX tag not found, return null
            
            //jump the OFX tag
            index = startIndex+5;
            tags.Push("OFX");

            while (index < ofxContent.Length)
            {
                if (ofxContent[index] == '<')
                {
                    // Find the end of the tag
                    int endIndex = ofxContent.IndexOf('>', index);
                    if (endIndex == -1) break; // No closing '>', exit loop

                    string tag = ofxContent.Substring(index + 1, endIndex - index - 1);
                    index = endIndex + 1; // Move index past this tag

                    if (tag.StartsWith("/"))
                    {
                        // Closing tag
                        tag = tag.Substring(1); // Remove '/'
                        if (tags.Count > 0 && tags.Peek() == tag)
                        {
                            tags.Pop(); // Pop the corresponding opening tag
                            currentObject = currentObject.Parent;//return to the parent object

                            if (tags.Count == 0 || currentObject == null)
                            {
                                break; // If no more tags, end parsing
                            }
                        }
                    }
                    else if (!tag.EndsWith("/")) // Ignore self-closing tags
                    {
                        // Opening tag or tag with value
                        endIndex = ofxContent.IndexOf('<', index);
                        if (endIndex == -1) break; // No more tags, exit loop

                        //check if current tag is already a child of the current object
                        OfxEntry entry=new OfxEntry(tag,null,currentObject);

                        //Handle value of tag
                        string value = ofxContent.Substring(index, endIndex - index).Trim();
                        if (!string.IsNullOrEmpty(value))
                        {
                            entry.Value = valueParser.ParseValue(tag, value);
                            currentObject.Children.Add(entry); //only add a valued child
                        }
                        else
                        {
                            //When no value
                            tags.Push(tag); //push onto the stack
                            currentObject.Children.Add(entry); //then add to the children
                            currentObject=currentObject.Children[currentObject.Children.Count-1]; //finally move pointer there
                        }

                        //shift index to next tag
                        index=endIndex;               
                    }
                }
            }

            //check stack to ensure all tags were closed
            if (tags.Count > 0)
            {
                throw new Exception("Invalid OFX content"); // Missing closing tag
            }

            return root;
        }

        /// <summary>
        /// Parse OFX content to JSON
        /// </summary>
        /// <param name="ofxContent">String containing OFX content to be parsed</param>
        /// <returns></returns>
        /// <exception cref="Exception">Triggered when OFX content is mal-formed</exception>
        /// <remarks>It uses Parse method to get high-level object and then convert it to JSON.</remarks>
        public string ParseToJSON(string ofxContent)
        {
            OfxEntry root = Parse(ofxContent);
            StringBuilder sb=new StringBuilder();

            if(root!=null)
            {
                return ParseEntryToJson(root);
            }
            else
            {
                return "{}";
            }
        }

        
        private String ParseEntryToJson(OfxEntry entry, bool isValue=false)
        {
            StringBuilder sb=new StringBuilder();

            if(entry.Parent==null)
            {
                sb.Append("{");
            }

            if(entry.Value!=null)
            {
                sb.Append($"\"{entry.Tag}\":");
                sb.Append(JsonSerializer.Serialize(entry.Value));
            }
            else
            {                
                //Get distinct children tags to avoid duplications in json
                List<string> lstChildrenTags=entry.Children.Select(x=>x.Tag).Distinct().ToList();

                if(!isValue)
                    sb.Append($"\"{entry.Tag}\":");

                sb.Append("{");

                int cntTags=lstChildrenTags.Count;
                foreach(string tag in lstChildrenTags)
                {
                    //Check if current entry Tag has duplicates
                    if(entry.Children.FindAll(x=>x.Tag==tag).Count>1)
                    {
                        sb.Append($"\"{tag}\":[");
                        int cntValues=entry.Children.FindAll(x=>x.Tag==tag).Count();

                        foreach(OfxEntry child in entry.Children.FindAll(x=>x.Tag==tag))
                        {
                            sb.Append(ParseEntryToJson(child, true));
                            
                            if(cntValues>1)
                                sb.Append(",");

                            cntValues--;
                        }
                        sb.Append("]");
                    }
                    else
                    {
                        sb.Append(ParseEntryToJson(entry.Children.First(x=>x.Tag==tag)));
                        
                        if(cntTags>1)
                            sb.Append(",");

                        cntTags--;
                    }
                } 
                sb.Append("}");          
            }

            if(entry.Parent==null)
            {
                sb.Append("}");
            }

            return sb.ToString();
        }
    }   
}