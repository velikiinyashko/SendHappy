namespace SendHappy.Models
{
    public class EmployeesBirthDayDTO
    {
        /// <summary>
        /// Имя сотрудника
        /// </summary>
        public string FirstName { get; set; } = "unknown firstname";
        /// <summary>
        /// Фамилия сотрудника
        /// </summary>
        public string MiddleName { get; set; } = "unknown middlename";
        /// <summary>
        /// Отчество сотрудника
        /// </summary>
        public string LastName { get; set; } = "unknown lastname";
        /// <summary>
        /// Должность сотрудника
        /// </summary>
        public string Position { get; set; } = "unknown position";
        /// <summary>
        /// Место работы
        /// </summary>
        public string Retail { get; set; } = "unknown retail";
        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime BirthDay { get; set; }
    }
}