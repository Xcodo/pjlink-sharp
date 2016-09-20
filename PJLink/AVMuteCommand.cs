// The C# PJLink library.
// Copyright (C) 2010 RV realtime visions GmbH (www.realtimevisions.com)
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rv
{
    public class AVMuteCommand : Command
    {
        private MuteStatus _status = MuteStatus.UNKNOWN;
        private Mute _cmdDetail;

        public enum Mute
        {
            QUERY,
            UNMUTE,
            MUTE
        }

        public enum MuteStatus
        {
            UNKNOWN,
            UNMUTED,
            MUTED
        }

        public AVMuteCommand(Mute setting)
        {
            _cmdDetail = setting;
        }

        internal override string getCommandString()
        {
            string cmdString = "%1AVMT ";

            switch (_cmdDetail)
            {
                case Mute.QUERY:
                    cmdString += "?";
                    break;
                case Mute.UNMUTE:
                    cmdString += "30";
                    break;
                case Mute.MUTE:
                    cmdString += "31";
                    break;
            }

            return cmdString;
        }

        internal override bool processAnswerString(string a)
        {
            if (!base.processAnswerString(a))
            {
                _status = MuteStatus.UNKNOWN;
                return false;
            }

            if(_cmdDetail == Mute.QUERY)
            {
                a = a.Replace("%1AVMT=", "");
                int retVal = int.Parse(a);
                if (retVal == 31)
                    _status = MuteStatus.MUTED;
                else if (retVal == 30)
                    _status = MuteStatus.UNMUTED;
                else
                    _status = MuteStatus.UNKNOWN;
            }

            return true;
        }


        public MuteStatus AVMute
        {
            get { return _status; }
        }

        public override string dumpToString()
        {
            string toRet = "AVMute" + _status.ToString();
            return toRet;
        }

    }
}
