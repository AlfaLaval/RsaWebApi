using Rsa.Common.Constants;
using Rsa.Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Word = Microsoft.Office.Interop.Word;

namespace DocumentGenerate
{
    class WordDocument
    {
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Generate(ReportAllDetailsDocVm reportData, string verifiedBy, List<ImageHouse> imageHouses)
        {
            if (reportData == null)
                throw new ApplicationException("Arrival Report should not be null");

            var filePath = CreateTempDoc(reportData.SafetyFirstCheck.JobOrderNumber);

            Word.Document wordDoc = null;
            Word.Application word = null;
            try
            {
                _logger.Info("Before new word app");
                word = new Word.Application();
                _logger.Info($"After new word app; visible:{word.Visible}");
                _logger.Info("Before word open");
                wordDoc = word.Documents.Open(FileName: filePath, ReadOnly: false);
                //Thread.Sleep(2000); // waiting to open word
                _logger.Info("After word open");
            }
            catch (Exception ex)
            {
                _logger.Error("Error opening word", ex);
                try
                {
                    _logger.Error("Re-try opening word");
                    if (word != null)
                    {
                        _logger.Info("Inside app open");
                        word = new Word.Application();

                    }
                    if (wordDoc != null)
                    {
                        _logger.Info("Inside word open");
                        wordDoc = word.Documents.Open(FileName: filePath, ReadOnly: false, Visible: false);
                    }
                    Thread.Sleep(5000); // waiting to open word
                    _logger.Info("Re-try opening word - success");
                }
                catch (Exception ex2)
                {
                    if (wordDoc != null)
                    {
                        _logger.Info("Word CLose1");
                        wordDoc.Close();
                    }
                    if (word != null)
                    {
                        _logger.Info("Word Quit1");
                        word.Quit();
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(word);
                    }
                    throw ex;
                }

            }

            try
            {
                _logger.Info($"Fill Word Data -  Started");

                FillRepotData(wordDoc, reportData,verifiedBy, imageHouses);

                _logger.Info($"Fill Word Data -  Completed");

                return filePath;
            }
            catch (Exception ex)
            {
                _logger.Error("Error in word generation", ex);
                throw ex;
            }
            finally
            {
                if (wordDoc != null)
                {
                    _logger.Info("Word CLose2");
                    wordDoc.Close();
                }
                if (word != null)
                {
                    _logger.Info("Word Quit2");
                    word.Quit();
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(word);
                }
            }

        }

        private string CreateTempDoc(string jobNumber)
        {
            //string tempFileName = $"{AppSettings.DocTempPath}{jobNumber}_{Guid.NewGuid().ToString()}.docx";
            string tempDir = $"{AppSettings.DocTempPath}{Guid.NewGuid().ToString()}";
            System.IO.Directory.CreateDirectory(tempDir);
            string tempFileName = $"{tempDir}\\DecanterReport_{jobNumber}.docx";
            System.IO.File.Copy(AppSettings.GoldenTemplate, tempFileName);

            return tempFileName;
        }

        private string GetValueOrSpace(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return " ";
            else
                return data;
        }
        public void FillRepotData(Word.Document wordDoc, ReportAllDetailsDocVm reportDocData,string verifiedBy, List<ImageHouse> imageHouses)
        {
            try
            {
                var sfc = reportDocData.SafetyFirstCheck;
                var cus = reportDocData.CustomerEquipmentActivity;
                
                if (sfc != null && cus != null)
                {

                    wordDoc.SelectContentControlsByTitle("sfc_Customer")[1].Range.Text = GetValueOrSpace(sfc.ProjectName);
                    wordDoc.SelectContentControlsByTitle("sfc_ServiceEngineer")[1].Range.Text = GetValueOrSpace(sfc.EngineerName);
                    wordDoc.SelectContentControlsByTitle("sfc_Job_No")[1].Range.Text = GetValueOrSpace(sfc.JobOrderNumber);
                    wordDoc.SelectContentControlsByTitle("sfc_startdate")[1].Range.Text = sfc.StartDate.ToShortDateString();
                    wordDoc.SelectContentControlsByTitle("sfc_contactno")[1].Range.Text = GetValueOrSpace(sfc.ContactNumber);
                    wordDoc.SelectContentControlsByTitle("sfc_sitesafetycontact")[1].Range.Text = GetValueOrSpace(sfc.SiteSafetyContact);
                    var sfcDetails = sfc.SafetyFirstCheckDetails;
                    if (sfcDetails != null)
                    {
                        var sfcd1 = sfcDetails.FirstOrDefault(f => f.CheckPointName == "STOP-THINK-ACT");
                        if (sfcd1 != null)
                        {
                            wordDoc.SelectContentControlsByTitle($"sfc_stopthinkact")[1].Checked = sfcd1.IsSelected;
                        }
                        wordDoc.SelectContentControlsByTitle($"sfc_stopthinkact_rm")[1].Range.Text = GetValueOrSpace(sfcd1?.Remarks);
                        var sfcd2 = sfcDetails.FirstOrDefault(f => f.CheckPointName == "Permit to Work(PTW)");
                        if (sfcd2 != null)
                        {
                            wordDoc.SelectContentControlsByTitle($"sfc_ptw")[1].Checked = sfcd2.IsSelected;
                        }
                        wordDoc.SelectContentControlsByTitle($"sfc_ptw_rm")[1].Range.Text = GetValueOrSpace(sfcd2.Remarks);
                        var sfcd3 = sfcDetails.FirstOrDefault(f => f.CheckPointName == "Fitness of Personnel");
                        if (sfcd3 != null)
                        {
                            wordDoc.SelectContentControlsByTitle($"sfc_fitness")[1].Checked = sfcd3.IsSelected;
                        }
                        wordDoc.SelectContentControlsByTitle($"sfc_fitness_rm")[1].Range.Text = GetValueOrSpace(sfcd3.Remarks);
                        var sfcd4 = sfcDetails.FirstOrDefault(f => f.CheckPointName == "Work Area Evaluation");
                        if (sfcd4 != null)
                        {
                            wordDoc.SelectContentControlsByTitle($"sfc_workareaeval")[1].Checked = sfcd4.IsSelected;
                        }
                        wordDoc.SelectContentControlsByTitle($"sfc_workareaeval_rm")[1].Range.Text = GetValueOrSpace(sfcd4.Remarks);
                        var sfcd5 = sfcDetails.FirstOrDefault(f => f.CheckPointName == "Evacuation Plan");
                        if (sfcd5 != null)
                        {
                            wordDoc.SelectContentControlsByTitle($"sfc_evalplan")[1].Checked = sfcd5.IsSelected;
                        }
                        wordDoc.SelectContentControlsByTitle($"sfc_evalplan_rm")[1].Range.Text = GetValueOrSpace(sfcd5.Remarks);
                        var sfcd6 = sfcDetails.FirstOrDefault(f => f.CheckPointName == "Method Statement Review");
                        if (sfcd6 != null)
                        {
                            wordDoc.SelectContentControlsByTitle($"sfc_methstmtrev")[1].Checked = sfcd6.IsSelected;
                        }
                        wordDoc.SelectContentControlsByTitle($"sfc_methstmtrev_rm")[1].Range.Text = GetValueOrSpace(sfcd6.Remarks);
                        var sfcd7 = sfcDetails.FirstOrDefault(f => f.CheckPointName == "Risk Assessment Review");
                        if (sfcd7 != null)
                        {
                            wordDoc.SelectContentControlsByTitle($"sfc_riskassessrev")[1].Checked = sfcd7.IsSelected;
                        }
                        wordDoc.SelectContentControlsByTitle($"sfc_riskassessrev_rm")[1].Range.Text = GetValueOrSpace(sfcd7.Remarks);
                        var sfcd8 = sfcDetails.FirstOrDefault(f => f.CheckPointName == "Mandatory PPE");
                        if (sfcd8 != null)
                        {
                            wordDoc.SelectContentControlsByTitle($"sfc_madatoryppe")[1].Checked = sfcd8.IsSelected;
                        }
                        wordDoc.SelectContentControlsByTitle($"sfc_madatoryppe_rm")[1].Range.Text = GetValueOrSpace(sfcd8.Remarks);
                        var sfcd9 = sfcDetails.FirstOrDefault(f => f.CheckPointName == "Condition of tools/gears");
                        if (sfcd9 != null)
                        {
                            wordDoc.SelectContentControlsByTitle($"sfc_cond_tools")[1].Checked = sfcd9.IsSelected;
                        }
                        wordDoc.SelectContentControlsByTitle($"sfc_cond_tools_rm")[1].Range.Text = GetValueOrSpace(sfcd9.Remarks);
                        var sfcd10 = sfcDetails.FirstOrDefault(f => f.CheckPointName == "First Aid");
                        if (sfcd10 != null)
                        {
                            wordDoc.SelectContentControlsByTitle($"sfc_firstaid")[1].Checked = sfcd10.IsSelected;
                        }
                        wordDoc.SelectContentControlsByTitle($"sfc_firstaid_rm")[1].Range.Text = GetValueOrSpace(sfcd10.Remarks);

                    }
                    wordDoc.SelectContentControlsByTitle("Job_No_Header")[1].Range.Text = GetValueOrSpace(sfc.JobOrderNumber);
                    //wordDoc.SelectContentControlsByTitle("Sub_No_Header")[1].Range.Text = GetValueOrSpace(sfc.JobOrderNumber);

                    wordDoc.SelectContentControlsByTitle("Cea_Customer")[1].Range.Text = GetValueOrSpace(sfc.ProjectName);
                    wordDoc.SelectContentControlsByTitle("Cea_ServiceEngineer")[1].Range.Text = GetValueOrSpace(sfc.EngineerName);
                    wordDoc.SelectContentControlsByTitle("Cea_ReportNumber")[1].Range.Text = GetValueOrSpace(sfc.JobOrderNumber);
                    wordDoc.SelectContentControlsByTitle("Cea_PreviousServiceDate")[1].Range.Text = cus.PreviousServiceDate.ToShortDateString();
                    wordDoc.SelectContentControlsByTitle("Cea_CurrentServiceDate")[1].Range.Text = cus.CurrentServiceDate.ToShortDateString();
                    wordDoc.SelectContentControlsByTitle("Cea_ReportDate")[1].Range.Text = cus.ReportDate.ToShortDateString();
                    wordDoc.SelectContentControlsByTitle("Cea_SiteLocation")[1].Range.Text = GetValueOrSpace(cus.SiteLocation);

                    var equipmentMapping = new Dictionary<string, string> { { "decanter", "Decanter" } };
                    if (equipmentMapping.TryGetValue(cus.Equipment, out string Equipment))
                        SelectValueFromDropDownSrc(wordDoc, "Cea_Equipment", Equipment);

                    //equipment details
                    wordDoc.SelectContentControlsByTitle("Cea_DecanterModel")[1].Range.Text = GetValueOrSpace(cus.DecanterModel);
                    wordDoc.SelectContentControlsByTitle("Cea_DecanterSerialNumber")[1].Range.Text = GetValueOrSpace(cus.DecanterSerialNumber);
                    wordDoc.SelectContentControlsByTitle("Cea_BowlSerialNumber")[1].Range.Text = GetValueOrSpace(cus.BowlSerialNumber);
                    wordDoc.SelectContentControlsByTitle("Cea_CustomerReference")[1].Range.Text = GetValueOrSpace(cus.CustomerReference);
                    wordDoc.SelectContentControlsByTitle("Cea_RunningHours")[1].Range.Text = cus.RunningHours.ToString();

                    wordDoc.SelectContentControlsByTitle("Cea_Controller")[1].Range.Text = GetValueOrSpace(cus.Controller);
                    wordDoc.SelectContentControlsByTitle("Cea_HmiModel")[1].Range.Text = GetValueOrSpace(cus.HmiModel);
                    wordDoc.SelectContentControlsByTitle("Cea_HmiSwVersion")[1].Range.Text = GetValueOrSpace(cus.HmiSwVersion);
                    wordDoc.SelectContentControlsByTitle("Cea_CpuModel")[1].Range.Text = GetValueOrSpace(cus.CpuModel);
                    wordDoc.SelectContentControlsByTitle("Cea_CpuSwVersion")[1].Range.Text = GetValueOrSpace(cus.CpuSwVersion);

                    //if(KeyValue.controllerMapping.TryGetValue(cus.Controller, out string controller))
                    //    SelectValueFromDropDownSrc(wordDoc, "Cea_Controller", controller);

                    //if (KeyValue.hmiModelMapping.TryGetValue(cus.HmiModel, out string hmiModel))
                    //    SelectValueFromDropDownSrc(wordDoc, "Cea_HmiModel", hmiModel);

                    //if (KeyValue.HmiSwVersionMapping.TryGetValue(cus.HmiSwVersion, out string hmiSwVersion))
                    //    SelectValueFromDropDownSrc(wordDoc, "Cea_HmiSwVersion", hmiSwVersion);

                    //if (KeyValue.CpuModelMapping.TryGetValue(cus.CpuModel, out string CpuModel))
                    //    SelectValueFromDropDownSrc(wordDoc, "Cea_CpuModel", CpuModel);

                    //if (KeyValue.CpuSwVersionMapping.TryGetValue(cus.CpuSwVersion, out string CpuSwVersion))
                    //    SelectValueFromDropDownSrc(wordDoc, "Cea_CpuSwVersion", CpuSwVersion);

                    //Activity
                    if (KeyValue.ScopeOfWrokMapping.TryGetValue(cus.ScopeOfWrok, out string ScopeOfWrok))
                        SelectValueFromDropDownSrc(wordDoc, "Cea_ScopeOfWrok", ScopeOfWrok);
                    wordDoc.SelectContentControlsByTitle("Cea_ScoperOfWorkOthers")[1].Range.Text = GetValueOrSpace(cus.ScoperOfWorkOthers);

                    if (KeyValue.WorkStatusMapping.TryGetValue(cus.WorkStatus, out string WorkStatus))
                        SelectValueFromDropDownSrc(wordDoc, "Cea_WorkStatus", WorkStatus);

                    if (KeyValue.DecanterStatusMapping.TryGetValue(cus.DecanterStatus, out string DecanterStatus))
                        SelectValueFromDropDownSrc(wordDoc, "Cea_DecanterStatus", DecanterStatus);
                }

                //Vibration Analysis Report
                var vibAna = reportDocData.VibrationAnalysisHeader;
                if(vibAna!=null)
                {
                    wordDoc.SelectContentControlsByTitle($"Vah_BsDryRunActive")[1].Checked = vibAna.BsDryRunActive;
                    wordDoc.SelectContentControlsByTitle($"Vah_BsProduction")[1].Checked = vibAna.BsProduction;
                    wordDoc.SelectContentControlsByTitle($"Vah_AsDryRun")[1].Checked = vibAna.AsDryRun;
                    wordDoc.SelectContentControlsByTitle($"Vah_AsWaterTest")[1].Checked = vibAna.AsWaterTest;
                    wordDoc.SelectContentControlsByTitle($"Vah_AsProduction")[1].Checked = vibAna.AsProduction;
                   
                    if (vibAna.VibrationAnalysis != null && vibAna.VibrationAnalysis.Count > 0)
                    {
                        foreach (var pu in KeyValue.parameterUnits)
                        {
                            var param = vibAna.VibrationAnalysis?.Where(w => $"{w.Parameter}~{w.Units}" == pu.Value).FirstOrDefault();
                            if (param != null)
                            {
                                wordDoc.SelectContentControlsByTitle($"{pu.Key}_Bs_DryRun")[1].Range.Text = GetValueOrSpace(param.BsDryRun);
                                wordDoc.SelectContentControlsByTitle($"{pu.Key}_Bs_Prod")[1].Range.Text = GetValueOrSpace(param.BsProduction);
                                wordDoc.SelectContentControlsByTitle($"{pu.Key}_As_DryRun")[1].Range.Text = GetValueOrSpace(param.AsDryRun);
                                wordDoc.SelectContentControlsByTitle($"{pu.Key}_As_Water")[1].Range.Text = GetValueOrSpace(param.AsWaterTest);
                                wordDoc.SelectContentControlsByTitle($"{pu.Key}_As_Prod")[1].Range.Text = GetValueOrSpace(param.AsProduction);
                            }
                            else
                            {
                                wordDoc.SelectContentControlsByTitle($"{pu.Key}_Bs_DryRun")[1].Delete(true);
                                wordDoc.SelectContentControlsByTitle($"{pu.Key}_Bs_Prod")[1].Delete(true);
                                wordDoc.SelectContentControlsByTitle($"{pu.Key}_As_DryRun")[1].Delete(true);
                                wordDoc.SelectContentControlsByTitle($"{pu.Key}_As_Water")[1].Delete(true);
                                wordDoc.SelectContentControlsByTitle($"{pu.Key}_As_Prod")[1].Delete(true);
                            }
                        }
                    }

                    wordDoc.SelectContentControlsByTitle($"Md_Check")[1].Checked = vibAna.MdMotor;
                    wordDoc.SelectContentControlsByTitle($"Bd_Check")[1].Checked = vibAna.BdMotor;

                    wordDoc.SelectContentControlsByTitle($"Md_DE_Main")[1].Range.Text = GetValueOrSpace(vibAna.MdDriveEndMain);
                    wordDoc.SelectContentControlsByTitle($"Md_NDE_Main")[1].Range.Text = GetValueOrSpace(vibAna.MdNonDriveEndMain);
                    wordDoc.SelectContentControlsByTitle($"Md_DE_Back")[1].Range.Text = GetValueOrSpace(vibAna.MdDriveEndBack);
                    wordDoc.SelectContentControlsByTitle($"Md_NDE_Back")[1].Range.Text = GetValueOrSpace(vibAna.MdNonDriveEndBack);
                    wordDoc.SelectContentControlsByTitle($"Bd_DE_Main")[1].Range.Text = GetValueOrSpace(vibAna.BdDriveEndMain);
                    wordDoc.SelectContentControlsByTitle($"Bd_NDE_Main")[1].Range.Text = GetValueOrSpace(vibAna.BdNonDriveEndMain);
                    wordDoc.SelectContentControlsByTitle($"Bd_DE_Back")[1].Range.Text = GetValueOrSpace(vibAna.BdDriveEndBack);
                    wordDoc.SelectContentControlsByTitle($"Bd_NDE_Back")[1].Range.Text = GetValueOrSpace(vibAna.BdNonDriveEndBack);
                    wordDoc.SelectContentControlsByTitle("Vah_Remarks")[1].Range.Text = GetValueOrSpace(vibAna.Remarks);
                    if (!string.IsNullOrWhiteSpace(vibAna.Remarks))
                    {
                        wordDoc.SelectContentControlsByTitle("Vah_Remarks")[1].Range.Font.Color = Word.WdColor.wdColorRed;
                    }
                }
                
                bool pageBreakedRecomm = false;
                bool pageBreakedObs = false;
                bool pageBreakedSpareParts = false;


                #region Recommendations
                var recommTableIndex = 0;

                for (int i = 1; i <= wordDoc.Tables.Count; i++)
                    if (wordDoc.Tables[i].Title == "RecommHead")
                    {
                        recommTableIndex = i;
                        break;
                    }
                if (recommTableIndex > 0 && reportDocData.Recommendations != null && reportDocData.Recommendations.Count > 0)
                {
                    wordDoc.Tables[recommTableIndex].Range.InsertBreak();
                    pageBreakedRecomm = true;
                    int sno = 1;
                    foreach (var recomm in reportDocData.Recommendations)
                    {
                        var row = wordDoc.Tables[recommTableIndex].Rows.Add();
                        row.Height = 30.0f; //1.2 cm
                        row.HeightRule = Word.WdRowHeightRule.wdRowHeightAtLeast;
                        row.Shading.BackgroundPatternColor = Word.WdColor.wdColorWhite;

                        row.Cells[1].Range.Text = $"0{sno}";
                        row.Cells[1].Range.Underline = Word.WdUnderline.wdUnderlineNone;
                        row.Cells[1].Range.Font.Bold = 0;

                        row.Cells[2].Range.Text = GetValueOrSpace(recomm.Remarks);
                        row.Cells[2].Range.Underline = Word.WdUnderline.wdUnderlineNone;
                        row.Cells[2].Range.Font.Bold = 0;
                        row.Cells[2].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;

                        row.Cells[3].Range.ContentControls.Add(Word.WdContentControlType.wdContentControlCheckBox).Checked = recomm.ImmediateAction;
                        row.Cells[4].Range.ContentControls.Add(Word.WdContentControlType.wdContentControlCheckBox).Checked = recomm.MidTermAction;
                        row.Cells[5].Range.ContentControls.Add(Word.WdContentControlType.wdContentControlCheckBox).Checked = recomm.Observation;
                        sno++;
                        //InsertOrRemoveImage(wordDoc, $"Recomm_Pic_{i}", recomm.EntityRefGuid, imageHouses);
                    }
                }
                else
                {
                    wordDoc.SelectContentControlsByTitle("RecommHead")[1].Delete(true);
                    if (recommTableIndex > 0)
                        wordDoc.Tables[recommTableIndex].Delete();
                } 
                #endregion

                #region Observations
                var obserTableIndex = 0;
                for (int i = 1; i <= wordDoc.Tables.Count; i++)
                    if (wordDoc.Tables[i].Title == "ObsHead")
                    {
                        obserTableIndex = i;
                        break;
                    }

                if (obserTableIndex > 0 && reportDocData.Observations != null 
                    && reportDocData.Observations.Count > 0)
                {
                    if(pageBreakedRecomm)
                        wordDoc.Tables[obserTableIndex].Range.InsertBreak();

                    pageBreakedObs = true;
                    int iteration = 0;
                    foreach (var obs in reportDocData.Observations)
                    {
                        //if (!KeyValue.observation.ContainsKey(obs.Title))
                        //    continue;

                        Word.Row row1 = null;
                        Word.Row row2 = null;
                        if (iteration > 0)
                        {
                            row1 = wordDoc.Tables[obserTableIndex].Rows.Add();
                            row2 = wordDoc.Tables[obserTableIndex].Rows.Add();
                        }
                        else
                        {
                            row1 = wordDoc.Tables[obserTableIndex].Rows[1];
                            row2 = wordDoc.Tables[obserTableIndex].Rows.Add();
                        }
                        row1.Height = 30.0f;
                        row1.HeightRule = Word.WdRowHeightRule.wdRowHeightAtLeast;

                        row2.Height = 30.0f;
                        row2.HeightRule = Word.WdRowHeightRule.wdRowHeightAtLeast;

                        row1.Borders[Word.WdBorderType.wdBorderTop].LineStyle = Word.WdLineStyle.wdLineStyleDouble;
                        row2.Borders[Word.WdBorderType.wdBorderBottom].LineStyle = Word.WdLineStyle.wdLineStyleDouble;

                        //row1
                        row1.Cells[1].Range.Text = obs.Title;

                        string remark = $"Observations:";
                        if (!string.IsNullOrWhiteSpace(obs.Remarks))
                        {
                            remark = $"Observations:\n{obs.Remarks}";
                        }
                        row1.Cells[2].Range.Text = remark;
                        row1.Cells[2].Range.Font.Bold = 0;
                        row1.Cells[2].Range.ListFormat.ApplyBulletDefault();

                        //making Observations: as Bold & removing bullet
                        object objStart = row1.Cells[2].Range.Start;
                        object objEnd = row1.Cells[2].Range.Start + remark.IndexOf(":");
                        Word.Range rngBold = wordDoc.Range(ref objStart, ref objEnd);
                        rngBold.Bold = 1;
                        rngBold.ListFormat.RemoveNumbers();

                        //row2
                        string imagePath = GetImagePath(obs.EntityRefGuid, imageHouses);

                        if (!string.IsNullOrWhiteSpace(imagePath) && System.IO.File.Exists(imagePath))
                        {
                            row2.Height = 120.0f;
                            row2.HeightRule = Word.WdRowHeightRule.wdRowHeightExactly;

                            var area = row2.Cells[1].Range.ContentControls
                                .Add(Word.WdContentControlType.wdContentControlPicture).Range
                                .InlineShapes.AddPicture(imagePath);
                        }
                        string actionTaken = $"Action Taken:";
                        if (!string.IsNullOrWhiteSpace(obs.ActionTaken))
                        {
                            actionTaken = $"Action Taken: {obs.ActionTaken}";
                        }
                        
                        row2.Cells[2].Range.Text = actionTaken;
                        row2.Cells[2].Range.Font.Bold = 0;
                        row2.Cells[2].Range.ListFormat.ApplyBulletDefault();

                        //making Action Taken: as Bold & removing bullet
                        object objStart1 = row2.Cells[2].Range.Start;
                        object objEnd1 = row2.Cells[2].Range.Start + actionTaken.IndexOf(":");
                        Word.Range rngBold1 = wordDoc.Range(ref objStart1, ref objEnd1);
                        rngBold1.Bold = 1;
                        rngBold1.ListFormat.RemoveNumbers();
                        iteration++;
                    }
                }
                else
                {
                    wordDoc.SelectContentControlsByTitle("ObsHead")[1].Delete(true);
                    if (obserTableIndex > 0)
                        wordDoc.Tables[obserTableIndex].Delete();
                }
                #endregion

                #region Spare Parts
                var sparePartTypes = new string[] { "USED", "RECOM" };
                int sparePartIter = 0;
                foreach (var sparePart in sparePartTypes)
                {
                    var sparePartsTableIndex = 0;

                    for (int i = 1; i <= wordDoc.Tables.Count; i++)
                        if (wordDoc.Tables[i].Title == sparePart)
                        {
                            sparePartsTableIndex = i;
                            break;
                        }
                    if (sparePartsTableIndex > 0 && reportDocData.SpareParts != null && reportDocData.SpareParts.Count > 0)
                    {
                        if (sparePartIter == 0)
                        {
                            wordDoc.Tables[sparePartsTableIndex].Range.InsertBreak();
                            pageBreakedSpareParts = true;
                            sparePartIter++;
                        }

                        int sno = 1;
                        foreach (var sp in reportDocData.SpareParts.Where(w => sparePart.Equals(w.Type, StringComparison.OrdinalIgnoreCase)))
                        {
                            var row = wordDoc.Tables[sparePartsTableIndex].Rows.Add();
                            row.Height = 25.0f; //1.0 cm
                            row.HeightRule = Word.WdRowHeightRule.wdRowHeightAtLeast;
                            row.Shading.BackgroundPatternColor = Word.WdColor.wdColorWhite;

                            row.Cells[1].Range.Text = $"{sno}";
                            row.Cells[1].Range.Underline = Word.WdUnderline.wdUnderlineNone;
                            row.Cells[1].Range.Font.Bold = 0;

                            row.Cells[2].Range.Text = GetValueOrSpace(sp.Description);
                            row.Cells[2].Range.Underline = Word.WdUnderline.wdUnderlineNone;
                            row.Cells[2].Range.Font.Bold = 0;
                            row.Cells[2].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;

                            row.Cells[3].Range.Text = GetValueOrSpace(sp.PartNo);
                            row.Cells[3].Range.Underline = Word.WdUnderline.wdUnderlineNone;
                            row.Cells[3].Range.Font.Bold = 0;
                            row.Cells[3].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;

                            row.Cells[4].Range.Text = sp.Quantity.ToString();
                            row.Cells[4].Range.Underline = Word.WdUnderline.wdUnderlineNone;
                            row.Cells[4].Range.Font.Bold = 0;
                            row.Cells[4].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            sno++;
                        }
                    }
                    else
                    {
                        //wordDoc.SelectContentControlsByTitle(sparePart)[1].Delete(true);
                        if (sparePartsTableIndex > 0)
                            wordDoc.Tables[sparePartsTableIndex].Delete();
                    }
                }
               
                #endregion

                //Ack //misc --SignCommSection
                var commTableIndex = 0;
                for (int i = 1; i <= wordDoc.Tables.Count; i++)
                    if (wordDoc.Tables[i].Title == "SignCommSection")
                    {
                        commTableIndex = i;
                        break;
                    }
                if (pageBreakedObs || pageBreakedRecomm || pageBreakedSpareParts)
                    wordDoc.Tables[commTableIndex].Range.InsertBreak();
                
                var miscData = reportDocData.Misc;
                if (miscData != null)
                {
                    wordDoc.SelectContentControlsByTitle($"misc_firmcomm")[1].Range.Text = GetValueOrSpace(miscData.FirmComments);
                    wordDoc.SelectContentControlsByTitle($"misc_custcomm")[1].Range.Text = GetValueOrSpace(miscData.CustomerComments);
                    wordDoc.SelectContentControlsByTitle($"Alfa_Ack_Name")[1].Range.Text = GetValueOrSpace(miscData.FirmName);
                    wordDoc.SelectContentControlsByTitle($"Alfa_Ack_Date")[1].Range.Text = miscData.FirmDate.ToShortDateString();
                    wordDoc.SelectContentControlsByTitle($"Cust_Ack_Name")[1].Range.Text = GetValueOrSpace(miscData.CustomerName);
                    wordDoc.SelectContentControlsByTitle($"Cust_Ack_Date")[1].Range.Text = miscData.CustomerDate.ToShortDateString(); ;
                }
                var firmSignature = imageHouses.FirstOrDefault(w => w.ImageLabel == StringConstants.FirmSignature );
                if (firmSignature != null)
                    InsertOrRemoveImage(wordDoc, "firm_sign", firmSignature.EntityRefGuid, imageHouses);
                var custSignature = imageHouses.FirstOrDefault(w => w.ImageLabel == StringConstants.CustomerSignature);
                if (custSignature != null)
                    InsertOrRemoveImage(wordDoc, "cust_sign", custSignature.EntityRefGuid, imageHouses);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetImagePath(Guid entityRefId, List<ImageHouse> imageHouses)
        {
            var img = imageHouses.Where(w => w.EntityRefGuid == entityRefId).FirstOrDefault();
            if (img != null)
            {
                var ext = img.Entity == "signature" ? "png" : "jpeg";
                return $"{AppSettings.ImageUploadPath}{img.ImageFileGuid}.{ext}";
            }
            return string.Empty;
        }

        private void InsertOrRemoveImage(Word.Document wordDoc, string title,Guid entityRefId, List<ImageHouse> imageHouses)
        {
            var img = imageHouses.Where(w => w.EntityRefGuid == entityRefId).FirstOrDefault();
            if (img != null)
            {
                var ext = img.Entity == "signature" ? "png" : "jpeg";
                string imagePath = $"{AppSettings.ImageUploadPath}{img.ImageFileGuid}.{ext}";
                try
                {
                    if (!string.IsNullOrWhiteSpace(imagePath))
                    {
                        Word.Range rngPic = wordDoc.SelectContentControlsByTag(title)[1].Range;
                        rngPic.InlineShapes.AddPicture(imagePath);
                    }
                    else
                    {
                        RemoveImage(wordDoc,title);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error in Insert Image", ex);
                }
            }
            else
            {
                RemoveImage(wordDoc, title);
            }
        }

        private void RemoveImage(Word.Document wordDoc, string title)
        {
            try
            {
                var rngPic = wordDoc.SelectContentControlsByTag(title)[1];
                rngPic.Delete(true);
            }
            catch (Exception ex)
            {
                _logger.Error("Error in Remove Image", ex);
            }
        }


        private void SelectValueFromDropDownSrc(Word.Document wordDoc,string title, string selectedValue)
        {
            var allDropDown = wordDoc.SelectContentControlsByTitle(title)[1].DropdownListEntries;

            for (int i = 1; i <= allDropDown.Count; i++)
            {
                if (allDropDown[i].Value == selectedValue)
                {
                    wordDoc.SelectContentControlsByTitle(title)[1].DropdownListEntries[i].Select();
                    break;
                }
            }
        }

    }
}
