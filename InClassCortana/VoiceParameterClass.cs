using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InClassCortana
{
    public class VoiceParameterClass
    {
        public string xValue { get; set; }
        public string yValue { get; set; }
        public string textSpoken { get; set; }
        public string commandName { get; set; }

        public VoiceParameterClass()
        {
            this.xValue = "None";
            this.yValue = "None";
            this.textSpoken = "None";
            this.commandName = "None";
        }
        public VoiceParameterClass(string xValue,
                                 string yValue,
                                 string textSpoken,
                                 string commandName)
        {
            this.xValue = xValue;
            this.yValue = yValue;
            this.textSpoken = textSpoken;
            this.commandName = commandName;
        }




    }
}
