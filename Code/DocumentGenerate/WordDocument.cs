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

                    var controllerMapping = new Dictionary<string, string> {
                        { "bcc", "BCC" },
                        { "2_touch", "2 Touch" },
                        { "dsc", "DSC" },
                        { "star_delta", "Star Delta" }
                    };
                    if(controllerMapping.TryGetValue(cus.Controller, out string controller))
                        SelectValueFromDropDownSrc(wordDoc, "Cea_Controller", controller);

                    var hmiModelMapping = new Dictionary<string, string> {
                        { "t_150", "T 150" },
                        { "ta_150", "TA 150" },
                        { "t15_br", "T15 BR" },
                        { "t7_a", "T7 A" },
                        { "bcc", "BCC" },
                    };
                    if (hmiModelMapping.TryGetValue(cus.HmiModel, out string hmiModel))
                        SelectValueFromDropDownSrc(wordDoc, "Cea_HmiModel", hmiModel);

                    var HmiSwVersionMapping = new Dictionary<string, string> {
                        { "V2_02_05", "V2.02.05" },
                        { "V2_07_03", "V2.07.03" },
                        { "V2_08_06", "V2.08.06" },
                        { "V2_08_07", "V2.08.07" },
                        { "V1_00_07", "V1.00.07" },
                        { "V2_08_08", "V2.08.08" }
                    };
                    if (HmiSwVersionMapping.TryGetValue(cus.HmiSwVersion, out string hmiSwVersion))
                        SelectValueFromDropDownSrc(wordDoc, "Cea_HmiSwVersion", hmiSwVersion);

                    var CpuModelMapping = new Dictionary<string, string> {
                        { "cp_1484", "CP 1484" },
                        { "cp_1484_1", "CP 1484-1" },
                        { "cp_1584", "CP 1584" },
                    };
                    if (CpuModelMapping.TryGetValue(cus.CpuModel, out string CpuModel))
                        SelectValueFromDropDownSrc(wordDoc, "Cea_CpuModel", CpuModel);

                    var CpuSwVersionMapping = new Dictionary<string, string> {
                        { "V2_07_02", "V2_07_02" },
                        { "V2_05_13", "V2_05_13" },
                        { "V2_06_05", "V2_06_05" },
                        { "V2.06.05", "V2.06.05" },
                        { "V2.03.105", "V2.03.105" },
                        { "V2.05.08", "V2.05.08" },
                    };
                    if (CpuSwVersionMapping.TryGetValue(cus.CpuSwVersion, out string CpuSwVersion))
                        SelectValueFromDropDownSrc(wordDoc, "Cea_CpuSwVersion", CpuSwVersion);


                    //Activity
                    var ScopeOfWrokMapping = new Dictionary<string, string> {
                        { "annual_service", "Annual Service" },
                        { "half_yearly_service", "Half Yearly Service" },
                        { "condition_audit", "Condition Audit" },
                        { "troubleshooting", "TroubleShooting" },
                        { "repairs", "Repairs" },
                        { "upgrade", "Upgrade" },
                        { "control_panel_inspection", "Control Panel Inspection" },
                        { "software_upgrade", "Software Upgrade" },
                        {"others","Others" }
                    };
                    if (ScopeOfWrokMapping.TryGetValue(cus.ScopeOfWrok, out string ScopeOfWrok))
                        SelectValueFromDropDownSrc(wordDoc, "Cea_ScopeOfWrok", ScopeOfWrok);
                    wordDoc.SelectContentControlsByTitle("Cea_ScoperOfWorkOthers")[1].Range.Text = GetValueOrSpace(cus.ScoperOfWorkOthers);


                    var WorkStatusMapping = new Dictionary<string, string> {
                        { "completed", "Completed" },
                        { "incomplete", "In Complete" }
                    };
                    if (WorkStatusMapping.TryGetValue(cus.WorkStatus, out string WorkStatus))
                        SelectValueFromDropDownSrc(wordDoc, "Cea_WorkStatus", WorkStatus);

                    var DecanterStatusMapping = new Dictionary<string, string> {
                        { "in_operation", "In Operation" },
                        { "standby", "Stand By" },
                        { "breakdown", "Breakdown" }
                    };
                    if (DecanterStatusMapping.TryGetValue(cus.DecanterStatus, out string DecanterStatus))
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

                    var parameterUnits = new Dictionary<string, string>() {
                        { "SludFeed", "Sludge Feed~m3/hr" },
                        { "Polymer", "Polymer Feed~lph" },

                        { "Oper_Diff", "Operational Data~Diff: rpm" },
                        { "Oper_Bowl", "Operational Data~Bowl rpm" },
                        { "Oper_Tor", "Operational Data~Torque (NM)" },

                        { "Bt_Md", "Bearing Temp(Deg C)~Temp: MD" },
                        { "Bt_Bd", "Bearing Temp(Deg C)~Temp: BD" },

                        { "Vm_Main_Min", "Measurements(mm/sec) Main Drive~Min" },
                        { "Vm_Main_Max", "Measurements(mm/sec) Main Drive~Max" },
                        { "Vm_Back_Min", "Measurements(mm/sec) Back Drive~Min" },
                        { "Vm_Back_Max", "Measurements(mm/sec) Back Drive~Max" },

                    };

                    if (vibAna.VibrationAnalysis != null && vibAna.VibrationAnalysis.Count > 0)
                    {
                        foreach (var pu in parameterUnits)
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

                //Recommendations
                if (reportDocData.Recommendations != null)
                {
                    var recommTableIndex = 0;

                    for (int i = 1; i <= wordDoc.Tables.Count; i++)
                        if (wordDoc.Tables[i].Title == "SampleRecomm")
                        {
                            recommTableIndex = i;
                            break;
                        }


                    if (recommTableIndex > 0)
                    {
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
                        }
                    }

                    //for(int i=1;i<=5;i++)
                    //{
                    //    if(reportDocData.Recommendations.Count >= i)
                    //    {
                    //        var recomm = reportDocData.Recommendations[i-1];
                    //        InsertOrRemoveImage(wordDoc, $"Recomm_Pic_{i}", recomm.EntityRefGuid, imageHouses);
                    //        wordDoc.SelectContentControlsByTitle($"Recomm_RM_{i}")[1].Range.Text = GetValueOrSpace(recomm.Remarks);
                    //        wordDoc.SelectContentControlsByTitle($"Recomm_IA_{i}")[1].Checked = recomm.ImmediateAction;
                    //        wordDoc.SelectContentControlsByTitle($"Recomm_MTA_{i}")[1].Checked = recomm.MidTermAction;
                    //        wordDoc.SelectContentControlsByTitle($"Recomm_Obs_{i}")[1].Checked = recomm.Observation;
                    //    }
                    //    else
                    //    {
                    //        RemoveImage(wordDoc, $"Recomm_Pic_{i}");
                    //        wordDoc.SelectContentControlsByTitle($"Recomm_RM_{i}")[1].Delete(true);
                    //        wordDoc.SelectContentControlsByTitle($"Recomm_IA_{i}")[1].Delete(true);
                    //        wordDoc.SelectContentControlsByTitle($"Recomm_MTA_{i}")[1].Delete(true);
                    //        wordDoc.SelectContentControlsByTitle($"Recomm_Obs_{i}")[1].Delete(true);
                    //    }
                    //}
                }

                //Observations
                var observations = new Dictionary<string, string> {
                    {"Dec_Out_Fp_","DecanterOutlook_FrameParts_"},
                    { "Vib_Dam_","VibrationDampers"},
                    { "Flx_Conn_","Flexibleconnections"},
                    { "MainDr_BackDr_","MainDrive_BackDriveMotor"},
                    { "Sli_Gate_Ins_","SlideGateInspection"},
                    { "Gear_Box_Ins_","GearBoxInspection"},
                    { "Dec_Fra_Bed_","DecanterFrameBed_Baffle"},
                    { "Small_End_Hub_","SmallEndHub"},
                    { "Large_End_Hub_","LargeEndHub"},
                    { "Conv_Feed_Zone_","Conveyor_Feedzonewearliner"},
                    { "Conv_Flight_Tiles_","Conveyor_Flight_Tiles"},
                    { "Feed_Tube_Prot_","Feedtube_ProtectingTube"},
                    { "Cond_Wear_Strip_","Conditionofwearstrips_Bowl_"},
                    { "Main_Bear_House_","MainberingHousingseat_Large_SmallEnd"},
                    { "Conv_Seat_","Conveyorbearingseat_Large_SmallEnd"},
                    { "Conv_Axial_Play_","ConveyorAxialplay_height_gapcheck"}
                };

                if (reportDocData.Observations != null)
                {
                    foreach (var obs in observations)
                    {
                        var dbObs = reportDocData.Observations.Where(w => w.Title == obs.Value).FirstOrDefault();
                        if (dbObs != null)
                        {
                            wordDoc.SelectContentControlsByTitle($"{obs.Key}Obs")[1].Range.Text = GetValueOrSpace(dbObs.Remarks);
                            wordDoc.SelectContentControlsByTitle($"{obs.Key}Act")[1].Range.Text = GetValueOrSpace(dbObs.ActionTaken);
                            InsertOrRemoveImage(wordDoc, $"{obs.Key}Pic", dbObs.EntityRefGuid, imageHouses);
                        }
                        else
                        {
                            wordDoc.SelectContentControlsByTitle($"{obs.Key}Obs")[1].Delete(true);
                            wordDoc.SelectContentControlsByTitle($"{obs.Key}Act")[1].Delete(true);
                            RemoveImage(wordDoc, $"{obs.Key}Pic");
                        }
                    }
                }

                //Ack //misc
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
