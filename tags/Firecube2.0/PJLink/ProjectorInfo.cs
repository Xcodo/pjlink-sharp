using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization; 
using System.IO;

namespace rv
{
    [Serializable]
    public class ProjectorInfo
    {
        private string _projectorHostName = "not set";
        private string _projectorName = "Unknown";
        private string _projectorManufacturerName = "Unknown";
        private string _projectorProductName = "Unknown";
        private PowerCommand.PowerStatus _powerStatus = PowerCommand.PowerStatus.UNKNOWN;

        private int _numOfLamps = 0;
        private List<LampStatusCommand.Status> _multiLampStatus;
        private List<int> _multiLampHours;

        private ErrorStatusCommand.Status _fanStatus = ErrorStatusCommand.Status.UNKNOWN;
        private ErrorStatusCommand.Status _lampStatus = ErrorStatusCommand.Status.UNKNOWN;
        private ErrorStatusCommand.Status _coverStatus = ErrorStatusCommand.Status.UNKNOWN;
        private ErrorStatusCommand.Status _filterStatus = ErrorStatusCommand.Status.UNKNOWN;
        private ErrorStatusCommand.Status _otherStatus = ErrorStatusCommand.Status.UNKNOWN;

        private InputCommand.InputType _input = InputCommand.InputType.UNKNOWN;
        private int _inputPort;


        public static ProjectorInfo create(PJLinkConnection c)
        {
            ProjectorInfo pi = new ProjectorInfo(); 
            pi._projectorHostName = c.HostName; 
            ProjectorNameCommand pnc = new ProjectorNameCommand();
            if (c.sendCommand(pnc) == Command.Response.SUCCESS)
                pi._projectorName = pnc.Name;
            else
                return pi; 

            ManufacturerNameCommand mnc = new ManufacturerNameCommand();
            if (c.sendCommand(mnc) == Command.Response.SUCCESS)
                pi._projectorManufacturerName = mnc.Manufacturer;

            ProductNameCommand prnc = new ProductNameCommand();
            if (c.sendCommand(prnc) == Command.Response.SUCCESS)
                pi._projectorProductName = prnc.ProductName;   
            
            ErrorStatusCommand esc = new ErrorStatusCommand(); 
            if (c.sendCommand(esc) == Command.Response.SUCCESS)
            {
                pi._fanStatus = esc.FanStatus;
                pi._lampStatus = esc.LampStatus;
                pi._coverStatus = esc.CoverStatus;
                pi._filterStatus = esc.FilterStatus;
                pi._otherStatus = esc.OtherStatus; 
            }

            PowerCommand pc = new PowerCommand(PowerCommand.Power.QUERY);
            if (c.sendCommand(pc) == Command.Response.SUCCESS)
                pi._powerStatus = pc.Status;

            LampStatusCommand lsc = new LampStatusCommand();
            if (c.sendCommand(lsc) == Command.Response.SUCCESS)
            {
                pi._multiLampStatus = lsc.StatusList;
                pi._multiLampHours = lsc.HoursList;
                pi._numOfLamps = lsc.NumOfLamps; 
            }

            InputCommand ic = new InputCommand();
            if (c.sendCommand(ic) == Command.Response.SUCCESS)
            {
                pi._input = ic.Input;
                pi._inputPort = ic.Port; 
            }

            return pi; 
        }

        public static ProjectorInfo create(string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ProjectorInfo));
            ProjectorInfo pi = (ProjectorInfo)serializer.Deserialize(new StringReader(xmlString));
            return pi; 
        }

        public string toXmlString()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ProjectorInfo));
            StringWriter sw = new StringWriter();
            serializer.Serialize(sw, this);
            string serializedXml = sw.ToString();
            return serializedXml; 
        }

        public string ProjectorHostName
        {
            get { return _projectorHostName; }
            set { _projectorHostName = value; }
        }
        public string ProjectorName
        {
            get { return _projectorName; }
            set { _projectorName = value; }
        }
        public string ProjectorProductName
        {
            get { return _projectorProductName; }
            set { _projectorProductName = value; }
        }
        public string ProjectorManufacturerName
        {
            get { return _projectorManufacturerName; }
            set { _projectorManufacturerName = value; }

        }
        public PowerCommand.PowerStatus PowerStatus
        {
            get { return _powerStatus; }
            set { _powerStatus = value; }
        }
        public int NumOfLamps
        {
            get { return _numOfLamps; }
            set { _numOfLamps = value; }
        }
        public List<LampStatusCommand.Status> MultiLampStatus
        {
            get { return _multiLampStatus; }            
        }
        public List<int> MultiLampHours
        {
            get { return _multiLampHours; }            
        }
        public ErrorStatusCommand.Status FanStatus
        {
            get { return _fanStatus; }
            set { _fanStatus = value; }
        }
        public ErrorStatusCommand.Status LampStatus
        {
            get { return _lampStatus; }
            set { _lampStatus = value; }
        }

        public ErrorStatusCommand.Status CoverStatus
        {
            get { return _coverStatus; }
            set { _coverStatus = value; }
        }
        public ErrorStatusCommand.Status FilterStatus
        {
            get { return _filterStatus; }
            set { _filterStatus = value; }
        }
        public InputCommand.InputType Input
        {
            get { return _input; }
            set { _input = value; }
        }
        public int InputPort
        {
            get { return _inputPort; }
            set { _inputPort = value; }
        }
    }
}
