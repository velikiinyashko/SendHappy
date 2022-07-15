using SendHappy.Models;
namespace SendHappy.Extension;
    public static class HtmlBody
    {
        public static string getBody(this List<EmployeesBirthDayDTO> birthDayDTOs)
        {
            string file;
            string path = $"{Environment.CurrentDirectory}/index.html";
            using (StreamReader reader = new StreamReader(path: path))
            {
                file = reader.ReadToEnd();
                file = file.Replace(@"@date", DateTime.Now.ToString("dd MMMM yyyy"));
                string table = string.Empty;
                birthDayDTOs.ForEach(s =>
                {
                    table += $"<tr style=\"background-color: #2B9793;\">" +
                             $"<td style=\"background-color: #2B9793; border-radius: 5px; font-size: 35px;\">" +
                             $"{s.FirstName + " " + s.MiddleName + " " + s.LastName }" +
                             $"</td>" +
                             $"<td style=\"border-radius: 5px; background-color: #126F6C; width: 30%;\">" +
                             $"{s.Position + " " + s.Retail}" +
                             $"</td>" +
                             $"</tr><tr style=\"background-color: #ffffff;\"></tr>";
                });
                file = file.Replace(@"@table", table);
            }
            return file;
        }
    }