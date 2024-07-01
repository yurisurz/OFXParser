namespace OFXParser
{
    class Program
    {
        static void Main(string[] args)
        {
            String filePath = args[0];
            StreamReader srOfx=new StreamReader(filePath);

            try{

                OfxParser ofxParser = new OfxParser();

                String parsedContent=ofxParser.ParseToJSON(srOfx.ReadToEnd());

                Console.WriteLine(parsedContent);
            }
            catch(Exception ex){
                Console.WriteLine(@"Error reading OFX file "+ ex.Message + "\n" + ex.StackTrace);
            }
            finally{
                srOfx.Close();
                srOfx=null;
            }
        }
    }

}