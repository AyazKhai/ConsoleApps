namespace ConsoleApp;

class Program 
{
    public static void Main(string[] args) 
    {
        try
        {
            Console.WriteLine("Введите относительный или полный путь в файлу");
            string inputPath = Console.ReadLine();

            string filename;

            if (Path.IsPathRooted(inputPath))
            {
                filename = inputPath;
            }
            else 
            {
                filename = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, inputPath));
            }

            if (!File.Exists(filename))
            {
                Console.WriteLine("Файл не найден!");
                return;
            }

            Dictionary<int, List<(int id, string name)>> hierarchy = new Dictionary<int, List<(int, string)>>();
            Dictionary<int, string> employees = new Dictionary<int, string>();
            List<int> rootCandidates = new List<int>();

            Console.WriteLine(filename);
            using (StreamReader reader = new StreamReader(filename)) 
            {
                string line;
                while ((line = reader.ReadLine()) != null ) 
                {
                    if(string.IsNullOrEmpty(line)) continue;

                    var parts = line.Split('|');
                    if (parts.Length >= 3)
                    {
                        int id = int.Parse(parts[0].Trim());
                        int managerId = int.Parse(parts[1].Trim());
                        string name = parts[2].Trim();

                        employees[id] = name;

                        if (managerId == 0)
                        {
                            rootCandidates.Add(id);
                        }
                        else
                        {
                            if (!hierarchy.ContainsKey(managerId))
                            {
                                hierarchy[managerId] = new List<(int, string)>();
                            }
                            hierarchy[managerId].Add((id, name));
                        }
                        continue;
                    }
                }

                if (rootCandidates.Count == 0)
                {
                    Console.WriteLine("В файле не найден корневой сотрудник.");
                    return;
                }

                foreach (var cond in rootCandidates) 
                {
                    PrintHierarchy(cond, hierarchy, employees, 0);
                }

            }

        }
        catch(Exception ex){ Console.WriteLine(ex.Message); }
    }

    private static void PrintHierarchy(int employeeId,
                                     Dictionary<int, List<(int id, string name)>> hierarchy,
                                     Dictionary<int, string> employees,
                                     int level)
    {
        string indent = new string('-', level * 4);
        Console.WriteLine($"{indent}{employees[employeeId]}");

        if (hierarchy.ContainsKey(employeeId))
        {
            foreach (var subordinate in hierarchy[employeeId])
            {
                PrintHierarchy(subordinate.id, hierarchy, employees, level + 1);
            }
        }
    }
}
