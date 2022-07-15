global using CronJobExpress;
using System.Diagnostics;
using SendHappy.Extension;

namespace SendHappy.Worker;

public class SendHappyWorker : CronJobService
{

    private readonly ILogger<SendHappyWorker> _logger;
    private record Env(string? From, string[]? To, string? Subject);
    private readonly Env SendEnv;
    public SendHappyWorker(IScheduleConfig<SendHappyWorker> conf, ILoggerFactory loggerFactory)
    : base(conf.CronExpression, conf.TimeZoneInfo)
    {
        _logger = loggerFactory.CreateLogger<SendHappyWorker>();
        SendEnv = LoadEnvironment();
    }

    private Env LoadEnvironment()
    {
        string[] to = Environment.GetEnvironmentVariable("TO") != null ?
        Environment.GetEnvironmentVariable("TO").Split(',') :
        throw new Exception("not found environment `TO`");
        string subj = Environment.GetEnvironmentVariable("SUBJECT") != null ?
        Environment.GetEnvironmentVariable("SUBJECT") :
        throw new Exception("not found environment `SUBJECT`");
        string from = Environment.GetEnvironmentVariable("FROM") != null ?
        Environment.GetEnvironmentVariable("FROM") :
        throw new Exception("not found environment `FROM`");

        return new(from, to, subj);
    }

    public override async Task DoJob(CancellationToken cancellationToken)
    {
        var stopWatch = Stopwatch.StartNew();
        _logger.LogInformation($"start worker to send birthday information");
        var date = DateTime.Now.Date;

        System.Net.Http.HttpClient httpClient = new();

        System.Net.Http.HttpRequestMessage req = new()
        {
            Method = HttpMethod.Get,
            RequestUri = new($"https://api.garzdrav.ru/Employees/birthday?day={date.Day}&month={date.Month}")
        };

        try
        {

            var response = await httpClient.SendAsync(req, cancellationToken);

            if (response.IsSuccessStatusCode)
            {

                List<SendHappy.Models.EmployeesBirthDayDTO> employees = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SendHappy.Models.EmployeesBirthDayDTO>>(await response.Content.ReadAsStringAsync());

                _logger.LogInformation($"load {employees.Count} entity employees");

                System.Net.Mail.SmtpClient client = new("mail.garzdrav.ru", 25);


                System.Net.Mail.MailMessage message = new()
                {
                    IsBodyHtml = true,
                    Body = employees.getBody(),
                    Subject = SendEnv.Subject,
                    From = new System.Net.Mail.MailAddress(SendEnv.From)
                };

                foreach (var i in SendEnv.To)
                {
                    message.To.Add(i);
                }

                client.SendAsync(message, null);

                _logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    message = $"Send card is success"
                }, Newtonsoft.Json.Formatting.Indented));
            }
            else
            {
                _logger.LogError(await response.Content.ReadAsStringAsync());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                error = ex.Message,
                innerException = ex.InnerException != null ? ex.InnerException.Message : ""
            }, Newtonsoft.Json.Formatting.Indented));
        }
        stopWatch.Stop();
        _logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(new
        {
            message = $"job {nameof(SendHappyWorker)} finished",
            elapsed = stopWatch.Elapsed
        }, Newtonsoft.Json.Formatting.Indented));
    }

}