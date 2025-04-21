using System;
using System.IO;
using System.Xml;
using System.Data;
using Aspose.Cells;

class Program
{
    static void Main()
    {

        try
        {
        //    string fileXLSX = "C:\\Users\\ayazh\\OneDrive\\Рабочий стол\\КАМАЗ2\\ConsoleApp\\ViewerMessages.xlsx";
        //    string fileXML = "C:\\Users\\ayazh\\OneDrive\\Рабочий стол\\КАМАЗ2\\ConsoleApp\\ViewerMessages.xml";
            string fileXLSX = GetFilePath(".xlsx");
            string fileXML = GetFilePath(".xml");

            Dictionary<string, string> dictionary = ReadExcelToDictionary(fileXLSX, 0);
            TranslateFromXML(fileXML, dictionary);
            Console.WriteLine("XML файл перезаписан.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

    }
    public static string GetFilePath( string type)
    {
        Console.WriteLine($"Введите относительный или полный путь к файлу {type} {AppDomain.CurrentDomain.BaseDirectory}");
        string input = Console.ReadLine();

        string filePath = Path.IsPathRooted(input) ? input :
                         Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, input));

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Файл {type} не найден!");
        }

        return filePath;
    }

    public static Dictionary<string, string> ReadExcelToDictionary(string input, int listId) 
    {
        var dictionary = new Dictionary<string, string>();
        Workbook wb = new Workbook(input);

        Worksheet worksheet = wb.Worksheets[listId];

        int rows = worksheet.Cells.MaxDataRow;
        int cols = 2;

        for (int i = 0; i <= rows; i++) 
        {
            string key = worksheet.Cells[i, 0].Value.ToString().Trim();
            string value = worksheet.Cells[i, 1].Value.ToString().Trim();

            
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, value);
                }
                else
                {
                    Console.WriteLine($"Предупреждение: дублирующийся ключ '{key}' в строке {i + 1}");
                }
            }
        }

        return dictionary;
    }

    public static void TranslateFromXML(string pathxml, Dictionary<string, string> dictionary)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(pathxml);

        XmlNodeList messageNodes = xmlDoc.SelectNodes("//message");
        foreach (XmlNode messageNode in messageNodes)
        {
            XmlNode eng = messageNode.SelectSingleNode("variant[@language='en_US']");
            if (eng == null || string.IsNullOrEmpty(eng.InnerText)) continue;

            XmlNode rus = messageNode.SelectSingleNode("variant[@language='ru_RU']");

            if (rus == null)
            {
                rus = xmlDoc.CreateElement("variant");
                XmlAttribute langAttr = xmlDoc.CreateAttribute("language");
                langAttr.Value = "ru_RU";
                rus.Attributes.Append(langAttr);
                messageNode.AppendChild(rus);
            }

            if (dictionary.TryGetValue(eng.InnerText, out string translation))
            {
                rus.InnerText = translation;
            }
        }
        xmlDoc.Save( pathxml);
        
    }
}