using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMon.Values.ExportImport.Core.FileFormatParameters;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Processing.Parse;

namespace RMon.ValuesExportImportService.Tests.Parse80020
{
    [TestClass]
    public class UnitTest
    {

        Xml80020ParsingParameters TaskParamsCreate() =>
            new()
            {
                MeasuringPoint = new Xml80020PointParameters("ArgNo")
                {
                    Channels = new List<Xml80020ChannelParameters>
                    {
                        new("01", "dHHA+"),
                        new("02", "dHHA-"),
                        new("03", "dHHR+"),
                    }
                },
                DeliveryPoint = new Xml80020PointParameters("wa")
                {
                    Channels = new List<Xml80020ChannelParameters>
                    {
                        new("01", "HA+"),
                    }
                }
            };


        [TestMethod]
        public async Task TestMethod1()
        {
            var logic = new ParseXml80020Logic(new DataRepositoryStub());

            var fileName = @"Parse80020\Files\80020_7713076301_20190101_6599155.xml";
            var fileBody = await File.ReadAllBytesAsync(fileName).ConfigureAwait(false);

            var taskParams = TaskParamsCreate();
            var idUser = 51;

            var taskLogger = new ParseTaskLoggerStub();
            var context = new ParseProcessingContext(null, null, taskLogger, idUser);
            try
            {
                var values = await logic.AnalyzeAsync(new List<LocalFile> {new(fileName, fileBody)}, taskParams, context, CancellationToken.None).ConfigureAwait(false);
                Assert.AreEqual(310, values.Count, "���������� ���� ���������� �������� �������.");
                //var aa = values.Count(t => t.TimeStamp.Year == 2021 && t.TimeStamp.Month == 03 && t.TimeStamp.Day == 21);
                Assert.AreEqual(303, values.Count(t => t.TimeStamp.Year == 2021 && t.TimeStamp.Month == 03 && t.TimeStamp.Day == 20), "��������� ������ �� �������� ��������� �� �����.");
                Assert.AreEqual(7, values.Count(t => t.TimeStamp.Year == 2021 && t.TimeStamp.Month == 03 && t.TimeStamp.Day == 21), "��������� ������ �� �������� �� ��������� ����� ��������� �� �����.");

                var normalValues = values.Where(t => t.IdTag == 1).ToList();
                Assert.AreEqual(96, normalValues.Count, "���������� �������� ��� ���� dHHA+ �������.");
                Assert.IsTrue(normalValues.All(t => t.Value.ValueFloat == 1 || t.Value.ValueFloat == 2), "�������� ������ �� �������� ��� ���� dHHA+ �� ���������.");


                /*� ���� �������� ��������� ���� - �����������, � ��������� �������� - �������*/
                var hourValues = values.Where(t => t.IdTag == 3).ToList();
                Assert.AreEqual(96, hourValues.Count, "���������� ������� �������� ��� ������������ ���� �������.");
                Assert.IsTrue(hourValues.All(t => t.Value.ValueFloat == 0.5 || t.Value.ValueFloat == 1.5), "�������� ������ �� ������� �������� ��� ������������ ���� �� ���������.");

                /*� ���� �������� ��������� ���� - �������, � ��������� �������� - �����������*/
                var halfHourValues = values.Where(t => t.IdTag == 4).ToList();
                Assert.AreEqual(22, halfHourValues.Count, "���������� ����������� �������� ��� �������� ���� �������.");
                Assert.IsTrue(halfHourValues.All(t => t.Value.ValueFloat == 3), "�������� ������ �� ����������� �������� ��� �������� ���� �� ���������.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
