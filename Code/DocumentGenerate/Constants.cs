using System.Collections.Generic;

namespace DocumentGenerate
{
    class Constants
    {
    }

    public static class KeyValue
    {
        public static IDictionary<string, string> observation = new Dictionary<string, string> {
                    { "DecanterOutlook_FrameParts_","Decanter Outlook (Frame Parts)"},
                    { "VibrationDampers","Vibration Dampers"},
                    { "Flexibleconnections","Flexible connections"},
                    { "MainDrive_BackDriveMotor","Main Drive/Back Drive Motor"},
                    { "SlideGateInspection","Slide Gate Inspection"},
                    { "GearBoxInspection","Gear Box Inspection"},
                    { "DecanterFrameBed_Baffle","Decanter Frame Bed / Baffle"},
                    { "SmallEndHub","Small End Hub"},
                    { "LargeEndHub","Large End Hub"},
                    { "Conveyor_Feedzonewearliner","Conveyor - Feed zone wear liner"},
                    { "Conveyor_Flight_Tiles","Conveyor - Flight / Tiles"},
                    { "Feedtube_ProtectingTube","Feed tube / Protecting Tube"},
                    { "Conditionofwearstrips_Bowl_","Condition of wear strips (Bowl)"},
                    { "MainberingHousingseat_Large_SmallEnd","Main bering Housing seat - Large / Small End"},
                    { "Conveyorbearingseat_Large_SmallEnd","Conveyor bearing seat - Large /Small End"},
                    { "ConveyorAxialplay_height_gapcheck","Conveyor Axial play - height / gap check"}
                };

        public static IDictionary<string, string> parameterUnits = new Dictionary<string, string>() {
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

        public static IDictionary<string, string> ScopeOfWrokMapping = new Dictionary<string, string> {
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
        public static IDictionary<string, string> CpuSwVersionMapping = new Dictionary<string, string> {
                        { "V2_07_02", "V2_07_02" },
                        { "V2_05_13", "V2_05_13" },
                        { "V2_06_05", "V2_06_05" },
                        { "V2.06.05", "V2.06.05" },
                        { "V2.03.105", "V2.03.105" },
                        { "V2.05.08", "V2.05.08" },
                    };
        public static IDictionary<string, string> CpuModelMapping = new Dictionary<string, string> {
                        { "cp_1484", "CP 1484" },
                        { "cp_1484_1", "CP 1484-1" },
                        { "cp_1584", "CP 1584" },
                    };
        public static IDictionary<string, string> hmiModelMapping = new Dictionary<string, string> {
                        { "t_150", "T 150" },
                        { "ta_150", "TA 150" },
                        { "t15_br", "T15 BR" },
                        { "t7_a", "T7 A" },
                        { "bcc", "BCC" },
                    };
        public static IDictionary<string, string> controllerMapping = new Dictionary<string, string> {
                        { "bcc", "BCC" },
                        { "2_touch", "2 Touch" },
                        { "dsc", "DSC" },
                        { "star_delta", "Star Delta" }
                    };
        public static IDictionary<string, string> HmiSwVersionMapping = new Dictionary<string, string> {
                        { "V2_02_05", "V2.02.05" },
                        { "V2_07_03", "V2.07.03" },
                        { "V2_08_06", "V2.08.06" },
                        { "V2_08_07", "V2.08.07" },
                        { "V1_00_07", "V1.00.07" },
                        { "V2_08_08", "V2.08.08" }
                    };
        public static IDictionary<string, string> DecanterStatusMapping = new Dictionary<string, string> {
                        { "in_operation", "In Operation" },
                        { "standby", "Stand By" },
                        { "breakdown", "Breakdown" }
                    };
        public static IDictionary<string, string> WorkStatusMapping = new Dictionary<string, string> {
                        { "completed", "Completed" },
                        { "incomplete", "In Complete" }
                    };
    }
}
