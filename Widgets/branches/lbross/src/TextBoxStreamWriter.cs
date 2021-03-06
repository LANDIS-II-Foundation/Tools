﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Widgets
{
    public class TextBoxStreamWriter : TextWriter
    //http://saezndaree.wordpress.com/2009/03/29/how-to-redirect-the-consoles-output-to-a-textbox-in-c/
    {
        TextBox _output = null;

        public TextBoxStreamWriter(TextBox output)
        {
            _output = output;
        }

        //public override void Write(char value)
        //{
        //    base.Write(value);
        //    _output.AppendText(value.ToString()); // When character data is written, append it to the text box.
        //}

        public override void WriteLine(string text)
        {
            base.WriteLine(text);
            _output.AppendText(text + "\n"); // When character data is written, append it to the text box.
        }


        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
