// See https://aka.ms/new-console-template for more information
using System.Configuration;
using System.Collections.Specialized;




namespace frem
{
    class Program
    {
        static void Main(string[] args)
        {
            NameValueCollection sAll;
            string[] files;
            sAll = ConfigurationManager.AppSettings;


            if (sAll == null)
            {
                Console.WriteLine("AppSettings is empty.");
                System.Environment.Exit(2);
            }

            files = Directory.GetFiles(path: sAll["path"], "*." + sAll["ext"]);

            int day_d = int.Parse(sAll["day_d"]);

            DateTime now = DateTime.Now;

            // Define quarters and create sets to store the relevant files
            var lastDaysFiles = files.Where(f => (now - File.GetLastWriteTime(f)).Days <= day_d);
            //var quarterlyFiles = files
            //    .GroupBy(f => new { Year = File.GetLastWriteTime(f).Year, Quarter = (File.GetLastWriteTime(f).Month - 1) / 3 + 1 })
            //    .Select(g => g.OrderByDescending(f => File.GetLastWriteTime(f)).First());  // Select the most recent file for each quarter
            var monthlyFiles = files
                .GroupBy(f => new { Year = File.GetLastWriteTime(f).Year, Month = File.GetLastWriteTime(f).Month })
                .Select(g => g.OrderByDescending(f => File.GetLastWriteTime(f)).First());  // Select the most recent file for each month
            var yearlyFiles = files
                .GroupBy(f => File.GetLastWriteTime(f).Year)
                .Select(g => g.OrderByDescending(f => File.GetLastWriteTime(f)).First());  // Select the most recent file for each year






            // Combine the sets of files to keep
            //var filesToKeep = lastDaysFiles.Union(quarterlyFiles).Union(yearlyFiles).ToList();
            // Combine the sets of files to keep
            var filesToKeep = lastDaysFiles.Union(monthlyFiles).Union(yearlyFiles).ToList();

            // Find and delete files that are not in the 'filesToKeep' list
            foreach (var file in files)
            {
                if (!filesToKeep.Contains(file))
                {
                    File.Delete(file);
                    Console.WriteLine($"Deleted: {file}");
                }
            }

            Console.WriteLine("Cleanup complete.");
        }
    }
}