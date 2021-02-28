using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentGenerate
{
    class Program
    {
        private static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string Env = AppSettings.Environment;
        static void Main(string[] args)
        {
            try
            {
                bool isExit = false;
                while (!isExit)
                {
                    Console.WriteLine(Env + "AlfaLaval Reports -- Document Polling Started\n");
                    RsaContext _context = new RsaContext();

                    var docToBeGenerate = _context.ReportHeaders.AsNoTracking().Where(w => w.IsDocTrigger).Select(s => s.ReportGuid).ToList();

                    if (docToBeGenerate.Any())
                    {
                        Console.WriteLine(Env + " -- Generate Document is available.");
                        _logger.Info($"Document to be generate : {docToBeGenerate.Count}");

                        for (int i = 0; i < docToBeGenerate.Count; i++)
                        {
                            try
                            {
                                new RsaReportService().GenerateWord(docToBeGenerate[i]);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Generation failed for ArrivalReportHeaderId : {docToBeGenerate[i]}, Error-{ex.ToString()}");
                                _logger.Error($"Generation failed for ArrivalReportHeaderId : {docToBeGenerate[i]}", ex);
                            }
                        }
                    }

                    Console.WriteLine(Env + " -- Document Polling Waiting mode\n");
                    Thread.Sleep(AppSettings.WaitingTimeForNextCycle);

                }

                //InspectionDbContext _context = new InspectionDbContext();

            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw ex;
            }
        }
    }
}
